//----------------------------------------------
// 
//				文字一个字一个字蹦出来
//----------------------------------------------

using UnityEngine;


[RequireComponent(typeof(UILabel))]
public class TweenLabelTextOneByOne : UITweener
{
	//cache component
	private UILabel mLabel;

	//起始字符串
	public string from = "";

	//目标字符串
	public string to = "";

	public UILabel cachedLabel
	{
		get
		{
			if (mLabel == null)
			{
				mLabel = GetComponent<UILabel>();
			}
			return mLabel;
		}
	}

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public string value
	{
		get
		{
			return cachedLabel.text;
		}
		set
		{
			cachedLabel.text = value;
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate(float factor, bool isFinished)
	{
		if(to.Length>0)
		{
			var f = (int)(Mathf.Lerp(0, (float)to.Length, factor));
			value = from + ParseSymbol(to,f);
		}
	}

    public void ForceEnd()
    {
        if (enabled)
        {
            tweenFactor = 1.0f;
        }
    }

	static string ParseSymbol(string str, int l)
	{
		var value = str.Substring(0, l);
		if (l < str.Length)
		{
			var idx = str.LastIndexOf("[", l-1);
			if (-1 != idx)
			{
				if (idx < str.Length - 2 && '-' == str[idx + 1] && ']' == str[idx + 2])
				{
					idx = str.LastIndexOf("[", idx - 1);
				}

				if (NGUIText.ParseSymbol(str, ref idx))
				{
					var idx2 = str.IndexOf("[-]");
					if (-1 != idx2 && l <= idx2 + 2)
					{
						value = str.Substring(0, idx2 + 3);
					}
				}
			}
			
		}
		return value;
	}


	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue() { to = value; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd() { value = to; }


	[ContextMenu("Test")]
	public void Test()
	{


			//Debug.Log(ParseSymbol(cachedLabel.text, i));
		
	}

}
