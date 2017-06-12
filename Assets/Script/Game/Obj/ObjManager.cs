#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

public class InitBaseData
{
    public int DataId;
    public float DirX;
    public float DirZ;
    public int HpMax;
    public int HpNow;
    public int Level;
    public int ModelId = -1;
    public int MpMax;
    public int MpNow;
    public string Name;
    public ulong ObjId;
    public ReasonType Reason;
    public bool UseFakeModel;
    public float X;
    public float Y;
    public float Z;
}

public class InitCharacterData : InitBaseData
{
    public string AllianceName = "";
    public eAreaState AreaState;
    public int Camp;
    public Dictionary<int, int> EquipModel = new Dictionary<int, int>();
    public bool IsDead;
    public bool IsMoving;
    public float MoveSpeed;
    public int Reborn;
    public List<Vector3> TargetPos;
    public Dictionary<int, int> TitleList = new Dictionary<int, int>();
    public int PkModel { get; set; }
    public int PkValue { get; set; }
}

public class InitNPCData : InitCharacterData
{
}

public class InitRetinueData : InitNPCData
{
    public ulong Owner;
}

public class InitOtherPlayerData : InitCharacterData
{
    public ulong RobotId;
    public int ServerId;
}

public class InitMyPlayerData : InitCharacterData
{
    public Dictionary<uint, int> Buff = new Dictionary<uint, int>();
}

public class InitDropItemData : InitBaseData
{
    public List<ulong> Owner = new List<ulong>();
    public bool PlayDrop;
    public int RemianSeconds;
    public Vector2 TargetPos;
}

public class ObjManager : Singleton<ObjManager>, IManager
{
    private const float ObstaclePriority = 5.0f;

    public ObjManager()
    {
        MyPlayer = null;
    }

    private int CheckObjOutOfRange = 30;
    private int LoadHeadCountPerFrame = 1;
    protected Queue<Action> mActionQueue = new Queue<Action>();
    private readonly HashSet<ulong> mFakeList = new HashSet<ulong>();

    private readonly LinkedList<KeyValuePair<float, ObjBase>> mInvisibleList =
        new LinkedList<KeyValuePair<float, ObjBase>>();

    private readonly Dictionary<ulong, ObjBase> mObjDict = new Dictionary<ulong, ObjBase>();
    public ObjMyPlayer MyPlayer { get; private set; }

    public Dictionary<ulong, ObjBase> ObjPool
    {
        get { return mObjDict; }
    }

    public void AddAction(Action act)
    {
        mActionQueue.Enqueue(act);
    }

    public bool CanLoad()
    {
        return LoadHeadCountPerFrame-- > 0;
    }

    public void CreateDropItemAsync(InitDropItemData initData, Action<ObjDropItem> callBack = null)
    {
        GameObject root = null;
        if (null != GameLogic.Instance && null != GameLogic.Instance.Scene)
        {
            root = GameLogic.Instance.Scene.DropItemRoot;
        }
        CreateObjAsync(initData, Resource.PrefabPath.DropItem,
            root, callBack);
    }

    public ObjMyPlayer CreateMainPlayer(InitMyPlayerData initData)
    {
        if (null != MyPlayer)
        {
            Logger.Error("mMainPlayer != null");
            Object.Destroy(MyPlayer.gameObject);
            MyPlayer = null;
        }

        var objRes = ResourceManager.PrepareResourceSync<GameObject>(Resource.PrefabPath.MyPlayer);
        if (null == objRes)
        {
            Logger.Debug("ERROR: Resources.Load[{0}]", Resource.PrefabPath.MyPlayer);
            return null;
        }

        var p = new Vector3(initData.X, initData.Y, initData.Z);
        if (initData.DirX == 0 && initData.DirZ == 0)
        {
            initData.DirX = 1.0f;
        }
        var q = Quaternion.LookRotation(new Vector3(initData.DirX, 0, initData.DirZ));
        var obj = Object.Instantiate(objRes, p, q) as GameObject;

        var avatar = obj.GetComponent<ActorAvatar>();


#if UNITY_EDITOR

        obj.name = string.Format("[{0}]({1})_{2}", initData.Name, initData.DataId, initData.ObjId);
#else
        obj.name = initData.ObjId.ToString();
#endif

        var character = obj.GetComponent<ObjCharacter>();
        character.Init(initData);
        obj.SetActive(true);

        mObjDict.Add(initData.ObjId, character);

        Logger.Debug("SUCCESS: Create Character[{0}]", character.GetObjId());

        MyPlayer = obj.GetComponent<ObjMyPlayer>();
        var e1 = new Character_Create_Event(initData.ObjId);
        EventDispatcher.Instance.DispatchEvent(e1);

        MyPlayer.TitleList = PlayerDataManager.Instance.TitleList;
        return MyPlayer;
    }

    public void CreateNPCAsync(InitNPCData initData, Action<ObjNPC> callBack = null)
    {
        GameObject root = null;
        if (null != GameLogic.Instance && null != GameLogic.Instance.Scene)
        {
            root = GameLogic.Instance.Scene.NpcRoot;
        }
        CreateObjAsync(initData, Resource.PrefabPath.NPC,
            root, callBack);
    }

    private void CreateObjAsync<T>(InitBaseData initData,
                                   string prefabPath,
                                   GameObject root = null,
                                   Action<T> callBack = null)
        where T : ObjBase
    {
        if (mObjDict.ContainsKey(initData.ObjId))
        {
            Logger.Warn("mObjPool.ContainsKey[{0}]", initData.ObjId.ToString());
            RemoveObj(initData.ObjId);
        }

        //ResourceManager.PrepareResource<GameObject>(prefabPath, prefab =>
        {
            var pos = new Vector3(initData.X, initData.Y, initData.Z);
            var go = ComplexObjectPool.NewObjectSync(prefabPath);
            var goTransform = go.transform;

            goTransform.localPosition = pos;
            goTransform.localRotation = Quaternion.identity;

            if (null != root)
            {
                goTransform.parent = root.transform;
            }
#if UNITY_EDITOR
            go.name = string.Format("[{0}]({1})_{2}", initData.Name, initData.DataId, initData.ObjId);
#endif
            go.SetActive(true);
            var obj = go.GetComponent<T>();
            try
            {
                obj.Init(initData, () =>
                {
                    if (null != callBack)
                    {
                        callBack(obj);
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Error("Create obj[{0}] failed! error:[{1}]", initData.ObjId, e.ToString());
#if !UNITY_EDITOR
                ComplexObjectPool.Release(go);
#endif
            }

            if (initData.UseFakeModel)
            {
                mFakeList.Add(initData.ObjId);
            }

            mObjDict.Add(initData.ObjId, obj);

            var e1 = new Character_Create_Event(initData.ObjId);
            EventDispatcher.Instance.DispatchEvent(e1);
            Logger.Info("CreateObjAsync Obj[{0}]", initData.ObjId.ToString());
        }
        //);
    }

    public void CreateOtherPlayerAsync(InitOtherPlayerData initData, Action<ObjOtherPlayer> callBack = null)
    {
        GameObject root = null;
        if (null != GameLogic.Instance && null != GameLogic.Instance.Scene)
        {
            root = GameLogic.Instance.Scene.OtherPlayerRoot;
        }

        if (GameLogic.Instance.Scene.OtherPlayerRootTransform.childCount - mFakeList.Count >
            GameSetting.Instance.MaxVisibleModelCount)
        {
            initData.UseFakeModel = true;
        }

        CreateObjAsync(initData, Resource.PrefabPath.OtherPlayer,
            root, callBack);
    }

    public void CreateRetinueAsync(InitRetinueData initData, Action<ObjRetinue> callBack = null)
    {
        CreateObjAsync(initData, Resource.PrefabPath.Retinue, null, callBack);
    }

    public ObjCharacter FindCharacterById(ulong objId)
    {
        ObjBase obj = null;
        if (mObjDict.TryGetValue(objId, out obj))
        {
            if (obj.IsCharacter())
            {
                obj.Invisible = false;
                return obj as ObjCharacter;
            }
        }

        return null;
    }


    public ObjBase FindObjByIdNoInvisable(ulong objId)
    {
        ObjBase obj = null;
        if (mObjDict.TryGetValue(objId, out obj))
        {
            return obj;
        }
        return null;
    }


    public ObjBase FindObjById(ulong objId)
    {
        ObjBase obj = null;
        if (mObjDict.TryGetValue(objId, out obj))
        {
            obj.Invisible = false;
            return obj;
        }
        return null;
    }

    public void PrepareMainPlayerSkillResources()
    {
        if (MyPlayer == null)
        {
            return;
        }

        if (null != PlayerDataManager.Instance.PlayerDataModel)
        {
            var skilldata = PlayerDataManager.Instance.PlayerDataModel.SkillData;
            var count = skilldata.EquipSkills.Count;

            for (var i = 0; i < count; i++)
            {
                var skill = skilldata.EquipSkills[i];

                // 没有主手武器，不能放技能
                if (MyPlayer.EquipList.ContainsKey(17))
                {
                    MyPlayer.PrepareSkillResources(skill.SkillId);
                }
            }
        }
    }

    public void RemoveAllObj()
    {
        var pool = new Dictionary<ulong, ObjBase>(mObjDict);
        {
            // foreach(var obj in pool)
            var __enumerator1 = (pool).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var obj = __enumerator1.Current;
                {
                    RemoveObj(obj.Key);
                }
            }
        }
        pool.Clear();
        mFakeList.Clear();
        mObjDict.Clear();
        mInvisibleList.Clear();
        MyPlayer = null;
    }

    public void RemoveObj(ulong objId)
    {
        var obj = FindObjById(objId);
        if (obj)
        {
            //obj.SetModel(null);
            obj.Destroy();
            mObjDict.Remove(objId);
            mFakeList.Remove(objId);

            var l = OptList<ulong>.List;
            l.Clear();
            var enumerator = mFakeList.GetEnumerator();
            var count = GameSetting.Instance.MaxVisibleModelCount;
            while (enumerator.MoveNext() &&
                   GameLogic.Instance.Scene.OtherPlayerRootTransform.childCount - mFakeList.Count <=
                   count)
            {
                var id = enumerator.Current;
                obj = mObjDict[id];
                (obj as ObjCharacter).UsingFakeModel = false;
                l.Add(id);
            }

            for (var i = 0; i < l.Count; ++i)
            {
                mFakeList.Remove(l[i]);
            }
        }
    }

    //删除除了主角以外的Obj
    public void RemoveObjExceptPlayer()
    {
        ulong id = null == MyPlayer ? TypeDefine.INVALID_ULONG : MyPlayer.GetObjId();
        var objList = new List<ulong>();
        {
            // foreach(var pair in ObjPool)
            var __enumerator5 = (ObjPool).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var pair = __enumerator5.Current;
                {
                    if (id == pair.Key)
                    {
                        continue;
                    }
                    objList.Add(pair.Key);
                }
            }
        }
        {
            var __list6 = objList;
            var __listCount6 = __list6.Count;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var objId = __list6[__i6];
                {
                    RemoveObj(objId);
                }
            }
        }
    }

    public void ShowHideOtherPlayerTitle(bool ishow)
    {
        ulong id = null == MyPlayer ? TypeDefine.INVALID_ULONG : MyPlayer.GetObjId();

        var __enumerator5 = (ObjPool).GetEnumerator();
        while (__enumerator5.MoveNext())
        {
            var pair = __enumerator5.Current;
            {
                if (id == pair.Key)
                {
                    continue;
                }

                if (pair.Value)
                {
                    var character = pair.Value as ObjCharacter;
                    if (character)
                    {
                        character.ShowHideOtherTitle(ishow);
                    }
                }
            }
        }
    }

    public void ResetShadow()
    {
        foreach (var objBase in mObjDict)
        {
            var obj = objBase.Value as ObjCharacter;
            if (obj == null)
            {
                continue;
            }

            if (obj.GetObjType() == OBJ.TYPE.MYPLAYER)
            {
                obj.InitShadow(GameSetting.Instance.ShowDynamicShadow);
            }
            else
            {
                obj.InitShadow();
            }
        }
    }

    public ObjCharacter SelectNearestCharacter(Vector3 pos,
                                               Func<ObjCharacter, bool> fucn = null,
                                               float maxDistance = 30.0f)
    {
        if (MyPlayer == null)
        {
            return null;
        }

        if (GameControl.Instance.JoyStickPressed)
        {
            ObjCharacter character = null;
            var distance = float.MaxValue;
            {
                // foreach(var pair in mObjDict)
                var __enumerator2 = (mObjDict).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var pair = __enumerator2.Current;
                    {
                        if (!pair.Value.IsCharacter())
                        {
                            continue;
                        }
                        if (pair.Value == MyPlayer)
                        {
                            continue;
                        }

                        var temp = pair.Value as ObjCharacter;

                        if (temp.GetObjType() == OBJ.TYPE.NPC)
                        {
                            var tempObj = temp as ObjNPC;
                            if (tempObj == null)
                            {
                                continue;
                            }
                            if (tempObj.TableNPC.Interactive != 1)
                            {
                                continue;
                            }
                        }

                        if (temp.Dead)
                        {
                            continue;
                        }

                        var dist = Vector2.Distance(temp.Position.xz(), MyPlayer.Position.xz());
                        if (maxDistance > 0 && dist > maxDistance)
                        {
                            continue;
                        }

                        // angle weighted 2 here.
                        var diff = (1.0f -
                                    Vector3.Dot((temp.Position - MyPlayer.Position).normalized, MyPlayer.TargetDirection))*
                                   2 + dist;
                        if (diff > distance)
                        {
                            continue;
                        }

                        if (null != fucn)
                        {
                            if (!fucn(temp))
                            {
                                continue;
                            }
                        }

                        //增加阻挡判断（如果碰到阻挡，降低阻挡里的物体的权值）

                        NavMeshHit hit;
                        if (NavMesh.Raycast(MyPlayer.Position, temp.Position, out hit, -1))
                        {
                            if (dist + ObstaclePriority > maxDistance)
                            {
                                continue;
                            }
                        }

                        character = temp;
                        distance = diff;
                    }
                }
            }
            return character;
        }
        else
        {
            ObjCharacter character = null;
            var distance = maxDistance;
            {
                // foreach(var pair in mObjDict)
                var __enumerator3 = (mObjDict).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var pair = __enumerator3.Current;
                    {
                        if (!pair.Value.IsCharacter())
                        {
                            continue;
                        }
                        if (pair.Value == MyPlayer)
                        {
                            continue;
                        }

                        var temp = pair.Value as ObjCharacter;

                        if (pair.Value.GetObjType() == OBJ.TYPE.NPC)
                        {
                            if ((temp as ObjNPC).TableNPC.Interactive != 1)
                            {
                                continue;
                            }
                        }

                        var diff = Vector2.Distance(temp.Position.xz(), pos.xz());
                        if (distance > 0 && diff > distance)
                        {
                            continue;
                        }

                        if (null != fucn)
                        {
                            if (!fucn(temp))
                            {
                                continue;
                            }
                        }

                        //增加阻挡判断（如果碰到阻挡，降低阻挡里的物体的权值）
                        NavMeshHit hit;
                        if (NavMesh.Raycast(MyPlayer.Position, temp.Position, out hit, -1))
                        {
                            if (distance > 0 && diff + ObstaclePriority > distance)
                            {
                                continue;
                            }
                        }

                        character = temp;
                        distance = diff;
                    }
                }
            }
            return character;
        }
    }

    public ObjBase SelectNearestObj(Vector3 pos, Func<ObjBase, bool> fucn = null)
    {
        if (MyPlayer == null)
        {
            return null;
        }

        ObjBase ret = null;
        var distance = -1.0f;
        {
            // foreach(var pair in mObjDict)
            var __enumerator4 = (mObjDict).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var pair = __enumerator4.Current;
                {
                    var obj = pair.Value;
                    if (obj == null)
                    {
                        continue;
                    }
                    if (obj.GetObjType() == OBJ.TYPE.NPC)
                    {
                        var npc = obj as ObjNPC;
                        if (npc == null || npc.TableNPC == null || npc.TableNPC.Interactive != 1)
                            continue;
                    }
                    if (null != fucn)
                    {
                        if (!fucn(obj))
                        {
                            continue;
                        }
                    }
                    var diff = Vector2.Distance(obj.Position.xz(), pos.xz());
                    if (distance > 0 && diff > distance)
                    {
                        continue;
                    }
                    //增加阻挡判断（如果碰到阻挡，降低阻挡里的物体的权值）
                    NavMeshHit hit;
                    if (NavMesh.Raycast(MyPlayer.Position, obj.Position, out hit, -1))
                    {
                        if (distance > 0 && diff + ObstaclePriority > distance)
                        {
                            continue;
                        }
                    }
                    ret = obj;
                    distance = diff;
                }
            }
        }
        return ret;
    }

    public ObjCharacter SelectTargetForSkill(ObjCharacter player, SkillRecord skillRecord)
    {
        var skillTargetType = (SkillTargetType) ObjMyPlayer.GetSkillData_Data(skillRecord, eModifySkillType.TargetType);

        if (GameControl.Instance.TargetObj != null && !GameControl.Instance.TargetObj.Dead &&
            Instance.MyPlayer.IsMyEnemy(GameControl.Instance.TargetObj))
        {
            // 这种技能，只能依靠自动瞄准，如果上一个目标可用，就继续打
            if (skillTargetType != SkillTargetType.SECTOR &&
                skillTargetType != SkillTargetType.RECT)
            {
                var dist = GameControl.GetSkillReleaseDistance(skillRecord);
                if (Vector3.Distance(player.Position, GameControl.Instance.TargetObj.Position) < dist)
                {
                    return GameControl.Instance.TargetObj;
                }
            }
            else
            {
                if (GameSetting.Instance.TargetSelectionAssistant && skillRecord.AutoEnemy == 1)
                {
                    // 如果需要辅助瞄准，技能配置也需要辅助瞄准，而且上一个目标可用，就继续打它
                    var dist = GameControl.GetSkillReleaseDistance(skillRecord);
                    if (Vector3.Distance(player.Position, GameControl.Instance.TargetObj.Position) < dist)
                    {
                        return GameControl.Instance.TargetObj;
                    }
                }
                else
                {
                    // 如果不需要辅助瞄准，技能也配置了不需要辅助瞄准，那就不瞄准了
                    return null;
                }
            }
        }

        ObjCharacter targetObj = null;
        if (skillTargetType == SkillTargetType.CIRCLE)
        {
            // nothing
        }
        else if (skillTargetType == SkillTargetType.SECTOR)
        {
            if (GameSetting.Instance.TargetSelectionAssistant && skillRecord.AutoEnemy == 1)
            {
                targetObj = SelectNearestCharacter(player.Position,
                    character => !character.Dead && player.IsMyEnemy(character), skillRecord.TargetParam[0]);
            }
        }
        else if (skillTargetType == SkillTargetType.RECT)
        {
            if (GameSetting.Instance.TargetSelectionAssistant && skillRecord.AutoEnemy == 1)
            {
                targetObj = SelectNearestCharacter(player.Position,
                    character => !character.Dead && player.IsMyEnemy(character), skillRecord.TargetParam[1]);
            }
        }
        else if (skillTargetType == SkillTargetType.TARGET_RECT)
        {
            targetObj = SelectNearestCharacter(player.Position,
                character => !character.Dead && player.IsMyEnemy(character));
        }
        else if (skillTargetType == SkillTargetType.TARGET_SECTOR)
        {
            targetObj = SelectNearestCharacter(player.Position,
                character => !character.Dead && player.IsMyEnemy(character));
        }
        else if (skillTargetType == SkillTargetType.SINGLE)
        {
            targetObj = SelectNearestCharacter(player.Position,
                character => !character.Dead && player.IsMyEnemy(character));
        }
        else if (skillTargetType == SkillTargetType.TARGET_CIRCLE)
        {
            targetObj = SelectNearestCharacter(player.Position,
                character => !character.Dead && player.IsMyEnemy(character));
        }

        return targetObj;
    }

    public void UpdateRadarMapLoction(List<RararCharDataModel> charaModels, float rate)
    {
        {
            // foreach(var model in charaModels)
            var enumerator4 = (charaModels).GetEnumerator();
            while (enumerator4.MoveNext())
            {
                var model = enumerator4.Current;
                {
                    if (model != null)
                    {
                        if (model.CharType != 2)
                        {
                            continue;
                        }
                        var obj = FindObjById(model.CharacterId);
                        if (obj == null)
                        {
                            continue;
                        }

                        if (model.Loction != obj.Position)
                        {
                            model.Loction = new Vector3(obj.Position.x * rate, obj.Position.z * rate, 0);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator Init()
    {
        yield return null;
    }

    public void Reset()
    {
        mObjDict.Clear();
        mFakeList.Clear();
        mInvisibleList.Clear();
        MyPlayer = null;
    }

    public void Tick(float delta)
    {
        if (mActionQueue.Count > 0)
        {
            var act = mActionQueue.Dequeue();
            try
            {
                act();
            }
            catch
            {
                // do nothing...
            }
        }

        LoadHeadCountPerFrame = 1;

//         CheckObjOutOfRange--;
//         if (CheckObjOutOfRange == 0)
//         {
//             // Check per 30 frame.
//             CheckObjOutOfRange = 30;
// 
//             OptList<ulong>.List.Clear();
//             foreach (var obj in mObjDict)
//             {
//                 if (obj.Value)
//                 {
//                     if (Vector3.Distance(obj.Value.Position, MyPlayer.Position) > GameLogic.Instance.Scene.TableScene.SeeArea * 3)
//                     {
//                         mInvisibleList.AddLast(new KeyValuePair<float, ObjBase>(Time.time + 30.0f, obj.Value));
//                         obj.Value.Invisible = true;
//                     }
//                 }
//             }
//         }
// 
//         if (mInvisibleList.Count > 0 && mInvisibleList.First.Value.Key > Time.time)
//         {
//             ObjBase obj = mInvisibleList.First.Value.Value;
//             if (obj.Invisible)
//             {
//                 Logger.Warn("Obj [{0}] 已经看不见很久了，需要被删除了", obj.GetObjId());
//                 RemoveObj(obj.GetObjId());
//             }
// 
//             mInvisibleList.RemoveFirst();
//         }
    }

    public void Destroy()
    {
        RemoveAllObj();
    }

    public void SelectObj(Func<ObjBase, bool> fucn)
    {
        if (fucn == null)
            return;
        var __enumerator4 = (mObjDict).GetEnumerator();
        while (__enumerator4.MoveNext())
        {
            var pair = __enumerator4.Current;
            {
                var obj = pair.Value;
                if (obj == null)
                {
                    continue;
                }

                if (!fucn(obj))
                {
                    return;
                }

            }
        }
      
    }
}