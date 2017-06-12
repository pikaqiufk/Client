using System;
#region using

using UnityEngine;

#endregion

[RequireComponent(typeof (UILabel))]
public class SignNumLogic : MonoBehaviour
{
    private UILabel mLabel;
    private int mNumber;

    public int Number
    {
        get { return mNumber; }
        set
        {
            if (mNumber == value)
            {
                return;
            }
            mNumber = value;
            var text = mNumber > 0 ? "+" : "";
            text += mNumber;
            mLabel.text = text;
        }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mLabel = GetComponent<UILabel>();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}