using System;
#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PlowLandFrame : MonoBehaviour
	{
	    // Use this for initialization
	    public GameObject ColliderObject;
	    public UISprite CropSprite;
	    //public UILabel ShowInfo;
	    public UISprite LockSprite;
	    private FarmCropDataModel dataModel;
	    public Animation SelectAnimation;
	    public Coroutine TimerCoroutine;
	
	    public FarmCropDataModel DataModel
	    {
	        get { return dataModel; }
	        set
	        {
	            if (DataModel != null)
	            {
	                DataModel.PropertyChanged -= OnPropertyChange;
	            }
	            dataModel = value;
	            RefrshShowInfo();
	            DataModel.PropertyChanged += OnPropertyChange;
	        }
	    }
	
	    public void OnClickLand()
	    {
	        if (SelectAnimation && SelectAnimation.clip)
	        {
	            SelectAnimation.Stop();
	            SelectAnimation.Play(SelectAnimation.clip.name);
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (DataModel != null)
	        {
	            DataModel.PropertyChanged -= OnPropertyChange;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (TimerCoroutine != null)
	        {
	            StopCoroutine(TimerCoroutine);
	            TimerCoroutine = null;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (TimerCoroutine != null)
	        {
	            StopCoroutine(TimerCoroutine);
	        }
	        TimerCoroutine = StartCoroutine(RefreshTimerCoroutine());
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnPropertyChange(object sender, PropertyChangedEventArgs e)
	    {
	        if (sender == DataModel)
	        {
	            RefrshShowInfo();
	        }
	    }
	
	    public void RefrshShowInfo()
	    {
	        var state = (LandState) DataModel.State;
	        if (state == LandState.Lock)
	        {
	            //ShowInfo.gameObject.SetActive(false);
	            CropSprite.gameObject.SetActive(false);
	            LockSprite.gameObject.SetActive(true);
	        }
	        else
	        {
	            LockSprite.gameObject.SetActive(false);
	            //ShowInfo.gameObject.SetActive(true);
	            if (state == LandState.Blank)
	            {
	                CropSprite.gameObject.SetActive(false);
	                //ShowInfo.text = "空地";
	            }
	            else if (state == LandState.Growing)
	            {
	                if (dataModel.Type == -1)
	                {
	                    return;
	                }
	                CropSprite.gameObject.SetActive(true);
	                var tbPlant = Table.GetPlant(dataModel.Type);
	                if (tbPlant == null)
	                {
	                    return;
	                }
	                //ShowInfo.text = tbPlant.PlantName + "正在成长";
	                var dif = (int) (DataModel.MatureTime - Game.Instance.ServerTime).TotalSeconds;
	                var step = -1;
	                if (dif < 0)
	                {
	                    step = tbPlant.GrowStepCount - 1;
	                }
	                else
	                {
	                    dif = tbPlant.MatureCycle*60 - dif;
	                    var length = tbPlant.MatureCycle*60/(tbPlant.GrowStepCount - 1);
	                    step = dif/length;
	                }
	                if (step < 0 || step >= tbPlant.StepPicID.Length)
	                {
	                    return;
	                }
	                var icon = tbPlant.StepPicID[step];
	                CropSprite.gameObject.SetActive(true);
	                GameUtils.SetSpriteIcon(CropSprite, icon);
	            }
	            else if (state == LandState.Mature)
	            {
	                if (dataModel.Type == -1)
	                {
	                    return;
	                }
	                var tbPlant = Table.GetPlant(dataModel.Type);
	                if (tbPlant == null)
	                {
	                    return;
	                }
	                var step = tbPlant.GrowStepCount - 1;
	                if (step < 0 || step >= tbPlant.StepPicID.Length)
	                {
	                    return;
	                }
	                var icon = tbPlant.StepPicID[step];
	                //ShowInfo.text = tbPlant.PlantName + "已经成熟";
	                CropSprite.gameObject.SetActive(true);
	                GameUtils.SetSpriteIcon(CropSprite, icon);
	            }
	        }
	    }
	
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
	
	    private IEnumerator RefreshTimerCoroutine()
	    {
	        while (true)
	        {
	            yield return new WaitForSeconds(1.0f);
	            RefrshShowInfo();
	        }
	        yield break;
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
}