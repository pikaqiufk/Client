#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DataTable;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

//CG播放类
/*
 * 播放的原理是在当前场景造一个新的CGRoot
 * 所有Actor和特效都造在这个上面
 * 包含UI
 * 加载一个文本脚本文件执行 这个类导出的函数
 * 
 * 要被导出的函数必须以cg为开头
 * 用[Tools/Export CG Function To Wrapper]自动导出到CGFunctionWrapper，CGFunctionWrapper不用手动编辑
 * Tools/Export CG Function To Wrapper输出的log贴在RegisterCallFunc()里
*/

/** 执行的文本文件格式是这样的   //是注释，可以空行
cgCloneMyPlayer(1)
cgWait(2.0)
cgPlayAction(1,0,6)
cgWait(1.0)
cgCreateActor(2,30,100.0,100.0,0.0,0.0)
cgCameraTowardToActor(2,2.0,3.0)
cgWait(1.0)
cgFaceToActor(2,1)
cgWait(3.0)

 * *  */

public class PlayCG : Singleton<PlayCG>
{
    //Camera Coroutine
    private Coroutine mCameraCoroutine;
    //指令字符串
    private readonly List<string> mCommands = new List<string>();
    //所有早出来的Actor
    private readonly Dictionary<int, Actor> mDictActor = new Dictionary<int, Actor>();
    //所有造出来的特效
    private readonly Dictionary<int, GameObject> mDictGo = new Dictionary<int, GameObject>();
    //脚本执行器
    public CallFunctionScript mExecutor = new CallFunctionScript();
    //根节点
    private CGLogic mLogic;
	private Dictionary<Transform,Coroutine> mMoveCoroutine = new Dictionary<Transform, Coroutine>();
    //CG播放完毕回掉函数
    private Action mPlayCallback;
    //iTween Pos
    private readonly List<Vector3> mPos = new List<Vector3>();
    //状态
    private State mState = State.Playing;
    //Wait Coroutine
    private Coroutine mWaitCoroutine;
    //是否要销毁自己，只有在播放CG之后就要切场景，这种才不需要销毁自己
    private bool mWillDestroySelf = true;

    private static string curPlayPath = "";

    /// <summary>
    ///     摄像机移动
    /// </summary>
    /// <param name="time">时间</param>
    /// <param name="p">目标位置</param>
    /// <param name="t">目标朝向</param>
    /// <returns></returns>
    private IEnumerator CameraCoroutine(float time, Vector3 pos, Vector3 forward)
    {
        if (time <= 0)
        {
            //yield return new WaitForEndOfFrame();
            mLogic.CGCamera.transform.position = pos;
            mLogic.CGCamera.transform.forward = forward;
        }
        else
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                mLogic.CGCamera.transform.position =
                    Vector3.Lerp(mLogic.CGCamera.transform.position,
                        pos,
                        Time.deltaTime/time);

                mLogic.CGCamera.transform.forward =
                    Vector3.Slerp(mLogic.CGCamera.transform.forward,
                        forward,
                        Time.deltaTime/time);

                yield return null;
            }
        }

        //ProcessCommand();
    }

    /// <summary>
    ///     挂载
    /// </summary>
    public void cgAttach(string name1, string point1, string name2, string point2)
    {
        Logger.Debug("cgAttach[{0}  {1}]  [{2}  {3}]", name1, point1, name2, point2);

        var go = Find(name1);
        if (null == go)
        {
            Logger.Debug("null == go");
            return;
        }
        var t1 = string.IsNullOrEmpty(point1) ? go.transform : go.transform.FindChildRecursive(point1);
        if (null == t1)
        {
            Logger.Debug("null == t1");
            return;
        }

        var go1 = Find(name2);
        if (null == go1)
        {
            Logger.Debug("null == go1");
            return;
        }
        var t2 = string.IsNullOrEmpty(point2) ? go1.transform : go1.transform.FindChildRecursive(point2);
        if (null == t2)
        {
            Logger.Debug("null == t2");
            return;
        }
        t2.parent = t1;
        ProcessCommand();
    }

    /// <summary>
    ///     摄像机恢复到游戏摄像机状态
    /// </summary>
    /// <param name="time">用时，0就瞬间</param>
    public void cgCameraRestore(float time = 0)
    {
        Logger.Debug("cgCameraRestore");

        var pos = GameLogic.Instance.MainCamera.transform.position;
        var dir = GameLogic.Instance.MainCamera.transform.forward;

        StartCameraCoroutine(time, pos, dir);

        ProcessCommand();
    }

    /// <summary>
    ///     摄像机朝向某个演员
    /// </summary>
    /// <param name="id"></param>
    /// <param name="heightOffset">演员脚底的偏移高度</param>
    /// <param name="time">用时，0就瞬间</param>
    public void cgCameraTowardActor(int id, float heightOffset = 1.5f, float time = 0)
    {
        Logger.Debug("cgCameraTowardActor");
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgCameraTowardActor({0})", id);
        }
        else
        {
            var lookAt = actor.Position + new Vector3(0, heightOffset, 0);
            var pos = mLogic.CGCamera.transform.position;
            var dir = lookAt - mLogic.CGCamera.transform.position;
            dir.Normalize();
            StartCameraCoroutine(time, pos, dir);
        }
        ProcessCommand();
    }

    /// <summary>
    ///     摄像机不转方向，朝着某个演员
    /// </summary>
    /// <param name="id"></param>
    /// <param name="distance">摄像机到演员脚底偏移的距离</param>
    /// <param name="heightOffset"></param>
    /// <param name="time"></param>
    public void cgCameraTowardActorWithDistance(int id, float heightOffset, float distance, float time = 0)
    {
        Logger.Debug("cgCameraTowardActorWithDistance");
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgCameraTowardActorWithDistance({0})", id);
        }
        else
        {
            var lookAt = actor.Position + new Vector3(0, heightOffset, 0);
            var pos = lookAt - mLogic.CGCamera.transform.forward*distance;
            var dir = mLogic.CGCamera.transform.forward;
            StartCameraCoroutine(time, pos, dir);
        }
        ProcessCommand();
    }

    /// <summary>
    ///     朝着某个演员有个固定的偏移
    /// </summary>
    /// <param name="id"></param>
    /// <param name="heightOffset">摄像机注视点：演员脚底高度偏移的距离</param>
    /// <param name="x">摄像机位置相对于注视点的x轴方向偏移</param>
    /// <param name="y">摄像机位置相对于注视点的y轴方向偏移</param>
    /// <param name="z">摄像机位置相对于注视点的z轴方向偏移</param>
    /// <param name="time">用时，0就瞬间</param>
    public void cgCameraTowardActorWithOffset(int id, float heightOffset, float x, float y, float z, float time = 0)
    {
        Logger.Debug("cgCameraTowardActorWithOffset");
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgCameraTowardActorWithOffset({0})", id);
        }
        else
        {
            var lookAt = actor.Position + new Vector3(0, heightOffset, 0);
            var pos = lookAt + new Vector3(x, y, z);
            var dir = -new Vector3(x, y, z);
            dir.Normalize();
            StartCameraCoroutine(time, pos, dir);
        }

        ProcessCommand();
    }

    /// <summary>
    ///     导出函数
    ///     克隆玩家主角
    /// </summary>
    /// <param name="id">id</param>
    public void cgCloneMyPlayer(int id)
    {
        Logger.Debug("CloneMyPlayer({0})", id);
        var player = ObjManager.Instance.MyPlayer;
        var dataId = player.GetDataId();
        var equip = player.EquipList;
        var obj = Actor.Create(dataId, equip, actor =>
        {
            actor.gameObject.name = "Actor_" + id.ToString();
            actor.Position = player.Position;
	        actor.GetComponent<NavMeshAgent>().Warp(actor.Position);
            actor.Direction = player.Direction;
            actor.TargetDirection = player.Direction;
            actor.transform.parent = mLogic.ObjRoot;

            mDictActor.Add(id, actor);

            ProcessCommand();
        }, LayerMask.NameToLayer(GAMELAYER.CGMainPlayer), false, true);
    }

    /// <summary>
    ///     创建一个演员
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="dataId">CharacterBase表id</param>
    /// <param name="x">坐标</param>
    /// <param name="z">坐标</param>
    /// <param name="a">朝向,角度(0~360)</param>
    public void cgCreateActor(int id, int dataId, float x, float z, float a)
    {
        Logger.Debug("CreateActor({0})", id);

        if (mDictActor.ContainsKey(id))
        {
            cgDestroyActor(id);
            Logger.Error("duplicate id[{0}]", id);
            return;
        }

        var obj = Actor.Create(dataId, null, actor =>
        {

            actor.gameObject.name = "Actor_" + id.ToString();

            var y = GameLogic.GetTerrainHeight(x, z);

            actor.Position = new Vector3(x, y, z);
            actor.Direction = new Vector3(Mathf.Sin(a/180.0f*Mathf.PI), 0, Mathf.Cos(a/180.0f*Mathf.PI));
            actor.TargetDirection = actor.Direction;
            actor.transform.parent = mLogic.ObjRoot;

            mDictActor.Add(id, actor);

            ProcessCommand();
        }, LayerMask.NameToLayer(GAMELAYER.CG));
    }

    /// <summary>
    ///     场景内造Prefab
    /// </summary>
    /// <param name="path">资源目录</param>
    /// <param name="name">名字</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void cgCreatePrefab(string path, string name, float x, float y, float z, float aX, float aY, float aZ)
    {
        //Logger.Debug("cgCreatePrefab[{0}  {1}]", name, aniName);
        ResourceManager.PrepareResource<GameObject>(path, res =>
        {
            var go = Object.Instantiate(res) as GameObject;
            go.name = name;
            go.transform.parent = mLogic.transform;
            go.transform.position = new Vector3(x, y, z);
            go.transform.forward = Quaternion.Euler(aX, aY, aZ)*Vector3.forward;

            ProcessCommand();
        });
    }

    /// <summary>
    ///     场景内删除Prefab
    /// </summary>
    public void cgDeletePrefab(string name)
    {
        Logger.Debug("cgDeletePrefab[{0}]", name);

        var t = Find(name);
        if (null == t)
        {
            Logger.Debug("cgDeletePrefab[{0}]   null == go", name);
            return;
        }
		Coroutine co = null;
		if (mMoveCoroutine.TryGetValue(t, out co))
		{
			mLogic.StopCoroutine(co);
			mMoveCoroutine.Remove(t);
		}
        Object.Destroy(t.gameObject);
        ProcessCommand();
    }

    /// <summary>
    ///     删除某个演员
    /// </summary>
    /// <param name="id"></param>
    public void cgDestroyActor(int id)
    {
        Logger.Debug("DestroyActor({0})", id);

        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgDestroyActor({0})", id);
        }
        else
        {
			Coroutine co = null;
			if (mMoveCoroutine.TryGetValue(actor.transform, out co))
			{
				mLogic.StopCoroutine(co);
				mMoveCoroutine.Remove(actor.transform);
			}
            actor.Destroy();
        }
        mDictActor.Remove(id);

        ProcessCommand();
    }

    /// <summary>
    ///     销毁特效
    /// </summary>
    /// <param name="id"></param>
    public void cgDestroyEffect(int id)
    {
        Logger.Debug("DestroyEffect[{0}]", id);
        if (mDictGo.ContainsKey(id))
        {
            Object.Destroy(mDictGo[id]);
            mDictGo.Remove(id);
        }

        ProcessCommand();
    }

    /// <summary>
    ///     演员面对某个位置
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void cgFaceTo(int id, float x, float z)
    {
        Logger.Debug("FaceTo({0})", id);
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgFaceTo({0})", id);
        }
        else
        {
            actor.FaceTo(new Vector3(x, 0, z));
        }

        ProcessCommand();
    }

    /// <summary>
    ///     朝向某个演员
    /// </summary>
    /// <param name="id"></param>
    /// <param name="targetId"></param>
    public void cgFaceToActor(int id, int targetId)
    {
        Logger.Debug("FaceToActor({0})", id);
        if (id == targetId)
        {
            Logger.Debug("Can't Face To Myself");
        }
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgFaceToActor({0})", id);
        }
        else
        {
            var targetCharacter = GetActor(targetId);
            actor.FaceTo(targetCharacter.Position);
        }

        ProcessCommand();
    }

    //-----------------------------------------以下是导出函数，导出函数要以cg为开头----------------------------------------

    /// <summary>
    ///     导出函数
    ///     淡入
    /// </summary>
    /// <param name="time">时间</param>
    public void cgFadein(float time)
    {
        Logger.Debug("cgFadein({0})", time);
        mLogic.DoFadein(time);
        ProcessCommand();
    }

    /// <summary>
    ///     导出函数
    ///     淡出
    /// </summary>
    /// <param name="time">时间</param>
    public void cgFadeout(float time)
    {
        Logger.Debug("cgFadeout({0})", time);
        mLogic.DoFadeout(time);
        ProcessCommand();
    }

    /// <summary>
    ///     触发一事件
    /// </summary>
    public void cgFireEvent(string name, float time)
    {
        Logger.Debug("cgGoFireEvent[{0}]", name);

        var go = Find(name);
        if (null == go)
        {
            Logger.Debug("cgGoFireEvent[{0}]  null == go", name);
            return;
        }

        var c = go.GetComponent<ICGPlayable>();
        if (null == c)
        {
            Logger.Debug("cgGoFireEvent[{0}]  null == c", name);
            return;
        }
        c.Play(time);
        ProcessCommand();
    }

    /// <summary>
    ///     iTween 清除位置列表
    /// </summary>
    public void cgITweenClearPos()
    {
        mPos.Clear();
        ProcessCommand();
    }

    /// <summary>
    ///     iTween开始
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="time">时间</param>
    /// <param name="orienttopath">朝向是否随着转向</param>
    public void cgITweenMoveTo(string name, float time, bool orienttopath)
    {
        var t = Find(name);
        if (null == t)
        {
            Logger.Debug("cgITweenMoveTo   null == go");
            return;
        }
        iTween.MoveTo(t.gameObject,
            iTween.Hash("path", mPos.ToArray(), "movetopath", true, "orienttopath", orienttopath, "time", time,
                "easetype", iTween.EaseType.linear));
        ProcessCommand();
    }

    /// <summary>
    ///     iTween 清除位置列表
    /// </summary>
    public void cgITweenPushPos(float x, float y, float z)
    {
        mPos.Add(new Vector3(x, y, z));
        ProcessCommand();
    }

    /// <summary>
    ///     移动
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="speed"></param>
    public void cgMove(int id, float x, float z, float speed, bool walk)
    {
        Logger.Debug("cgMove({0})", id);

        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find cgMove({0})", id);
        }
        else
        {
            actor.Move(new Vector3(x, 0, z), speed, walk);
        }
        ProcessCommand();
    }

    /// <summary>
    ///     移动到
    /// </summary>
    public void cgMoveTo(string name, float x, float y, float z, float time)
    {
        Logger.Debug("cgMoveTo[{0}]", name);

        var go = Find(name);
        if (null == go)
        {
            Logger.Debug("cgRotate[{0}]  null == go", name);
            return;
        }

        var position = new Vector3(x, y, z);
// 		if (null != mMoveCoroutine)
// 		{
// 			mLogic.StopCoroutine(mMoveCoroutine);
// 			mMoveCoroutine = null;
// 		}
	    Coroutine co = null;
	    if (mMoveCoroutine.TryGetValue(go.transform, out co))
	    {
			mLogic.StopCoroutine(co);
			mMoveCoroutine.Remove(go.transform);
	    }
		co = mLogic.StartCoroutine(PositionCoroutine(go.transform, time, position));
		mMoveCoroutine.Add(go.transform, co);

        ProcessCommand();
    }

    /// <summary>
    ///     播放动作
    /// </summary>
    /// <param name="id"></param>
    /// <param name="aniId">动作id</param>
    /// <param name="nextAniId">当前动作播放后的动作id，如果前面是循环动作就不会后面的动作id</param>
    public void cgPlayAction(int id, int aniId, int nextAniId)
    {
        Logger.Debug("PlayAction({0})", id);

        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find PlayAction({0})", id);
        }
        else
        {
            actor.PlayAnimation(aniId, aniName =>
            {
                if (-1 != nextAniId)
                {
                    actor.PlayAnimation(nextAniId);
                }
            });
        }

        ProcessCommand();
    }

    /// <summary>
    ///     让场景内的Obj播放动画
    /// </summary>
    /// <param name="name">obj名字</param>
    /// <param name="aniName">动作名字</param>
    public void cgPlayAnimation(string name, string aniName, string next, string effectRootName)
    {
        Logger.Debug("cgPlayAnimation[{0}  {1}]", name, aniName);

        var t = Find(name);
        if (null == t)
        {
            Logger.Debug("cgPlayAnimation[{0}  {1}]   null == go", name, aniName);
            return;
        }
        var go = t.gameObject;
        var ani = go.gameObject.GetComponent<Animation>();
        if (null == ani)
        {
            Logger.Debug("cgPlayAnimation[{0}  {1}]   null == ani", name, aniName);
            return;
        }

        if (!string.IsNullOrEmpty(aniName))
        {
            //ani.CrossFade(aniName, 0.2f);
            ani.CrossFade(aniName);
        }

        if (!string.IsNullOrEmpty(effectRootName))
        {
            var effect = go.transform.FindChildRecursive(effectRootName);
            if (null != effect)
            {
                effect.gameObject.SetActive(true);
                GameUtils.ResetEffect(effect.gameObject);
            }
        }


        if (!string.IsNullOrEmpty(next))
        {
            ani.CrossFadeQueued(next);
        }

        ProcessCommand();
    }

    /// <summary>
    ///     播放BGM
    /// </summary>
    public void cgPlayBGM(int soundId, float fadeout, float fadein)
    {
        Logger.Debug("cgPlayBGM[{0}]", soundId);

        SoundManager.Instance.PlayBGMusic(soundId, fadeout, fadein);

        ProcessCommand();
    }

    /// <summary>
    ///     播放特效
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type">Effect表id</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void cgPlayEffect(int id, int type, float x, float y, float z, float rX, float rY, float rZ, float scale)
    {
        Logger.Debug("PlayEffect[{0}]", id);

        var table = Table.GetEffect(type);
        if (null == table)
        {
            return;
        }

        if (mDictGo.ContainsKey(id))
        {
            Object.Destroy(mDictGo[id]);
            mDictGo.Remove(id);
        }

        ResourceManager.PrepareResource<GameObject>(table.Path, res =>
        {
            var go = Object.Instantiate(res) as GameObject;
            go.transform.parent = mLogic.transform;
            go.transform.position = new Vector3(x, y, z);
            go.transform.localScale = new Vector3(scale, scale, scale);
            go.transform.forward = Quaternion.Euler(rX, rY, rZ) * Vector3.forward;
            mDictGo.Add(id, go);

            ProcessCommand();
        });
    }

    /// <summary>
    ///     播放声音
    /// </summary>
    public void cgPlaySound(int soundId)
    {
        Logger.Debug("cgPlaySound[{0}]", soundId);
        SoundManager.Instance.PlaySoundEffect(soundId);

        ProcessCommand();
    }

    /// <summary>
    ///     播放声音
    /// </summary>
    public void cgPlaySoundByRole(int soundId1, int soundId2, int soundId3)
    {
        Logger.Debug("cgPlaySoundByRole[{0},{1},{2}]", soundId1, soundId2, soundId3);
        var role = PlayerDataManager.Instance.GetRoleId();
        var id = soundId1;
        if (1 == role)
        {
            id = soundId2;
        }
        else if (2 == role)
        {
            id = soundId3;
        }
        if (-1 != id)
        {
            SoundManager.Instance.PlaySoundEffect(id);
        }


        ProcessCommand();
    }

    /// <summary>
    ///     吐泡泡
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dictId"></param>
    /// <param name="time"></param>
    public void cgPopTalk(int id, int dictId, float time)
    {
        Logger.Debug("cgPopTalk[{0}][{1}][{2}]", id, dictId, time);
        var actor = GetActor(id);

        if (null == actor)
        {
            Logger.Error("Can't find cgPopTalk({0})", id);
        }
        else
        {
            var pop = mLogic.GetFreePop();
            var point = actor.GetMountPoint((int) MountPoint.Top);
            if (null == point)
            {
                point = actor.gameObject.transform;
            }
            var offset = (float) actor.CharModelRecord.HeadInfoHeight + (actor.HasWing() ? 0.4f : 0);
            offset /= 2;
            pop.SetOwner(point.gameObject, offset);

            var str = GameUtils.GetDictionaryText(dictId);
            var ret = ExpressionHelper.GetExpressionString(null, str);
            pop.SetText(ret, time);
        }
        ProcessCommand();
    }

    /// <summary>
    ///     播放声音
    /// </summary>
    public void cgRotate(string name, float x, float y, float z, float time)
    {
        Logger.Debug("cgRotate[{0}]", name);

        var go = Find(name);
        if (null == go)
        {
            Logger.Debug("cgRotate[{0}]  null == go", name);
            return;
        }

        var forward = Quaternion.Euler(x, y, z)*Vector3.forward;
        mLogic.StartCoroutine(DirectionCoroutine(go.transform, time, forward));

        ProcessCommand();
    }

	public void cgScale(string name, float s, float time)
	{
		Logger.Debug("cgScale[{0}]", name);

		var go = Find(name);
		if (null == go)
		{
			Logger.Debug("cgScale[{0}]  null == go", name);
			return;
		}

		var scale = Vector3.one*s;
		mLogic.StartCoroutine(ScaleCoroutine(go.transform, time, scale));

		ProcessCommand();
	}
    /// <summary>
    ///     设置摄像机位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="time">用时，0就瞬间</param>
    public void cgSetCamera(float x, float y, float z, float time)
    {
        Logger.Debug("SetCamera");

        var pos = new Vector3(x, y, z);
        var dir = mLogic.CGCamera.transform.forward;

        StartCameraCoroutine(time, pos, dir);

        ProcessCommand();
    }

    /// <summary>
    ///     设置设想位置和欧拉角
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="aX"></param>
    /// <param name="aY"></param>
    /// <param name="aZ"></param>
    /// <param name="time">用时，0就瞬间</param>
    public void cgSetCameraEx(float x, float y, float z, float aX, float aY, float aZ, float time)
    {
        Logger.Debug("cgSetCameraEx");
        var pos = new Vector3(x, y, z);
        var forward = Quaternion.Euler(aX, aY, aZ)*Vector3.forward;

        StartCameraCoroutine(time, pos, forward);

        ProcessCommand();
    }

    /// <summary>
    ///     设置位置和朝向
    /// </summary>
    /// <param name="id"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="a">朝向,角度(0~360)</param>
    public void cgSetPosDir(int id, float x, float z, float a)
    {
        Logger.Debug("cgSetPosDir({0})", id);
        var actor = GetActor(id);
        if (null == actor)
        {
            Logger.Error("Can't find PlayAction({0})", id);
        }
        else
        {
			var pos = new Vector3(x, GameLogic.GetTerrainHeight(x, z), z);
	        actor.Position = pos;
			actor.GetComponent<NavMeshAgent>().Warp(pos);
            actor.Direction = new Vector3(Mathf.Sin(a/180.0f*Mathf.PI), 0, Mathf.Cos(a/180.0f*Mathf.PI));
            actor.TargetDirection = actor.Direction;
        }
        ProcessCommand();
    }

    /// <summary>
    ///     是否同步朝向
    /// </summary>
    public void cgSetSyncActorDirection(int id, bool flag)
    {
        var actor = GetActor(id);

        if (null == actor)
        {
            Logger.Error("Can't find cgPopTalk({0})", id);
        }
        else
        {
            actor.SyncDirectionFlag = flag;
        }
        ProcessCommand();
    }

    /// <summary>
    ///     播放台词
    /// </summary>
    /// <param name="dialog">obj名字</param>
    public void cgShowDialog(int dictId, float time)
    {
        var str = GameUtils.GetDictionaryText(dictId);
        var ret = ExpressionHelper.GetExpressionString(null, str);
        mLogic.ShowDialog(ret, time);
        ProcessCommand();
    }

    /*
	public void cgPlayAnimation(string name, string path1, string aniName, string path2, string next)
	{
		Logger.Debug("cgPlayAnimation[{0}  {1}]", name, aniName);

		var go = Find(name);
		if (null == go)
		{
			Logger.Debug("cgPlayAnimation[{0}  {1}]   null == go", name, aniName);
			return;
		}

		var ani = go.GetComponent<Animation>();
		if (null == ani)
		{
			ani = go.AddComponent<Animation>();
		}

		if (null == ani.GetClip(aniName))
		{
			ResourceManager.PrepareResource<AnimationClip>(path1, (clip1) =>
			{
				ani.AddClip(clip1, aniName);
				ani.CrossFade(aniName, 0.2f);
			});
		}
		else
		{
			ani.CrossFade(aniName, 0.2f);	
		}


		if (!string.IsNullOrEmpty(next))
		{
			if (null == ani.GetClip(next))
			{
				ResourceManager.PrepareResource<AnimationClip>(path2, (clip2) =>
				{
					ani.AddClip(clip2, next);
					ani.CrossFade(next, 0.2f);
				});
			}
			else
			{
				ani.CrossFade(aniName, 0.2f);
			}

			ani.CrossFadeQueued(next);	
		}

		ProcessCommand();
	}
	*/

    /// <summary>
    ///     停止动画
    /// </summary>
    public void cgStopAnimation(string name)
    {
        Logger.Debug("cgStopAnimation[{0}]", name);

        var go = Find(name);
        if (null == go)
        {
            Logger.Debug("null == go");
            return;
        }

        var ani = go.GetComponent<Animation>();
        if (null == ani)
        {
            Logger.Debug("cgStopAnimation[{0}]   null == ani", name);
            return;
        }

        ani.Stop();
        ProcessCommand();
    }

    /// <summary>
    ///     等待多长时间
    /// </summary>
    /// <param name="time"></param>
    public void cgWait(float time)
    {
        Logger.Debug("Wait({0})", time);

        if (null != mWaitCoroutine)
        {
            mLogic.StopCoroutine(mWaitCoroutine);
        }
        mWaitCoroutine = mLogic.StartCoroutine(WaitCoroutine(time));
    }

	/// <summary>
	///     等待多长时间
	/// </summary>
	/// <param name="time"></param>
	public void cgCameraBezeir(float time,float x1, float y1, float z1, float x2, float y2, float z2,bool orientToPath)
	{
		StopCamera();
		var paths = new Vector3[2];
		//paths[0] = mLogic.CGCamera.transform.position;
		paths[0] = new Vector3(x1,y1,z1);
		paths[1] = new Vector3(x2, y2, z2);
		iTween.MoveTo(mLogic.CGCamera.gameObject,
		iTween.Hash("path", paths, 
		"movetopath", true,
		"orienttopath", orientToPath,
		"time", time,
		"easetype",iTween.EaseType.linear));

		ProcessCommand();
	}

	public void cgCameraBezeirLookAt(float time, float x1, float y1, float z1, float x2, float y2, float z2, float tx, float ty, float tz)
	{
		StopCamera();
		var paths = new Vector3[2];
		//paths[0] = mLogic.CGCamera.transform.position;
		paths[0] = new Vector3(x1, y1, z1);
		paths[1] = new Vector3(x2, y2, z2);
		iTween.MoveTo(mLogic.CGCamera.gameObject,
		iTween.Hash("path", paths,
		"movetopath", true,
		"looktarget", new Vector3(tx,ty,tz),
		"time", time,
		"easetype", iTween.EaseType.linear));

		ProcessCommand();
	}

    //销毁当前的CG
    public void Destroy()
    {
        mDictActor.Clear();
        mDictGo.Clear();
        mLogic = null;
        var root = GameObject.Find("CGRoot");
        if (null != root)
        {
            Object.Destroy(root);
        }
    }

    /// <summary>
    ///     转向Coroutine
    /// </summary>
    private IEnumerator DirectionCoroutine(Transform xform, float time, Vector3 forward)
    {
        if (time <= 0)
        {
            xform.forward = forward;
        }
        else
        {
            while (time > 0)
            {
                time -= Time.deltaTime;

                xform.forward =
                    Vector3.Slerp(xform.forward,
                        forward,
                        Time.deltaTime/time);

                yield return null;
            }
        }
    }

	private IEnumerator ScaleCoroutine(Transform xform, float time, Vector3 scale)
	{
		if (time <= 0)
		{
			xform.localScale = scale;
		}
		else
		{
			while (time > 0)
			{
				time -= Time.deltaTime;

				xform.localScale =
					Vector3.Slerp(xform.localScale,
						scale,
						Time.deltaTime / time);

				yield return null;
			}
		}
	}
    private Transform Find(string name)
    {
        Transform t = null;
        if (null != mLogic)
        {
            t = mLogic.transform.FindChild(name);
        }
        if (null == t)
        {
            var go = GameObject.Find(name);
            if (null != go)
            {
                return go.transform;
            }
        }
        return t;
    }

    //获得Actor
    private Actor GetActor(int id)
    {
        Actor actor;
        if (mDictActor.TryGetValue(id, out actor))
        {
            return actor;
        }
        return null;
    }

    //初始化
    public bool Init()
    {
        RegisterCallFunc();
        Reset();
        return true;
    }

    public static void PlayById(int id, Action callback = null, bool willDestroy = true)
    {
        var table = Table.GetStory(id);
        if (null != table)
        {
            if (0 == table.AnimType)
            {
				PlayMovie.Play(table.Path, callback, 1 == table.IsPassAnimation);
            }
            else
            {
                Instance.PlayCGFile(table.Path, callback , 1 == table.IsPassAnimation, willDestroy);
            }
            curPlayPath = table.Path;
            PlatformHelper.UMEvent("PlayCG", "play", table.Path);
        }
    }

    //播放CG
    public void PlayCGFile(string path, Action callback = null, bool skipable = true, bool willDestroy = true)
    {
        Reset();
        mWillDestroySelf = willDestroy;
        //读取资源
        var asset = ResourceManager.PrepareResourceSync<TextAsset>(path);
        if (null == asset)
        {
            Logger.Error("PlayCGFile null == res|path[{0}]", path);
            return;
        }

        //转换成字符串
        var source = Encoding.Default.GetString(asset.bytes);
        if (string.IsNullOrEmpty(source))
        {
            Logger.Error("string.IsNullOrEmpty(source)");
            return;
        }

        mState = State.Playing;

        mPlayCallback = callback;

        //清除指令
        mCommands.Clear();

        //初步处理指令字符串
        var temp = source.Split('\n');
        for (var i = 0; i < temp.Length; i++)
        {
            if (string.IsNullOrEmpty(temp[i]))
            {
                continue;
            }
            if (temp[i].Equals("\r"))
            {
                continue;
            }
            if (temp[i].Contains("//"))
            {
                continue;
            }
            mCommands.Add(temp[i]);
        }

        //加载prefab
        var res = ResourceManager.PrepareResourceSync<GameObject>("UI/CG/CGRoot"); //, (res) =>
        {
            //对当前游戏进行屏蔽
            Camera camera = null;
            if (null != GameLogic.Instance)
            {
                InputManager.Instance.enabled = false;
                GameLogic.Instance.MainCamera.active = false;
                //UIManager.Instance.UIRoot.active = false;
                //SoundManager.Instance.active = false;

                camera = GameLogic.Instance.MainCamera;
            }
            else
            {
                var cameraGo = GameObject.Find("Main Camera");
                if (null != cameraGo)
                {
                    camera = cameraGo.GetComponent<Camera>();
                }
            }


            var go = Object.Instantiate(res) as GameObject;
            go.name = "CGRoot";
            mLogic = go.GetComponent<CGLogic>();

            mLogic.SkipBtn.gameObject.SetActive(skipable);

            //先把 CG摄像机和游戏摄像机重合
            if (null != camera)
            {
                mLogic.CGCamera.transform.position = camera.transform.position;
                mLogic.CGCamera.transform.rotation = camera.transform.rotation;
                mLogic.CGCamera.fieldOfView = camera.fieldOfView;
                mLogic.CGCamera.farClipPlane = camera.farClipPlane;
                mLogic.CGCamera.nearClipPlane = camera.nearClipPlane;
            }


            //开始执行指令
            ProcessCommand();
        }
        //);

        curPlayPath = path;
    }

    /// <summary>
    ///     转向Coroutine
    /// </summary>
    private IEnumerator PositionCoroutine(Transform xform, float time, Vector3 position)
    {
        if (time <= 0)
        {
            xform.position = position;
        }
        else
        {
            while (time > 0)
            {
                time -= Time.deltaTime;

                xform.position =
                    Vector3.Lerp(xform.position,
                        position,
                        Time.deltaTime/time);

                yield return null;
            }
        }
    }

    //处理指令
    private bool ProcessCommand()
    {
        if (State.End == mState)
        {
            return false;
        }


        while (mCommands.Count > 0)
        {
            var code = mCommands[0];
            mCommands.RemoveAt(0);
            mExecutor.Execute(code);

            return false;
        }

        //mLogic.Skip();
        Stop();
        return true;
    }

    //注册所有函数
    public void RegisterCallFunc()
    {
        mExecutor.UnRgisterAllFunction();

		mExecutor.RegisterFunction("cgAttach", CGFunctionWrapper.cgAttach);
		mExecutor.RegisterFunction("cgCameraRestore", CGFunctionWrapper.cgCameraRestore);
		mExecutor.RegisterFunction("cgCameraTowardActor", CGFunctionWrapper.cgCameraTowardActor);
		mExecutor.RegisterFunction("cgCameraTowardActorWithDistance", CGFunctionWrapper.cgCameraTowardActorWithDistance);
		mExecutor.RegisterFunction("cgCameraTowardActorWithOffset", CGFunctionWrapper.cgCameraTowardActorWithOffset);
		mExecutor.RegisterFunction("cgCloneMyPlayer", CGFunctionWrapper.cgCloneMyPlayer);
		mExecutor.RegisterFunction("cgCreateActor", CGFunctionWrapper.cgCreateActor);
		mExecutor.RegisterFunction("cgCreatePrefab", CGFunctionWrapper.cgCreatePrefab);
		mExecutor.RegisterFunction("cgDeletePrefab", CGFunctionWrapper.cgDeletePrefab);
		mExecutor.RegisterFunction("cgDestroyActor", CGFunctionWrapper.cgDestroyActor);
		mExecutor.RegisterFunction("cgDestroyEffect", CGFunctionWrapper.cgDestroyEffect);
		mExecutor.RegisterFunction("cgFaceTo", CGFunctionWrapper.cgFaceTo);
		mExecutor.RegisterFunction("cgFaceToActor", CGFunctionWrapper.cgFaceToActor);
		mExecutor.RegisterFunction("cgFadein", CGFunctionWrapper.cgFadein);
		mExecutor.RegisterFunction("cgFadeout", CGFunctionWrapper.cgFadeout);
		mExecutor.RegisterFunction("cgFireEvent", CGFunctionWrapper.cgFireEvent);
		mExecutor.RegisterFunction("cgITweenClearPos", CGFunctionWrapper.cgITweenClearPos);
		mExecutor.RegisterFunction("cgITweenMoveTo", CGFunctionWrapper.cgITweenMoveTo);
		mExecutor.RegisterFunction("cgITweenPushPos", CGFunctionWrapper.cgITweenPushPos);
		mExecutor.RegisterFunction("cgMove", CGFunctionWrapper.cgMove);
		mExecutor.RegisterFunction("cgMoveTo", CGFunctionWrapper.cgMoveTo);
		mExecutor.RegisterFunction("cgPlayAction", CGFunctionWrapper.cgPlayAction);
		mExecutor.RegisterFunction("cgPlayAnimation", CGFunctionWrapper.cgPlayAnimation);
		mExecutor.RegisterFunction("cgPlayBGM", CGFunctionWrapper.cgPlayBGM);
		mExecutor.RegisterFunction("cgPlayEffect", CGFunctionWrapper.cgPlayEffect);
		mExecutor.RegisterFunction("cgPlaySound", CGFunctionWrapper.cgPlaySound);
		mExecutor.RegisterFunction("cgPlaySoundByRole", CGFunctionWrapper.cgPlaySoundByRole);
		mExecutor.RegisterFunction("cgPopTalk", CGFunctionWrapper.cgPopTalk);
		mExecutor.RegisterFunction("cgRotate", CGFunctionWrapper.cgRotate);
		mExecutor.RegisterFunction("cgScale", CGFunctionWrapper.cgScale);
		mExecutor.RegisterFunction("cgSetCamera", CGFunctionWrapper.cgSetCamera);
		mExecutor.RegisterFunction("cgSetCameraEx", CGFunctionWrapper.cgSetCameraEx);
		mExecutor.RegisterFunction("cgSetPosDir", CGFunctionWrapper.cgSetPosDir);
		mExecutor.RegisterFunction("cgSetSyncActorDirection", CGFunctionWrapper.cgSetSyncActorDirection);
		mExecutor.RegisterFunction("cgShowDialog", CGFunctionWrapper.cgShowDialog);
		mExecutor.RegisterFunction("cgStopAnimation", CGFunctionWrapper.cgStopAnimation);
		mExecutor.RegisterFunction("cgWait", CGFunctionWrapper.cgWait);
		mExecutor.RegisterFunction("cgCameraBezeir", CGFunctionWrapper.cgCameraBezeir);
		mExecutor.RegisterFunction("cgCameraBezeirLookAt", CGFunctionWrapper.cgCameraBezeirLookAt);




    }

    //重置
    public void Reset()
    {
        //mCommands.Clear();
        {
            // foreach(var pair in mDictActor)
            var __enumerator1 = (mDictActor).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var pair = __enumerator1.Current;
                {
                    pair.Value.Destroy();
                }
            }
        }
        mDictActor.Clear();
        mDictGo.Clear();
        if (null != mLogic)
        {
            //mLogic.StopAllCoroutines();
            if (mWillDestroySelf)
            {
                Object.Destroy(mLogic.gameObject);
            }
        }
        mLogic = null;
    }

    /// <summary>
    ///     开启摄像机Coroutine
    /// </summary>
    /// <param name="time"></param>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    private void StartCameraCoroutine(float time, Vector3 pos, Vector3 dir)
    {
	    StopCamera();
        mCameraCoroutine = mLogic.StartCoroutine(CameraCoroutine(time, pos, dir));
    }

	private void StopCamera()
	{
		if (null != mCameraCoroutine)
		{
			mLogic.StopCoroutine(mCameraCoroutine);
			mCameraCoroutine = null;
		}
		iTween.Stop(mLogic.CGCamera.gameObject);
	}
    //停止，可以外面掉
    public void Stop(bool isSkip=false)
    {
        Reset();
        mState = State.End;
        if (null != GameLogic.Instance)
        {
            GameLogic.Instance.MainCamera.active = true;
            UIManager.Instance.UIRoot.active = true;
            InputManager.Instance.enabled = true;
            //SoundManager.Instance.active = true;    
        }


        if (null != mPlayCallback)
        {
            mPlayCallback();
            mPlayCallback = null;
        }
        if (isSkip)
        {
            PlatformHelper.UMEvent("PlayCG", "stop", curPlayPath);
        }
        else
        {
            PlatformHelper.UMEvent("PlayCG", "over", curPlayPath);
        }
        
        curPlayPath = "";
    }

    /// <summary>
    ///     等待的Coroutinue
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator WaitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        ProcessCommand();
    }

    private enum State
    {
        Playing = 0,
        End
    }
}