#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

#endregion

public class CreateFakeCharacter : MonoBehaviour
{
    public UIWidget BackgroundWidget;
    //public UIWidget Backkground;

    [HideInInspector] public ObjFakeCharacter Character;
    public int CustomRenderQueue = -1;
    [HideInInspector] private CreateFakeCharacter ElfCreator;
    public Vector3 ElfOffset = new Vector3(1, 0, -0.5f);
    private Coroutine mRunningCoroutine;
    private ulong UniqueId;

    public void Create(int dataId,
                       Dictionary<int, int> dictEquip = null,
                       Action<ObjFakeCharacter> callback = null,
                       int elfId = -1,
                       bool coroutine = false,
                       int layer = 5,
                       bool bsync = false)
    {
        DestroyFakeCharacter();

        StopCreatingCoroutine();

        if (coroutine)
        {
            //��һ��Coroutine�������ڵ�ǰֱ֡�ӵ��ԭ����?����ȡDrawCall��RenderQʱDrawCall����null
            mRunningCoroutine =
                ResourceManager.Instance.StartCoroutine(CreateCoroutine(dataId, bsync, dictEquip, callback, elfId, layer));
        }
        else
        {
            CreateChar(dataId, bsync, dictEquip, callback, elfId, layer);
        }
    }

    //�ӳ���ObjCharacter
    private void CreateChar(int dataId,
                            bool bsync,
                            Dictionary<int, int> dictEquip = null,
                            Action<ObjFakeCharacter> callback = null,
                            int elfId = -1,
                            int layer = 5)
    {
        if (null != BackgroundWidget)
        {
            if (null != BackgroundWidget.drawCall)
            {
                CustomRenderQueue = BackgroundWidget.drawCall.renderQueue + +1;
                CustomRenderQueue += BackgroundWidget.SkippedRenderQueue/2;
            }
        }

        Character = ObjFakeCharacter.Create(dataId, dictEquip, character =>
        {
            if (Character.GetObjId() != character.GetObjId())
            {
                character.Destroy();
                return;
            }

            var xform = Character.gameObject.transform;

            //Character.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
            xform.parent = gameObject.transform;
            xform.localPosition = Vector3.zero;
            xform.localScale = Vector3.one;
            xform.forward = Vector3.back;

            if (null != callback)
            {
                callback(Character);

                if (-1 != elfId)
                {
                    if (null == ElfCreator)
                    {
                        ElfCreator = gameObject.AddComponent<CreateFakeCharacter>();
                        ElfCreator.CustomRenderQueue = CustomRenderQueue;
                    }
                    ElfCreator.DestroyFakeCharacter();

                    var tbElf = Table.GetElf(elfId);
                    if (tbElf == null)
                    {
                        return;
                    }
                    var elfDataId = tbElf.ElfModel;
                    var tableCharacter = Table.GetCharacterBase(elfDataId);
                    if (null == tableCharacter)
                    {
                        return;
                    }

                    var tempUniqueId = UniqueId;
                    ElfCreator.Create(elfDataId, null, elf =>
                    {
                        if (character.State == ObjState.Deleted)
                        {
                            elf.Destroy();
                            return;
                        }

                        if (!gameObject.active)
                        {
                            ElfCreator.Character = null;
                            elf.Destroy();
                            return;
                        }

                        if (null == Character)
                        {
                            ElfCreator.Character = null;
                            elf.Destroy();
                            return;
                        }

// 						if (tempUniqueId != UniqueId)
// 						{
// 							ElfCreator.Character = null;
// 							elf.Destroy();
// 							return;
// 						}

                        var t = elf.gameObject.transform;
                        t.parent = Character.gameObject.transform.parent;

                        t.forward = Vector3.back;
                        //var angle = Character.gameObject.transform.localRotation.eulerAngles.y - 90.0f;
                        var scale = tbElf.CameraZoom*0.0001f;
                        if (scale <= 0)
                        {
                            Logger.Debug("!!!tbElf.CameraZoom<=0");
                            scale = 1;
                        }
                        var heightOffset = tbElf.Offset*0.0001f;
                        var temp = ElfOffset + new Vector3(0, heightOffset, 0f);
                        t.localPosition = temp;
                        t.localScale = Vector3.one*scale;

                        t.parent = Character.gameObject.transform.parent;
                    }, -1, false, layer);
                }
            }
        }, layer, bsync, CustomRenderQueue, UniqueId++);
    }

    private IEnumerator CreateCoroutine(int dataId,
                                        bool bsync,
                                        Dictionary<int, int> dictEquip = null,
                                        Action<ObjFakeCharacter> callback = null,
                                        int elfId = -1,
                                        int layer = 5)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CreateChar(dataId, bsync, dictEquip, callback, elfId, layer);
    }

    public void DestroyFakeCharacter()
    {
        if (null != ElfCreator)
        {
            ElfCreator.DestroyFakeCharacter();
        }

        if (null != Character)
        {
            Character.Destroy();
            Character = null;
        }
    }

    public void SetActiveFakeCharacter(bool active)
    {

        if (null != Character)
        {
            Character.gameObject.SetActive(active);
        }
    }


    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        StopCreatingCoroutine();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void StopCreatingCoroutine()
    {
        if (null != mRunningCoroutine)
        {
            ResourceManager.Instance.StopCoroutine(mRunningCoroutine);
            mRunningCoroutine = null;
        }
    }
}