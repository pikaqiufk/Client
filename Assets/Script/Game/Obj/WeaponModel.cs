#region using

using System;
using UnityEngine;
using DataTable;
#endregion

public class WeaponModel : MonoBehaviour
{
    private GameObject mModel;
    private State mState = State.Deleted;

    public void CreateModel(int equipId, Action<GameObject> callback = null, bool showPrefab = false)
    {
        if (mState != State.Deleted)
        {
            return;
        }
#if UNITY_EDITOR
        gameObject.name = "AnimationModel";
#endif

        var tbEquip = Table.GetEquipBase(equipId);
        if (tbEquip == null)
        {
            return;
        }
        var tbMont = Table.GetWeaponMount(tbEquip.EquipModel);
        if (tbMont == null)
        {
            return;
        }

        mState = State.Loading;

        var path = showPrefab ? tbMont.ShowPath : tbMont.Path;
        ComplexObjectPool.NewObject(path, go =>
        {
            if (null == go)
            {
                return;
            }

            if (mState == State.Deleted)
            {
                ComplexObjectPool.Release(go);
                return;
            }

            if (mState == State.Ok)
            {
                ComplexObjectPool.Release(mModel);
            }

            var goTransform = go.transform;
            goTransform.parent = transform;
            goTransform.localPosition = Vector3.zero;
            goTransform.localRotation = Quaternion.identity;
            goTransform.localScale = Vector3.one;

            mModel = go;
            mState = State.Ok;

            var render = go.GetComponent<Renderer>();
            if (null != render)
            {
                var tbEquipModel = Table.GetEquipModelView(tbMont.Enchance[tbEquip.Ladder]);
                var mat = render.material;
                if (null != mat && null != tbEquipModel)
                {
                    mat.SetColor("_BColor",
                        new Color(tbEquipModel.FlowRed / 255.0f, tbEquipModel.FlowGreen / 255.0f, tbEquipModel.FlowBlue / 255.0f,
                            tbEquipModel.FlowAlpha / 255.0f));
                    mat.SetColor("_TexColor",
                        new Color(tbEquipModel.SepcularRed / 255.0f, tbEquipModel.SepcularGreen / 255.0f, tbEquipModel.SepcularBlue / 255.0f,
                        tbEquipModel.SepcularAlpha / 255.0f));
                }
            }

            if (null != callback)
            {
                callback(go);
            }
        });
    }

    public void RemoveAllCompent()
    {
        if (mModel)
        {
            var effect = mModel.GetComponent<RotateAround>();
            if(effect)
                Destroy(effect);
            //UnityEngine.Object.Destroy(mModel);
        }
    }

    public void DestroyModel()
    {
        mState = State.Deleted;
        if (null != mModel)
        {
            ComplexObjectPool.Release(mModel);
            mModel = null;
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (null != mModel)
        {
            ComplexObjectPool.Release(mModel);
            mModel = null;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private enum State
    {
        Loading,
        Ok,
        Deleted
    }
}
