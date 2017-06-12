using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AnalyzePrefab : AnalyzeBase
{
    public AnalyzePrefab(ResourceRedundanceManager mgr) : base(mgr)
    {
    }

    public override int GetAnalyzeType()
    {
        return (int)(AnalyzeType.AT_PREFAB);
    }

    public override void Analyze(ResourceUnit res)
    {
        AnalyzeYAMLResource(res);

    }


}
