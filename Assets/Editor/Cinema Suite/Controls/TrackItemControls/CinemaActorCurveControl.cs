﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CutsceneItemControlAttribute(typeof(CinemaActorClipCurve))]
public class CinemaActorCurveControl : CinemaCurveControl
{
    private const float THRESHOLD = 0.000001f;
    private const float QUATERNION_THRESHOLD = 0.5f;
    private bool hasUserInteracted = false;

    public override void UpdateCurveWrappers(CinemaClipCurveWrapper clipWrapper)
    {
        base.UpdateCurveWrappers(clipWrapper);
    }

    public override void PostUpdate(DirectorControlState state)
    {
        CinemaActorClipCurve clipCurve = Wrapper.Behaviour as CinemaActorClipCurve;
        if (clipCurve == null) return;

        hasUndoRedoBeenPerformed = (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed");
        if ((HaveCurvesChanged || hasUndoRedoBeenPerformed) && state.IsInPreviewMode)
        {
            clipCurve.SampleTime(state.ScrubberPosition);
            HaveCurvesChanged = false;
            hasUserInteracted = false;
        }
        else
        {
            checkToAddNewKeyframes(clipCurve, state);
        }
    }

    protected override void showContextMenu(Behaviour behaviour)
    {
        CinemaActorClipCurve clipCurve = behaviour as CinemaActorClipCurve;
        if (clipCurve == null) return;

        List<KeyValuePair<string, string>> currentCurves = new List<KeyValuePair<string, string>>();
        {
            var __list1 = clipCurve.CurveData;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var data = (MemberClipCurveData)__list1[__i1];
                {
                    KeyValuePair<string, string> curveStrings = new KeyValuePair<string, string>(data.Type, data.PropertyName);
                    currentCurves.Add(curveStrings);
                }
            }
        }
        GenericMenu createMenu = new GenericMenu();

        if (clipCurve.Actor != null)
        {
            Component[] components = DirectorHelper.getValidComponents(clipCurve.Actor.gameObject);

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                MemberInfo[] members = DirectorHelper.getValidMembers(component);
                for (int j = 0; j < members.Length; j++)
                {
                    AddCurveContext context = new AddCurveContext();
                    context.clipCurve = clipCurve;
                    context.component = component;
                    context.memberInfo = members[j];
                    if (!currentCurves.Contains(new KeyValuePair<string, string>(component.GetType().Name, members[j].Name)))
                    {
                        createMenu.AddItem(new GUIContent(string.Format("Add Curve/{0}/{1}", component.GetType().Name, DirectorHelper.GetUserFriendlyName(component, members[j]))), false, addCurve, context);
                    }
                }
            }
            createMenu.AddSeparator(string.Empty);
        }
        createMenu.AddItem(new GUIContent("Copy"), false, copyItem, behaviour);
        createMenu.AddSeparator(string.Empty);
        createMenu.AddItem(new GUIContent("Clear"), false, deleteItem, clipCurve);
        createMenu.ShowAsContext();
    }

    private void addCurve(object userData)
    {
        AddCurveContext arg = userData as AddCurveContext;
        if (arg != null)
        {
            Type t = null;
            PropertyInfo property = arg.memberInfo as PropertyInfo;
            FieldInfo field = arg.memberInfo as FieldInfo;
            bool isProperty = false;
            if (property != null)
            {
                t = property.PropertyType;
                isProperty = true;
            }
            else if (field != null)
            {
                t = field.FieldType;
                isProperty = false;
            }
            Undo.RecordObject(arg.clipCurve, "Added Curve");
            arg.clipCurve.AddClipCurveData(arg.component, arg.memberInfo.Name, isProperty, t);
            EditorUtility.SetDirty(arg.clipCurve);
        }
    }

    private void checkToAddNewKeyframes(CinemaActorClipCurve clipCurve, DirectorControlState state)
    {
        if (state.IsInPreviewMode && IsEditing && GUIUtility.hotControl == 0 &&
            (clipCurve.Firetime <= state.ScrubberPosition &&
            state.ScrubberPosition <= clipCurve.Firetime + clipCurve.Duration) && clipCurve.Actor != null)
        {
            Undo.RecordObject(clipCurve, "Auto Key Created");
            bool hasDifferenceBeenFound = false;
            {
                var __list2 = clipCurve.CurveData;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var data = (MemberClipCurveData)__list2[__i2];
                    {
                        if (data.Type == string.Empty || data.PropertyName == string.Empty) continue;

                        Component component = clipCurve.Actor.GetComponent(data.Type);
                        object value = clipCurve.GetCurrentValue(component, data.PropertyName, data.IsProperty);

                        PropertyTypeInfo typeInfo = data.PropertyType;
                        if (typeInfo == PropertyTypeInfo.Int || typeInfo == PropertyTypeInfo.Long || typeInfo == PropertyTypeInfo.Float ||
                        typeInfo == PropertyTypeInfo.Double)
                        {
                            float x = (float)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);

                            hasDifferenceBeenFound |= addKeyOnUserInteraction(x, curve1Value, data.Curve1, state.ScrubberPosition);

                        }
                        else if (typeInfo == PropertyTypeInfo.Vector2)
                        {
                            Vector2 vec2 = (Vector2)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);
                            float curve2Value = data.Curve2.Evaluate(state.ScrubberPosition);

                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec2.x, curve1Value, data.Curve1, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec2.y, curve2Value, data.Curve2, state.ScrubberPosition);
                        }
                        else if (typeInfo == PropertyTypeInfo.Vector3)
                        {
                            Vector3 vec3 = (Vector3)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);
                            float curve2Value = data.Curve2.Evaluate(state.ScrubberPosition);
                            float curve3Value = data.Curve3.Evaluate(state.ScrubberPosition);

                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec3.x, curve1Value, data.Curve1, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec3.y, curve2Value, data.Curve2, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec3.z, curve3Value, data.Curve3, state.ScrubberPosition);

                        }
                        else if (typeInfo == PropertyTypeInfo.Vector4)
                        {
                            Vector4 vec4 = (Vector4)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);
                            float curve2Value = data.Curve2.Evaluate(state.ScrubberPosition);
                            float curve3Value = data.Curve3.Evaluate(state.ScrubberPosition);
                            float curve4Value = data.Curve4.Evaluate(state.ScrubberPosition);

                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec4.x, curve1Value, data.Curve1, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec4.y, curve2Value, data.Curve2, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec4.z, curve3Value, data.Curve3, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(vec4.w, curve4Value, data.Curve4, state.ScrubberPosition);

                        }
                        else if (typeInfo == PropertyTypeInfo.Quaternion)
                        {
                            Quaternion quaternion = (Quaternion)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);
                            float curve2Value = data.Curve2.Evaluate(state.ScrubberPosition);
                            float curve3Value = data.Curve3.Evaluate(state.ScrubberPosition);
                            float curve4Value = data.Curve4.Evaluate(state.ScrubberPosition);

                            for (int j = 0; j < data.Curve1.length; j++)
                            {
                                Keyframe k = data.Curve1[j];
                                if (k.time == state.ScrubberPosition)
                                {
                                    Keyframe newKeyframe = new Keyframe(k.time, quaternion.x, k.inTangent, k.outTangent);
                                    newKeyframe.tangentMode = k.tangentMode;
                                    AnimationCurveHelper.MoveKey(data.Curve1, j, newKeyframe);
                                }
                            }

                            for (int j = 0; j < data.Curve2.length; j++)
                            {
                                Keyframe k = data.Curve2[j];
                                if (k.time == state.ScrubberPosition)
                                {
                                    Keyframe newKeyframe = new Keyframe(k.time, quaternion.y, k.inTangent, k.outTangent);
                                    newKeyframe.tangentMode = k.tangentMode;
                                    AnimationCurveHelper.MoveKey(data.Curve2, j, newKeyframe);
                                }
                            }

                            for (int j = 0; j < data.Curve3.length; j++)
                            {
                                Keyframe k = data.Curve3[j];
                                if (k.time == state.ScrubberPosition)
                                {
                                    Keyframe newKeyframe = new Keyframe(k.time, quaternion.z, k.inTangent, k.outTangent);
                                    newKeyframe.tangentMode = k.tangentMode;
                                    AnimationCurveHelper.MoveKey(data.Curve3, j, newKeyframe);
                                }
                            }

                            for (int j = 0; j < data.Curve4.length; j++)
                            {
                                Keyframe k = data.Curve4[j];
                                if (k.time == state.ScrubberPosition)
                                {
                                    Keyframe newKeyframe = new Keyframe(k.time, quaternion.w, k.inTangent, k.outTangent);
                                    newKeyframe.tangentMode = k.tangentMode;
                                    AnimationCurveHelper.MoveKey(data.Curve4, j, newKeyframe);
                                }
                            }

                            Quaternion curveValue = new Quaternion(curve1Value, curve2Value, curve3Value, curve4Value);
                            float angle = Vector3.Angle(quaternion.eulerAngles, curveValue.eulerAngles);
                            hasDifferenceBeenFound = hasDifferenceBeenFound || angle > QUATERNION_THRESHOLD;
                            if (angle > QUATERNION_THRESHOLD && hasUserInteracted)
                            {
                                data.Curve1.AddKey(state.ScrubberPosition, quaternion.x);
                                data.Curve2.AddKey(state.ScrubberPosition, quaternion.y);
                                data.Curve3.AddKey(state.ScrubberPosition, quaternion.z);
                                data.Curve4.AddKey(state.ScrubberPosition, quaternion.w);
                                hasUserInteracted = true;
                            }
                        }
                        else if (typeInfo == PropertyTypeInfo.Color)
                        {
                            Color color = (Color)value;
                            float curve1Value = data.Curve1.Evaluate(state.ScrubberPosition);
                            float curve2Value = data.Curve2.Evaluate(state.ScrubberPosition);
                            float curve3Value = data.Curve3.Evaluate(state.ScrubberPosition);
                            float curve4Value = data.Curve4.Evaluate(state.ScrubberPosition);

                            hasDifferenceBeenFound |= addKeyOnUserInteraction(color.r, curve1Value, data.Curve1, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(color.g, curve2Value, data.Curve2, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(color.b, curve3Value, data.Curve3, state.ScrubberPosition);
                            hasDifferenceBeenFound |= addKeyOnUserInteraction(color.a, curve4Value, data.Curve4, state.ScrubberPosition);
                        }
                    }
                }
            }
            if (hasDifferenceBeenFound)
            {
                hasUserInteracted = true;
                EditorUtility.SetDirty(clipCurve);
            }
        }
    }

    private bool addKeyOnUserInteraction(float value, float curveValue, AnimationCurve curve, float scrubberPosition)
    {
        bool differenceFound = false;
        if (!(Math.Abs(value - curveValue) < THRESHOLD))
        {
            differenceFound = true;
            if (hasUserInteracted)
            {
                bool doesKeyExist = false;
                for (int j = 0; j < curve.length; j++)
                {
                    Keyframe k = curve[j];
                    if (k.time == scrubberPosition)
                    {
                        Keyframe newKeyframe = new Keyframe(k.time, value, k.inTangent, k.outTangent);
                        newKeyframe.tangentMode = k.tangentMode;
                        AnimationCurveHelper.MoveKey(curve, j, newKeyframe);
                        doesKeyExist = true;
                    }
                }
                if (!doesKeyExist)
                {
                    Keyframe kf = new Keyframe(scrubberPosition, value);
                    AnimationCurveHelper.AddKey(curve, kf);
                }
            }
        }
        return differenceFound;
    }

    private class AddCurveContext
    {
        public CinemaActorClipCurve clipCurve;
        public Component component;
        public MemberInfo memberInfo;
    }
}
