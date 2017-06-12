using PigeonCoopToolkit.Effects.Trails;
using UnityEditor;
using System.Collections.Generic;

namespace PigeonCoopToolkit.TouchInputManager.Editor
{
    [InitializeOnLoad]
    public class ExecutionOrderSetter
    {
        static ExecutionOrderSetter()
        {
            var scriptlist = new List<string>
            {
                typeof (TrailRenderer_Base).Name,
                typeof (Trail).Name,
                typeof (SmoothTrail).Name,
                typeof (SmokeTrail).Name,
                typeof (SmokePlume).Name
            };
            {
                // foreach(var monoScript in MonoImporter.GetAllRuntimeMonoScripts())
                var __enumerator1 = (MonoImporter.GetAllRuntimeMonoScripts()).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var monoScript = (MonoScript)__enumerator1.Current;
                    {
                        if (!scriptlist.Contains(monoScript.name)) continue;

                        if (MonoImporter.GetExecutionOrder(monoScript) != 1000)
                            MonoImporter.SetExecutionOrder(monoScript, 1000);
                    }
                }
            }
        }
    }
}
