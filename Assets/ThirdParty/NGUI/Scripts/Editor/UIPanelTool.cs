//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Panel wizard that allows enabling / disabling and selecting panels in the scene.
/// </summary>

public class UIPanelTool : EditorWindow
{
    static public UIPanelTool instance;

    enum Visibility
    {
        Visible,
        Hidden,
    }

    class Entry
    {
        public UIPanel panel;
        public bool isEnabled = false;
        public bool widgetsEnabled = false;
        public List<UIWidget> widgets = new List<UIWidget>();
    }

    /// <summary>
    /// First sort by depth, then alphabetically, then by instance ID.
    /// </summary>

    static int Compare(Entry a, Entry b)
    {
        if (a != b && a != null && b != null)
        {
            if (a.panel.depth < b.panel.depth) return -1;
            if (a.panel.depth > b.panel.depth) return 1;
            int val = string.Compare(a.panel.name, b.panel.name);
            if (val != 0) return val;
            return (a.panel.GetInstanceID() < b.panel.GetInstanceID()) ? -1 : 1;
        }
        return 0;
    }

    Vector2 mScroll = Vector2.zero;

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }
    void OnSelectionChange() { Repaint(); }

    /// <summary>
    /// Collect a list of panels.
    /// </summary>

    static List<UIPanel> GetListOfPanels()
    {
        List<UIPanel> panels = NGUIEditorTools.FindAll<UIPanel>();

        for (int i = panels.Count; i > 0;)
        {
            if (!panels[--i].showInPanelTool)
            {
                panels.RemoveAt(i);
            }
        }
        return panels;
    }

    /// <summary>
    /// Get a list of widgets managed by the specified transform's children.
    /// </summary>

    static void GetWidgets(Transform t, List<UIWidget> widgets)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            UIWidget w = child.GetComponent<UIWidget>();
            if (w != null) widgets.Add(w);
            else if (child.GetComponent<UIPanel>() == null) GetWidgets(child, widgets);
        }
    }

    /// <summary>
    /// Get a list of widgets managed by the specified panel.
    /// </summary>

    static List<UIWidget> GetWidgets(UIPanel panel)
    {
        List<UIWidget> widgets = new List<UIWidget>();
        if (panel != null) GetWidgets(panel.transform, widgets);
        return widgets;
    }

    /// <summary>
    /// Draw the custom wizard.
    /// </summary>

    void OnGUI()
    {
        List<UIPanel> panels = GetListOfPanels();

        if (panels != null && panels.Count > 0)
        {
            UIPanel selectedPanel = NGUITools.FindInParents<UIPanel>(Selection.activeGameObject);

            // First, collect a list of panels with their associated widgets
            List<Entry> entries = new List<Entry>();
            Entry selectedEntry = null;
            bool allEnabled = true;
            {
                var __list1 = panels;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var panel = (UIPanel)__list1[__i1];
                    {
                        Entry ent = new Entry();
                        ent.panel = panel;
                        ent.widgets = GetWidgets(panel);
                        ent.isEnabled = panel.enabled && NGUITools.GetActive(panel.gameObject);
                        ent.widgetsEnabled = ent.isEnabled;

                        if (ent.widgetsEnabled)
                        {
                            {
                                var __list5 = ent.widgets;
                                var __listCount5 = __list5.Count;
                                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                                {
                                    var w = (UIWidget)__list5[__i5];
                                    {
                                        if (!NGUITools.GetActive(w.gameObject))
                                        {
                                            allEnabled = false;
                                            ent.widgetsEnabled = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else allEnabled = false;
                        entries.Add(ent);
                    }
                }
            }
            // Sort the list by depth
            entries.Sort(Compare);

            mScroll = GUILayout.BeginScrollView(mScroll);

            NGUIEditorTools.SetLabelWidth(80f);
            bool showAll = DrawRow(null, null, allEnabled);
            NGUIEditorTools.DrawSeparator();
            {
                var __list2 = entries;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var ent = (Entry)__list2[__i2];
                    {
                        if (DrawRow(ent, selectedPanel, ent.widgetsEnabled))
                        {
                            selectedEntry = ent;
                        }
                    }
                }
            }
            GUILayout.EndScrollView();

            if (showAll)
            {
                {
                    var __list3 = entries;
                    var __listCount3 = __list3.Count;
                    for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                    {
                        var ent = (Entry)__list3[__i3];
                        {
                            NGUITools.SetActive(ent.panel.gameObject, !allEnabled);
                        }
                    }
                }
            }
            else if (selectedEntry != null)
            {
                NGUITools.SetActive(selectedEntry.panel.gameObject, !selectedEntry.widgetsEnabled);
            }
        }
        else
        {
            GUILayout.Label("No UI Panels found in the scene");
        }
    }

    /// <summary>
    /// Helper function used to print things in columns.
    /// </summary>

    bool DrawRow(Entry ent, UIPanel selected, bool isChecked)
    {
        bool retVal = false;
        string panelName, layer, depth, widgetCount, drawCalls, clipping, triangles;

        if (ent != null)
        {
            panelName = ent.panel.name;
            layer = LayerMask.LayerToName(ent.panel.gameObject.layer);
            depth = ent.panel.depth.ToString();
            widgetCount = ent.widgets.Count.ToString();
            drawCalls = ent.panel.drawCalls.Count.ToString();
            clipping = (ent.panel.clipping != UIDrawCall.Clipping.None) ? "Yes" : "";

            int triangeCount = 0;
            {
                var __list4 = ent.panel.drawCalls;
                var __listCount4 = __list4.Count;
                for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var dc = __list4[__i4];
                    triangeCount += dc.triangles;
                }
            }
            triangles = triangeCount.ToString();
        }
        else
        {
            panelName = "Panel's Name";
            layer = "Layer";
            depth = "DP";
            widgetCount = "WG";
            drawCalls = "DC";
            clipping = "Clip";
            triangles = "Tris";
        }

        if (ent != null) GUILayout.Space(-1f);

        if (ent != null)
        {
            GUI.backgroundColor = ent.panel == selected ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
            GUI.backgroundColor = Color.white;
        }
        else
        {
            GUILayout.BeginHorizontal();
        }

        GUI.contentColor = (ent == null || ent.isEnabled) ? Color.white : new Color(0.7f, 0.7f, 0.7f);
        if (isChecked != EditorGUILayout.Toggle(isChecked, GUILayout.Width(20f))) retVal = true;

        GUILayout.Label(depth, GUILayout.Width(30f));

        if (GUILayout.Button(panelName, EditorStyles.label, GUILayout.MinWidth(100f)))
        {
            if (ent != null)
            {
                Selection.activeGameObject = ent.panel.gameObject;
                EditorUtility.SetDirty(ent.panel.gameObject);
            }
        }

        GUILayout.Label(layer, GUILayout.Width(ent == null ? 65f : 70f));
        GUILayout.Label(widgetCount, GUILayout.Width(30f));
        GUILayout.Label(drawCalls, GUILayout.Width(30f));
        GUILayout.Label(clipping, GUILayout.Width(30f));
        GUILayout.Label(triangles, GUILayout.Width(30f));

        if (ent == null)
        {
            GUILayout.Label("Stc", GUILayout.Width(24f));
        }
        else
        {
            bool val = ent.panel.widgetsAreStatic;

            if (val != EditorGUILayout.Toggle(val, GUILayout.Width(20f)))
            {
                ent.panel.widgetsAreStatic = !val;
                EditorUtility.SetDirty(ent.panel.gameObject);
#if !UNITY_3_5
                if (NGUITransformInspector.instance != null)
                    NGUITransformInspector.instance.Repaint();
#endif
            }
        }
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();
        return retVal;
    }
}
