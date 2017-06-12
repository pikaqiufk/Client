//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventSystem;

/// <summary>
/// Draw Call Viewer shows a list of draw calls created by NGUI and lets you hide them selectively.
/// </summary>

public class UIDrawCallViewer : EditorWindow
{
    static public UIDrawCallViewer instance;

    enum Visibility
    {
        Visible,
        Hidden,
    }

    enum ShowFilter
    {
        AllPanels,
        SelectedPanel,
    }

    Vector2 mScroll = Vector2.zero;

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }
    void OnSelectionChange() { Repaint(); }

    /// <summary>
    /// Draw the custom wizard.
    /// </summary>

    void OnGUI()
    {
        var dcs = UIDrawCall.activeList;

        dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
        {
            return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
        });

        if (dcs.size == 0)
        {
            EditorGUILayout.HelpBox("No NGUI draw calls present in the scene", MessageType.Info);
            return;
        }

        UIPanel selectedPanel = NGUITools.FindInParents<UIPanel>(Selection.activeGameObject);

        if (selectedPanel != null)
        {
            if (GUILayout.Button("Auto adjust depth 1"))
            {
                AutoAdjustDepth1(selectedPanel);
            }
            if (GUILayout.Button("Auto adjust depth 2"))
            {
                AutoAdjustDepth2(dcs);
            }
            if (GUILayout.Button("Auto adjust depth 3"))
            {
                AutoAdjustDepth3(dcs);
            }
            if (GUILayout.Button("Auto adjust depth 4"))
            {
                AutoAdjustDepth4(selectedPanel);
            }
        }

        GUILayout.Space(12f);

        NGUIEditorTools.SetLabelWidth(100f);
        ShowFilter show = (NGUISettings.showAllDCs ? ShowFilter.AllPanels : ShowFilter.SelectedPanel);

        if ((ShowFilter)EditorGUILayout.EnumPopup("Draw Call Filter", show) != show)
            NGUISettings.showAllDCs = !NGUISettings.showAllDCs;

        GUILayout.Space(6f);

        if (selectedPanel == null && !NGUISettings.showAllDCs)
        {
            EditorGUILayout.HelpBox("No panel selected", MessageType.Info);
            return;
        }

        NGUIEditorTools.SetLabelWidth(80f);
        mScroll = GUILayout.BeginScrollView(mScroll);

        int dcCount = 0;

        for (int i = 0; i < dcs.size; ++i)
        {
            UIDrawCall dc = dcs[i];
            string key = "Draw Call " + (i + 1);
            bool highlight = (selectedPanel == null || selectedPanel == dc.manager);

            if (!highlight)
            {
                if (!NGUISettings.showAllDCs) continue;

                if (UnityEditor.EditorPrefs.GetBool(key, true))
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f);
                }
                else
                {
                    GUI.contentColor = new Color(0.85f, 0.85f, 0.85f);
                }
            }
            else GUI.contentColor = Color.white;

            ++dcCount;
            string name = key + " of " + dcs.size;
            if (!dc.isActive) name = name + " (HIDDEN)";
            else if (!highlight) name = name + " (" + dc.manager.name + ")";

            if (NGUIEditorTools.DrawHeader(name, key))
            {
                GUI.color = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);

                NGUIEditorTools.BeginContents();
                EditorGUILayout.ObjectField("Material", dc.dynamicMaterial, typeof(Material), false);

                int count = 0;

                for (int a = 0; a < UIPanel.list.Count; ++a)
                {
                    UIPanel p = UIPanel.list[a];

                    for (int b = 0; b < p.widgets.Count; ++b)
                    {
                        UIWidget w = p.widgets[b];
                        if (w.drawCall == dc) ++count;
                    }
                }

                string myPath = NGUITools.GetHierarchy(dc.manager.cachedGameObject);
                string remove = myPath + "\\";
                string[] list = new string[count + 1];
                list[0] = count.ToString();
                count = 0;

                for (int a = 0; a < UIPanel.list.Count; ++a)
                {
                    UIPanel p = UIPanel.list[a];

                    for (int b = 0; b < p.widgets.Count; ++b)
                    {
                        UIWidget w = p.widgets[b];

                        if (w.drawCall != dc) continue;
                        string path = NGUITools.GetHierarchy(w.cachedGameObject);
                        list[++count] = count + ". " + (string.Equals(path, myPath) ? w.name : path.Replace(remove, ""));
                    }
                }

                GUILayout.BeginHorizontal();
                int sel = EditorGUILayout.Popup("Widgets", 0, list);
                NGUIEditorTools.DrawPadding();
                GUILayout.EndHorizontal();

                if (sel != 0)
                {
                    count = 0;

                    for (int a = 0; a < UIPanel.list.Count; ++a)
                    {
                        UIPanel p = UIPanel.list[a];

                        for (int b = 0; b < p.widgets.Count; ++b)
                        {
                            UIWidget w = p.widgets[b];

                            if (w.drawCall == dc && ++count == sel)
                            {
                                Selection.activeGameObject = w.gameObject;
                                break;
                            }
                        }
                    }
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Render Q", dc.finalRenderQueue.ToString(), GUILayout.Width(120f));
                bool draw = (Visibility)EditorGUILayout.EnumPopup(dc.isActive ? Visibility.Visible : Visibility.Hidden) == Visibility.Visible;
                NGUIEditorTools.DrawPadding();
                GUILayout.EndHorizontal();

                if (dc.isActive != draw)
                {
                    dc.isActive = draw;
                    NGUITools.SetDirty(dc.manager);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Triangles", dc.triangles.ToString(), GUILayout.Width(120f));

                if (dc.manager != selectedPanel)
                {
                    if (GUILayout.Button("Select the Panel"))
                    {
                        Selection.activeGameObject = dc.manager.gameObject;
                    }
                    NGUIEditorTools.DrawPadding();
                }
                GUILayout.EndHorizontal();

                if (dc.manager.clipping != UIDrawCall.Clipping.None && !dc.isClipped)
                {
                    EditorGUILayout.HelpBox("You must switch this material's shader to Unlit/Transparent Colored or Unlit/Premultiplied Colored in order for clipping to work.",
                        MessageType.Warning);
                }

                NGUIEditorTools.EndContents();
                GUI.color = Color.white;
            }
        }

        if (dcCount == 0)
        {
            EditorGUILayout.HelpBox("No draw calls found", MessageType.Info);
        }

        GUILayout.EndScrollView();
    }

    public static int WidgetComparison(UIWidget x, UIWidget y)
    {
        var xDepth = x.depth;
        var yDepth = y.depth;
        if (xDepth < yDepth)
            return -1;
        if (xDepth > yDepth)
            return 1;
        return 0;
    }

    private static void AutoAdjustDepth1(UIPanel root)
    {
        root.ActivePanel();
    }

    //处理depth
    private static void AutoAdjustDepth2(BetterList<UIDrawCall> dcs)
    {
        var list = new List<UIWidget>();
        {
            // foreach(var dc in dcs)
            var __enumerator1 = (dcs).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var dc = __enumerator1.Current;
                {
                    list.AddRange(from panel in UIPanel.list from w in panel.widgets where w.drawCall == dc select w);
                }
            }
        }
        var depth = -1;
        {
            var __list2 = list;
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var w = __list2[__i2];
                {
                    if (depth == -1)
                    {
                        depth = Math.Max(0, w.depth);
                    }
                    w.depth = depth++;
                }
            }
        }
    }

    private static void AutoAdjustDepth3(BetterList<UIDrawCall> dcs)
    {
        if (UIPanel.list.Count == 0)
        {
            return;
        }

        //把所有dc对应的widgets搞出来
        var dcWidgets = new List<List<UIWidget>>();
        //depth => UIWidget
        var widgetSequence = new Dictionary<int, UIWidget>();
        {
            // foreach(var dc in dcs)
            var __enumerator5 = (dcs).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var dc = __enumerator5.Current;
                {
                    var widgets = new List<UIWidget>();
                    foreach (var p in UIPanel.list)
                    {
                        foreach (var w in p.widgets.ToArray())
                        {
                            if (w.drawCall != dc)
                            {
                                continue;
                            }
                            widgets.Add(w);
                            try
                            {
                                widgetSequence.Add(w.depth, w);
                            }
                            catch (ArgumentException)
                            {
                                Logger.Error("w = {0}, depth = {1}, same with {2}", w, w.depth, widgetSequence[w.depth]);
                                throw;
                            }
                        }
                    }
                    widgets.Sort(WidgetComparison);
                    dcWidgets.Add(widgets);
                }
            }
        }

        // 找到下一个相同atlas的dc
        // 看看能不能合并
        // 如果能合并，那合并完成后继续
        // 如果不能合并，那选下一个继续
        for (int i = 0, imax = dcs.size; i < imax; i++)
        {
            var widgetsi = dcWidgets[i];
            if (widgetsi.Count == 0)
            {
                continue;
            }
            var dci = dcs[i];
            // 找到下一个相同的atlas
            for (int j = i + 1; j < imax; j++)
            {
                var widgetsj = dcWidgets[j];
                if (widgetsj.Count == 0)
                {
                    continue;
                }
                var dcj = dcs[j];
                var l = dci.dynamicMaterial;
                var r = dcj.dynamicMaterial;
                if (l.shader != r.shader || l.mainTexture != r.mainTexture) continue;
                var passed = true;
                {
                    var __list7 = widgetsj;
                    var __listCount7 = __list7.Count;
                    for (int __i7 = 0; __i7 < __listCount7; ++__i7)
                    {
                        var w = __list7[__i7];
                        {
                            var w1Bounds = w.GetBounds();
                            for (int k = i + 1; k < j; ++k)
                            {
                                var widgetsk = dcWidgets[k];
                                foreach (var wk in widgetsk)
                                {
                                    if (wk.GetBounds().Intersects(w1Bounds))
                                    {
                                        passed = false;
                                        break;
                                    }
                                }
                                if (!passed)
                                {
                                    break;
                                }
                            }
                            if (!passed)
                            {
                                break;
                            }
                        }
                    }
                }
                if (passed)
                {
                    var depth0 = widgetsi[widgetsi.Count - 1].depth + 1;
                    var depth1 = widgetsj[0].depth;
                    var dic = new Dictionary<int, UIWidget>();
                    for (int k = depth0; k < depth1; k++)
                    {
                        var w = widgetSequence[k];
                        w.depth += widgetsj.Count;
                        dic.Add(w.depth, w);
                    }
                    for (int k = 0; k < widgetsj.Count; k++)
                    {
                        var w = widgetsj[k];
                        w.drawCall = dci;
                        w.depth = depth0 + k;
                        dic.Add(w.depth, w);
                        widgetsi.Add(w);
                    }
                    {
                        // foreach(var pair in dic)
                        var __enumerator8 = (dic).GetEnumerator();
                        while (__enumerator8.MoveNext())
                        {
                            var pair = __enumerator8.Current;
                            {
                                widgetSequence[pair.Key] = pair.Value;
                            }
                        }
                    }
                    widgetsj.Clear();
                }
                else
                {
                    passed = true;
                    {
                        var __list10 = widgetsi;
                        var __listCount10 = __list10.Count;
                        for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                        {
                            var w = __list10[__i10];
                            {
                                var w1Bounds = w.GetBounds();
                                for (int k = i + 1; k < j; ++k)
                                {
                                    var widgetsk = dcWidgets[k];
                                    foreach (var wk in widgetsk)
                                    {
                                        if (wk.GetBounds().Intersects(w1Bounds))
                                        {
                                            passed = false;
                                            break;
                                        }
                                    }
                                    if (!passed)
                                    {
                                        break;
                                    }
                                }
                                if (!passed)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (!passed)
                    {
                        continue;
                    }
                    var depth0 = widgetsi[widgetsi.Count - 1].depth + 1;
                    var depth1 = widgetsj[0].depth;
                    var dic = new Dictionary<int, UIWidget>();
                    for (int k = depth0; k < depth1; k++)
                    {
                        var w = widgetSequence[k];
                        w.depth -= widgetsi.Count;
                        dic.Add(w.depth, w);
                    }
                    for (int k = 0; k < widgetsi.Count; k++)
                    {
                        var w = widgetsi[k];
                        w.drawCall = dcj;
                        w.depth = depth1 - widgetsi.Count + k;
                        dic.Add(w.depth, w);
                        widgetsj.Add(w);
                    }
                    {
                        // foreach(var pair in dic)
                        var __enumerator11 = (dic).GetEnumerator();
                        while (__enumerator11.MoveNext())
                        {
                            var pair = __enumerator11.Current;
                            {
                                widgetSequence[pair.Key] = pair.Value;
                            }
                        }
                    }
                    widgetsi.Clear();
                    break;
                }
            }
        }
    }

    private static void AutoAdjustDepth4(UIPanel root)
    {
        root.DeactivePanel();
    }

    public static bool AutoAdjustDepth(UIPanel root, int step)
    {
        switch (step)
        {
            case 0:
                {
                    AutoAdjustDepth1(root);
                }
                return false;
            case 1:
                {
                    var dcs = UIDrawCall.activeList;
                    dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
                    {
                        return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
                    });
                    AutoAdjustDepth2(dcs);
                }
                return false;
            case 2:
                {
                    var dcs = UIDrawCall.activeList;
                    dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
                    {
                        return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
                    });
                    AutoAdjustDepth3(dcs);
                }
                return false;
            case 3:
                {
                    AutoAdjustDepth4(root);
                }
                return false;
        }

        return true;
    }

    public static void AutoAdjustDepth(UIPanel root)
    {
        var type = typeof(UIPanel);
        var method = type.GetMethod("LateUpdate", BindingFlags.NonPublic | BindingFlags.Instance);

        method.Invoke(root, new object[] { });
        var dcs = UIDrawCall.activeList;
        dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
        {
            return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
        });

        AutoAdjustDepth1(root);
        method.Invoke(root, new object[] { });
        dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
        {
            return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
        });

        AutoAdjustDepth2(dcs);
        method.Invoke(root, new object[] { });
        dcs.Sort(delegate (UIDrawCall a, UIDrawCall b)
        {
            return a.finalRenderQueue.CompareTo(b.finalRenderQueue);
        });

        AutoAdjustDepth3(dcs);
        AutoAdjustDepth4(root);
    }
}

public static class MyExtension
{
    public static Bounds GetZNormalized(this Bounds bounds, float z = 0.0f)
    {
        var center = bounds.center;
        var size = bounds.size;
        var ret = new Bounds(new Vector3(center.x, center.y, z), new Vector3(size.x, size.y, z));
        return ret;
    }
    public static Bounds GetBounds(this UIWidget w)
    {
        var t = w.transform;
        var e = t.eulerAngles;
        t.eulerAngles = Vector3.zero;
        var cs = w.worldCorners;
        var b = new Bounds(new Vector3(cs[0].x + cs[2].x, cs[0].y + cs[1].y, 0) / 2,
            new Vector3(cs[2].x - cs[0].x, cs[1].y - cs[0].y, 0));
        t.eulerAngles = e;
        return b;
    }

    public static Dictionary<Transform, bool> InactivedWidgets = new Dictionary<Transform, bool>();
    public static Dictionary<UISprite, bool> InactivedSprites = new Dictionary<UISprite, bool>();
    public static Dictionary<UIPanel, UIDrawCall.Clipping> ClipedPanels = new Dictionary<UIPanel, UIDrawCall.Clipping>();

    public static void ActivePanel(this UIPanel panel)
    {
        if (InactivedWidgets.Count > 0)
        {
            Logger.Error("InactivedWidgets.Count > 0!!!");
            return;
        }
        panel.transform.ActiveChilds();
    }

    private static void ActiveChilds(this Transform t)
    {
        var sprite = t.GetComponent<UISprite>();
        if (sprite && sprite.atlas == null)
        {
            InactivedSprites.Add(sprite, false);
            var atlas = Resources.FindObjectsOfTypeAll<UIAtlas>()[0];
            sprite.atlas = atlas;
            sprite.spriteName = atlas.spriteList[0].name;
        }
        var panel = t.GetComponent<UIPanel>();
        if (panel && panel.clipping != UIDrawCall.Clipping.None)
        {
            ClipedPanels.Add(panel, panel.clipping);
            panel.clipping = UIDrawCall.Clipping.None;
        }
        for (int i = 0, imax = t.childCount; i < imax; i++)
        {
            var c = t.GetChild(i);
            if (!c.gameObject.active)
            {
                InactivedWidgets.Add(c, false);
                c.gameObject.SetActive(true);
            }
            c.ActiveChilds();
        }
    }

    public static void DeactivePanel(this UIPanel panel)
    {
        if (InactivedWidgets.Count == 0)
        {
            Logger.Error("InactivedWidgets.Count == 0!!!");
            return;
        }
        panel.transform.DeactiveChilds();
        InactivedWidgets.Clear();
        InactivedSprites.Clear();
    }

    private static void DeactiveChilds(this Transform t)
    {
        var sprite = t.GetComponent<UISprite>();
        if (sprite && InactivedSprites.ContainsKey(sprite))
        {
            sprite.atlas = null;
        }
        var panel = t.GetComponent<UIPanel>();
        if (panel && ClipedPanels.ContainsKey(panel))
        {
            panel.clipping = ClipedPanels[panel];
        }
        for (int i = 0, imax = t.childCount; i < imax; i++)
        {
            var c = t.GetChild(i);
            c.DeactiveChilds();
            if (InactivedWidgets.ContainsKey(c))
            {
                c.gameObject.SetActive(false);
            }
        }
    }
}
