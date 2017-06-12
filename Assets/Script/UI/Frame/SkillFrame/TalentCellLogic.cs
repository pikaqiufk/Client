using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

public class TalentCellLogic : MonoBehaviour
{
    public float EffectScale = 1.0f;
    // Use this for initialization

    private bool isMoving;
    private GameObject mEnableEffect;
    private GameObject mOpenEffect;
    public int TalentId;
    public TalentCellDataModel CellDataModel { get; set; }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        CellDataModel.PropertyChanged -= OnPropertyChangedEvent;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnDrag(Vector2 delta)
    {
        isMoving = true;
    }

    private void OnPress(bool bPress)
    {
        if (bPress)
        {
            var e = new UIEvent_SkillFrame_TalentBallClick(-1);
            EventDispatcher.Instance.DispatchEvent(e);
            isMoving = false;
        }
        else
        {
            if (!isMoving)
            {
                var e = new UIEvent_SkillFrame_TalentBallClick(TalentId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    private void OnPropertyChangedEvent(object o, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "TalentEnable")
        {
            ShowOpenEffect(CellDataModel.TalentEnable);
        }
        else if (args.PropertyName == "ShowLine")
        {
            ShowConnectLineAndEffect(CellDataModel.ShowLine);
        }

        else if (args.PropertyName == "Count")
        {
            RefreshTalentBallLabel(CellDataModel.Count);
        }

        else if (args.PropertyName == "ShowLockTotal")
        {
            RefreshTalentLock(CellDataModel.ShowLockTotal);
        }

        else if (args.PropertyName == "LevelLock")
        {
            RefreshLevelLock(CellDataModel.LevelLock);
        }

        else if (args.PropertyName == "NeedLevel")
        {
            RefreshLevelLabel(CellDataModel.NeedLevel);
        }
    }

    private void OnTalentBallClick()
    {
//          UIEvent_SkillFrame_TalentBallClick e = new UIEvent_SkillFrame_TalentBallClick(TalentId);
//          EventDispatcher.Instance.DispatchEvent(e);
    }

    private void RefreshParticleEffect()
    {
        ShowOpenEffect(CellDataModel.TalentEnable);
        ShowConnectLineAndEffect(CellDataModel.ShowLine);
    }

    private void RefreshTalentBallLabel(int count)
    {
        var TalentTable = Table.GetTalent(TalentId);
        var countLabel = GetComponentInChildren<UILabel>();
        if (null != countLabel)
        {
            countLabel.text = string.Format("{0}/{1}", count, TalentTable.MaxLayer);
        }
        else
        {
            Logger.Error("can not find uilabel from TalentCell!!");
        }
    }

    private void RefreshTalentLock(bool bshow)
    {
        var t = gameObject.transform.FindChild("LockIcon");
        if (null != t)
        {
            var sp = t.GetComponent<UISprite>();
            if (null != sp)
            {
                sp.gameObject.SetActive(bshow);
            }
        }
    }

    private void RefreshLevelLock(bool bshow)
    {
        var t = gameObject.transform.FindChild("LockLevel");
        if (null != t)
        {
            var label = t.GetComponent<UILabel>();
            if (null != label)
            {
                label.gameObject.SetActive(bshow);
            }
        }
    }

    private void RefreshLevelLabel(int level)
    {
        var t = gameObject.transform.FindChild("LockLevel");
        if (null != t)
        {
            var label = t.GetComponent<UILabel>();
            if (null != label)
            {
                label.text = string.Format(GameUtils.GetDictionaryText(1039),level);
            }
        }
    }

    private void ShowConnectLineAndEffect(bool bShow)
    {
        if (bShow)
        {
            var slider = gameObject.GetComponentInChildren<UISlider>();
            if (null != slider)
            {
                slider.value = 1.0f;
            }

            ResourceManager.PrepareResource<GameObject>
                ("Effect/UI/SkillFrame/UI_Talent_Enabled.prefab", res =>
                {
                    if (mEnableEffect)
                    {
                        NGUITools.Destroy(mEnableEffect);
                    }
                    mEnableEffect = NGUITools.AddChild(gameObject, res);
                    var scale = mEnableEffect.transform.localScale;
                    scale.x = scale.x*EffectScale;
                    scale.y = scale.y*EffectScale;
                    mEnableEffect.transform.localScale = scale;
                });
        }
        else
        {
            var slider = gameObject.GetComponentInChildren<UISlider>();
            if (null != slider)
            {
                slider.value = 0;
            }

            if (mEnableEffect)
            {
                NGUITools.Destroy(mEnableEffect);
                mEnableEffect = null;
            }
        }
    }

    private void ShowOpenEffect(bool bShow)
    {
        if (bShow)
        {
            ResourceManager.PrepareResource<GameObject>
                ("Effect/UI/SkillFrame/UI_Talent_Open.prefab", res =>
                {
                    if (mOpenEffect)
                    {
                        NGUITools.Destroy(mOpenEffect);
                    }
                    mOpenEffect = NGUITools.AddChild(gameObject, res);
                    var scale = mOpenEffect.transform.localScale;
                    scale.x = scale.x*EffectScale;
                    scale.y = scale.y*EffectScale;
                    mOpenEffect.transform.localScale = scale;
                });
        }
        else
        {
            if (mOpenEffect)
            {
                NGUITools.Destroy(mOpenEffect);
                mOpenEffect = null;
            }
        }
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        RefreshTalentBallLabel(CellDataModel.Count);
        RefreshParticleEffect();
        RefreshTalentLock(CellDataModel.ShowLockTotal);
        RefreshLevelLock(CellDataModel.LevelLock);
        RefreshLevelLabel(CellDataModel.NeedLevel);

        CellDataModel.PropertyChanged += OnPropertyChangedEvent;

        var ballButton = GetComponent<UIButton>();
        if (null == ballButton)
        {
            Logger.Error("can't find button at TalentBall !");
        }
        else
        {
            ballButton.onClick.Add(new EventDelegate(OnTalentBallClick));
        }


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Update is called once per frame
    private void Update()
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
}