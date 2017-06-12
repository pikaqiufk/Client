#region using

using System;
using System.Collections;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    public void AddBuff(BuffResult buff)
    {
        if (null == ObjManager.Instance.MyPlayer)
        {
            return;
        }

        // 如果5秒内，这个buff还没有被处理，那就算了
        // 通常这种情况发生于切后台后，服务器向客户端广播了很多buff
        // 但是客户端在后台没法处理，就累积起来了
        if (buff.ViewTime > 0 && buff.Type != BuffType.HT_RELIVE && buff.Type != BuffType.HT_DIE &&
            buff.Type != BuffType.HT_ADDBUFF &&
            buff.Type != BuffType.HT_DELBUFF && buff.Type != BuffType.HT_MOVE)
        {
            var viewTime = Extension.FromServerBinary((long) buff.ViewTime).AddSeconds(5);
            if (viewTime < Game.Instance.ServerTime)
            {
                return;
            }
        }

        var character = ObjManager.Instance.FindCharacterById(buff.TargetObjId);
        if (null == character)
        {
            //Logger.Warn("Don't find character[{0}].Buff[{1}]", buff.TargetObjId, (int)buff.Type);
            return;
        }

        if (buff.Type == BuffType.HT_RELIVE)
        {
            character.Relive();
            return;
        }

        if (buff.Type != BuffType.HT_MOVE)
        {
            character.ShowDamage(buff);
        }

        switch (buff.Type)
        {
            case BuffType.HT_NORMAL:
            case BuffType.HT_CRITICAL:
            case BuffType.HT_EXCELLENT:
            {
                //如果是这种伤害类型就不播被击动作
                if (buff.Param.Count > 0)
                {
                    if (3 == buff.Param[0])
                    {
                        break;
                    }
                }

                if (character.GetObjType() == OBJ.TYPE.NPC)
                {
                    if (character.GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                    {
                        //character.GetAnimationController().Animation.Blend("Hit", 0.5f);
                        character.PlayAnimation(OBJ.CHARACTER_ANI.HIT, ani =>
                        {
                            if (character.GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
                            {
                                character.PlayAnimation(OBJ.CHARACTER_ANI.RUN);
                            }
                        });
                    }
                    else
                    {
                        character.DoHurt();
                    }
                }
                else
                {
                    character.DoHurt();
                }
            }
                break;
            case BuffType.HT_MISS:
                break;
            case BuffType.HT_CHANGE_SCENE:
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buff));
                character.AddBuff(buff.BuffId, buff.BuffTypeId, buff.SkillObjId, buff.TargetObjId, false);
            }
                break;
            case BuffType.HT_ADDBUFF:
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buff));
                character.AddBuff(buff.BuffId, buff.BuffTypeId, buff.SkillObjId, buff.TargetObjId);
                var table = Table.GetBuff(buff.BuffTypeId);
                if (null != table)
                {
                    if (6 == table.Type)
                    {
//眩晕
                        character.DoDizzy();
                    }
                }
            }
                break;
            case BuffType.HT_DIE:
            {
                var delay =
                    (float) (Extension.FromServerBinary((long) buff.ViewTime) - Game.Instance.ServerTime).TotalSeconds;
                if (delay <= 0.01)
                {
                    character.DoDie();
                }
                else
                {
                    character.DeleteObjTime = Time.time + delay;
                    character.DelayDie(delay);
                }
                Logger.Debug("Die character[{0}].Buff[{1}]", buff.TargetObjId, (int) buff.Type);
            }
                break;
            case BuffType.HT_HEALTH:
                break;
            case BuffType.HT_DELBUFF:
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buff));
                character.RemoveBuff(buff.BuffId);

                var table = Table.GetBuff(buff.BuffTypeId);
                if (null != table)
                {
                    if (6 == table.Type)
                    {
//眩晕
                        if (character.GetCurrentStateName() == OBJ.CHARACTER_STATE.DIZZY)
                        {
                            character.DoIdle();
                        }
                    }
                }
            }
                break;
            case BuffType.HT_RELIVE:
                character.DoIdle();
                break;
            case BuffType.HT_EFFECT:
            {
                var record = Table.GetBuff(buff.BuffTypeId);
                if (record == null)
                {
                    Logger.Log2Bugly(" AddBuff record = null");
                    return;
                }
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buff));

                // 技能范围指示
                if (buff.Param.Count == 6)
                {
                    ShowSkillIndicator(buff, character);
                }
                if (ObjManager.Instance == null || ObjManager.Instance.MyPlayer == null)
                {
                    Logger.Log2Bugly(" AddBuff ObjManager.Instance.MyPlayer= null");
                    return;
                }

                if (SoundManager.Instance == null || EffectManager.Instance == null)
                {
                    Logger.Log2Bugly(" AddBuff SoundManager.Instance= null");
                    return;
                }

                var isSelfCast = buff.SkillObjId == ObjManager.Instance.MyPlayer.GetObjId();
                var showCameraShake = buff.TargetObjId == ObjManager.Instance.MyPlayer.GetObjId() ||
                                      isSelfCast;
                {
                    var oldBuff = character.GetBuff(buff.BuffId);
                    if (oldBuff != null)
                    {
                        // 如果这个buff的特效还在加载，就先不播放了
                        if (oldBuff.State == BuffState.LoadingEffect)
                        {
                            return;
                        }

                        var effects = oldBuff.EffectRef;
                        // 如果这个buff的特效还没有结束，就不用播放这个特效了
                        for (var i = 0; i < effects.Count; i++)
                        {
                            if (EffectManager.Instance.HasEffect(effects[i].Uuid))
                            {
                                return;
                            }
                        }

                        for (var i = 0; i < oldBuff.EffectId.Count; i++)
                        {
                            if (EffectManager.Instance.HasEffect(oldBuff.EffectId[i]))
                            {
                                return;
                            }
                        }

                        if (record.Sound != -1)
                        {
                            if (oldBuff.SoundId > 0)
                            {
                                SoundManager.Instance.StopSoundEffectByTag(oldBuff.SoundId);
                                SoundManager.Instance.PlaySoundEffect(record.Sound, 1.0f, oldBuff.SoundId, isSelfCast);
                            }
                            else
                            {
                                oldBuff.SoundId = SoundManager.NextTag;
                                SoundManager.Instance.PlaySoundEffect(record.Sound, 1.0f, oldBuff.SoundId, isSelfCast);
                            }
                        }
                    }
                    else
                    {
                        SoundManager.Instance.PlaySoundEffect(record.Sound, 1, 0, isSelfCast);
                    }

                    var __array3 = record.Effect;
                    var __arrayLength3 = __array3.Length;
                    for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
                    {
                        var effect = __array3[__i3];
                        {
                            if (effect == -1)
                            {
                                continue;
                            }

                            var tableData = Table.GetEffect(effect);
                            if (tableData != null)
                            {
                                Vector3? pos = null;
                                if (buff.Param.Count == 6)
                                {
                                    var x = buff.Param[1]/100.0f;
                                    var z = buff.Param[2]/100.0f;
                                    var y = GameLogic.GetTerrainHeight(x, z);
                                    pos = new Vector3(x, y, z);
                                }
                                EffectManager.Instance.CreateEffect(tableData, character, pos, null, null,
                                    (tableData.BroadcastType == 0 && showCameraShake) || tableData.BroadcastType == 1);
                            }
                        }
                    }
                }
            }
                break;
            case BuffType.HT_NODAMAGE:
                break;
            case BuffType.HT_Fire_DAMAGE:
                break;
            case BuffType.HT_Ice_DAMAGE:
                break;
            case BuffType.HT_Poison_DAMAGE:
                break;
            case BuffType.HT_MOVE:
            {
                if (buff.Param[3] > 0)
                {
                    character.StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[3]),
                        () => { character.ForceMoveTo(buff.Param[0]/1000.0f, buff.Param[1]/1000.0f, buff.Param[2]); }));
                }
                else
                {
                    character.ForceMoveTo(buff.Param[0]/1000.0f, buff.Param[1]/1000.0f, buff.Param[2]);
                }
            }
                break;
            case BuffType.HT_MANA:
                break;
            case BuffType.HT_REBOUND:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DoUseSkill(ulong objId, int skillId, ulong targetId)
    {
        StartCoroutine(SendUseSkillRequestCoroutine(objId, skillId, targetId));
    }

    public IEnumerator SendUseSkillRequestCoroutine(ulong objId, int skillId, ulong targetId)
    {
        //Logger.Error("SendUseSkillRequestCoroutine ------Send--- {1}--- {0}", Game.Instance.ServerTime, skillId);
        if (skillId == PlayerDataManager.Instance.GetSkillNoWeapon())
        {
            skillId = PlayerDataManager.Instance.GetNormalSkill(false);
        }

        var requset = new CharacterUseSkillMsg();
        var obj = ObjManager.Instance.FindCharacterById(objId);
        var skillStates = PlayerDataManager.Instance.PlayerDataModel.SkillData.SkillStates;
        SkillStateData skillState;
        if (skillStates.TryGetValue(skillId, out skillState))
        {
            skillState.State = SkillState.Send;
        }
        else
        {
            skillState = new SkillStateData();
            skillState.SkillId = skillId;
            skillState.State = SkillState.Send;
            skillStates.Add(skillId, skillState);
        }

        var x = obj.Position.x;
        var z = obj.Position.z;
        var dirX = obj.TargetDirection.x;
        var dirZ = obj.TargetDirection.z;
        requset.CharacterId = objId;
        requset.SkillId = skillId;
        requset.TargetObjId.Add(targetId);
        requset.Pos = new PositionData
        {
            Pos = new Vector2Int32
            {
                x = GameUtils.MultiplyPrecision(x),
                y = GameUtils.MultiplyPrecision(z)
            },
            Dir = new Vector2Int32
            {
                x = GameUtils.MultiplyPrecision(dirX),
                y = GameUtils.MultiplyPrecision(dirZ)
            }
        };
        Logger.Info(".......SendUseSkillRequest.....objId : {0}..skillId : {1}", objId, skillId);
        var msg = Instance.SendUseSkillRequest(requset);
        yield return msg.SendAndWaitUntilDone();
        skillState.State = SkillState.Rece;
        //Logger.Error("SendUseSkillRequestCoroutine ------Rece---{1}--- {0}", Game.Instance.ServerTime,skillId);
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                EventDispatcher.Instance.DispatchEvent(new UiEventChangeOutLineTime());
                if (msg.Response.Items.Count < 1)
                {
                    yield break;
                }
                skillId = msg.Response.Items[0];
                if (obj.GetObjType() == OBJ.TYPE.MYPLAYER)
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_UseSkill(skillId));
                    //SkillReleaseNetBack e = new SkillReleaseNetBack(skillId, true);
                    //EventDispatcher.Instance.DispatchEvent(e);
                }
                yield break;
            }
            if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterDie
                || msg.ErrorCode == (int) ErrorCodes.Error_SkillNotUse)
            {
//异步逻辑可能发送接受不一致
                Logger.Info("[DoUseSkill]  SkillId : {0}  msg.ErrorCode={1}", skillId, msg.ErrorCode.ToString());
            }
        }
        //技能释放返回错误
        if (obj.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            var ee = new SkillReleaseNetBack(skillId, false);
            EventDispatcher.Instance.DispatchEvent(ee);
        }
    }

    private void ShowSkillIndicator(BuffResult buff, ObjCharacter character)
    {
        var skillId = buff.Param[0];

        var data = Table.GetSkill(skillId);
        if (data == null)
        {
            Logger.Log2Bugly("ShowSkillIndicator data =null");
            return;
        }
        if (GameLogic.Instance == null || GameLogic.Instance.Scene == null)
        {
            Logger.Log2Bugly("ShowSkillIndicator GameLogic.Instance =null");
            return;
        }
        //SkillTargetType targetType = (SkillTargetType)data.TargetType;
        var targetType = (SkillTargetType) ObjMyPlayer.GetSkillData_Data(data, eModifySkillType.TargetType);
        if (targetType == SkillTargetType.CIRCLE)
        {
            // correct the direction of npc or monster.
            character.TargetDirection = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);

            var para0 = ObjMyPlayer.GetSkillData_Data(data, eModifySkillType.TargetParam1);

            GameLogic.Instance.Scene.CreateSkillRangeIndicator(character.gameObject,
                Scene.SkillRangeIndicatorType.Circle,
                para0, 0, Color.red, (caster, receiver) =>
                {
                    StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () =>
                    {
                        Destroy(caster);
                        Destroy(receiver);
                    }));
                }, true);
        }
        else if (targetType == SkillTargetType.SECTOR || targetType == SkillTargetType.TARGET_SECTOR)
        {
            // correct the direction of npc or monster.
            character.TargetDirection = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);

            GameLogic.Instance.Scene.CreateSkillRangeIndicator(character.gameObject, Scene.SkillRangeIndicatorType.Fan,
                data.TargetParam[0], data.TargetParam[1], Color.red, (caster, receiver) =>
                {
                    StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () =>
                    {
                        Destroy(caster);
                        Destroy(receiver);
                    }));
                }, true);
        }
        else if (targetType == SkillTargetType.RECT || targetType == SkillTargetType.TARGET_RECT)
        {
            // correct the direction of npc or monster.
            character.TargetDirection = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);

            GameLogic.Instance.Scene.CreateSkillRangeIndicator(character.gameObject,
                Scene.SkillRangeIndicatorType.Rectangle,
                data.TargetParam[0], data.TargetParam[1], Color.red, (caster, receiver) =>
                {
                    StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () =>
                    {
                        Destroy(caster);
                        Destroy(receiver);
                    }));
                }, true);
        }
        else if (targetType == SkillTargetType.SINGLE ||
                 targetType == SkillTargetType.TARGET_CIRCLE)
        {
            var x = buff.Param[1]/100.0f;
            var z = buff.Param[2]/100.0f;
            var y = GameLogic.GetTerrainHeight(x, z);
            var o = new GameObject();
            var objTransform = o.transform;
            objTransform.parent = GameLogic.Instance.Scene.GlobalSkillIndicatorRoot.transform;
            objTransform.position = new Vector3(x, y, z);
            objTransform.forward = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);
            if (targetType == SkillTargetType.TARGET_CIRCLE)
            {
                GameLogic.Instance.Scene.CreateSkillRangeIndicator(o,
                    Scene.SkillRangeIndicatorType.Circle,
                    data.TargetParam[0], 0, Color.red,
                    (caster, receiver) =>
                    {
                        StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () => { Destroy(o); }));
                    }, true);
            }
            else if (targetType == SkillTargetType.TARGET_SECTOR)
            {
                GameLogic.Instance.Scene.CreateSkillRangeIndicator(o,
                    Scene.SkillRangeIndicatorType.Fan,
                    data.TargetParam[0], data.TargetParam[1], Color.red, (caster, receiver) =>
                    {
                        var casterTransform = caster.transform;
                        casterTransform.position = new Vector3(x, y, z);
                        casterTransform.forward = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);

                        StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () => { Destroy(o); }));
                    }, true);
            }
            else if (targetType == SkillTargetType.TARGET_RECT)
            {
                GameLogic.Instance.Scene.CreateSkillRangeIndicator(o,
                    Scene.SkillRangeIndicatorType.Rectangle,
                    data.TargetParam[0], data.TargetParam[1], Color.red, (caster, receiver) =>
                    {
                        var casterTransform = caster.transform;
                        casterTransform.position = new Vector3(x, y, z);
                        casterTransform.forward = new Vector3(buff.Param[3]/1000.0f, 0, buff.Param[4]/1000.0f);

                        StartCoroutine(WaitToDoSomething(TimeSpan.FromMilliseconds(buff.Param[5]), () => { Destroy(o); }));
                    }, true);
            }
        }
    }

    public IEnumerator WaitAddBuff(TimeSpan span, BuffResult buff)
    {
        yield return new WaitForSeconds((float) span.TotalSeconds);
        AddBuff(buff);
    }

    public IEnumerator WaitToDoSomething(TimeSpan span, Action act)
    {
        yield return new WaitForSeconds((float) span.TotalSeconds);
        try
        {
            act();
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
    }

    public void NotifyUseSkillList(CharacterUseSkillMsgList msg)
    {
        {
            // foreach(var skill in msg.Skills)
            var __enumerator6 = (msg.Skills).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var skill = __enumerator6.Current;
                {
                    NotifyUseSkill(skill);
                }
            }
        }
    }

    public void NotifyUseSkill(CharacterUseSkillMsg msg)
    {
        if (ObjManager.Instance == null)
        {
            Logger.Log2Bugly(" ObjManager.Instance =null");
            return;
        }
        if (msg == null || msg.Pos == null || msg.Pos.Pos == null)
        {
            Logger.Log2Bugly("NotifyUseSkill msg =null");
            return;
        }
        var chararcter = ObjManager.Instance.FindCharacterById(msg.CharacterId);
        if (null == chararcter)
        {
            Logger.Warn("NotifyUseSkill Cannot find obj[{0}]", msg.CharacterId);
            return;
        }

        if (chararcter.Dead)
        {
            return;
        }

        //放技能时的坐标
        var p = GameLogic.GetTerrainPosition(GameUtils.DividePrecision(msg.Pos.Pos.x),
            GameUtils.DividePrecision(msg.Pos.Pos.y));

        //放技能时的朝向
        var dir = new Vector3(GameUtils.DividePrecision(msg.Pos.Dir.x), 0, GameUtils.DividePrecision(msg.Pos.Dir.y));

        //如果距离差距过大就直接拉过来
        var diff = (chararcter.Position.xz() - p.xz()).magnitude;

        ObjCharacter mainTargetCharacter = null;

        if (chararcter.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            PlatformHelper.Event("Skill", "Auto", msg.SkillId);
            //调整位置
            if (diff > GameSetting.Instance.MainPlayerSkillPosErrorDistance)
            {
                chararcter.Position = p;
            }

            //调整朝向
            chararcter.TargetDirection = dir;
            {
                var __list1 = msg.TargetObjId;
                var __listCount1 = __list1.Count;
                for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var id = __list1[__i1];
                    {
                        var target = ObjManager.Instance.FindCharacterById(id);
                        if (null != target)
                        {
                            mainTargetCharacter = target;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            if (chararcter.GetObjType() == OBJ.TYPE.OTHERPLAYER)
            {
                if (diff > GameSetting.Instance.OtherPlayerSkillPosErrorDistance)
                {
                    chararcter.Position = p;
                }
            }
            else
            {
                if (diff > GameSetting.Instance.NPCSkillPosErrorDistance)
                {
                    chararcter.DelayedMove = p;
                }
            }

            var adjustDir = false;
            {
                var __list2 = msg.TargetObjId;
                var __listCount2 = __list2.Count;
                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var id = __list2[__i2];
                    {
                        var target = ObjManager.Instance.FindCharacterById(id);
                        if (null != target)
                        {
                            mainTargetCharacter = target;
                            chararcter.Target = target;
                            if (chararcter.GetObjType() == OBJ.TYPE.NPC)
                            {
                                if (((ObjNPC) chararcter).TowardPlayer)
                                {
                                    chararcter.FaceTo(target.Position);
                                }
                            }
                            else
                            {
                                chararcter.FaceTo(target.Position);
                            }
                            adjustDir = true;
                            break;
                        }
                    }
                }
            }
            if (!adjustDir)
            {
                chararcter.TargetDirection = dir;
            }
        }

        var skillData = Table.GetSkill(msg.SkillId);
        if (null == skillData)
        {
            Logger.Fatal("Can't find skill[{0}]", msg.SkillId);
            return;
        }

        //放技能
        chararcter.UseSkill(skillData, msg.TargetObjId);

        if (chararcter.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            var e = new SkillReleaseNetBack(skillData.Id, true);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        if (ObjManager.Instance.MyPlayer == null)
        {
            Logger.Log2Bugly("ObjManager.Instance.MyPlayer == null");
            return;
        }

        if (msg.CharacterId != ObjManager.Instance.MyPlayer.GetObjId()
            && msg.TargetObjId.Contains(ObjManager.Instance.MyPlayer.GetObjId()))
        {
            PlayerDataManager.Instance.SetSelectTargetData(chararcter, 1);
        }

        if (msg.CharacterId == ObjManager.Instance.MyPlayer.GetObjId()
            && ObjManager.Instance.MyPlayer != mainTargetCharacter
            && mainTargetCharacter != null
            && mainTargetCharacter.Dead == false)
        {
            PlayerDataManager.Instance.SetSelectTargetData(mainTargetCharacter, 2);
        }
        //Logger.Info("NotifyUseSkill Obj[{0}] skill[{1}]", msg.CharacterId, msg.SkillId);
    }

    public void SyncBuff(BuffResultMsg msg)
    {
        {
            var __list4 = msg.buff;
            var __listCount4 = __list4.Count;
            for (var __i4 = 0; __i4 < __listCount4; ++__i4)
            {
                var buff = __list4[__i4];
                {
                    var time = Extension.FromServerBinary((long) buff.ViewTime);
                    if (time > Game.Instance.ServerTime)
                    {
                        var span = time - Game.Instance.ServerTime;
                        StartCoroutine(WaitAddBuff(span, buff));
                    }
                    else
                    {
                        AddBuff(buff);
                    }
                }
            }
        }
    }

    public void NotifyShootBulletList(BulletMsgList msg)
    {
        {
            // foreach(var bullet in msg.Bullets)
            var __enumerator7 = (msg.Bullets).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var bullet = __enumerator7.Current;
                {
                    NotifyShootBullet(bullet);
                }
            }
        }
    }

    public void NotifyShootBullet(BulletMsg msg)
    {
        Action act = () =>
        {
            var tableData = Table.GetBullet(msg.BulletId);
            var caster = ObjManager.Instance.FindCharacterById(msg.CasterId);
            if (null == caster)
            {
                Logger.Warn("NotifyShootBullet: null==caster[{0}]", msg.CasterId);
                return;
            }

            if (ObjState.Normal != caster.State &&
                ObjState.Dieing != caster.State)
            {
                Logger.Warn("NotifyShootBullet: caster[{0}] state=[{1}]", msg.CasterId, caster.State.ToString());
                return;
            }

            var bulletPath = tableData.Path;
            var speed = tableData.Speed;
            if (caster)
            {
                var pos = caster.Position;
                var emitObj = caster.GetMountPoint(tableData.CasterMountPoint);
                if (null == emitObj)
                {
                    Logger.Error("NotifyShootBullet: Can not find mount point [{0}] in [{1}]",
                        tableData.CasterMountPoint, caster.name);
                }
                else
                {
                    pos = emitObj.position;
                }
                {
                    var __list5 = msg.TargetObjId;
                    var __listCount5 = __list5.Count;
                    for (var __i5 = 0; __i5 < __listCount5; ++__i5)
                    {
                        var targetId = __list5[__i5];
                        {
                            var target = ObjManager.Instance.FindCharacterById(targetId);
                            if (null == target)
                            {
                                continue;
                            }

                            var id = target.GetObjId();
                            var p = target.Position;
                            //ResourceManager.PrepareResource<GameObject>(bulletPath, (res) =>
                            ComplexObjectPool.NewObject(bulletPath, obj =>
                            {
                                if (obj == null)
                                {
                                    return;
                                }

                                obj.transform.position = pos;

                                Missile missile;
                                if (tableData.DirRangeX == 0 && tableData.DirRangeY == 0 && tableData.DirRangeZ == 0)
                                {
                                    missile = obj.GetComponent<Missile>();
                                    if (missile == null)
                                    {
                                        missile = obj.AddComponent<Missile>();
                                    }
                                }
                                else
                                {
                                    var dmissile = obj.GetComponent<DirectionalMissile>();
                                    if (dmissile == null)
                                    {
                                        dmissile = obj.AddComponent<DirectionalMissile>();
                                    }

                                    var v = Random.Range(0, Mathf.PI*2);

                                    dmissile.StartDirection =
                                        new Vector3(
                                            Mathf.Sin(v)*
                                            Random.Range(tableData.DirRangeX*0.8f, tableData.DirRangeX*1.2f),
                                            Mathf.Abs(Mathf.Cos(v))*
                                            Random.Range(tableData.DirRangeY*0.8f, tableData.DirRangeY*1.2f),
                                            Random.Range(tableData.DirRangeZ*0.8f, tableData.DirRangeZ*1.2f));

                                    missile = dmissile;
                                }

                                if (target.GetModel() == null || target.GetModel().gameObject == null)
                                {
                                    missile.Target = target.ObjTransform;
                                }
                                else
                                {
                                    var targetObj = target.GetMountPoint(tableData.BearMountPoint);
                                    if (null == targetObj)
                                    {
                                        Logger.Error("NotifyShootBullet: Can not find mount point [{0}] in [{1}]",
                                            tableData.BearMountPoint, target.name);
                                        missile.Target = target.ObjTransform;
                                    }
                                    else
                                    {
                                        missile.Target = targetObj;
                                    }
                                }

                                missile.Init(speed, p, bullet =>
                                {
                                    var tar = ObjManager.Instance.FindCharacterById(id);
                                    if (null != tar)
                                    {
                                        tar.DoHurt();
                                    }

                                    if (!string.IsNullOrEmpty(tableData.HitEffect))
                                    {
                                        var effectId = Convert.ToInt32(tableData.HitEffect);
                                        if (-1 != effectId)
                                        {
                                            var data = Table.GetEffect(effectId);
                                            EffectManager.Instance.CreateEffect(data, tar, null,
                                                (effect, eId) =>
                                                {
                                                    effect.gameObject.transform.position =
                                                        bullet.gameObject.transform.position;
                                                }, null,
                                                (data.BroadcastType == 0 &&
                                                 targetId == ObjManager.Instance.MyPlayer.GetObjId()) ||
                                                data.BroadcastType == 1);
                                        }
                                    }
                                });

                                missile.enabled = true;
                            }, o =>
                            {
                                OptList<Bullet>.List.Clear();
                                o.GetComponents(OptList<Bullet>.List);
                                {
                                    var __array1 = OptList<Bullet>.List;
                                    var __arrayLength1 = __array1.Count;
                                    for (var __i2 = 0; __i2 < __arrayLength1; ++__i2)
                                    {
                                        var b = __array1[__i2];
                                        b.enabled = false;
                                    }
                                }
                                OptList<TrailRenderer_Base>.List.Clear();
                                o.GetComponentsInChildren(OptList<TrailRenderer_Base>.List);
                                {
                                    var __array1 = OptList<TrailRenderer_Base>.List;
                                    var __arrayLength1 = __array1.Count;
                                    for (var __i2 = 0; __i2 < __arrayLength1; ++__i2)
                                    {
                                        var t = __array1[__i2];
                                        t.ClearSystem(false);
                                        t.Emit = true;
                                    }
                                }
                            });
                        }
                    }
                }
            }
        };

        var time = Extension.FromServerBinary((long) msg.ViewTime);
        if (time > Game.Instance.ServerTime)
        {
            StartCoroutine(WaitToDoSomething(time - Game.Instance.ServerTime, act));
        }
        else
        {
            act();
        }
    }
}