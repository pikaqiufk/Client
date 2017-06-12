#region using

using System;
using UnityEngine;

#endregion

public class AnimationModel : MonoBehaviour
{
    private Animation mAnimation;
    private string mAnimationName = "";
    private GameObject mModel;
    private State mState = State.Deleted;

    public void CreateModel(string prefab, string animationPath = "", Action<GameObject> callback = null)
    {
        if (mState != State.Deleted)
        {
            return;
        }
#if UNITY_EDITOR
        gameObject.name = "AnimationModel";
#endif
        mState = State.Loading;
        ComplexObjectPool.NewObject(prefab, go =>
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
            
            if (!string.IsNullOrEmpty(animationPath))
            {
				var ani = go.GetComponent<Animation>();
				if (null == ani)
				{
					ani = go.AddComponent<Animation>();
				}
				mAnimation = ani;
				ani.enabled = true;

                var s = animationPath.LastIndexOf("/");
                var l = animationPath.LastIndexOf(".anim");
                var aniName = animationPath.Substring(s + 1, l - s - 1);
                if (!string.IsNullOrEmpty(aniName))
                {
                    mAnimationName = aniName;
                    var clip = ani.GetClip(aniName);
                    if (null == clip)
                    {
                        ResourceManager.PrepareResource<AnimationClip>(animationPath, aniClip =>
                        {
                            aniClip.wrapMode = WrapMode.Loop;
                            ani.AddClip(aniClip, aniName);
                            ani.Play(aniName);
                            if (null != callback)
                            {
                                callback(go);
                            }
                        });
                    }
                    else
                    {
                        ani.Play(aniName);
                        if (null != callback)
                        {
                            callback(go);
                        }
                    }
                }
            }
            else
            {
                if (null != callback)
                {
                    callback(go);
                }
            }
        });
    }

    public void RemoveAllCompent()
    {
        if (mModel)
        {
            var componet = mModel.GetComponent<RotateAround>();
            if (componet != null)
            {
                Destroy(componet);
            }
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

    public void PlayAnimation()
    {
        if (null != mAnimation)
        {
            if (!string.IsNullOrEmpty(mAnimationName))
            {
                mAnimation.CrossFade(mAnimationName, 0.2f);
            }
        }
    }

    private enum State
    {
        Loading,
        Ok,
        Deleted
    }
}