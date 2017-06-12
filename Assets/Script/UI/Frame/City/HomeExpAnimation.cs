using System;
#region using

using System.Collections;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
    public class HomeExpAnimation : MonoBehaviour
	{
	    public GameObject GetAnim;
	    public CurveMove MyCurveMove;
	    public float WaitDuration = 0.5f;
	
	    public void CityHomeExpPlay()
	    {
	        if (gameObject.activeInHierarchy)
	        {
	            //float duration = 1f;
	            //var tweenpostion = gameObject.GetComponent<TweenScale>();
	            //if (tweenpostion != null)
	            //{
	            //    duration = tweenpostion.duration;
	            //}
	            StartCoroutine(GetExpWaitTimeSeconds(WaitDuration, 0));
	        }
	    }
	
	    private IEnumerator GetExpWaitTimeSeconds(float seconds, int type)
	    {
	        yield return new WaitForSeconds(seconds);
	        if (!gameObject.activeInHierarchy)
	        {
	            yield break;
	        }
	        if (type == 0)
	        {
	            if (MyCurveMove != null)
	            {
	                MyCurveMove.Play();
	            }
	        }
	        if (type == 1)
	        {
	            if (GetAnim != null)
	            {
	                GetAnim.SetActive(false);
	            }
	            Destroy(gameObject);
	        }
	    }
	
	    public void Init(Transform from,
	                     Transform to,
	                     BagItemDataModel item,
	                     Vector3 StartAddPos = default(Vector3),
	                     float Duration = 0.5f,
	                     Vector3 highVecotr = default(Vector3))
	    {
	        if (gameObject.activeInHierarchy)
	        {
	            var trans = gameObject.transform;
	            trans.parent = from.parent;
	
	            var worldPos = from.position;
	            var fromPos = trans.parent.InverseTransformPoint(worldPos);
	            fromPos += StartAddPos;
	            fromPos.z = 0;
	            worldPos = to.position;
	            var toPos = trans.parent.InverseTransformPoint(worldPos);
	            toPos.z = 0;
	            if (GetAnim)
	            {
	                GetAnim.SetActive(false);
	            }
	            MyCurveMove = gameObject.GetComponent<CurveMove>();
	            var bind = gameObject.GetComponent<BindDataRoot>();
	            bind.SetBindDataSource(item);
	            trans.localPosition = fromPos;
	            trans.localScale = Vector3.one;
	
	            var tweenpostion = gameObject.GetComponent<TweenPosition>();
	            if (tweenpostion != null)
	            {
	                tweenpostion.from = trans.localPosition;
	                tweenpostion.to = trans.localPosition + highVecotr;
	            }
	
	            MyCurveMove.From = fromPos;
	            MyCurveMove.To = toPos;
	            MyCurveMove.Duration = Duration;
	            MyCurveMove.HighPostion = (MyCurveMove.To - MyCurveMove.From)/2 + highVecotr/2;
	            var delegete = new EventDelegate(() => { SetExpAndGetAnim(); });
	            MyCurveMove.OnFinish.Add(delegete);
	        }
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (!gameObject)
	        {
	            return;
	        }
	
	        if (MyCurveMove != null)
	        {
	            MyCurveMove.OnFinish.Clear();
	        }
	
	        Destroy(gameObject);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void SetExpAndGetAnim()
	    {
	        if (GetAnim)
	        {
	            GetAnim.SetActive(true);
	        }
	        var rotation = gameObject.GetComponent<TweenRotation>();
	        if (rotation != null)
	        {
	            rotation.enabled = false;
	        }
	        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
	        StartCoroutine(GetExpWaitTimeSeconds(0.5f, 1));
	    }
	}
}