using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class NGUIWidgetsDepthManager : ScriptableWizard
{
    public GameObject NGUIRootObject = null;
    public int numberOfLevelsToSearch = 10;

    Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>
        widgetHierarchy = new
        Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>();

    protected Vector2 scrollPosition = Vector2.zero;

    [MenuItem("NGUI/Widgets Depth Manager")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("NGUI Widgets Depth Manager", typeof(NGUIWidgetsDepthManager));
    }

    void OnGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);

        this.NGUIRootObject = (GameObject)
            EditorGUILayout.ObjectField("Root Object", this.NGUIRootObject, typeof(GameObject), true, GUILayout.ExpandWidth(true));

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Find Widgets Under Root"))
            {
                this.findObjs();
                this.Repaint();
            }
            if (GUILayout.Button("Root Object Clear"))
            {
                this.NGUIRootObject = null;
                this.widgetHierarchy.Clear();
                this.Repaint();
            }
        }
        GUILayout.EndHorizontal();

        if (this.widgetHierarchy.Count == 0)
        {
            GUILayout.Label("Use 'Find Widgets Under Root' to get the widgets and their depths and Z Pos.");
        }
        else
        {
            if (this.NGUIRootObject == null)
            {
                this.NGUIRootObject = null;
                this.widgetHierarchy.Clear();
                this.Repaint();
            }
            else
            {
                this.drawWidgetsScrollView();
                GUILayout.Label("If you have more than one atlas 'RenderQueue' value is equal \n 'Atlas Z position' of the small value of the widget will appear in front of the other.");
            }

        }

    }

    protected void drawWidgetsScrollView()
    {
        this.scrollPosition =
            EditorGUILayout.BeginScrollView(this.scrollPosition);

        List<string> sortedPanelPaths = new List<string>(
            this.widgetHierarchy.Keys);
        sortedPanelPaths.Sort();
        {
            var __list1 = sortedPanelPaths;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var panelPath = (string)__list1[__i1];
                {
                    Vector3 tPanelPos;
                    GUILayout.Label(string.Format("Panel: {0}", panelPath), EditorStyles.boldLabel);

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.ObjectField(
                            this.objectAtTransPathRoot(panelPath),
                            typeof(GameObject), false);
                        GameObject tPanelObj = this.objectAtTransPathRoot(panelPath);
                        tPanelPos = tPanelObj.transform.localPosition;
                        EditorGUILayout.LabelField("Z Pos:", GUILayout.Width(40), GUILayout.MaxWidth(40));
                        tPanelPos.z = EditorGUILayout.FloatField(tPanelObj.transform.localPosition.z, GUILayout.MaxWidth(60));
                        tPanelObj.transform.localPosition = tPanelPos;
                    }
                    GUILayout.EndHorizontal();

                    Dictionary<string, Dictionary<int, List<UIWidget>>>
                        hierarchyUnderPanel = this.widgetHierarchy[panelPath];

                    List<string> sortedAtlasNames = new List<string>(
                        hierarchyUnderPanel.Keys);
                    sortedAtlasNames.Sort();

                    int tAtlasCnt = 0;
                    {
                        var __list11 = sortedAtlasNames;
                        var __listCount11 = __list11.Count;
                        for (int __i11 = 0; __i11 < __listCount11; ++__i11)
                        {
                            var atlasName = (string)__list11[__i11];
                            {
                                tAtlasCnt++;
                                Dictionary<int, List<UIWidget>> hierarchyUnderAtlas =
                                        hierarchyUnderPanel[atlasName];

                                List<int> sortedDepths = new List<int>(
                                    hierarchyUnderAtlas.Keys);
                                sortedDepths.Sort();

                                float tZPos = 0f;
                                float tMinZPos = float.MaxValue;
                                float tMaxZpos = -float.MaxValue;
                                int tTotWidgetCnt = 0;
                                int i = 0;
                                UIWidget tSetWidget = null;

                                GUILayout.BeginVertical();
                                {
                                    //					GUILayout.Space(32.0f);
                                    GUILayout.Label(string.Format("{0}. Atlas: {1}", tAtlasCnt, atlasName), EditorStyles.boldLabel);
                                    {
                                        var __list13 = sortedDepths;
                                        var __listCount13 = __list13.Count;
                                        for (int __i13 = 0; __i13 < __listCount13; ++__i13)
                                        {
                                            var depth = (int)__list13[__i13];
                                            {
                                                List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];
                                                {
                                                    var __list20 = widgetsAtDepth;
                                                    var __listCount20 = __list20.Count;
                                                    for (int __i20 = 0; __i20 < __listCount20; ++__i20)
                                                    {
                                                        var widget = (UIWidget)__list20[__i20];
                                                        {
                                                            Vector3 tPos = widget.transform.localPosition;
                                                            tMinZPos = Mathf.Min(tMinZPos, tPos.z);
                                                            tMaxZpos = Mathf.Max(tMaxZpos, tPos.z);
                                                            tTotWidgetCnt++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    {
                                        var __list15 = sortedDepths;
                                        var __listCount15 = __list15.Count;
                                        for (int __i15 = 0; __i15 < __listCount15; ++__i15)
                                        {
                                            var depth = (int)__list15[__i15];
                                            {
                                                List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];
                                                {
                                                    var __list21 = widgetsAtDepth;
                                                    var __listCount21 = __list21.Count;
                                                    for (int __i21 = 0; __i21 < __listCount21; ++__i21)
                                                    {
                                                        var widget = (UIWidget)__list21[__i21];
                                                        {
                                                            i++;
                                                            Vector3 tPos = widget.transform.localPosition;

                                                            if (i == tTotWidgetCnt)
                                                            {
                                                                tPos.z = tMaxZpos;
                                                                widget.transform.localPosition = tPos;
                                                                tSetWidget = widget;
                                                            }
                                                            else
                                                            {
                                                                tPos.z = tMinZPos;
                                                                widget.transform.localPosition = tPos;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    float tSumPos = tMinZPos + tPanelPos.z;
                                    int tDiv;
                                    if (tTotWidgetCnt > 1)
                                    {
                                        tDiv = 2;
                                        tZPos = tSumPos + (tMaxZpos - tMinZPos) / tDiv;
                                    }
                                    else
                                    {
                                        tDiv = tTotWidgetCnt;
                                        tZPos = tSumPos;
                                    }


                                    Vector3 tAtlasPos = tSetWidget.transform.localPosition;

                                    GUILayout.BeginHorizontal();
                                    {
                                        int t_QueNum;
                                        GUILayout.Space(30f);
                                        EditorGUILayout.LabelField("1) RenderQueue:", GUILayout.Width(105), GUILayout.MaxWidth(105));

                                        tSetWidget.material.renderQueue = EditorGUILayout.IntField(tSetWidget.material.renderQueue, GUILayout.Width(50), GUILayout.MinWidth(50));
                                        NGUISetRenderQueue t_Ques = (NGUISetRenderQueue)tSetWidget.GetComponent(typeof(NGUISetRenderQueue));

                                        if (t_Ques != null)
                                        {
                                            t_Ques.m_RenderQueueNum = tSetWidget.material.renderQueue;
                                        }

                                        if (GUILayout.Button("Set", GUILayout.Width(50), GUILayout.MaxWidth(50)))
                                        {
                                            t_QueNum = tSetWidget.material.renderQueue;
                                            if (t_Ques == null)
                                            {
                                                t_Ques = (NGUISetRenderQueue)tSetWidget.gameObject.AddComponent("NGUISetRenderQueue");
                                            }

                                            t_Ques.m_RenderQueueNum = t_QueNum;
                                            tSetWidget.material.renderQueue = t_QueNum;
                                            Selection.activeObject = t_Ques.gameObject;
                                            EditorApplication.SaveScene();
                                            GUIUtility.keyboardControl = 0;
                                        }
                                        EditorGUILayout.LabelField(", 2) Atlas Z Pos:", GUILayout.Width(100), GUILayout.MaxWidth(100));
                                        tAtlasPos.z = (EditorGUILayout.FloatField(tZPos, GUILayout.Width(50), GUILayout.MinWidth(50)) - tSumPos) * tDiv + tMinZPos;
                                        tSetWidget.transform.localPosition = tAtlasPos;

                                        if (GUILayout.Button("Atlas Z Pos Clear", GUILayout.Width(120)))
                                        {
                                            {
                                                var __list17 = sortedDepths;
                                                var __listCount17 = __list17.Count;
                                                for (int __i17 = 0; __i17 < __listCount17; ++__i17)
                                                {
                                                    var depth = (int)__list17[__i17];
                                                    {
                                                        List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];
                                                        {
                                                            var __list22 = widgetsAtDepth;
                                                            var __listCount22 = __list22.Count;
                                                            for (int __i22 = 0; __i22 < __listCount22; ++__i22)
                                                            {
                                                                var widget = (UIWidget)__list22[__i22];
                                                                {
                                                                    Vector3 tPos = widget.transform.localPosition;
                                                                    tPos.z = 0;
                                                                    widget.transform.localPosition = tPos;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            GUIUtility.keyboardControl = 0;
                                        }

                                    }
                                    GUILayout.EndHorizontal();

                                }
                                GUILayout.EndVertical();
                                {
                                    var __list19 = sortedDepths;
                                    var __listCount19 = __list19.Count;
                                    for (int __i19 = 0; __i19 < __listCount19; ++__i19)
                                    {
                                        var depth = (int)__list19[__i19];
                                        {
                                            List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];
                                            {
                                                var __list23 = widgetsAtDepth;
                                                var __listCount23 = __list23.Count;
                                                for (int __i23 = 0; __i23 < __listCount23; ++__i23)
                                                {
                                                    var widget = (UIWidget)__list23[__i23];
                                                    {
                                                        this.drawDepthsWidget(depth, widget, tPanelPos.z);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    protected void drawDepthsWidget(int depth, UIWidget widget, float tPanelZPos)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(64.0f);
            EditorGUILayout.ObjectField(widget, typeof(UIWidget), true, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                Selection.activeObject = widget.gameObject;
            }

            GUILayout.Space(10.0f);
            widget.depth = EditorGUILayout.IntField("Depth:", widget.depth);

        }
        GUILayout.EndHorizontal();
    }

    protected void findObjs()
    {
        if (this.NGUIRootObject == null)
        {
            return;
        }

        this.widgetHierarchy.Clear();

        this.findObjsUnderTransformTree(0, this.numberOfLevelsToSearch - 1, this.NGUIRootObject.transform, "", ref this.widgetHierarchy);
    }

    protected void findObjsUnderTransformTree(int recursionLevel,
        int maxRecursionLevel, Transform currentTransform,
        string parentPanelPath,
        ref Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>
            widgetHierarchy)
    {
        if (recursionLevel > maxRecursionLevel)
        {
            return;
        }

        UIPanel currentPanel = currentTransform.gameObject.GetComponent<UIPanel>();

        if (currentPanel != null)
        {
            string underPath = AnimationUtility.CalculateTransformPath(currentTransform, this.NGUIRootObject.transform);
            if (underPath.Length > 0)
            {
                parentPanelPath = string.Format("{0}/{1}", this.NGUIRootObject.name, underPath);
            }
            else
            {
                parentPanelPath = this.NGUIRootObject.name;
            }
        }
        {
            // foreach(var child in currentTransform)
            var __enumerator2 = (currentTransform).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var child = (Transform)__enumerator2.Current;
                {
                    UIWidget widget = child.gameObject.GetComponent<UIWidget>();
                    if (widget != null)
                    {
                        string atlasName =
                            NGUIWidgetsDepthManager.atlasNameFromObj(widget);

                        int depth = widget.depth;

                        Dictionary<string, Dictionary<int, List<UIWidget>>>
                            hierarchyUnderPanel = null;

                        if (!widgetHierarchy.TryGetValue(parentPanelPath, out hierarchyUnderPanel))
                        {
                            hierarchyUnderPanel =
                                new Dictionary<string, Dictionary<int, List<UIWidget>>>();
                            widgetHierarchy[parentPanelPath] = hierarchyUnderPanel;
                        }

                        Dictionary<int, List<UIWidget>> hierarchyUnderAtlas = null;

                        if (!hierarchyUnderPanel.TryGetValue(atlasName, out hierarchyUnderAtlas))
                        {
                            hierarchyUnderAtlas = new Dictionary<int, List<UIWidget>>();
                            hierarchyUnderPanel[atlasName] = hierarchyUnderAtlas;
                        }

                        List<UIWidget> widgetsAtDepth = null;

                        if (!hierarchyUnderAtlas.TryGetValue(depth, out widgetsAtDepth))
                        {
                            widgetsAtDepth = new List<UIWidget>();
                            hierarchyUnderAtlas[depth] = widgetsAtDepth;
                        }
                        widgetsAtDepth.Add(widget);
                    }

                    this.findObjsUnderTransformTree(recursionLevel + 1,
                        maxRecursionLevel, child, parentPanelPath, ref widgetHierarchy);
                }
            }
        }
    }

    public GameObject objectAtTransPathRoot(string path)
    {
        GameObject objectAtPath = null;
        string subPath = path.Substring(this.NGUIRootObject.name.Length);
        if (subPath.Length < 1)
        {
            objectAtPath = this.NGUIRootObject;
        }
        else
        {
            subPath = subPath.Substring(1);
            Transform transformAtPath = this.NGUIRootObject.transform.Find(subPath);
            if (transformAtPath != null)
            {
                objectAtPath = transformAtPath.gameObject;
            }
        }
        return objectAtPath;

    }

    static public string atlasNameFromObj(UIWidget widget)
    {
        string atlasName = "Not Atlas";
        UIAtlas atlas = null;

        if (widget is UISprite)
        {
            atlas = ((UISprite)widget).atlas;
        }
        else if (widget is UILabel)
        {
            UIFont font = ((UILabel)widget).font;
            atlas = font.atlas;
        }

        if (atlas != null)
        {
            atlasName = atlas.gameObject.name;
        }
        return atlasName;
    }


}