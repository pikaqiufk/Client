using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AnalyzeMat : AnalyzeBase
{
    public AnalyzeMat(ResourceRedundanceManager mgr) : base(mgr)
    {

    }

    public override int GetAnalyzeType()
    {
        return (int)(AnalyzeType.AT_MAT);
    }

    public override void Analyze(ResourceUnit res)
    {
        AnalyzeYAMLResource(res);

    }

}

