using System;
#region using

using PathologicalGames;
using UnityEngine;

#endregion

public class PopTalkBoard : MonoBehaviour
{
    public Camera CGCamera;
    public UILabel Label;
    public WorldTo2DCameraConstraint mConstraint;
    private float mOffset;
    private float mTime = 3.0f;
    public Camera UICamera;
	public int MaxWidthCharacterNumber = 10;
	public int MaxWidth = 220;
    public void SetOwner(GameObject owner, float offset)
    {
        var objTransform = gameObject.transform;
        objTransform.localScale = new Vector3(1, 1, 1);

        mOffset = offset;

        mConstraint.target = owner.transform;
        mConstraint.offset = new Vector3(0, offset, 0);
    }

    public void SetText(string str, float time)
    {
        Label.text = str;
        mTime = time;
	    if (str.Length > MaxWidthCharacterNumber)
	    {
		    Label.overflowMethod = UILabel.Overflow.ResizeHeight;
			Label.width = MaxWidth;
	    }
	    else
		{
			Label.overflowMethod = UILabel.Overflow.ResizeFreely;
		    
	    }
        if (mTime > 0)
        {
            gameObject.active = true;
        }
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
		try
		{
#endif
        mConstraint.targetCamera = CGCamera;
        mConstraint.orthoCamera = UICamera;


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
        if (mTime > 0)
        {
            mTime -= Time.deltaTime;
            if (mTime <= 0)
            {
                gameObject.active = false;
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
}