//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// This script adds the NGUI menu options to the Unity Editor.
/// </summary>

static public class NGUIMenu
{
    #region Selection

    static public GameObject SelectedRoot() { return NGUIEditorTools.SelectedRoot(); }

    [MenuItem("NGUI/Selection/Bring To Front &#=", false, 0)]
    static public void BringForward2()
    {
        int val = 0;
        for (int i = 0; i < Selection.gameObjects.Length; ++i)
            val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1000);

        if ((val & 1) != 0)
        {
            NGUITools.NormalizePanelDepths();
            if (UIPanelTool.instance != null)
                UIPanelTool.instance.Repaint();
        }
        if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
    }

    [MenuItem("NGUI/Selection/Bring To Front &#=", true)]
    static public bool BringForward2Validation() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Selection/Push To Back &#-", false, 0)]
    static public void PushBack2()
    {
        int val = 0;
        for (int i = 0; i < Selection.gameObjects.Length; ++i)
            val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1000);

        if ((val & 1) != 0)
        {
            NGUITools.NormalizePanelDepths();
            if (UIPanelTool.instance != null)
                UIPanelTool.instance.Repaint();
        }
        if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
    }

    [MenuItem("NGUI/Selection/Push To Back &#-", true)]
    static public bool PushBack2Validation() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Selection/Adjust Depth By +1 %=", false, 0)]
    static public void BringForward()
    {
        int val = 0;
        for (int i = 0; i < Selection.gameObjects.Length; ++i)
            val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1);
        if (((val & 1) != 0) && UIPanelTool.instance != null)
            UIPanelTool.instance.Repaint();
    }

    [MenuItem("NGUI/Selection/Adjust Depth By +1 %=", true)]
    static public bool BringForwardValidation() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Selection/Adjust Depth By -1 %-", false, 0)]
    static public void PushBack()
    {
        int val = 0;
        for (int i = 0; i < Selection.gameObjects.Length; ++i)
            val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1);
        if (((val & 1) != 0) && UIPanelTool.instance != null)
            UIPanelTool.instance.Repaint();
    }

    [MenuItem("NGUI/Selection/Adjust Depth By -1 %-", true)]
    static public bool PushBackValidation() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Selection/Make Pixel Perfect &#p", false, 0)]
    static void PixelPerfectSelection()
    {
        {
            // foreach(var t in Selection.transforms)
            var __enumerator1 = (Selection.transforms).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var t = (Transform)__enumerator1.Current;
                NGUITools.MakePixelPerfect(t);
            }
        }
    }

    [MenuItem("NGUI/Selection/Make Pixel Perfect &#p", true)]
    static bool PixelPerfectSelectionValidation()
    {
        return (Selection.activeTransform != null);
    }

    #endregion
    #region Create

    [MenuItem("NGUI/Create/Sprite &#s", false, 6)]
    static public void AddSprite()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);

        if (go != null)
        {
            Selection.activeGameObject = NGUISettings.AddSprite(go).gameObject;
        }
        else Debug.Log("You must select a game object first.");
    }

    [MenuItem("NGUI/Create/Label &#l", false, 6)]
    static public void AddLabel()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);

        if (go != null)
        {
            Selection.activeGameObject = NGUISettings.AddLabel(go).gameObject;
        }
        else Debug.Log("You must select a game object first.");
    }

    [MenuItem("NGUI/Create/Texture &#t", false, 6)]
    static public void AddTexture()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);

        if (go != null)
        {
            Selection.activeGameObject = NGUISettings.AddTexture(go).gameObject;
        }
        else Debug.Log("You must select a game object first.");
    }

    [MenuItem("NGUI/Create/Unity 2D Sprite &#d", false, 6)]
    static public void AddSprite2D()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);
        if (go != null) Selection.activeGameObject = NGUISettings.Add2DSprite(go).gameObject;
        else Debug.Log("You must select a game object first.");
    }

    [MenuItem("NGUI/Create/Widget &#w", false, 6)]
    static public void AddWidget()
    {
        GameObject go = NGUIEditorTools.SelectedRoot(true);

        if (go != null)
        {
            Selection.activeGameObject = NGUISettings.AddWidget(go).gameObject;
        }
        else Debug.Log("You must select a game object first.");
    }

    [MenuItem("NGUI/Create/", false, 6)]
    static void AddBreaker123() { }

    [MenuItem("NGUI/Create/Anchor (Legacy)", false, 6)]
    static void AddAnchor2() { Add<UIAnchor>(); }

    [MenuItem("NGUI/Create/Panel", false, 6)]
    static void AddPanel()
    {
        UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
        Selection.activeGameObject = (panel == null) ? NGUIEditorTools.SelectedRoot(true) : panel.gameObject;
    }

    [MenuItem("NGUI/Create/Scroll View", false, 6)]
    static void AddScrollView()
    {
        UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
        if (panel == null) panel = NGUIEditorTools.SelectedRoot(true).GetComponent<UIPanel>();
        panel.clipping = UIDrawCall.Clipping.SoftClip;
        panel.name = "Scroll View";
        panel.gameObject.AddComponent<UIScrollView>();
        Selection.activeGameObject = panel.gameObject;
    }

    [MenuItem("NGUI/Create/Grid", false, 6)]
    static void AddGrid() { Add<UIGrid>(); }

    [MenuItem("NGUI/Create/Table", false, 6)]
    static void AddTable() { Add<UITable>(); }

    static T Add<T>() where T : MonoBehaviour
    {
        T t = NGUITools.AddChild<T>(SelectedRoot());
        Selection.activeGameObject = t.gameObject;
        return t;
    }

    [MenuItem("NGUI/Create/2D UI", false, 6)]
    [MenuItem("Assets/NGUI/Create 2D UI", false, 1)]
    static void Create2D() { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D); }

    [MenuItem("NGUI/Create/2D UI", true)]
    [MenuItem("Assets/NGUI/Create 2D UI", true, 1)]
    static bool Create2Da()
    {
        if (UIRoot.list.Count == 0 || UICamera.list.size == 0) return true;
        {
            // foreach(var c in UICamera.list)
            var __enumerator2 = (UICamera.list).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var c = (UICamera)__enumerator2.Current;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (NGUITools.GetActive(c) && c.camera.isOrthoGraphic)
#else
                if (NGUITools.GetActive(c) && c.GetComponent<Camera>().orthographic)
#endif
                    return false;
            }
        }
        return true;
    }

    [MenuItem("NGUI/Create/3D UI", false, 6)]
    [MenuItem("Assets/NGUI/Create 3D UI", false, 1)]
    static void Create3D() { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Advanced3D); }

    [MenuItem("NGUI/Create/3D UI", true)]
    [MenuItem("Assets/NGUI/Create 3D UI", true, 1)]
    static bool Create3Da()
    {
        if (UIRoot.list.Count == 0 || UICamera.list.size == 0) return true;
        {
            // foreach(var c in UICamera.list)
            var __enumerator3 = (UICamera.list).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var c = (UICamera)__enumerator3.Current;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (NGUITools.GetActive(c) && !c.camera.isOrthoGraphic)
#else
                if (NGUITools.GetActive(c) && !c.GetComponent<Camera>().orthographic)
#endif
                    return false;
            }
        }
        return true;
    }

    #endregion
    #region Attach

    static void AddIfMissing<T>() where T : Component
    {
        if (Selection.activeGameObject != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; ++i)
                Selection.gameObjects[i].AddMissingComponent<T>();
        }
        else Debug.Log("You must select a game object first.");
    }

    static bool Exists<T>() where T : Component
    {
        GameObject go = Selection.activeGameObject;
        if (go != null) return go.GetComponent<T>() != null;
        return false;
    }

    [MenuItem("NGUI/Attach/Collider &#c", false, 7)]
    static public void AddCollider()
    {
        if (Selection.activeGameObject != null)
        {
            for (int i = 0; i < Selection.gameObjects.Length; ++i)
                NGUITools.AddWidgetCollider(Selection.gameObjects[i]);
        }
        else Debug.Log("You must select a game object first, such as your button.");
    }

    //[MenuItem("NGUI/Attach/Anchor", false, 7)]
    //static public void Add1 () { AddIfMissing<UIAnchor>(); }

    //[MenuItem("NGUI/Attach/Anchor", true)]
    //static public bool Add1a () { return !Exists<UIAnchor>(); }

    //[MenuItem("NGUI/Attach/Stretch (Legacy)", false, 7)]
    //static public void Add2 () { AddIfMissing<UIStretch>(); }

    //[MenuItem("NGUI/Attach/Stretch (Legacy)", true)]
    //static public bool Add2a () { return !Exists<UIStretch>(); }

    //[MenuItem("NGUI/Attach/", false, 7)]
    //static public void Add3s () {}

    [MenuItem("NGUI/Attach/Button Script", false, 7)]
    static public void Add3() { AddIfMissing<UIButton>(); }

    [MenuItem("NGUI/Attach/Toggle Script", false, 7)]
    static public void Add4() { AddIfMissing<UIToggle>(); }

    [MenuItem("NGUI/Attach/Slider Script", false, 7)]
    static public void Add5() { AddIfMissing<UISlider>(); }

    [MenuItem("NGUI/Attach/Scroll Bar Script", false, 7)]
    static public void Add6() { AddIfMissing<UIScrollBar>(); }

    [MenuItem("NGUI/Attach/Progress Bar Script", false, 7)]
    static public void Add7() { AddIfMissing<UIProgressBar>(); }

    [MenuItem("NGUI/Attach/Popup List Script", false, 7)]
    static public void Add8() { AddIfMissing<UIPopupList>(); }

    [MenuItem("NGUI/Attach/Input Field Script", false, 7)]
    static public void Add9() { AddIfMissing<UIInput>(); }

    [MenuItem("NGUI/Attach/Key Binding Script", false, 7)]
    static public void Add10() { AddIfMissing<UIKeyBinding>(); }

    [MenuItem("NGUI/Attach/Key Navigation Script", false, 7)]
    static public void Add10a() { AddIfMissing<UIKeyNavigation>(); }

    [MenuItem("NGUI/Attach/Play Tween Script", false, 7)]
    static public void Add11() { AddIfMissing<UIPlayTween>(); }

    [MenuItem("NGUI/Attach/Play Animation Script", false, 7)]
    static public void Add12() { AddIfMissing<UIPlayAnimation>(); }

    [MenuItem("NGUI/Attach/Play Sound Script", false, 7)]
    static public void Add13() { AddIfMissing<UIPlaySound>(); }

    [MenuItem("NGUI/Attach/Localization Script", false, 7)]
    static public void Add14() { AddIfMissing<UILocalize>(); }

    #endregion
    #region Tweens

    [MenuItem("NGUI/Tween/Alpha", false, 8)]
    static void Tween1() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenAlpha>(); }

    [MenuItem("NGUI/Tween/Alpha", true)]
    static bool Tween1a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

    [MenuItem("NGUI/Tween/Color", false, 8)]
    static void Tween2() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenColor>(); }

    [MenuItem("NGUI/Tween/Color", true)]
    static bool Tween2a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

    [MenuItem("NGUI/Tween/Width", false, 8)]
    static void Tween3() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenWidth>(); }

    [MenuItem("NGUI/Tween/Width", true)]
    static bool Tween3a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

    [MenuItem("NGUI/Tween/Height", false, 8)]
    static void Tween4() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenHeight>(); }

    [MenuItem("NGUI/Tween/Height", true)]
    static bool Tween4a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

    [MenuItem("NGUI/Tween/Position", false, 8)]
    static void Tween5() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenPosition>(); }

    [MenuItem("NGUI/Tween/Position", true)]
    static bool Tween5a() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Tween/Rotation", false, 8)]
    static void Tween6() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenRotation>(); }

    [MenuItem("NGUI/Tween/Rotation", true)]
    static bool Tween6a() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Tween/Scale", false, 8)]
    static void Tween7() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenScale>(); }

    [MenuItem("NGUI/Tween/Scale", true)]
    static bool Tween7a() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Tween/Transform", false, 8)]
    static void Tween8() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenTransform>(); }

    [MenuItem("NGUI/Tween/Transform", true)]
    static bool Tween8a() { return (Selection.activeGameObject != null); }

    [MenuItem("NGUI/Tween/Volume", false, 8)]
    static void Tween9() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenVolume>(); }

    [MenuItem("NGUI/Tween/Volume", true)]
    static bool Tween9a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<AudioSource>() != null); }

    [MenuItem("NGUI/Tween/Field of View", false, 8)]
    static void Tween10() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenFOV>(); }

    [MenuItem("NGUI/Tween/Field of View", true)]
    static bool Tween10a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

    [MenuItem("NGUI/Tween/Orthographic Size", false, 8)]
    static void Tween11() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenOrthoSize>(); }

    [MenuItem("NGUI/Tween/Orthographic Size", true)]
    static bool Tween11a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

    #endregion
    #region Open

    [MenuItem("NGUI/Open/Atlas Maker", false, 9)]
    [MenuItem("Assets/NGUI/Open Atlas Maker", false, 0)]
    static public void OpenAtlasMaker()
    {
        EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true).Show();
    }

    [MenuItem("NGUI/Open/Font Maker", false, 9)]
    [MenuItem("Assets/NGUI/Open Bitmap Font Maker", false, 0)]
    static public void OpenFontMaker()
    {
        EditorWindow.GetWindow<UIFontMaker>(false, "Font Maker", true).Show();
    }

    [MenuItem("NGUI/Open/", false, 9)]
    [MenuItem("Assets/NGUI/", false, 0)]
    static public void OpenSeparator2() { }

    [MenuItem("NGUI/Open/Prefab Toolbar", false, 9)]
    static public void OpenPrefabTool()
    {
        EditorWindow.GetWindow<UIPrefabTool>(false, "Prefab Toolbar", true).Show();
    }

    [MenuItem("NGUI/Open/Panel Tool", false, 9)]
    static public void OpenPanelWizard()
    {
        EditorWindow.GetWindow<UIPanelTool>(false, "Panel Tool", true).Show();
    }

    [MenuItem("NGUI/Open/Draw Call Tool", false, 9)]
    static public void OpenDCTool()
    {
        EditorWindow.GetWindow<UIDrawCallViewer>(false, "Draw Call Tool", true).Show();
    }

    [MenuItem("NGUI/Open/Camera Tool", false, 9)]
    static public void OpenCameraWizard()
    {
        EditorWindow.GetWindow<UICameraTool>(false, "Camera Tool", true).Show();
    }

    [MenuItem("NGUI/Open/Widget Wizard (Legacy)", false, 9)]
    static public void CreateWidgetWizard()
    {
        EditorWindow.GetWindow<UICreateWidgetWizard>(false, "Widget Tool", true).Show();
    }

    //[MenuItem("NGUI/Open/UI Wizard (Legacy)", false, 9)]
    //static public void CreateUIWizard ()
    //{
    //    EditorWindow.GetWindow<UICreateNewUIWizard>(false, "UI Tool", true).Show();
    //}

    #endregion
    #region Options

    [MenuItem("NGUI/Options/Transform Move Gizmo/Turn On", false, 10)]
    static public void TurnGizmosOn()
    {
        NGUISettings.showTransformHandles = true;
        NGUIEditorTools.HideMoveTool(false);
    }

    [MenuItem("NGUI/Options/Transform Move Gizmo/Turn On", true, 10)]
    static public bool TurnGizmosOnCheck() { return !NGUISettings.showTransformHandles; }

    [MenuItem("NGUI/Options/Transform Move Gizmo/Turn Off", false, 10)]
    static public void TurnGizmosOff() { NGUISettings.showTransformHandles = false; }

    [MenuItem("NGUI/Options/Transform Move Gizmo/Turn Off", true, 10)]
    static public bool TurnGizmosOffCheck() { return NGUISettings.showTransformHandles; }

    [MenuItem("NGUI/Options/Handles/Turn On", false, 10)]
    static public void TurnHandlesOn() { UIWidget.showHandlesWithMoveTool = true; }

    [MenuItem("NGUI/Options/Handles/Turn On", true, 10)]
    static public bool TurnHandlesOnCheck() { return !UIWidget.showHandlesWithMoveTool; }

    [MenuItem("NGUI/Options/Handles/Turn Off", false, 10)]
    static public void TurnHandlesOff() { UIWidget.showHandlesWithMoveTool = false; }

    [MenuItem("NGUI/Options/Handles/Turn Off", true, 10)]
    static public bool TurnHandlesOffCheck() { return UIWidget.showHandlesWithMoveTool; }

    [MenuItem("NGUI/Options/Handles/Set to Blue", false, 10)]
    static public void SetToBlue() { NGUISettings.colorMode = NGUISettings.ColorMode.Blue; }

    [MenuItem("NGUI/Options/Handles/Set to Blue", true, 10)]
    static public bool SetToBlueCheck() { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Blue; }

    [MenuItem("NGUI/Options/Handles/Set to Orange", false, 10)]
    static public void SetToOrange() { NGUISettings.colorMode = NGUISettings.ColorMode.Orange; }

    [MenuItem("NGUI/Options/Handles/Set to Orange", true, 10)]
    static public bool SetToOrangeCheck() { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Orange; }

    [MenuItem("NGUI/Options/Handles/Set to Green", false, 10)]
    static public void SetToGreen() { NGUISettings.colorMode = NGUISettings.ColorMode.Green; }

    [MenuItem("NGUI/Options/Handles/Set to Green", true, 10)]
    static public bool SetToGreenCheck() { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Green; }

    [MenuItem("NGUI/Options/Inspector Look/Set to Minimalistic", false, 10)]
    static public void SetToMin()
    {
        NGUISettings.minimalisticLook = true;
        if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
    }

    [MenuItem("NGUI/Options/Inspector Look/Set to Minimalistic", true, 10)]
    static public bool SetToMinCheck() { return !NGUISettings.minimalisticLook; }

    [MenuItem("NGUI/Options/Inspector Look/Set to Distinct", false, 10)]
    static public void SetToDistinct()
    {
        NGUISettings.minimalisticLook = false;
        if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
    }

    [MenuItem("NGUI/Options/Inspector Look/Set to Distinct", true, 10)]
    static public bool SetToDistinctCheck() { return NGUISettings.minimalisticLook; }

    [MenuItem("NGUI/Options/Inspector Look/Set to Unified", false, 10)]
    static public void SetToUnified()
    {
        NGUISettings.unifiedTransform = true;
        if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
    }

    [MenuItem("NGUI/Options/Inspector Look/Set to Unified", true, 10)]
    static public bool SetToUnifiedCheck() { return !NGUISettings.unifiedTransform; }

    [MenuItem("NGUI/Options/Inspector Look/Set to Traditional", false, 10)]
    static public void SetToTraditional()
    {
        NGUISettings.unifiedTransform = false;
        if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
    }

    [MenuItem("NGUI/Options/Inspector Look/Set to Traditional", true, 10)]
    static public bool SetToTraditionalCheck() { return NGUISettings.unifiedTransform; }

    [MenuItem("NGUI/Options/Snapping/Turn On", false, 10)]
    static public void TurnSnapOn() { NGUISnap.allow = true; }

    [MenuItem("NGUI/Options/Snapping/Turn On", true, 10)]
    static public bool TurnSnapOnCheck() { return !NGUISnap.allow; }

    [MenuItem("NGUI/Options/Snapping/Turn Off", false, 10)]
    static public void TurnSnapOff() { NGUISnap.allow = false; }

    [MenuItem("NGUI/Options/Snapping/Turn Off", true, 10)]
    static public bool TurnSnapOffCheck() { return NGUISnap.allow; }

    [MenuItem("NGUI/Options/Guides/Set to Always On", false, 10)]
    static public void TurnGuidesOn() { NGUISettings.drawGuides = true; }

    [MenuItem("NGUI/Options/Guides/Set to Always On", true, 10)]
    static public bool TurnGuidesOnCheck() { return !NGUISettings.drawGuides; }

    [MenuItem("NGUI/Options/Guides/Set to Only When Needed", false, 10)]
    static public void TurnGuidesOff() { NGUISettings.drawGuides = false; }

    [MenuItem("NGUI/Options/Guides/Set to Only When Needed", true, 10)]
    static public bool TurnGuidesOffCheck() { return NGUISettings.drawGuides; }

    [MenuItem("NGUI/Options/Reset Prefab Toolbar", false, 10)]
    static public void ResetPrefabTool()
    {
        if (UIPrefabTool.instance == null) OpenPrefabTool();
        UIPrefabTool.instance.Reset();
        UIPrefabTool.instance.Repaint();
    }

    [MenuItem("NGUI/Extras/Switch to 2D Colliders", false, 10)]
    static public void SwitchTo2D()
    {
        BoxCollider[] colliders = NGUITools.FindActive<BoxCollider>();

        for (int i = 0; i < colliders.Length; ++i)
        {
            BoxCollider c = colliders[i];
            GameObject go = c.gameObject;

            UICamera cam = UICamera.FindCameraForLayer(go.layer);
            if (cam == null) continue;
            if (cam.eventType == UICamera.EventType.World_3D) continue;
            if (cam.eventType == UICamera.EventType.World_2D) continue;

            cam.eventType = UICamera.EventType.UI_2D;

            Vector3 center = c.center;
            Vector3 size = c.size;
            NGUITools.DestroyImmediate(c);

            BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
            bc.size = size;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			bc.center = center;
#else
            bc.offset = center;
#endif
            bc.isTrigger = true;
            NGUITools.SetDirty(go);

            UIPanel p = NGUITools.FindInParents<UIPanel>(go);

            if (p != null)
            {
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				if (p.rigidbody != null) NGUITools.Destroy(p.rigidbody);
#else
                if (p.GetComponent<Rigidbody>() != null) NGUITools.Destroy(p.GetComponent<Rigidbody>());
#endif
                // It's unclear if having a 2D rigidbody actually helps or not
                //if (p.GetComponent<Rigidbody2D>() == null)
                //{
                //    Rigidbody2D rb = p.gameObject.AddComponent<Rigidbody2D>();
                //    rb.isKinematic = true;
                //}
            }
        }
    }

    [MenuItem("NGUI/Extras/Switch to 3D Colliders", false, 10)]
    static public void SwitchTo3D()
    {
        BoxCollider2D[] colliders = NGUITools.FindActive<BoxCollider2D>();

        for (int i = 0; i < colliders.Length; ++i)
        {
            BoxCollider2D c = colliders[i];
            GameObject go = c.gameObject;

            UICamera cam = UICamera.FindCameraForLayer(go.layer);
            if (cam == null) continue;
            if (cam.eventType == UICamera.EventType.World_3D) continue;
            if (cam.eventType == UICamera.EventType.World_2D) continue;

            cam.eventType = UICamera.EventType.UI_3D;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Vector3 center = c.center;
#else
            Vector3 center = c.offset;
#endif
            Vector3 size = c.size;
            NGUITools.DestroyImmediate(c);

            BoxCollider bc = go.AddComponent<BoxCollider>();

            if (bc != null)
            {
                bc.size = size;
                bc.center = center;
                bc.isTrigger = true;
            }
            NGUITools.SetDirty(go);

            UIPanel p = NGUITools.FindInParents<UIPanel>(go);

            if (p != null)
            {
                if (p.GetComponent<Rigidbody2D>() != null)
                    NGUITools.Destroy(p.GetComponent<Rigidbody2D>());

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				if (p.rigidbody == null)
#else
                if (p.GetComponent<Rigidbody>() == null)
#endif
                {
                    Rigidbody rb = p.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }
    }

    [MenuItem("NGUI/Extras/Align Scene View to UI", false, 10)]
    static public void AlignSVToUI()
    {
        GameObject go = Selection.activeGameObject ?? UICamera.list[0].gameObject;
        Camera cam = NGUITools.FindCameraForLayer(go.layer);
        SceneView sv = SceneView.lastActiveSceneView;
        Camera svc = sv.camera;
        svc.nearClipPlane = cam.nearClipPlane;
        svc.farClipPlane = cam.farClipPlane;
        sv.size = Mathf.Sqrt(svc.aspect) / 0.7071068f;
        sv.pivot = cam.transform.position;
        sv.rotation = cam.transform.rotation;
        sv.orthographic = true;
        sv.Repaint();
    }

    [MenuItem("NGUI/Extras/Align Scene View to UI", true, 10)]
    static public bool AlignSVToUICheck()
    {
        if (SceneView.lastActiveSceneView == null) return false;
        if (UICamera.list.size == 0) return false;

        GameObject go = Selection.activeGameObject ?? UICamera.list[0].gameObject;
        if (go == null) return false;

        Camera cam = NGUITools.FindCameraForLayer(go.layer);
        if (cam == null || !cam.orthographic) return false;
        return true;
    }

    [MenuItem("NGUI/Extras/RecaculateUIDepth")]
    public static void RecaculateUIDepth()
    {
        LookScript();
        return;
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }
        var ttt = selectObj.GetComponentInParent<Camera>();
        UICamera.currentCamera = ttt;

        string path = "D:\\Test\\" + selectObj.name + ".txt";
        System.IO.StreamWriter wr = new System.IO.StreamWriter(path, false, Encoding.UTF8);
        int depth = 0;
        diffScale = selectObj.transform.root.localScale.x;

        //Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(selectObj.transform.localPosition);
        //Vector3 localPos = selectObj.transform.InverseTransformPoint(selectObj.transform.position);
        //diffX = worldPos.x;
        //diffY = worldPos.y;
        LookUIGameObject(wr, selectObj.transform, depth, selectObj);

        wr.Flush();
        wr.Close();
    }

    [MenuItem("NGUI/Extras/FixScrollView")]
    public static void FixScrollView()
    {
        var allObjects = EnumAssets.EnumGameObjectAtPath("Assets" + "/Res/UI");
        foreach (var obj in allObjects)
        {
            var scrollviews = obj.GetComponentsInChildren<UIScrollViewSimple>(true);
            foreach (var scrollview in scrollviews)
            {
                FixScrollView(scrollview);
            }
        }
    }

    public static string GetTransformFullName(Transform tans)
    {
        var ret = "";
        if (tans == null)
        {
            return ret;
        }
        ret = tans.name;
        var parent = tans.parent;
        while (parent != null)
        {
            ret = parent.name + "." + ret;
            parent = parent.parent;
        }
        
        return ret;
    }
    public static void FixScrollView(UIScrollViewSimple scrollView)
    {
        if (scrollView == null)
        {
            return;
        }
        var pannel = scrollView.GetComponent<UIPanel>();
        if (scrollView.transform.childCount != 1)
        {
            return;
        }

        if (pannel.clipping != UIDrawCall.Clipping.SoftClip)
        {
            return;
        }

        if (Math.Abs(pannel.clipOffset.x) > 0.1f
            || Math.Abs(pannel.clipOffset.y) > 0.1f)
        {
            Logger.Error("pannel.clipOffset != Zero  name = {0} clipOffset = {1}", GetTransformFullName(scrollView.transform), pannel.clipOffset);
            return;
        }
        if (Math.Abs(scrollView.transform.localPosition.x) > 0.1f
            || Math.Abs(scrollView.transform.localPosition.y) > 0.1f)
        {
            Logger.Error("transform.LocalPoation != Zero  name = {0} LocalPoation= {1}", GetTransformFullName(scrollView.transform), scrollView.transform.localPosition);
            return;
        }

        var grid = scrollView.transform.GetChild(0).GetComponent<UIGridSimple>();
        if (grid == null)
        {
            return;
        }

        if (scrollView.movement == UIScrollViewSimple.Movement.Vertical)
        {
            var y = pannel.baseClipRegion.y + pannel.baseClipRegion.w/2.0f - grid.cellHeight/2.0f -
                    pannel.clipSoftness.y/2;
            if (Math.Abs(grid.transform.localPosition.y - y) > 10)
            {
                Logger.Error("Grid LocalPoation != Error  name = {0} LocalPoation= {1} Should Y ={2}", GetTransformFullName(grid.transform), grid.transform.localPosition, y);
            }
        }
        else if (scrollView.movement == UIScrollViewSimple.Movement.Horizontal)
        {
            var x = pannel.baseClipRegion.x -
                    (pannel.baseClipRegion.z/2.0f - grid.cellWidth/2.0f - pannel.clipSoftness.x/2);
                    
            if (Math.Abs(grid.transform.localPosition.x - x) > 10)
            {
                Logger.Error("Grid LocalPoation != Error  name = {0} LocalPoation= {1} Should X ={2}", GetTransformFullName(grid.transform), grid.transform.localPosition, x);
            }
        }
    }

    public static void LookScript()
    {
        string path2 = "D:\\Test\\UILogLookScript.txt";
        System.IO.StreamWriter wr2 = new System.IO.StreamWriter(path2, false, Encoding.UTF8);
        var gos = EnumAssets.EnumGameObjectRecursiveAtPath("Assets" + "/Res/UI");

         //gos = EnumAssets.EnumGameObjectRecursiveAtPath("Assets" + "/Res/UI/Arena");
//         foreach (var go in gos)
//         {
//             var v4 = go.GetComponent<UISprite>();
//             if (null == v4)
//             {
//                 continue;
//             }
//             if (null == v4.atlas)
//             {
//                 continue;
//             }
// 
//             if (v4.atlas.name == "Icon" && v4.spriteName.Contains("icon_"))
//             {
//                 borderIcon = v1.name + ":" + v4.width + "," + v4.height;
//                 if (v4.width == 64 && v4.height == 64)
//                 {
//                     v4.width = 70;
//                     v4.height = 70;
//                 }
//                 else if (v4.width == 66 && v4.height == 66)
//                 {
//                     v4.width = 70;
//                     v4.height = 70;
//                 }
//             }
//         }
//         AssetDatabase.Refresh();
//         AssetDatabase.SaveAssets();
//         return;

        {
            // foreach(var go in gos)
            var __enumerator8 = (gos).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var go = __enumerator8.Current;
                {
                    //
                    var cs = go.GetComponentsInChildren<UISprite>(true);
                    //GameObject.DestroyImmediate(c.gameObject, true);

                     foreach (var c in cs)
                     {
                        if(c.atlas == null ) continue;
                        if(c.spriteName == null) continue;
                        if (c.atlas.name == "Icon" && c.spriteName == "frame")
                        {
                            int cc = c.transform.childCount;
                            var p = c.transform;
                            if (cc == 0)
                            {
                                if (c.transform.parent != null)
                                {
                                    p = c.transform.parent;
                                }
                            }
                            string parentStr = p.name;
                            string sonStr = "";
                            string iconStr = "";
                            string borderIcon = "";
                            for (int i = 0; i < p.transform.childCount; i++)
                            {
                                var v1 = p.transform.GetChild(i);
                                sonStr += v1.name + "|";
                                //查询图标
                                var v3 = v1.GetComponent<UIClassBinding>();
                                if (v3 != null)
                                {
                                    foreach (UIClassBinding.UIClassBindingItemInner data in v3.BindingDatas)
                                    {
                                        if (data.Target.name.Contains("atlas"))
                                        {
                                            var v5 = v1.GetComponent<UISprite>();
                                            if (v5 != null)
                                            {
                                                iconStr = v1.name + ":" + v5.width + "|" + v5.height;
                                                if (v5.width == 64 && v5.height == 64)
                                                {
                                                    v5.width = 66;
                                                    v5.height = 66;
                                                }
                                                break;
                                            }
                                            Logger.Error("{0},{1},{2},p={3},s={4},i={5},b={6}", go.name, c.name + ":" + c.width + "|" + c.height, c.transform.childCount, parentStr, sonStr, iconStr, borderIcon);
                                        }
                                    }
                                    
                                }
                                //查找边框

                                var v4 = v1.GetComponent<UISprite>();
                                if (v4 != null)
                                {
                                    if (v4.atlas != null)
                                    {
                                        if (v4.atlas.name == "Icon" && v4.spriteName.Contains("icon_"))
                                        {
                                            borderIcon = v1.name + ":" + v4.width + "|" + v4.height;
                                            if (v4.width == 64 && v4.height == 64)
                                            {
                                                v4.width = 70;
                                                v4.height = 70;
                                            }
                                            else if (v4.width == 66 && v4.height == 66)
                                            {
                                                v4.width = 70;
                                                v4.height = 70;
                                            }
                                        }
                                        
                                    }
                                }

                                //var v2 = v1.GetComponent<UISprite>();
                                //if (v2==null) continue;
                                //if (v2.atlas == null) continue;
                                //if (v2.atlas.name == "Icon")
                                //{
                                //}
                            }
                            wr2.WriteLine("{0},{1},{2},p={3},s={4},i={5},b={6}", go.name, c.name + ":" + c.width + "|" + c.height, c.transform.childCount, parentStr, sonStr, iconStr, borderIcon);
                       }
                        //GameObject.DestroyImmediate(c, true);
                    }

                    //foreach (var c in cs)
                    //{
                    //    GameObject.DestroyImmediate(c.gameObject, true);
                    //}
                }
            }
        }
        
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        wr2.Flush();
        wr2.Close();

        return;
    }
    public static void LookBoxColider()
    {
        string path2 = "D:\\Test\\UILog.txt";
        System.IO.StreamWriter wr2 = new System.IO.StreamWriter(path2, false, Encoding.UTF8);
        var gos = EnumAssets.EnumGameObjectAtPath("Assets" + "/Res/UI");
        {
            // foreach(var go in gos)
            var __enumerator8 = (gos).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var go = __enumerator8.Current;
                {
                    var cs = go.GetComponentsInChildren<BoxCollider>(true);
                    foreach (var c in cs)
                    {
                        if (c.transform.parent != null)
                        {
                            var p1 = c.transform.parent.GetComponent<ChatMessageLogic>();
                            if (p1 != null)
                            {
                                continue;
                            }
                        }
                        var p2 = c.GetComponent<ChatLableLogic>();
                        if (p2 != null)
                        {
                            continue;
                        }
                        if (c.size.x < 35 || c.size.y < 35)
                        {
                            wr2.WriteLine("{3}={0}[{1},{2}]", c.name, c.size.x, c.size.y, go);
                        }
                    }
                }
            }
        }
        wr2.Flush();
        wr2.Close();
        return;
    }

    public static float diffX = 0;
    public static float diffY = 0;
    public static float diffScale = 0;
    //public static Transform bg = null;

    public class rect
    {
        public float x = 0;
        public float y = 0;
        public float w = 0;
        public float h = 0;
    }
    public static rect GetBox(UIWidget.Pivot p, float x, float y, float w, float h, float sX, float sY)
    {
        rect result = new rect();
        float fW = w * sX;
        float fH = h * sY;
        switch (p)
        {
            case UIWidget.Pivot.TopLeft:
                result.x = x;
                result.y = y - fH;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.Top:
                result.x = x - fW / 2;
                result.y = y - fH;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.TopRight:
                result.x = x - fW;
                result.y = y - fH;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.Left:
                result.x = x;
                result.y = y - fH / 2;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.Center:
                result.x = x - fW / 2;
                result.y = y - fH / 2;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.Right:
                result.x = x - fW;
                result.y = y - fH / 2;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.BottomLeft:
                result.x = x;
                result.y = y;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.Bottom:
                result.x = x - fW / 2;
                result.y = y;
                result.w = fW;
                result.h = fH;
                break;
            case UIWidget.Pivot.BottomRight:
                result.x = x - fW;
                result.y = y;
                result.w = fW;
                result.h = fH;
                break;
            default:
                throw new ArgumentOutOfRangeException("p");
        }
        return result;
    }
    public static void LookUIGameObject(System.IO.StreamWriter wr, Transform uiDad, int depth, GameObject p)
    {
        int thisDepth = -1;
        float x = 0;
        float y = 0;
        float w = 0;
        float h = 0;
        //Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(uiDad.localPosition);
        Vector3 localPos = p.transform.InverseTransformPoint(uiDad.position);
        string pos = "";
        x = localPos.x;
        y = localPos.y;

        bool isHavePanel = false;
        var panel = uiDad.GetComponent<UIPanel>();
        //var widget = child.GetComponent<UIWidget>();
        if (panel)
        {
            if (thisDepth == -1)
            {
                thisDepth = panel.depth;
            }
            isHavePanel = true;
        }


        bool isHaveUISprite = false;
        string SpriteStr = "";
        var UISprite = uiDad.GetComponent<UISprite>();
        if (UISprite)
        {
            if (thisDepth == -1)
            {
                thisDepth = UISprite.depth;
            }
            SpriteStr = string.Format("\t[Sprite:atlas={0}]", UISprite.atlas);

            //rect LabelRect = GetBox(UISprite.pivot, x, y, UISprite.width, UISprite.height, UISprite.transform.localScale.x, UISprite.transform.localScale.y);
            //pos = string.Format("{0},{1},{2},{3}", LabelRect.x, LabelRect.y, LabelRect.w, LabelRect.h);
        }


        string LableStr = "";
        var UILabel = uiDad.GetComponent<UILabel>();
        if (UILabel)
        {
            if (thisDepth == -1)
            {
                thisDepth = UILabel.depth;
            }
            if (UILabel.applyGradient) //勾选颜色
            {

            }
            //UILabel.pivot
            //string LabelError = string.Format("{0}{1}",
            //    UILabel.fontSize%2 == 1 ? ":fontSize=" + UILabel.fontSize : "",
            //    UILabel.applyGradient
            //        ? 
            //        string.Format("{0}",UILabel.gradientTop.r == UILabel.gradientBottom.r &&UILabel.gradientTop.g == UILabel.gradientBottom.g &&UILabel.gradientTop.b == UILabel.gradientBottom.b? ":Gradient Same":
            //        string.Format("{0}", Math.Abs(UILabel.gradientTop.r - UILabel.gradientBottom.r) < 100 && Math.Abs(UILabel.gradientTop.g - UILabel.gradientBottom.g) < 100 && Math.Abs(UILabel.gradientTop.b - UILabel.gradientBottom.b) < 100 ? ":Gradient Near" : ""))
            //        : ""
            //        );
            //if (LabelError != "")
            //{
            //    LabelError = string.Format("\t[Error{0}]", LabelError);
            //}
            string LabelError1 = UILabel.fontSize % 2 == 1 ? ":fontSize=" + UILabel.fontSize : "";
            string LabelError2 = "";
            if (UILabel.applyGradient)
            {
                if (Math.Abs(UILabel.gradientTop.r - UILabel.gradientBottom.r) < 0.01 && Math.Abs(UILabel.gradientTop.g - UILabel.gradientBottom.g) < 0.01 && Math.Abs(UILabel.gradientTop.b - UILabel.gradientBottom.b) < 0.01)
                {
                    LabelError2 = ":Gradient Same";
                }
                else if (Math.Abs(UILabel.gradientTop.r - UILabel.gradientBottom.r) < 0.4 && Math.Abs(UILabel.gradientTop.g - UILabel.gradientBottom.g) < 0.4 && Math.Abs(UILabel.gradientTop.b - UILabel.gradientBottom.b) < 0.4)
                {
                    LabelError2 = ":Gradient Near";
                }
                else if (UILabel.color.r * 255 - 255 < -1 || UILabel.color.g * 255 - 255 < -1 || UILabel.color.b * 255 - 255 < -1)
                {
                    LabelError2 = ":Color Not White";
                }
            }

            string LabelError3 = "";
            if (UILabel.effectStyle != UILabel.Effect.Shadow)
            {
                LabelError3 = string.Format(":Effect={0}", UILabel.effectStyle);
            }

            string LabelError4 = "";
            string LabelError = "";
            if (LabelError1 != "" || LabelError2 != "" || LabelError3 != "" || LabelError4 != "")
            {
                LabelError = string.Format("\t[Error{0}{1}{2}{3}]", LabelError1, LabelError2, LabelError3, LabelError4);
            }
            LableStr = string.Format("\t[Label:size={0}:color={1},{2},{3}{4}]{5}", UILabel.fontSize, UILabel.color.r * 255, UILabel.color.g * 255, UILabel.color.b * 255, UILabel.applyGradient ? ":Gradient={true}" : "", LabelError);

            //rect LabelRect = GetBox(UILabel.pivot, x, y, UILabel.width, UILabel.height, UILabel.transform.localScale.x, UILabel.transform.localScale.y);
            //pos = string.Format("{0},{1},{2},{3}", LabelRect.x, LabelRect.y, LabelRect.w, LabelRect.h);

        }


        bool isHaveUIWidget = false;
        var UIWidget = uiDad.GetComponent<UIWidget>();
        if (UIWidget)
        {
            if (thisDepth == -1)
            {
                thisDepth = UIWidget.depth;
            }
            isHaveUIWidget = true;
            rect LabelRect = GetBox(UIWidget.pivot, x, y, UIWidget.width, UIWidget.height, UIWidget.transform.localScale.x, UIWidget.transform.localScale.y);
            pos = string.Format("\t[box={0:#0.##},{1:#0.##},{2:#0.##},{3:#0.##}]", LabelRect.x, LabelRect.y, LabelRect.w, LabelRect.h);
        }

        string outStr = "";
        for (int i = 0; i < depth; i++)
        {
            outStr += "\t";
        }

        wr.WriteLine("{0}{1}{2}\t{3}{4}{5}{6}{7}{8}", outStr, "", uiDad.name, thisDepth, isHavePanel ? "\t[Panel]" : "", SpriteStr, LableStr, isHaveUIWidget ? "\t[Widget]" : "", pos);
        for (int i = 0; i < uiDad.childCount; i++)
        {
            var child = uiDad.GetChild(i);
            LookUIGameObject(wr, child, depth + 1, p);
        }
        //foreach (string s in newFile)
        //{
        //    wr.WriteLine(s);
        //}
    }

    [MenuItem("NGUI/Extras/Reset Depth", false, 10)]
    static public void ResetSepth()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }

        int originalPanelDepth = 0;
        int startDepth = -1000;
        UIPanel panel = selectObj.GetComponent<UIPanel>();
        if (panel)
        {
            originalPanelDepth = panel.depth;
            startDepth = 0;
        }
        else
        {
            var parent = selectObj.transform.parent;
            int nWidgetCount = 0;
            while (panel == null && parent != null)
            {
                parent.gameObject.GetComponent<UIPanel>();
                panel = parent.GetComponent<UIPanel>();
                parent = parent.transform.parent;

                if (parent != null && parent.GetComponent<UIWidget>() != null && startDepth == -1000)
                {
                    startDepth = parent.GetComponent<UIWidget>().depth;
                }
            }
            if (panel == null)
            {
                return;
            }

            originalPanelDepth = panel.depth;
            if (startDepth == -1000)
            {
                startDepth = 0;
            }

            if (selectObj.GetComponent<UIWidget>() != null)
            {
                selectObj.GetComponent<UIWidget>().depth = startDepth;
            }
        }

        ResetDepth(selectObj.transform, ref startDepth, originalPanelDepth);
    }

    [MenuItem("NGUI/Extras/Colloder Tag (UI)", false, 10)]
    static public void SetUITag()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }
        var children = selectObj.GetComponentsInChildren<BoxCollider>(true);
        {
            var __array4 = children;
            var __arrayLength4 = __array4.Length;
            for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var child = __array4[__i4];
                {
                    child.gameObject.tag = "UI";
                }
            }
        }
    }

    [MenuItem("NGUI/Extras/Check Bind Target", false, 10)]
    static public void CheckBindTarget()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            Debug.LogError("Choose a GameObject first");
            return;
        }

        var children = selectObj.GetComponentsInChildren<UIClassBinding>();
        {
            var __array5 = children;
            var __arrayLength5 = __array5.Length;
            for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
            {
                var child = __array5[__i5];
                {
                    {
                        // foreach(var bind in child.BindingDatas)
                        var __enumerator6 = (child.BindingDatas).GetEnumerator();
                        while (__enumerator6.MoveNext())
                        {
                            var bind = __enumerator6.Current;
                            {
                                if (bind.Target.target == null)
                                {
                                    Debug.LogError(string.Format("Target is Null ,Path = {0}", GameUtils.GetFullPath(child.transform)));
                                    continue;
                                }
                                if (bind.Target.target.gameObject != child.gameObject)
                                {
                                    Debug.LogError(string.Format("Target is Not equal,Path = {0}", GameUtils.GetFullPath(child.transform)));
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        Debug.Log("Check OK");
    }

    static public void ResetDepth(Transform obj, ref int depth, int pannelDepth)
    {
        if (obj == null)
        {
            return;
        }
        for (int i = 0; i < obj.childCount; i++)
        {
            //var dep = depth;
            //var pdep = pannelDepth;
            var child = obj.GetChild(i);
            var widget = child.GetComponent<UIWidget>();
            if (widget != null)
            {
                depth++;
                widget.depth = depth;
            }
            var pannel = child.GetComponent<UIPanel>();
            if (pannel != null)
            {
                pannelDepth++;
                pannel.depth = pannelDepth;
                int Newdepth = 0;
                ResetDepth(child, ref Newdepth, pannelDepth);
                return;
            }
            ResetDepth(child, ref depth, pannelDepth);
        }
        //         for (int i = 0; i < obj.childCount; i++)
        //         {
        //             var child = obj.GetChild(i);
        //             ResetDepth(child, depth, pannelDepth);
        //         }
    }

    [MenuItem("GameObject/Align View To Selected UI", false, 999)]
    static public void AlignSVWithSelectedUI() { AlignSVToUI(); }

    [MenuItem("GameObject/Align View To Selected UI", true, 999)]
    static public bool AlignSVWithSelectedUICheck()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null) return false;
        return AlignSVToUICheck();
    }
    #endregion

    [MenuItem("NGUI/Normalize Depth Hierarchy &#0", false, 11)]
    static public void Normalize() { NGUITools.NormalizeDepths(); }

    [MenuItem("NGUI/", false, 11)]
    static void Breaker() { }

    [MenuItem("NGUI/Help", false, 12)]
    static public void Help() { NGUIHelp.Show(); }
}
