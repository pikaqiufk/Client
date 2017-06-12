#region using

using System;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

// 人物换装
public class ActorAvatar : MonoBehaviour
{
    protected Dictionary<string, AvatarInfo> _avatarInfo = new Dictionary<string, AvatarInfo>(); // 换装信息

    private readonly Queue<KeyValuePair<string, WaitingResource>> _avatarLoadQueue =
        new Queue<KeyValuePair<string, WaitingResource>>();

    protected GameObject _body; // 基础模型动画
    protected int _bodyModelId;
    protected int _loading;
    private readonly Dictionary<int, List<GameObject>> _parts = new Dictionary<int, List<GameObject>>();
    public string BodyModel;
    public string ChestModel;
    public string FootModel;
    public string HandModel;
    public string HeadModel;
    public string LegModel;
    private int mRenderQueue = -1;
    private readonly Dictionary<int, string> mWeaponModels = new Dictionary<int, string>();
    public GameObject WingGameObject;

    public GameObject Body
    {
        get { return _body; }
        set
        {
            if (_body == value)
            {
                return;
            }

            _body = value;
            if (_body)
            {
                _body.SetRenderQueue(mRenderQueue);
            }
        }
    }

    public bool IsMainPlayer { get; set; }
    public int Layer { get; set; }
    public int LayerMask { get; set; }

    public int RenderQueue
    {
        get { return mRenderQueue; }
        set { mRenderQueue = value; }
    }

    private void AddEffect(Transform mp, GameObject obj, Vector3 p, Quaternion q, int part)
    {
        if (_body == null)
        {
            return;
        }

        var parent = mp;
        var o = obj;
        var objTransform = o.transform;
        objTransform.parent = parent;
        objTransform.localPosition = p;
        objTransform.localRotation = q;
        objTransform.localScale = Vector3.one;
        o.SetLayerRecursive(Layer, LayerMask);
        o.SetRenderQueue(mRenderQueue);
        _parts[part].Add(o);
    }

    // 给人物换装
    public void ChangePart(string part, string model, EquipModelViewRecord record, bool sync)
    {
        SetPartModel(part, model);

        // 如果还没有加载完基础模型，则等待
        if (_body == null || _loading > 0)
        {
            _avatarLoadQueue.Enqueue(new KeyValuePair<string, WaitingResource>(part,
                new WaitingResource {ModelPath = model, ModelViewRecord = record}));
            return;
        }

        //AvatarData adata = DataMgr.Instance.GetAvatarData(avatarId);
        _loading++;
        ComplexObjectPool.NewObject(model, obj =>
        {
            _loading--;

            if (GetPartModel(part) != model)
            {
                ComplexObjectPool.Release(obj);
                TryContinue();
                return;
            }

            if (null != obj)
            {
                ChangePart(obj, part, record, sync);
            }
            TryContinue();
        }, null, null, sync);
    }

    // 替换部件
    private void ChangePart(GameObject avatarModel, string partName, EquipModelViewRecord record, bool sync)
    {
        // 先卸载当前部件
        AvatarInfo currentInfo;
        if (_avatarInfo.TryGetValue(partName, out currentInfo))
        {
            if (currentInfo.avatarPart != null)
            {
                Destroy(currentInfo.avatarPart);
                currentInfo.avatarPart = null;
            }

            if (currentInfo.defaultPart != null)
            {
                currentInfo.defaultPart.SetActive(true);
            }
        }

        // avatarModel是一个resource，并没有实例化
        if (avatarModel == null)
        {
            return;
        }

        if (_body == null)
        {
            ComplexObjectPool.Release(avatarModel);
            return;
        }

        // 需要替换的部件
        var avatarPart = GetPart(avatarModel.transform, partName);
        if (avatarPart == null)
        {
            ComplexObjectPool.Release(avatarModel);
			Logger.Error("{0} should contain a node name {1}",avatarModel.transform.name, partName);
            return;
        }

        // 将原始部件隐藏
        var bodyPart = GetPart(_body.transform, partName);
        if (bodyPart != null)
        {
            bodyPart.gameObject.SetActive(false);
        }

        // 设置到body上的新物件
        var newPart = new GameObject(partName);
        var partTransform = newPart.transform;
        partTransform.parent = _body.transform;
        partTransform.localPosition = Vector3.zero;
        partTransform.localRotation = Quaternion.Euler(0, 0, 0);
        var newPartRender = newPart.AddComponent<SkinnedMeshRenderer>();
        var avatarRender = avatarPart.GetComponent<SkinnedMeshRenderer>();
        {
            // foreach(var item in avatarPart.transform)
            var __enumerator6 = (avatarPart.transform).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var item = (Transform) __enumerator6.Current;
                {
                    var child = Instantiate(item.gameObject) as GameObject;
                    if (null == child)
                    {
                        continue;
                    }
                    var childTransform = child.transform;
                    childTransform.parent = partTransform;
                    childTransform.localPosition = item.localPosition;
                    childTransform.localScale = item.localScale;
                    childTransform.localRotation = item.localRotation;
                }
            }
        }

        newPart.SetLayerRecursive(Layer, LayerMask);
        newPart.SetRenderQueue(mRenderQueue);

        // 刷新骨骼模型数据
        SetBones(newPart, avatarPart.gameObject, _body);
        newPartRender.sharedMesh = avatarRender.sharedMesh;
        newPartRender.sharedMaterials = avatarRender.sharedMaterials;

        ComplexObjectPool.Release(avatarModel);

        const string MainTexVariableName = "_MainTex";

        if (IsMainPlayer)
        {
            _loading++;
            ResourceManager.PrepareResource<Material>(Resource.Material.MainPlayerMaterial, mat =>
            {
                _loading--;
                var newMat = new Material(mat);

                if (!newPartRender)
                {
                    return;
                }

                newMat.SetTexture(MainTexVariableName, newPartRender.sharedMaterial.GetTexture(MainTexVariableName));
                newPartRender.material = newMat;

                if (record != null)
                {
                    newMat.SetColor("_BColor",
                        new Color(record.FlowRed/255.0f, record.FlowGreen/255.0f, record.FlowBlue/255.0f,
                            record.FlowAlpha/255.0f));
                    newMat.SetColor("_TexColor",
                        new Color(record.SepcularRed/255.0f, record.SepcularGreen/255.0f, record.SepcularBlue/255.0f,
                            record.SepcularAlpha/255.0f));
                }
                else
                {
                    newMat.SetColor("_BColor", Color.black);
                    newMat.SetColor("_TexColor", Color.black);
                }

                ResourceManager.ChangeShader(partTransform);

                TryContinue();
            }, true, true, sync);
        }
        else
        {
            var newMat = new Material(newPartRender.sharedMaterial);

            if (!newPartRender)
            {
                return;
            }

            newMat.SetTexture(MainTexVariableName, newPartRender.sharedMaterial.GetTexture(MainTexVariableName));
            newPartRender.material = newMat;

            if (record != null)
            {
                newMat.SetColor("_BColor",
                    new Color(record.FlowRed/255.0f, record.FlowGreen/255.0f, record.FlowBlue/255.0f,
                        record.FlowAlpha/255.0f));
                newMat.SetColor("_TexColor",
                    new Color(record.SepcularRed/255.0f, record.SepcularGreen/255.0f, record.SepcularBlue/255.0f,
                        record.SepcularAlpha/255.0f));
            }
            else
            {
                newMat.SetColor("_BColor", Color.black);
                newMat.SetColor("_TexColor", Color.black);
            }

            ResourceManager.ChangeShader(partTransform);
        }


        // 记录换装信息
        var info = new AvatarInfo();
        info.partName = partName;
        if (bodyPart != null)
        {
            info.defaultPart = bodyPart.gameObject;
        }
        else
        {
            info.defaultPart = null;
        }

        info.avatarPart = newPart;
        _avatarInfo[partName] = info;

        // 检查Body的Animation的CullType， 否则会出现当所有装备都卸下时，动画没有了的问题
        _body.animation.cullingType = AnimationCullingType.BasedOnUserBounds;
        _body.animation.localBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(1, 2, 1));
    }

    public void Destroy()
    {
        {
            // foreach(var avatarInfo in _avatarInfo)
            var __enumerator2 = (_avatarInfo).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var avatarInfo = __enumerator2.Current;
                {
                    if (avatarInfo.Value.avatarPart != null)
                    {
                        Destroy(avatarInfo.Value.avatarPart);
                    }

                    if (avatarInfo.Value.defaultPart != null)
                    {
                        avatarInfo.Value.defaultPart.SetActive(true);
                    }
                }
            }
        }
        {
            // foreach(var part in _parts)
            var __enumerator3 = (_parts).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var part = __enumerator3.Current;
                {
                    {
                        var __list5 = part.Value;
                        var __listCount5 = __list5.Count;
                        for (var __i5 = 0; __i5 < __listCount5; ++__i5)
                        {
                            var o = __list5[__i5];
                            {
                                ComplexObjectPool.Release(o);
                            }
                        }
                    }
                }
            }
        }


        if (_body)
        {
            OptList<Renderer>.List.Clear();
            _body.GetComponentsInChildren(OptList<Renderer>.List);
            {
                var __array1 = OptList<Renderer>.List;
                var __arrayLength1 = __array1.Count;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var renderer = __array1[__i1];
                    {
                        renderer.enabled = true;
                    }
                }
            }
        }

        _parts.Clear();
        _avatarLoadQueue.Clear();
        _avatarInfo.Clear();
        if (_avatarInfo.Count == 0)
        {
            _avatarInfo.Add("Foot", new AvatarInfo {partName = "Foot"});
            _avatarInfo.Add("Chest", new AvatarInfo {partName = "Chest"});
            _avatarInfo.Add("Hand", new AvatarInfo {partName = "Hand"});
            _avatarInfo.Add("Head", new AvatarInfo {partName = "Head"});
            _avatarInfo.Add("Leg", new AvatarInfo {partName = "Leg"});
        }

        _loading = 0;
        _body = null;
        mWeaponModels.Clear();
        WingGameObject = null;

        BodyModel = string.Empty;
        FootModel = string.Empty;
        ChestModel = string.Empty;
        HandModel = string.Empty;
        LegModel = string.Empty;
        HeadModel = string.Empty;
    }

    private static Transform FindChild(Transform t, string searchName)
    {
        {
            // foreach(var c in t)
            var __enumerator4 = (t).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var c = (Transform) __enumerator4.Current;
                {
                    var partName = c.name;
                    if (partName == searchName)
                    {
                        return c;
                    }
                    var r = FindChild(c, searchName);
                    if (r != null)
                    {
                        return r;
                    }
                }
            }
        }
        return null;
    }

    // 递归遍历子物体
    private static Transform GetPart(Transform t, string searchName)
    {
        {
            // foreach(var c in t)
            var __enumerator1 = (t).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var c = (Transform) __enumerator1.Current;
                {
                    var partName = c.name;

                    if (partName == searchName)
                    {
                        return c;
                    }
                    var r = GetPart(c, searchName);
                    if (r != null)
                    {
                        return r;
                    }
                }
            }
        }
        return null;
    }

    private string GetPartModel(string part)
    {
        switch (part)
        {
            case "Foot":
                return FootModel;
            case "Chest":
                return ChestModel;
            case "Hand":
                return HandModel;
            case "Head":
                return HeadModel;
            case "Leg":
                return LegModel;
        }

        return string.Empty;
    }

    // 创建模型
    public void LoadModel(string model, bool sync)
    {
        _loading++;
        ComplexObjectPool.NewObject(Resource.Dir.Model + model, obj =>
        {
            _loading--;
            Body = obj;

            Body.transform.parent = gameObject.transform;
            TryContinue();
        }, null, null, sync);
    }

    public void MountWeapon(MountPoint p,
                            string model,
                            WeaponMountRecord mountRecord,
                            EquipModelViewRecord record,
                            int part,
                            bool sync,
                            Action<GameObject> callback = null)
    {
        mWeaponModels[part] = model;

        // remove all the objs with the same tag.
        List<GameObject> l;
        if (!_parts.TryGetValue(part, out l))
        {
            l = new List<GameObject>();
            _parts.Add(part, l);
        }
        var lCount0 = l.Count;
        for (var i = 0; i < lCount0; i++)
        {
            ComplexObjectPool.Release(l[i], Layer == UnityEngine.LayerMask.NameToLayer("UI"));
        }
        l.Clear();

        if (string.IsNullOrEmpty(model))
        {
            TryContinue();
            return;
        }

        if (_body == null || _loading > 0)
        {
            _avatarLoadQueue.Enqueue(new KeyValuePair<string, WaitingResource>(p.ToString(),
                new WaitingResource
                {
                    ModelPath = model,
                    ModelViewRecord = record,
                    IsWeapon = true,
                    Part = part,
                    Callback = callback,
                    WeaponMountRecord = mountRecord
                }));
            return;
        }

        _loading++;
        ComplexObjectPool.NewObject(model, obj =>
        {
            _loading--;

            string value;
            if (mWeaponModels.TryGetValue(part, out value) && value != model)
            {
                ComplexObjectPool.Release(obj, Layer == UnityEngine.LayerMask.NameToLayer("UI"));
                TryContinue();
                return;
            }

            if (null != obj)
            {
                if (!MountWeapon(p, obj, mountRecord, record, part, sync, callback))
                {
                    ComplexObjectPool.Release(obj);
                    TryContinue();
                    return;
                }
            }

            if (record == null)
            {
                TryContinue();
                return;
            }

            var recordEffectPathLength1 = record.EffectPath.Length;
            for (var i = 0; i < recordEffectPathLength1; i++)
            {
                if (!string.IsNullOrEmpty(record.EffectPath[i]))
                {
                    _loading++;
                    var index = i;
                    ComplexObjectPool.NewObject(record.EffectPath[index], effect =>
                    {
                        _loading--;
                        string _model;
                        if (mWeaponModels.TryGetValue(part, out _model) && _model != model)
                        {
                            ComplexObjectPool.Release(effect, Layer == UnityEngine.LayerMask.NameToLayer("UI"));
                            TryContinue();
                            return;
                        }

                        if (obj != null && effect != null)
                        {
                            if (record.EffectMount[index] == -1)
                            {
                                AddEffect(obj.transform, effect,
                                    new Vector3(record.EffectPosX[index], record.EffectPosY[index],
                                        record.EffectPosZ[index]),
                                    Quaternion.Euler(record.EffectDirX[index], record.EffectDirY[index],
                                        record.EffectDirZ[index]), part);
                            }
                            else
                            {
                                AddEffect(
                                    GetPart(_body.transform, ((MountPoint) record.EffectMount[index]).ToString()),
                                    effect,
                                    new Vector3(record.EffectPosX[index], record.EffectPosY[index],
                                        record.EffectPosZ[index]),
                                    Quaternion.Euler(record.EffectDirX[index], record.EffectDirY[index],
                                        record.EffectDirZ[index]), part);
                            }
                        }
                        TryContinue();
                    }, null, null, sync);
                }
            }

            TryContinue();
        }, null, null, sync);
    }

    private bool MountWeapon(MountPoint p,
                             GameObject obj,
                             WeaponMountRecord mountRecord,
                             EquipModelViewRecord record,
                             int part,
                             bool sync,
                             Action<GameObject> callback)
    {
        if (_body == null)
        {
            return false;
        }

        if (obj == null)
        {
            return false;
        }

        var parent = GetPart(_body.transform, p.ToString());

        if (parent == null)
        {
            return false;
        }

        var o = obj;
        var objTransform = o.transform;
        objTransform.parent = parent;
        objTransform.localPosition = new Vector3(mountRecord.PosX, mountRecord.PosY, mountRecord.PosZ);
        objTransform.localRotation = Quaternion.Euler(mountRecord.DirX, mountRecord.DirY, mountRecord.DirZ);
        objTransform.localScale = Vector3.one;
        o.SetLayerRecursive(Layer, LayerMask);
        o.SetRenderQueue(mRenderQueue);
        _parts[part].Add(o);

        if (callback != null)
        {
            callback(o);
        }

        Material material;
        Renderer renderer = o.renderer;
        if (renderer != null)
        {
            material = renderer.sharedMaterial;
        }
        else
        {
            renderer = objTransform.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer == null)
            {
                Logger.Log2Bugly("MountWeapon renderer is null, {0}, {1}, {2}", p, obj.name, record.Id);
                return false;
            }
            material = renderer.sharedMaterial;
        }

        if (material == null)
        {
            return false;
        }

        // 翅膀没有这个
        if (!material.HasProperty("_BColor"))
        {
            return true;
        }

        const string MainTexVariableName = "_MainTex";

        if (IsMainPlayer)
        {
            _loading++;
            ResourceManager.PrepareResource<Material>(Resource.Material.MainPlayerMaterial, mat =>
            {
                _loading--;

                if (!mat)
                {
                    TryContinue();
                    return;
                }

                if (!o || !renderer)
                {
                    TryContinue();
                    return;
                }

                if (!material)
                {
                    TryContinue();
                    return;
                }

                var newMat = new Material(mat);
                newMat.SetTexture(MainTexVariableName, material.GetTexture(MainTexVariableName));
                renderer.material = newMat;

                if (record != null)
                {
                    newMat.SetColor("_BColor",
                        new Color(record.FlowRed/255.0f, record.FlowGreen/255.0f, record.FlowBlue/255.0f,
                            record.FlowAlpha/255.0f));
                    newMat.SetColor("_TexColor",
                        new Color(record.SepcularRed/255.0f, record.SepcularGreen/255.0f, record.SepcularBlue/255.0f,
                            record.SepcularAlpha/255.0f));
                }
                else
                {
                    newMat.SetColor("_BColor", Color.black);
                    newMat.SetColor("_TexColor", Color.black);
                }

                ResourceManager.ChangeShader(objTransform);

                TryContinue();
            }, true, true, sync);
        }
        else
        {
            if (renderer)
            {
                var newMat = new Material(material);
                newMat.SetTexture(MainTexVariableName, material.GetTexture(MainTexVariableName));
                renderer.material = newMat;

                if (record != null)
                {
                    newMat.SetColor("_BColor",
                        new Color(record.FlowRed/255.0f, record.FlowGreen/255.0f, record.FlowBlue/255.0f,
                            record.FlowAlpha/255.0f));
                    newMat.SetColor("_TexColor",
                        new Color(record.SepcularRed/255.0f, record.SepcularGreen/255.0f, record.SepcularBlue/255.0f,
                            record.SepcularAlpha/255.0f));
                }
                else
                {
                    newMat.SetColor("_BColor", Color.black);
                    newMat.SetColor("_TexColor", Color.black);
                }

                ResourceManager.ChangeShader(objTransform);
            }
        }

        return true;
    }

    /// <summary>
    ///     移除当前部件，当前部件变为默认部件
    /// </summary>
    /// <param name="part"></param>
    public void RemovePart(string part)
    {
        AvatarInfo currentInfo;
        if (_avatarInfo.TryGetValue(part, out currentInfo))
        {
            if (currentInfo.avatarPart != null)
            {
                ComplexObjectPool.Release(currentInfo.avatarPart);
                currentInfo.avatarPart = null;
            }

            if (currentInfo.defaultPart != null)
            {
                currentInfo.defaultPart.SetActive(true);
            }
        }
    }

    // 刷新骨骼数据   将root物体的bodyPart骨骼更新为avatarPart
    private static void SetBones(GameObject goBodyPart, GameObject goAvatarPart, GameObject root)
    {
        var bodyRender = goBodyPart.GetComponent<SkinnedMeshRenderer>();
        var avatarRender = goAvatarPart.GetComponent<SkinnedMeshRenderer>();
        var myBones = new Transform[avatarRender.bones.Length];
        var avatarRenderbonesLength2 = avatarRender.bones.Length;
        for (var i = 0; i < avatarRenderbonesLength2; i++)
        {
            myBones[i] = FindChild(root.transform, avatarRender.bones[i].name);
        }
        bodyRender.bones = myBones;
    }

    private void SetPartModel(string part, string model)
    {
        switch (part)
        {
            case "Foot":
                FootModel = model;
                break;
            case "Chest":
                ChestModel = model;
                break;
            case "Hand":
                HandModel = model;
                break;
            case "Head":
                HeadModel = model;
                break;
            case "Leg":
                LegModel = model;
                break;
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        if (_avatarInfo.Count == 0)
        {
            _avatarInfo.Add("Foot", new AvatarInfo {partName = "Foot"});
            _avatarInfo.Add("Chest", new AvatarInfo {partName = "Chest"});
            _avatarInfo.Add("Hand", new AvatarInfo {partName = "Hand"});
            _avatarInfo.Add("Head", new AvatarInfo {partName = "Head"});
            _avatarInfo.Add("Leg", new AvatarInfo {partName = "Leg"});
        }


        //         LoadModel(BodyModel);
        // 
        //         ChangePart("Foot", FootModel);
        //         ChangePart("Chest", ChestModel);
        //         ChangePart("Hand", HandModel);
        //         ChangePart("Leg", LegModel);
        //         ChangePart("Head", HeadModel);
        // 
        //         MountWeapon(MountPoint.RightWeapen, RightWeapon); 
        //         MountWeapon(MountPoint.LeftWeapen, LeftWeapon);
        // 
        //         MountWeapon(MountPoint.Center, Wing);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void TryContinue()
    {
        if (_loading == 0)
        {
            // 换装请求
            if (_avatarLoadQueue.Count > 0)
            {
                var avatar = _avatarLoadQueue.Dequeue();
                if (avatar.Value.IsWeapon)
                {
                    string value;
                    if (mWeaponModels.TryGetValue(avatar.Value.Part, out value) && value == avatar.Value.ModelPath)
                    {
                        MountWeapon((MountPoint) Enum.Parse(typeof (MountPoint), avatar.Key), avatar.Value.ModelPath,
                            avatar.Value.WeaponMountRecord,
                            avatar.Value.ModelViewRecord, avatar.Value.Part, avatar.Value.Sync, avatar.Value.Callback);
                    }
                    else
                    {
                        TryContinue();
                    }
                }
                else
                {
                    if (GetPartModel(avatar.Key) == avatar.Value.ModelPath)
                    {
                        ChangePart(avatar.Key, avatar.Value.ModelPath, avatar.Value.ModelViewRecord, avatar.Value.Sync);
                    }
                    else
                    {
                        TryContinue();
                    }
                }
            }
        }
    }

    // 换装的部件信息
    protected class AvatarInfo
    {
        public GameObject avatarPart;
        public GameObject defaultPart;
        public string partName;
    }

    private class WaitingResource
    {
        public Action<GameObject> Callback;
        public bool IsWeapon;
        public string ModelPath;
        public EquipModelViewRecord ModelViewRecord;
        public int Part;
        public readonly bool Sync = false;
        public WeaponMountRecord WeaponMountRecord;
    }
}