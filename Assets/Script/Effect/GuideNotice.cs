using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Effect
{
    class GuideNotice : MonoBehaviour
    {
        public Vector3 InitScale = new Vector3(8.0f, 8.0f, 1.0f);
        public Vector3 MinLoopScale = new Vector3(1.2f, 1.2f, 1.0f);
        public Vector3 MaxLoopScale = new Vector3(1.5f, 1.5f, 1.0f);
        public float InitToMinScaleTime = 0.4f;
        public float MinToMaxScaleTime = 0.62f;

        private int stage = 0;
        private float scaleTime;
        private float span = 1.0f;
        private UISprite sprite;

        void OnEnable()
        {
#if !UNITY_EDITOR
try
{
#endif

            transform.localScale = new Vector3(InitScale.x, InitScale.y, InitScale.z);
            sprite = gameObject.GetComponent<UISprite>();
            ChangeState(0);
        
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

        void Update()
        {
#if !UNITY_EDITOR
try
{
#endif

            scaleTime += Time.deltaTime;
            if (scaleTime > span)
                scaleTime = span;

            if (Math.Abs(span) < 0.000001f)
            {
                return;
            }
            if (stage == 0)
            {
                transform.localScale = Vector3.Lerp(InitScale, MinLoopScale, scaleTime / span);

                if (sprite != null)
                {
                    sprite.alpha = scaleTime / span;
                }                    
                if (scaleTime >= span)
                {
                    ChangeState(1);
                    sprite.alpha = 1.0f;
                }
            }
            else if (stage == 1)
            {
                transform.localScale = Vector3.Lerp(MinLoopScale, MaxLoopScale, scaleTime / span);

                if (scaleTime >= span)
                {
                    ChangeState(2);
                }
            }
            else if (stage == 2)
            {
                transform.localScale = Vector3.Lerp(MaxLoopScale, MinLoopScale, scaleTime / span);
                
                if (scaleTime >= span)
                {
                    ChangeState(1);
                }
            }
        
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

        private void ChangeState(int s)
        {
            stage = s;
            if (s == 0)
            {
                span = InitToMinScaleTime;
                if (sprite != null)
                    sprite.alpha = 0.0f;
            }
            else if (s == 1)
            {
                span = MinToMaxScaleTime;
            }
            else if (s == 2)
            {
                span = MinToMaxScaleTime;
            }
            scaleTime = 0.0f;
        }
    }
}
