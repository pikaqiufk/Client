
#region using

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



#endregion

namespace GameUI
{
	public class PuzzleMoveFrame : MonoBehaviour
	{
	    public const int MaxLength = 4;
	    public UITexture Background;
	    public Transform Container;
	    private List<PuzzleNode> allPiece = new List<PuzzleNode>();
	    private bool isInit;
	    private PuzzleNode lastNode;
	    private readonly List<Vector2> originalList = new List<Vector2>();
	    private readonly Dictionary<int, PuzzleNode> pieceList = new Dictionary<int, PuzzleNode>();
	    private int xLength = 4;
	    private int yLength = 4;
	    public PuzzleFinish OnPuzzleFinish;
	    public int TotalHeight = 500;
	    public int TotalWidth = 400;
	    public bool IsFinish { get; set; }
	    public bool IsStart { get; set; }
	
	    private int HNode
	    {
	        get { return TotalHeight/yLength; }
	    }
	
	    private int WNode
	    {
	        get { return TotalWidth/xLength; }
	    }
	
	    public bool CheckIsFinish()
	    {
	        {
	            // foreach(var node in mPieceList)
	            var __enumerator2 = (pieceList).GetEnumerator();
	            while (__enumerator2.MoveNext())
	            {
	                var node = __enumerator2.Current;
	                {
	                    if (node.Value.Show == false)
	                    {
	                        continue;
	                    }
	                    if (node.Value.CurrentX != node.Value.TargetX
	                        || node.Value.CurrentY != node.Value.TargetY)
	                    {
	                        return false;
	                    }
	                }
	            }
	        }
	        return true;
	    }
	
	    public void Finish()
	    {
	        IsFinish = true;
	        Background.enabled = true;
	        {
	            // foreach(var node in mPieceList)
	            var __enumerator3 = (pieceList).GetEnumerator();
	            while (__enumerator3.MoveNext())
	            {
	                var node = __enumerator3.Current;
	                {
	                    if (node.Value.Show == false)
	                    {
	                        continue;
	                    }
	                    node.Value.Texture.transform.localScale = Vector3.one;
	                    node.Value.Texture.gameObject.SetActive(false);
	                    node.Value.Show = false;
	                    var collder = node.Value.Texture.GetComponent<BoxCollider>();
	                    if (collder)
	                    {
	                        collder.enabled = false;
	                    }
	                }
	            }
	        }
	    }
	
	    private void Init()
	    {
	        lastNode = null;
	        pieceList.Clear();
	        for (var i = 0; i < Container.childCount; i++)
	        {
	            var node = new PuzzleNode();
	            node.Show = false;
	            var c = Container.GetChild(i);
	            var t = c.GetComponent<UITexture>();
	            NGUITools.UpdateWidgetCollider(t.gameObject);
	            node.Texture = t;
	            var tTransform = t.transform;
	            tTransform.gameObject.SetActive(false);
	            tTransform.collider.enabled = false;
	            var s = tTransform.GetChild(0);
	            node.Select = s;
	            node.Select.gameObject.SetActive(false);
	            var index = i/MaxLength*10 + i%MaxLength;
	            pieceList.Add(index, node);
	            var e = c.GetComponent<UIEventTrigger>();
	            e.onClick.Add(new EventDelegate(() => { OnTextureClick(index); }));
	            tTransform.localScale = Vector3.one*0.95f;
	        }
	        isInit = true;
	    }
	
	    public void OnTextureClick(int index)
	    {
	        var node = pieceList[index];
	        if (lastNode == null)
	        {
	            lastNode = node;
	            lastNode.Select.gameObject.SetActive(true);
	        }
	        else
	        {
	            if (lastNode == node)
	            {
	                lastNode.Select.gameObject.SetActive(false);
	                lastNode = null;
	                return;
	            }
	            lastNode.Select.gameObject.SetActive(false);
	            var x = lastNode.CurrentX;
	            var y = lastNode.CurrentY;
	            lastNode.CurrentX = node.CurrentX;
	            lastNode.CurrentY = node.CurrentY;
	            node.CurrentX = x;
	            node.CurrentY = y;
	            node.Texture.transform.localPosition = new Vector3(-TotalWidth/2 + WNode/2 + node.CurrentX*WNode,
	                -TotalHeight/2 + HNode/2 + node.CurrentY*HNode, 0);
	            lastNode.Texture.transform.localPosition = new Vector3(-TotalWidth/2 + WNode/2 + lastNode.CurrentX*WNode,
	                -TotalHeight/2 + HNode/2 + lastNode.CurrentY*HNode, 0);
	            lastNode = null;
	            if (CheckIsFinish())
	            {
	                Finish();
	                OnPuzzleFinish();
	            }
	        }
	    }
	
	    public void Reset()
	    {
	        if (isInit == false)
	        {
	            Init();
	        }
	
	        foreach (var node in pieceList)
	        {
	            node.Value.Texture.gameObject.SetActive(false);
	            node.Value.Show = false;
	            var collder = node.Value.Texture.GetComponent<BoxCollider>();
	            if (collder)
	            {
	                collder.enabled = false;
	            }
	        }
	
	        IsStart = false;
	        IsFinish = false;
	        Background.enabled = true;
	        lastNode = null;
	
	        xLength = Random.Range(2, 4 + 1);
	        yLength = Random.Range(3, 4 + 1);
	
	        originalList.Clear();
	        for (var i = 0; i < xLength; i++)
	        {
	            for (var j = 0; j < yLength; j++)
	            {
	                originalList.Add(new Vector2(i, j));
	            }
	        }
	
	        var list = new List<Vector2>();
	        var count = xLength*yLength;
	        for (var i = 0; i < count; i++)
	        {
	            list.Insert(Random.Range(0, i), originalList[i]);
	        }
	
	        var fx = 1.0f/xLength;
	        var fy = 1.0f/yLength;
	        for (var i = 0; i < xLength; i++)
	        {
	            for (var j = 0; j < yLength; j++)
	            {
	                var index = i*10 + j;
	                var node = pieceList[index];
	                var t = node.Texture;
	                t.mainTexture = Background.mainTexture;
	                t.uvRect = new Rect(fx*(i), fy*(j), fx, fy);
	                node.Select.gameObject.SetActive(false);
	                node.TargetX = i;
	                node.TargetY = j;
	                node.Texture.SetDimensions(WNode, HNode);
	                var posIndex = i*yLength + j;
	                var pos = list[posIndex];
	                node.CurrentX = (int) pos.x;
	                node.CurrentY = (int) pos.y;
	                node.Show = true;
	                t.transform.localPosition = new Vector3(-TotalWidth/2 + WNode/2 + node.CurrentX*WNode,
	                    -TotalHeight/2 + HNode/2 + node.CurrentY*HNode, 0);
	                t.gameObject.SetActive(false);
	            }
	        }
	    }
	
	    public void ReStart()
	    {
	        IsStart = true;
	        Background.enabled = false;
	        {
	            // foreach(var node in mPieceList)
	            var __enumerator1 = (pieceList).GetEnumerator();
	            while (__enumerator1.MoveNext())
	            {
	                var node = __enumerator1.Current;
	                {
	                    if (node.Value.Show == false)
	                    {
	                        continue;
	                    }
	                    node.Value.Texture.transform.localScale = Vector3.one*0.95f;
	                    node.Value.Texture.gameObject.SetActive(true);
	                    var collder = node.Value.Texture.GetComponent<BoxCollider>();
	                    if (collder)
	                    {
	                        collder.enabled = true;
	                    }
	                }
	            }
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        Background.width = TotalWidth;
	        Background.height = TotalHeight;
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public delegate void PuzzleFinish();
	
	    public class PuzzleNode
	    {
	        public int CurrentX;
	        public int CurrentY;
	        public Transform Select;
	        public bool Show;
	        public int TargetX;
	        public int TargetY;
	        public UITexture Texture;
	    }
	}
}