#region using

using System.Collections.Generic;

#endregion

namespace GameUI
{
	public class AutoPiont
	{
	    public AutoPiont()
	    {
	        for (var i = 0; i < 4; i++)
	        {
	            AutoNodes.Add(new AutoPtNode());
	        }
	    }
	
	    public List<AutoPtNode> AutoNodes = new List<AutoPtNode>(4);
	    private readonly AutoPtNodeRateCompare RateCompare = new AutoPtNodeRateCompare();
	    private readonly AutoPtNodeWeightCompare RateWeight = new AutoPtNodeWeightCompare();
	    public int TotalPoint { get; set; }
	
	    private void Distribut(int index, float targerRate = 0.0f)
	    {
	        if (TotalPoint == 0)
	        {
	            return;
	        }
	        var totalWeight = 0;
	        var totalNeed = 0;
	
	        for (var i = 0; i <= index; i++)
	        {
	            var node = AutoNodes[i];
	            if (node.Weight == 0)
	            {
	                continue;
	            }
	            totalWeight += node.Weight;
	            if (targerRate > 0)
	            {
	                totalNeed += ((int) (node.Weight*targerRate) - node.Point);
	            }
	        }
	        if (targerRate > 0)
	        {
	            if (TotalPoint > totalNeed)
	            {
	                TotalPoint -= totalNeed;
	                for (var i = 0; i <= index; i++)
	                {
	                    var node = AutoNodes[i];
	                    if (node.Weight == 0)
	                    {
	                        continue;
	                    }
	                    node.Point = (int) (node.Weight*targerRate);
	                }
	            }
	            else
	            {
	                if (index > 0)
	                {
	                    Distribut(index - 1, targerRate);
	                }
	                else
	                {
	                    AutoNodes[0].Point += TotalPoint;
	                    TotalPoint = 0;
	                }
	            }
	        }
	        else
	        {
	            var rate = TotalPoint/(float) totalWeight;
	            for (var i = 0; i <= index; i++)
	            {
	                var node = AutoNodes[i];
	                if (node.Weight == 0)
	                {
	                    continue;
	                }
	                var p = (int) (rate*node.Weight);
	                node.Point += p;
	                TotalPoint -= p;
	            }
	
	            if (TotalPoint > 0)
	            {
	                if (index > 0)
	                {
	                    Distribut(index - 1, targerRate);
	                }
	                else
	                {
	                    AutoNodes[0].Point += TotalPoint;
	                    TotalPoint = 0;
	                }
	            }
	        }
	    }
	
	    public void DistributionPoint()
	    {
	        AutoNodes.Sort(RateCompare);
	        for (var i = 1; i < 4; i++)
	        {
	            var curNode = AutoNodes[i];
	            AutoNodes.Sort(0, i, RateWeight);
	            if (curNode.Weight == 0)
	            {
	                if (i == 1)
	                {
	                    AutoNodes[0].Point += TotalPoint;
	                    TotalPoint = 0;
	                }
	                else
	                {
	                    Distribut(i - 1);
	                }
	            }
	            else
	            {
	                var targetRate = curNode.Point/(float) curNode.Weight;
	                Distribut(i - 1, targetRate);
	            }
	        }
	    }
	
	    public class AutoPtNodeRateCompare : IComparer<AutoPtNode>
	    {
	        public int Compare(AutoPtNode a, AutoPtNode b)
	        {
	            if (a.Weight == 0)
	            {
	                return 1;
	            }
	            if (b.Weight == 0)
	            {
	                return -1;
	            }
	            var aRate = (float) a.Point/a.Weight;
	            var bRate = (float) b.Point/b.Weight;
	            if (bRate > aRate)
	            {
	                return -1;
	            }
	            if (bRate < aRate)
	            {
	                return 1;
	            }
	            return 0;
	        }
	    }
	
	    public class AutoPtNodeWeightCompare : IComparer<AutoPtNode>
	    {
	        public int Compare(AutoPtNode a, AutoPtNode b)
	        {
	            if (a.Weight == 0)
	            {
	                return 1;
	            }
	            if (b.Weight == 0)
	            {
	                return -1;
	            }
	            return b.Weight - a.Weight;
	        }
	    }
	
	    public class AutoPtNode
	    {
	        public int Index;
	        public int Point;
	        public int Weight;
	    }
	}
}