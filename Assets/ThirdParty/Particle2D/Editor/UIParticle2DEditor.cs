//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(Particle2D), true)]
public class UIParticle2DEditor : UIWidgetInspector
{
    bool isSpriteListFoldout = true;
    bool isEmitterFoldout = true;
    bool isSizeFoldout = true;

    // Intivisualy copy
    static AnimationCurve copiedAnimationCurve = new AnimationCurve();


    void SelectSprite(int targetIndex, string spriteName)
    {
        Particle2D obj = target as Particle2D;
        NGUIEditorTools.RegisterUndo("Sprite Change", target);
        obj.sprites[targetIndex] = spriteName;
        Repaint();
    }

    bool DrawSpriteField(UIAtlas atlas, string spriteName, int spriteIndex, SpriteSelectorForParticle2D.Callback callback, params GUILayoutOption[] options)
    {
        if (atlas.GetSprite(spriteName) == null)
            spriteName = "";

        if (NGUIEditorTools.DrawPrefixButton(spriteName, options))
        {
            NGUISettings.atlas = atlas;
            NGUISettings.selectedSprite = spriteName;
            SpriteSelectorForParticle2D.Show(callback, spriteIndex);
            return true;
        }
        return false;
    }

    void OnSelectAtlas(Object obj)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("atlas");
        sp.objectReferenceValue = obj;
        serializedObject.ApplyModifiedProperties();
        NGUITools.SetDirty(serializedObject.targetObject);
        NGUISettings.atlas = obj as UIAtlas;
    }


    /// <summary>
    /// Draw all the custom properties such as sprite type, flip setting, fill direction, etc.
    /// </summary>

    protected override void DrawCustomProperties()
    {
        Particle2D obj = target as Particle2D;

        //--------------------------------------------
        // Atlas

        GUILayout.BeginHorizontal();
        if (NGUIEditorTools.DrawPrefixButton("Atlas")) ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
        SerializedProperty atlas = NGUIEditorTools.DrawProperty("", serializedObject, "atlas", GUILayout.MinWidth(20f));

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (atlas != null)
            {
                UIAtlas atl = atlas.objectReferenceValue as UIAtlas;
                NGUISettings.atlas = atl;
                NGUIEditorTools.Select(atl.gameObject);
            }
        }
        GUILayout.EndHorizontal();


        //--------------------------------------------
        // Sprites

        if (isSpriteListFoldout = EditorGUILayout.Foldout(isSpriteListFoldout, "Sprites", EditorStyles.foldout))
        {
            if (atlas != null)
            {
                int idx = 0;
                {
                    var __list1 = obj.sprites;
                    var __listCount1 = __list1.Count;
                    for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                    {
                        var str = (string)__list1[__i1];
                        {
                            GUILayout.BeginHorizontal();
                            DrawSpriteField(atlas.objectReferenceValue as UIAtlas, obj.sprites[idx], idx, SelectSprite);

                            if (obj.sprites.Count > 1)
                            {
                                if (GUILayout.Button("X", GUILayout.Width(32)))
                                {
                                    NGUIEditorTools.RegisterUndo("Sprite Delete", target);
                                    obj.sprites.Remove(str);
                                    break;
                                }
                            }
                            GUILayout.EndHorizontal();
                            idx++;
                        }
                    }
                }
                if (GUILayout.Button("Add Sprite", GUILayout.Height(16)))
                {
                    if (obj.atlas.spriteList.Count >= 1)
                    {
                        NGUIEditorTools.RegisterUndo("Sprite Add", target);
                        obj.sprites.Add(obj.atlas.spriteList[0].name);
                    }
                }
            }

            //--------------------------------------------
            // Depth
            EditorGUIUtility.LookLikeControls(80f, 0);
            GUILayout.Space(3f);
            DrawDepth(serializedObject, mWidget, false);

            //--------------------------------------------
            // Color
            GUILayout.Space(3f);
            NGUIEditorTools.DrawProperty("Color Tint", serializedObject, "mColor", GUILayout.MinWidth(20f));
        }


        //--------------------------------------------
        // Emitter type
        if (isEmitterFoldout = EditorGUILayout.Foldout(isEmitterFoldout, "Emitter Settings", EditorStyles.foldout))
        {
            // Looping
            bool replacementWorldSpace = EditorGUILayout.Toggle("World Space", obj.worldSpace);
            if (obj.worldSpace != replacementWorldSpace)
            {
                NGUIEditorTools.RegisterUndo("World Position Change", target);
                obj.worldSpace = replacementWorldSpace;
            }

            Particle2D.Emitter replacementEmitter = (Particle2D.Emitter)EditorGUILayout.EnumPopup("Emitter Type", obj.emitter);
            if (obj.emitter != replacementEmitter)
            {
                NGUIEditorTools.RegisterUndo("Max particles Change", target);
                obj.emitter = replacementEmitter;
            }

            if (obj.emitter == Particle2D.Emitter.CIRCLE)
            {
                float replacementRadius = EditorGUILayout.FloatField("Radius", obj.emitterRadius);
                if (replacementRadius < 0) replacementRadius = 0;
                if (obj.emitterRadius != replacementRadius)
                {
                    NGUIEditorTools.RegisterUndo("Radius Change", target);
                    obj.emitterRadius = replacementRadius;
                }

            }
            else if (obj.emitter == Particle2D.Emitter.RECT)
            {
                EditorGUIUtility.LookLikeControls(40f, 0);
                GUILayout.BeginHorizontal();
                float replacementWidth = EditorGUILayout.FloatField("Width", obj.emitterWidth);
                float replacementHeight = EditorGUILayout.FloatField("Height", obj.emitterHeight);
                if (obj.emitterWidth != replacementWidth || obj.emitterHeight != replacementHeight)
                {
                    NGUIEditorTools.RegisterUndo("Emitter Size Change", target);
                    obj.emitterWidth = replacementWidth;
                    obj.emitterHeight = replacementHeight;
                }
                GUILayout.EndHorizontal();
                EditorGUIUtility.LookLikeControls(80f, 0);
            }
            EditorGUILayout.Space();

            //--------------------------------------------
            // Spwn rate
            float replacementSpwnRate = EditorGUILayout.FloatField("Spwn Rate", obj.spwnRate);
            if (replacementSpwnRate <= 0.001f) replacementSpwnRate = 0.001f;
            if (obj.spwnRate != replacementSpwnRate)
            {
                NGUIEditorTools.RegisterUndo("Spwn Rate Change", target);
                obj.spwnRate = replacementSpwnRate;
            }

            //--------------------------------------------
            // Looping
            bool replacementLooping = EditorGUILayout.Toggle("Looping", obj.looping);
            if (obj.looping != replacementLooping)
            {
                NGUIEditorTools.RegisterUndo("Looping Change", target);
                obj.looping = replacementLooping;
            }
            if (!obj.looping)
            {
                //--------------------------------------------
                // Max particles
                int replacementMaxParticles = EditorGUILayout.IntField("Max particles", obj.maxParticles);
                if (replacementMaxParticles <= 0) replacementMaxParticles = 1;
                if (obj.maxParticles != replacementMaxParticles)
                {
                    NGUIEditorTools.RegisterUndo("Max particles Change", target);
                    obj.maxParticles = replacementMaxParticles;
                }

                //--------------------------------------------
                // Duration
                float replacementDuration = EditorGUILayout.FloatField("Duration", obj.duration);
                if (replacementDuration < 0) replacementDuration = 0;
                if (obj.duration != replacementDuration)
                {
                    NGUIEditorTools.RegisterUndo("Duration Change", target);
                    obj.duration = replacementDuration;
                }

                bool replacementPlayOnce = EditorGUILayout.Toggle("Play Once", obj.playOnce);
                if (obj.playOnce != replacementPlayOnce)
                {
                    NGUIEditorTools.RegisterUndo("Play Once Change", target);
                    obj.playOnce = replacementPlayOnce;
                }
            }

            //--------------------------------------------
            // Life time
            EditorGUIUtility.LookLikeControls(80f, 0);
            GUILayout.BeginHorizontal();
            float replacementMinLifeTime = EditorGUILayout.FloatField("Min Life", obj.minLifeTime);
            float replacementMaxLifeTime = EditorGUILayout.FloatField("Max Life", obj.maxLifeTime);
            if (replacementMinLifeTime <= 0) replacementMinLifeTime = 1;
            if (replacementMaxLifeTime <= 0) replacementMaxLifeTime = 1;
            if (obj.minLifeTime != replacementMinLifeTime || obj.maxLifeTime != replacementMaxLifeTime)
            {
                NGUIEditorTools.RegisterUndo("Life Time Change", target);
                obj.minLifeTime = replacementMinLifeTime;
                obj.maxLifeTime = replacementMaxLifeTime;
            }
            GUILayout.EndHorizontal();
        }

        //--------------------------------------------
        // Size
        if (isSizeFoldout = EditorGUILayout.Foldout(isSizeFoldout, "Size Settings", EditorStyles.foldout))
        {
            EditorGUIUtility.LookLikeControls(80f, 0);
            GUILayout.BeginHorizontal();
            float replacementMinSize = EditorGUILayout.FloatField("Min Size", obj.minSize);
            float replacementMaxSize = EditorGUILayout.FloatField("Max Size", obj.maxSize);
            if (replacementMinSize <= 0) replacementMinSize = 1;
            if (replacementMaxSize <= 0) replacementMaxSize = 1;
            if (obj.minSize != replacementMinSize || obj.maxSize != replacementMaxSize)
            {
                NGUIEditorTools.RegisterUndo("Size Change", target);
                obj.minSize = replacementMinSize;
                obj.maxSize = replacementMaxSize;
            }
            GUILayout.EndHorizontal();
            bool replacementSizeRate = EditorGUILayout.BeginToggleGroup("Size Rate", obj.useSizeRate);
            if (obj.useSizeRate != replacementSizeRate)
            {
                NGUIEditorTools.RegisterUndo("Use Size Rate Change", target);
                obj.useSizeRate = replacementSizeRate;
            }
            if (obj.useSizeRate)
            {
                GUILayout.BeginHorizontal();
                obj.sizeRate = EditorGUILayout.CurveField("Size", obj.sizeRate);
                if (GUILayout.Button("Copy", GUILayout.Width(40)))
                {
                    copiedAnimationCurve = new AnimationCurve(obj.sizeRate.keys);
                }
                if (GUILayout.Button("Paste", GUILayout.Width(40)))
                {
                    NGUIEditorTools.RegisterUndo("Paste", target);
                    obj.sizeRate = new AnimationCurve(copiedAnimationCurve.keys);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUIUtility.LookLikeControls();
        }

        //--------------------------------------------
        // Velocity
        EditorGUIUtility.LookLikeControls();
        bool replacementVelocity = EditorGUILayout.Foldout(obj.useVelocity, "Use Velocity?", EditorStyles.toggleGroup);
        if (obj.useVelocity != replacementVelocity)
        {
            NGUIEditorTools.RegisterUndo("Use Velocity Change", target);
            obj.useVelocity = replacementVelocity;
        }
        if (obj.useVelocity)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtility.LookLikeControls(80f, 0);
            float replacementMinAngle = EditorGUILayout.FloatField("Min Angle", obj.minAngle);
            float replacementMaxAngle = EditorGUILayout.FloatField("Max Angle", obj.maxAngle);
            if (replacementMinAngle != obj.minAngle || replacementMaxAngle != obj.maxAngle)
            {
                NGUIEditorTools.RegisterUndo("Angle Change", target);
                obj.minAngle = replacementMinAngle;
                obj.maxAngle = replacementMaxAngle;
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.MinMaxSlider(ref obj.minAngle, ref obj.maxAngle, -360f, 360f);

            GUILayout.BeginHorizontal();
            float replacementMinPower = EditorGUILayout.FloatField("Min Power", obj.minPower);
            float replacementMaxPower = EditorGUILayout.FloatField("Max Power", obj.maxPower);
            if (replacementMinPower < 0f) replacementMinPower = 0f;
            if (replacementMaxPower < 0f) replacementMaxPower = 0f;
            if (replacementMinPower > 99999999f) replacementMinPower = 99999999f;
            if (replacementMaxPower > 99999999f) replacementMaxPower = 99999999f;
            if (replacementMinPower != obj.minPower || replacementMaxPower != obj.maxPower)
            {
                NGUIEditorTools.RegisterUndo("Angle Change", target);
                obj.minPower = replacementMinPower;
                obj.maxPower = replacementMaxPower;
            }
            GUILayout.EndHorizontal();

            //--------------------------------------------
            // Acceleration
            bool replacementAcceleration = EditorGUILayout.BeginToggleGroup("Acceleration", obj.useAcceleration);
            if (obj.useAcceleration != replacementAcceleration)
            {
                NGUIEditorTools.RegisterUndo("Use Acceleration Change", target);
                obj.useAcceleration = replacementAcceleration;
            }
            if (obj.useAcceleration)
            {
                GUILayout.BeginHorizontal();
                obj.accelerationRate = EditorGUILayout.CurveField("Accel", obj.accelerationRate);
                if (GUILayout.Button("Copy", GUILayout.Width(40)))
                {
                    copiedAnimationCurve = new AnimationCurve(obj.accelerationRate.keys);
                }
                if (GUILayout.Button("Paste", GUILayout.Width(40)))
                {
                    NGUIEditorTools.RegisterUndo("Paste", target);
                    obj.accelerationRate = new AnimationCurve(copiedAnimationCurve.keys);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();

            //--------------------------------------------
            // Force
            bool replacementForce = EditorGUILayout.BeginToggleGroup("Force", obj.useForce);
            if (obj.useForce != replacementForce)
            {
                NGUIEditorTools.RegisterUndo("Use Force Change", target);
                obj.useForce = replacementForce;
            }
            if (obj.useForce)
            {
                Vector2 replacementForceForce = EditorGUILayout.Vector2Field("Force", obj.force);
                if (obj.force != replacementForceForce)
                {
                    NGUIEditorTools.RegisterUndo("Force Change", target);
                    obj.force = replacementForceForce;
                }
            }
            EditorGUILayout.EndToggleGroup();

            EditorGUIUtility.LookLikeControls();
        }

        //--------------------------------------------
        // Rotation
        EditorGUIUtility.LookLikeControls();
        bool replacementRotation = EditorGUILayout.Foldout(obj.useRotation, "Use Rotation?", EditorStyles.toggleGroup);
        if (obj.useRotation != replacementRotation)
        {
            NGUIEditorTools.RegisterUndo("Use Rotation Change", target);
            obj.useRotation = replacementRotation;
        }
        if (obj.useRotation)
        {
            EditorGUIUtility.LookLikeControls(80f, 0);
            Vector3 replacementMinRotation = EditorGUILayout.Vector3Field("Min Rotation", obj.minRotation);
            Vector3 replacementMaxRotation = EditorGUILayout.Vector3Field("Max Rotation", obj.maxRotation);
            if (obj.minRotation != replacementMinRotation || obj.maxRotation != replacementMaxRotation)
            {
                NGUIEditorTools.RegisterUndo("Rotation Change", target);
                obj.minRotation = replacementMinRotation;
                obj.maxRotation = replacementMaxRotation;
            }

            // Rotation Rate
            bool replacementRotationRate = EditorGUILayout.BeginToggleGroup("Rotation Rate", obj.useRotationRate);
            if (obj.useRotationRate != replacementRotationRate)
            {
                NGUIEditorTools.RegisterUndo("Use Rotation Rate Change", target);
                obj.useRotationRate = replacementRotationRate;
            }
            GUILayout.BeginHorizontal();
            obj.rotationRateX = EditorGUILayout.CurveField("X", obj.rotationRateX);
            if (GUILayout.Button("Copy", GUILayout.Width(40)))
            {
                copiedAnimationCurve = new AnimationCurve(obj.rotationRateX.keys);
            }
            if (GUILayout.Button("Paste", GUILayout.Width(40)))
            {
                NGUIEditorTools.RegisterUndo("Paste", target);
                obj.rotationRateX = new AnimationCurve(copiedAnimationCurve.keys);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            obj.rotationRateY = EditorGUILayout.CurveField("Y", obj.rotationRateY);
            if (GUILayout.Button("Copy", GUILayout.Width(40)))
            {
                copiedAnimationCurve = new AnimationCurve(obj.rotationRateY.keys);
            }
            if (GUILayout.Button("Paste", GUILayout.Width(40)))
            {
                NGUIEditorTools.RegisterUndo("Paste", target);
                obj.rotationRateY = new AnimationCurve(copiedAnimationCurve.keys);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            obj.rotationRateZ = EditorGUILayout.CurveField("Z", obj.rotationRateZ);
            if (GUILayout.Button("Copy", GUILayout.Width(40)))
            {
                copiedAnimationCurve = new AnimationCurve(obj.rotationRateZ.keys);
            }
            if (GUILayout.Button("Paste", GUILayout.Width(40)))
            {
                NGUIEditorTools.RegisterUndo("Paste", target);
                obj.rotationRateZ = new AnimationCurve(copiedAnimationCurve.keys);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
            EditorGUIUtility.LookLikeControls();
        }

        //--------------------------------------------
        // Animate color
        EditorGUIUtility.LookLikeControls();
        bool replacementAnimatedColor = EditorGUILayout.Foldout(obj.useAnimatedColor, "Use Animated Color?", EditorStyles.toggleGroup);
        if (obj.useAnimatedColor != replacementAnimatedColor)
        {
            NGUIEditorTools.RegisterUndo("Use Animated Color Change", target);
            obj.useAnimatedColor = replacementAnimatedColor;
        }
        if (obj.useAnimatedColor)
        {
            if (obj.colors.Count < 2)
            {
                obj.colors.Add(Color.white);
                obj.colors.Add(new Color(1, 1, 1, 0));
            }

            int idx = 0;
            {
                var __list2 = obj.colors;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var col = (Color)__list2[__i2];
                    {
                        GUILayout.BeginHorizontal();
                        obj.colors[idx] = EditorGUILayout.ColorField(col);

                        if (obj.colors.Count > 2)
                        {
                            if (GUILayout.Button("X", GUILayout.Width(32)))
                            {
                                NGUIEditorTools.RegisterUndo("Color Delete", target);
                                obj.colors.RemoveAt(idx);
                                break;
                            }
                        }
                        GUILayout.EndHorizontal();
                        idx++;
                    }
                }
            }
            if (GUILayout.Button("Add Color", GUILayout.Height(16)))
            {
                NGUIEditorTools.RegisterUndo("Color Add", target);
                obj.colors.Add(Color.white);
            }
        }
    }


    static void DrawDepth(SerializedObject so, UIWidget w, bool isPrefab)
    {
        if (isPrefab) return;

        GUILayout.Space(2f);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Sprite Depth");

            if (GUILayout.Button("Back", GUILayout.MinWidth(46f)))
            {
                {
                    // foreach(var go in Selection.gameObjects)
                    var __enumerator3 = (Selection.gameObjects).GetEnumerator();
                    while (__enumerator3.MoveNext())
                    {
                        var go = (GameObject)__enumerator3.Current;
                        {
                            UIWidget pw = go.GetComponent<UIWidget>();
                            if (pw != null) pw.depth = w.depth - 1;
                        }
                    }
                }
            }

            NGUIEditorTools.DrawProperty("", so, "mDepth", GUILayout.MinWidth(20f));

            if (GUILayout.Button("Forward", GUILayout.MinWidth(60f)))
            {
                {
                    // foreach(var go in Selection.gameObjects)
                    var __enumerator4 = (Selection.gameObjects).GetEnumerator();
                    while (__enumerator4.MoveNext())
                    {
                        var go = (GameObject)__enumerator4.Current;
                        {
                            UIWidget pw = go.GetComponent<UIWidget>();
                            if (pw != null) pw.depth = w.depth + 1;
                        }
                    }
                }
            }
        }
        GUILayout.EndHorizontal();

        int matchingDepths = 1;

        UIPanel p = w.panel;

        if (p != null)
        {
            for (int i = 0; i < p.widgets.Count; ++i)
            {
                UIWidget pw = p.widgets[i];
                if (pw != w && pw.depth == w.depth)
                    ++matchingDepths;
            }
        }

        if (matchingDepths > 1)
        {
            EditorGUILayout.HelpBox(matchingDepths + " widgets are sharing the depth value of " + w.depth, MessageType.Info);
        }
    }
}
#endif
