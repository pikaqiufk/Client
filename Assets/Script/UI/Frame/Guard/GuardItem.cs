
#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class GuardItem : MonoBehaviour
    {
        public int Index;

        private void Awake()
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

        public void OnClick()
        {
            EventDispatcher.Instance.DispatchEvent(new GuardItemOperation(0, Index));
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
    }
}
