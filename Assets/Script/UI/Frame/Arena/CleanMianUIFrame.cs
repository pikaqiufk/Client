#region using

using System;
using UnityEngine;

#endregion

namespace GameUI
{
	public class CleanMianUIFrame : MonoBehaviour
	{
	    public UITexture BackGround;
	    public int CleanCount;
	    private Color[] clors;
	    private readonly int Diameter = 48;
	    private int flag = -1;
	    public int FullCount;
	    public UITexture Mask;
	    private bool isHover;
	    private bool isPress;
	    private Vector2 lastTouch = new Vector2(-1, -1);
	    private Texture2D operateTexture;
	    public CleanFinish OnCleanFinish;
	    private readonly int rdius = 24;
	    private readonly int radiusSquare = 576;
	    private Vector2 saleVec;
	    public UILabel ScheduleLab;
	    public BoxCollider TouchCollider;
	    public bool IsFinish { get; set; }
	    public bool IsStart { get; set; }
	
	    private void calculateClear()
	    {
	        var pos = SetMouseToBackgroud(Input.mousePosition);
	        if (lastTouch.x < 0)
	        {
	            ClearPoint((int) pos.x, (int) pos.y, true);
	            lastTouch = pos;
	        }
	        else
	        {
	            ClearLine(lastTouch, pos);
	            lastTouch = pos;
	        }
	    }
	
	    private Vector2 SetMouseToBackgroud(Vector3 mouse)
	    {
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
	        var localPos = BackGround.transform.InverseTransformPoint(worldPos);
	        var ret = new Vector2(localPos.x + BackGround.width/2.0f, localPos.y + BackGround.height/2.0f);
	        return ret;
	    }
	
	    public bool CheckIsFinish()
	    {
	        var p = CleanCount/(float) FullCount;
	        if (p > 0.8f)
	        {
	            return true;
	        }
	        return false;
	    }
	
	    private void ClearLine(Vector2 a, Vector2 b)
	    {
	        if (Mathf.Abs(a.y - b.y) < 2)
	        {
	            var y = (int) a.y;
	            var begin = Mathf.Min((int) a.x, (int) b.x);
	            var end = Mathf.Max((int) a.x, (int) b.x);
	            for (var i = begin - 2; i <= end + 2; i++)
	            {
	                ClearPoint(i, y);
	            }
	        }
	        else
	        {
	            if (Mathf.Abs(a.x - b.x) < 2)
	            {
	                var x = (int) a.x;
	                var begin = Mathf.Min((int) a.y, (int) b.y);
	                var end = Mathf.Max((int) a.y, (int) b.y);
	                for (var i = begin - 2; i <= end + 2; i++)
	                {
	                    ClearPoint(x, i);
	                }
	            }
	            else
	            {
	                var k = (a.y - b.y)/(a.x - b.x);
	                if (Mathf.Abs(k) < 0.5)
	                {
	                    ClearLineX(a, b);
	                }
	                else
	                {
	                    ClearLineY(a, b);
	                }
	            }
	        }
	        operateTexture.Apply();
	    }
	
	    private void ClearLineX(Vector2 a, Vector2 b)
	    {
	        var begin = new Vector2();
	        var end = new Vector2();
	        if ((int) a.x > (int) b.x)
	        {
	            begin = b;
	            end = a;
	        }
	        else if ((int) a.x < (int) b.x)
	        {
	            begin = a;
	            end = b;
	        }
	
	        var k = (end.y - begin.y)/(end.x - begin.x);
	        for (var i = (int) begin.x - 1; i < (int) end.x + 1; i++)
	        {
	            var y = (int) ((i - begin.x)*k + begin.y);
	            ClearPoint(i, y);
	        }
	    }
	
	    private void ClearLineY(Vector2 a, Vector2 b)
	    {
	        var begin = new Vector2();
	        var end = new Vector2();
	        if ((int) a.y > (int) b.y)
	        {
	            begin = b;
	            end = a;
	        }
	        else if ((int) a.y < (int) b.y)
	        {
	            begin = a;
	            end = b;
	        }
	
	        var k = (end.y - begin.y)/(end.x - begin.x);
	        var last = -1;
	
	        for (var i = (int) begin.y - 2; i < (int) end.y + 2; i++)
	        {
	            var x = (int) ((i - begin.y)/k + begin.x);
	            ClearPoint(x, i);
	        }
	    }
	
	    private void ClearPoint(int pointX, int pointY, bool apply = false)
	    {
	        for (var i = 0; i < Diameter; i++)
	        {
	            for (var j = 0; j < Diameter; j++)
	            {
	                var x = i + pointX - rdius;
	                if (x < 0 || x > BackGround.width)
	                {
	                    continue;
	                }
	                var y = j + pointY - rdius;
	                if (y < 0 || y > BackGround.height)
	                {
	                    continue;
	                }
	                var ix = (int) (x/saleVec.x);
	                var iy = (int) (y/saleVec.y);
	                var c = operateTexture.GetPixel(ix, iy);
	                if (Math.Abs(c.a) < 0.01)
	                {
	                    continue;
	                }
	                if (!GetChange(i, j))
	                {
	                    continue;
	                }
	                c.a = 0.0f;
	
	                CleanCount++;
	                operateTexture.SetPixel(ix, iy, new Color(c.r, c.g, c.b, c.a));
	            }
	        }
	        if (apply)
	        {
	            operateTexture.Apply();
	        }
	    }
	
	    public void Finish()
	    {
	        Mask.gameObject.SetActive(false);
	    }
	
	    private bool GetChange(int x, int y)
	    {
	        var len = new Vector2(x - rdius, y - rdius);
	        if (len.sqrMagnitude < radiusSquare)
	        {
	            return true;
	        }
	        return false;
	    }
	
	    public void OnClickBg()
	    {
	        if (!IsStart)
	        {
	            return;
	        }
	        isPress = true;
	        isHover = true;
	        calculateClear();
	    }
	
	    public void OnHoverOut()
	    {
	        isHover = false;
	        if (isPress)
	        {
	            calculateClear();
	            lastTouch.x = -1;
	        }
	    }
	
	    public void OnHoverOver()
	    {
	        isHover = true;
	        if (isPress)
	        {
	            calculateClear();
	        }
	    }
	
	    public void OnPressBg()
	    {
	        isPress = true;
	        if (isHover)
	        {
	            calculateClear();
	        }
	    }
	
	    public void OnReleaseBg()
	    {
	        isPress = false;
	        if (isHover)
	        {
	            calculateClear();
	            lastTouch.x = -1;
	        }
	    }
	
	    public void Reset()
	    {
	        Mask.gameObject.SetActive(true);
	        IsStart = false;
	        IsFinish = false;
	        TouchCollider.enabled = false;
	        if (operateTexture != null)
	        {
	            Destroy(operateTexture);
	        }
	
	        CleanCount = 0;
	
	        var ret = ResourceManager.PrepareResourceSync<Texture2D>("Texture/Statue/clean_mask.png", true, false);
	        operateTexture = new Texture2D(ret.width, ret.height, TextureFormat.ARGB32, false);
	        clors = ret.GetPixels(0, 0, ret.width, ret.height);
	
	        saleVec = new Vector2(Mask.width/(float) ret.width, Mask.height/(float) ret.height);
	        operateTexture.SetPixels(0, 0, operateTexture.width, operateTexture.height, clors);
	        FullCount = operateTexture.width*operateTexture.height;
	        //Destroy(ret);
	        Mask.mainTexture = operateTexture;
	        operateTexture.Apply();
	        ScheduleLab.text = string.Format("{0}%", 100);
	    }
	
	    public void Restart()
	    {
	        IsStart = true;
	        TouchCollider.enabled = true;
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
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (IsStart == false)
	        {
	            return;
	        }
	        if (isHover && isPress)
	        {
	            calculateClear();
	            if (flag != CleanCount)
	            {
	                flag = CleanCount;
	                var p = CleanCount*100/FullCount;
	                ScheduleLab.text = string.Format("{0}%", p);
	
	                if (CheckIsFinish() && !IsFinish)
	                {
	                    IsFinish = true;
	                    Finish();
	                    OnCleanFinish();
	                }
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
	
	    public delegate void CleanFinish();
	}
}