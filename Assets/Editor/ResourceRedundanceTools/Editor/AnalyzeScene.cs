﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AnalyzeScene : AnalyzeBase
{
    public AnalyzeScene(ResourceRedundanceManager mgr) : base(mgr)
    {

    }

    public override int GetAnalyzeType()
    {
        return (int)(AnalyzeType.AT_SCENE);
    }

    public override void Analyze(ResourceUnit res)
    {
        AnalyzeYAMLResource(res);

    }


}
