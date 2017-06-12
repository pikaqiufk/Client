#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ObjCommand;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class MieShiSceneMapController : IControllerBase
{
    public MieShiSceneMapDataModel DataModel { get; set; }
    private float MAP_HIGHT = 512;
    private bool mIsDrawPath;
    private SceneRecord mSceneRecord;
    private float requestTime;
    private float requestDelta = 1.0f;
    private Dictionary<ulong, MapRadarDataModel> dataModelsDict = new Dictionary<ulong, MapRadarDataModel>();

    public MieShiSceneMapController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(RefresSceneMap.EVENT_TYPE, OnRefresSceneMap);
        EventDispatcher.Instance.AddEventListener(Postion_Change_Event.EVENT_TYPE, OnPostionChange);
        EventDispatcher.Instance.AddEventListener(MieShiMapSceneClickLoction.EVENT_TYPE, OnMapSceneClickLoction);
        EventDispatcher.Instance.AddEventListener(MapSceneDrawPath.EVENT_TYPE, OnMapSceneDrawPath);
        EventDispatcher.Instance.AddEventListener(MapSceneCancelPath.EVENT_TYPE, OnMapSceneCancelPath);
    }

    public Vector3 ConvertMapToScene(Vector3 loc)
    {
        var x = loc.x/DataModel.MapWidth*mSceneRecord.TerrainHeightMapWidth + mSceneRecord.TerrainHeightMapWidth/2.0f;
        var z = loc.y/DataModel.MapHeight*mSceneRecord.TerrainHeightMapLength + mSceneRecord.TerrainHeightMapLength/2.0f;

        return new Vector3(x, 0, z);
    }

    public Vector3 ConvertSceneToMap(Vector3 loc)
    {
        var x = (loc.x - mSceneRecord.TerrainHeightMapWidth / 2.0f) / mSceneRecord.TerrainHeightMapWidth * DataModel.MapWidth;
        var y = (loc.z - mSceneRecord.TerrainHeightMapLength / 2.0f) / mSceneRecord.TerrainHeightMapLength *
                DataModel.MapHeight;

        return new Vector3(x, y, 0);
    }

    public void DrawPathLoction(List<Vector3> pos)
    {
        var __enumerator1 = (DataModel.PathList).GetEnumerator();
        while (__enumerator1.MoveNext())
        {
            var model = __enumerator1.Current;
            {
                model.IsShow = false;
            }
        }

        DataModel.PathList.Clear();


        pos.Insert(0, ObjManager.Instance.MyPlayer.Position);

        var target = GetPointsOnLines(pos, 10.0f);
        {
            var __list2 = target;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var vector3 = __list2[__i2];
                {
                    var pathData = new ScenePathDataModel();
                    pathData.Loction = vector3;
                    DataModel.PathList.Add(pathData);
                }
            }
        }
    }

    public void OnRefresSceneMap(int scendId)
    {
        if (!SceneManager.Instance.isInMieshiFuben())
        {
            ClearMonsterList();
            return;
        }

        DataModel.SceneId = scendId;
        mSceneRecord = Table.GetScene(scendId);
        if (mSceneRecord == null)
        {
            return;
        }

        var scale = 1f * mSceneRecord.TerrainHeightMapWidth / mSceneRecord.TerrainHeightMapLength;
        if (scale > 1.0f)
        {
            DataModel.MapWidth = (int)(MAP_HIGHT);
            DataModel.MapHeight = (int)(MAP_HIGHT / scale);
        }
        else
        {
            DataModel.MapWidth = (int)(MAP_HIGHT * scale);
            DataModel.MapHeight = (int)MAP_HIGHT;
        }

        var obj = ObjManager.Instance.MyPlayer;
        if (obj)
        {
            PostionChange(obj.Position);
        }
    }

    public void DrawTargetPathLoction(Vector3 point, float offset = 0.05f)
    {
        if (!ObjManager.Instance.MyPlayer)
        {
            return;
        }

        var tagetPos = ObjManager.Instance.MyPlayer.CalculatePath(point, offset);
        if (tagetPos.Count == 0)
        {
            mIsDrawPath = false;
            return;
        }
        DrawPathLoction(tagetPos);
    }

    public void FlyTo(float x, float y, int sceneId = -1)
    {
        if (sceneId == -1)
        {
            sceneId = GameLogic.Instance.Scene.SceneTypeId;
        }
        if (sceneId == GameLogic.Instance.Scene.SceneTypeId)
        {
            var vec = new Vector3(x, 0, y);
            vec.y = GameLogic.GetTerrainHeight(x, y);
            var path = new NavMeshPath();
            NavMesh.CalculatePath(ObjManager.Instance.MyPlayer.Position, vec, -1, path);
            if (path.corners.Length <= 0)
            {
                //目标地点不能到达
                var e = new ShowUIHintBoard(270116);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }
        GameUtils.FlyTo(sceneId, x, y);
    }

    public List<Vector3> GetPointsOnLines(List<Vector3> l, float d)
    {
        var result = new List<Vector3>();
        var dist = 0.0f;
        var lCount0 = l.Count - 1;
        for (var i = 0; i < lCount0; i++)
        {
            var s = ConvertSceneToMap(l[i]);
            var e = ConvertSceneToMap(l[i + 1]);
            var dir = (e - s).normalized;
            while ((e - s).magnitude + dist > d)
            {
                s = s + dir*(d - dist);
                dist = 0;
                result.Add(s);
            }

            dist += (e - s).magnitude;
        }
        return result;
    }

    public IEnumerator Goto(int sceneid)
    {
        using (new BlockingLayerHelper(0))
        {
            var table = Table.GetScene(sceneid);
            var msg = NetManager.Instance.ChangeSceneRequest(sceneid);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (null != SceneManager.Instance)
                    {
                        var message = string.Format(GameUtils.GetDictionaryText(210204), table.Name, table.ConsumeMoney);
                        PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold -= table.ConsumeMoney;
                        SceneManager.Instance.ChangeSceneOverMessage = message;
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void OnBtnTranfer(IEvent ievent)
    {
        var e = ievent as UIEvent_SceneMap_BtnTranfer;

        var sceneRecord = Table.GetScene(e.SceneId);
        if (sceneRecord == null)
        {
            return;
        }
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold < sceneRecord.ConsumeMoney)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210203));
            return;
        }
        if (PlayerDataManager.Instance.GetLevel() < sceneRecord.LevelLimit)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000108));
            return;
        }
        if (sceneRecord.IsPublic != 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005011));
            return;
        }
        if (GameLogic.Instance.Scene != null)
        {
            if (GameLogic.Instance.Scene.SceneTypeId == e.SceneId)
            {
                return;
            }
        }
    }

    public void OnMapSceneCancelPath(IEvent ievent)
    {
        DataModel.PathList.Clear();
        mIsDrawPath = false;
    }

    public void OnMapSceneClickLoction(IEvent ievent)
    {
        var e = ievent as MieShiMapSceneClickLoction;
        var loc = ConvertMapToScene(e.Loction);
        var isVip = false;
        if (isVip)
        {
            FlyTo(loc.x, loc.z);
            return;
        }

        if (ObjManager.Instance.MyPlayer.Dead)
        {
            return;
        }

        if (ObjManager.Instance.MyPlayer.MoveTo(loc))
        {
            ObjManager.Instance.MyPlayer.LeaveAutoCombat();
            mIsDrawPath = true;
            DrawTargetPathLoction(loc);
        }
        else
        {
            DataModel.PathList.Clear();
            mIsDrawPath = false;
        }
    }

    public void OnMapSceneDrawPath(IEvent ievent)
    {
        if (mSceneRecord == null || State != FrameState.Open)
            return;

        var e = ievent as MapSceneDrawPath;

        mIsDrawPath = true;
        DrawTargetPathLoction(e.Postion, e.Offset);
    }

    private IEnumerator RequestNpcPosCorotion()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GetSceneNpcPos(1);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                requestTime = 0.0f;
                RefreshMonsterList(msg.Response);
            }
        }
    }

    public void OnPostionChange(IEvent ievent)
    {
        var e = ievent as Postion_Change_Event;
        PostionChange(e.Loction);
    }

    public void OnPropertyChangeToggle(object sender, PropertyChangedEventArgs e)
    {
    }

    public void OnRefresSceneMap(IEvent ievent)
    {
        var e = ievent as RefresSceneMap;
        OnRefresSceneMap(e.SceneId);
    }

    public void PostionChange(Vector3 objLoction)
    {
        if (mSceneRecord == null)
            return;

        if (ObjManager.Instance.MyPlayer == null
            || ObjManager.Instance.MyPlayer.ObjTransform == null)
        {
            return;
        }
        var v = ObjManager.Instance.MyPlayer.ObjTransform.eulerAngles;
        DataModel.PalyerLocX = (int)objLoction.x;
        DataModel.PalyerLocY = (int)objLoction.z;
        DataModel.SelfMapLoction = ConvertSceneToMap(objLoction);
        DataModel.SelfMapRotation = new Vector3(0, 0, -v.y - 45f);
        if (DataModel.PathList.Count > 0)
        {
            if (ObjManager.Instance.MyPlayer.Dead || (!ObjManager.Instance.MyPlayer.IsMoving()))
            {
                mIsDrawPath = false;
                DataModel.PathList.Clear();
                return;
            }

            var start = false;
            for (var i = DataModel.PathList.Count - 1; i >= 0; i--)
            {
                var pathDataModel = DataModel.PathList[i];

                if (start)
                {
                    pathDataModel.IsShow = false;
                    DataModel.PathList.Remove(pathDataModel);
                }
                else
                {
                    if ((pathDataModel.Loction - DataModel.SelfMapLoction).sqrMagnitude < 20.0f)
                    {
                        pathDataModel.IsShow = false;
                        DataModel.PathList.Remove(pathDataModel);
                        start = true;
                    }
                }
            }
        }
        if (DataModel.PathList.Count == 0 && mIsDrawPath)
        {
            mIsDrawPath = false;
        }
    }

    public void RefreshMonsterList(SceneNpcPosList posList)
    {
        var enumorator1 = DataModel.CharaModels.GetEnumerator();
        while (enumorator1.MoveNext())
        {
            var data = enumorator1.Current;
            if (data != null)
            {
                data.CharType = -1;
            }
        }

        if (posList != null)
        {
            var enumorator4 = posList.NpcIdPosList.GetEnumerator();
            while (enumorator4.MoveNext())
            {
                var pos = enumorator4.Current;
                if (pos != null)
                {
                    CreateMinimapCharacter(new Vector3(pos.PosX, 0, pos.PosY), pos.type, pos.Id, pos.npcId);
                }
            }

            var playerId = ObjManager.Instance.MyPlayer.GetObjId();
            var enumorator = posList.NpcList.GetEnumerator();
            while (enumorator.MoveNext())
            {
                var pos = enumorator.Current;
                if (pos != null)
                {
                    if (playerId != pos.Id)
                    {
                        CreateMinimapCharacter(new Vector3(pos.PosX, 0, pos.PosY), pos.type, pos.Id, -1);
                    }
                }
            }
        }

        var enumorator3 = DataModel.CharaModels.GetEnumerator();
        while (enumorator3.MoveNext())
        {
            var data = enumorator3.Current;
            if (data != null && data.CharType == -1)
            {
                dataModelsDict.Remove(data.CharacterId);
                var e1 = new MieShiSceneMapRemoveRadar(data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
        }

        DataModel.CharaModels.RemoveAll(gc => gc.CharType == -1);
    }

    public void ClearMonsterList()
    {
        var enumorator = DataModel.CharaModels.GetEnumerator();
        while (enumorator.MoveNext())
        {
            var pos = enumorator.Current;

            var e1 = new MieShiSceneMapRemoveRadar(pos.CharacterId);
            EventDispatcher.Instance.DispatchEvent(e1);
        }

        DataModel.CharaModels.Clear();
        dataModelsDict.Clear();
    }

    public void CleanUp()
    {
        DataModel = new MieShiSceneMapDataModel();
    }


    public void OnRemoveCharacter(IEvent ievent)
    {
        var e = ievent as Character_Remove_Event;
        if (e != null)
        {
            var charId = e.CharacterId;
            RemoveMinimapCharacter(charId);
        }
    }

    private void RemoveMinimapCharacter(ulong charId)
    {
        MapRadarDataModel dataModel;
        if (dataModelsDict.TryGetValue(charId, out dataModel))
        {
            var e1 = new MieShiSceneMapRemoveRadar(charId);
            EventDispatcher.Instance.DispatchEvent(e1);
            DataModel.CharaModels.Remove(dataModel);
        }
        else
        {
            Logger.Error("RemoveMinimapCharacter charId not exit!");
        }
    }

    private void CreateMinimapCharacter(Vector3 pos, int type, ulong id, int npcId)
    {
        MapRadarDataModel radarDataModel;
        if (!dataModelsDict.TryGetValue(id, out radarDataModel))
        {
            radarDataModel = new MapRadarDataModel();
            radarDataModel.CharacterId = id;
            radarDataModel.CharType = 0;
            radarDataModel.Name = "";
            if (npcId != -1)
            {
                var tbNpc = Table.GetNpcBase(npcId);
                if (tbNpc != null)
                {
                    radarDataModel.Name = tbNpc.Name;
                }

                var mapTrans = SceneManager.Instance.GetMapTransferList(GameLogic.Instance.Scene.SceneTypeId);
                var enumerator = mapTrans.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null && enumerator.Current.NpcID == npcId)
                    {
                        radarDataModel.Pos = new Vector3(enumerator.Current.OffsetX, enumerator.Current.OffsetY, 0.0f);
                        break;
                    }
                }
            }

            var prefab = "";
            if (type == 0)
            { // player
                prefab = "UI/SceneMap/MieShiMapPlayer.prefab";
            }
            else if (type == 1)
            { // 炮塔
                prefab = "UI/SceneMap/MieShiMapPaoTa.prefab";
            }
            else if (type == 2)
            { // 圣坛
                prefab = "UI/SceneMap/MieShiMapShengTan.prefab";
            }
            else if (type == 3)
            { // Boss
                prefab = "UI/SceneMap/MieShiMapBOSS.prefab";
            }
            else if (type == 4)
            { // monster
                prefab = "UI/SceneMap/MieShiMapMonster.prefab";
            }
            else if (type == 5)
            { // 宝箱
                prefab = "UI/SceneMap/MieShiMapBaoXiang.prefab";            
            }

            dataModelsDict[id] = radarDataModel;

            DataModel.CharaModels.Add(radarDataModel);
            var e1 = new MieShiSceneMapRadar(radarDataModel, 1, prefab);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
        else
        {
            radarDataModel.CharType = 0;
        }

        radarDataModel.Loction = ConvertSceneToMap(pos);
    }

    public Vector3 RotaVector3(Vector3 start, Vector3 axit, float angle)
    {
        var aaa =
            new Vector3(
                (start.x - axit.x) * Mathf.Cos(angle * Mathf.PI / 180) -
                (start.y - axit.y) * Mathf.Sin(angle * Mathf.PI / 180)
                + axit.x,
                (start.x - axit.x) * Mathf.Sin(angle * Mathf.PI / 180)
                + (start.y -
                   axit.y)
                * Mathf.Cos(angle * Mathf.PI / 180) + axit.y);

        return aaa;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "ConvertSceneToMap")
        {
            var loc = (Vector3) param[0];
            return ConvertSceneToMap(loc);
        }
        return null;
    }

    public void OnShow()
    {
        var e = new SceneMapNotifyTeam(true);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void Close()
    {
        var e = new SceneMapNotifyTeam(false);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void Tick()
    {
        if (State == FrameState.Open)
        {
            requestTime += Time.deltaTime;
            if (requestTime >= requestDelta)
            {
                NetManager.Instance.StartCoroutine(RequestNpcPosCorotion());
                requestTime = 0.0f;
            }
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        {
            // foreach(var model in DataModel.PathList)
            var __enumerator5 = (DataModel.PathList).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var model = __enumerator5.Current;
                {
                    model.IsShow = false;
                }
            }
        }
        DataModel.PathList.Clear();

        if (mIsDrawPath)
        {
            var target = ObjManager.Instance.MyPlayer.TargetPos;
            DrawPathLoction(target);
        }

        requestTime = requestDelta;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}