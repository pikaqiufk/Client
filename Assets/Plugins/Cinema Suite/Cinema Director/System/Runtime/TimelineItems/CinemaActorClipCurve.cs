using CinemaDirector.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    [Serializable, CutsceneItemAttribute("Curve Clip", "Actor Curve Clip", CutsceneItemGenre.CurveClipItem)]
    public class CinemaActorClipCurve : CinemaClipCurve, IRevertable
    {
        // Options for reverting in editor.
        [SerializeField]
        private RevertMode editorRevertMode = RevertMode.Revert;

        // Options for reverting during runtime.
        [SerializeField]
        private RevertMode runtimeRevertMode = RevertMode.Revert;

        public GameObject Actor
        {
            get
            {
                GameObject actor = null;
                if (transform.parent != null)
                {
                    CurveTrack track = transform.parent.GetComponent<CurveTrack>();
                    if (track != null && track.Actor != null)
                    {
                        actor = track.Actor.gameObject;
                    }
                }
                return actor;
            }
        }

        protected override void initializeClipCurves(MemberClipCurveData data, Component component)
        {
            object value = GetCurrentValue(component, data.PropertyName, data.IsProperty);
            PropertyTypeInfo typeInfo = data.PropertyType;
            float startTime = Firetime;
            float endTime = Firetime + Duration;

            if (typeInfo == PropertyTypeInfo.Int || typeInfo == PropertyTypeInfo.Long || typeInfo == PropertyTypeInfo.Float || typeInfo == PropertyTypeInfo.Double)
            {
                float x = (float)value;
                data.Curve1 = AnimationCurve.Linear(startTime, x, endTime, x);
            }
            else if (typeInfo == PropertyTypeInfo.Vector2)
            {
                Vector2 vec2 = (Vector2)value;
                data.Curve1 = AnimationCurve.Linear(startTime, vec2.x, endTime, vec2.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec2.y, endTime, vec2.y);
            }
            else if (typeInfo == PropertyTypeInfo.Vector3)
            {
                Vector3 vec3 = (Vector3)value;
                data.Curve1 = AnimationCurve.Linear(startTime, vec3.x, endTime, vec3.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec3.y, endTime, vec3.y);
                data.Curve3 = AnimationCurve.Linear(startTime, vec3.z, endTime, vec3.z);
            }
            else if (typeInfo == PropertyTypeInfo.Vector4)
            {
                Vector4 vec4 = (Vector4)value;
                data.Curve1 = AnimationCurve.Linear(startTime, vec4.x, endTime, vec4.x);
                data.Curve2 = AnimationCurve.Linear(startTime, vec4.y, endTime, vec4.y);
                data.Curve3 = AnimationCurve.Linear(startTime, vec4.z, endTime, vec4.z);
                data.Curve4 = AnimationCurve.Linear(startTime, vec4.w, endTime, vec4.w);
            }
            else if (typeInfo == PropertyTypeInfo.Quaternion)
            {
                Quaternion quaternion = (Quaternion)value;
                data.Curve1 = AnimationCurve.Linear(startTime, quaternion.x, endTime, quaternion.x);
                data.Curve2 = AnimationCurve.Linear(startTime, quaternion.y, endTime, quaternion.y);
                data.Curve3 = AnimationCurve.Linear(startTime, quaternion.z, endTime, quaternion.z);
                data.Curve4 = AnimationCurve.Linear(startTime, quaternion.w, endTime, quaternion.w);
            }
            else if (typeInfo == PropertyTypeInfo.Color)
            {
                Color color = (Color)value;
                data.Curve1 = AnimationCurve.Linear(startTime, color.r, endTime, color.r);
                data.Curve2 = AnimationCurve.Linear(startTime, color.g, endTime, color.g);
                data.Curve3 = AnimationCurve.Linear(startTime, color.b, endTime, color.b);
                data.Curve4 = AnimationCurve.Linear(startTime, color.a, endTime, color.a);
            }
        }

        public object GetCurrentValue(Component component, string propertyName, bool isProperty)
        {
            if (component == null || propertyName == string.Empty) return null;
            Type type = component.GetType();
            object value = null;
            if (isProperty)
            {
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                value = propertyInfo.GetValue(component, null);
            }
            else
            {
                FieldInfo fieldInfo = type.GetField(propertyName);
                value = fieldInfo.GetValue(component);
            }
            return value;
        }

        public override void Initialize()
        {
            {
                var __list1 = CurveData;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var memberData = (MemberClipCurveData)__list1[__i1];
                    {
                        memberData.Initialize(Actor);
                    }
                }
            }
        }

        /// <summary>
        /// Cache the initial state of the curve clip manipulated values.
        /// </summary>
        /// <returns>The Info necessary to revert this event.</returns>
        public RevertInfo[] CacheState()
        {
            List<RevertInfo> reverts = new List<RevertInfo>();
            if (Actor != null)
            {
                {
                    var __list2 = CurveData;
                    var __listCount2 = __list2.Count;
                    for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                    {
                        var memberData = (MemberClipCurveData)__list2[__i2];
                        {
                            Component component = Actor.GetComponent(memberData.Type);
                            reverts.Add(new RevertInfo(this, component, memberData.PropertyName, memberData.getCurrentValue(component)));
                        }
                    }
                }
            }
            return reverts.ToArray();
        }

        /// <summary>
        /// Sample the curve clip at the given time.
        /// </summary>
        /// <param name="time">The time to evaulate for.</param>
        public void SampleTime(float time)
        {
            if (Actor == null) return;
            if (Firetime <= time && time <= Firetime + Duration)
            {
                {
                    var __list3 = CurveData;
                    var __listCount3 = __list3.Count;
                    for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                    {
                        var memberData = (MemberClipCurveData)__list3[__i3];
                        {
                            if (memberData.Type == string.Empty || memberData.PropertyName == string.Empty) continue;

                            Component component = Actor.GetComponent(memberData.Type);
                            Type componentType = component.GetType();

                            object value = evaluate(memberData, time);

                            if (memberData.IsProperty)
                            {
                                PropertyInfo propertyInfo = componentType.GetProperty(memberData.PropertyName);
                                propertyInfo.SetValue(component, value, null);

                                //object result = propertyInfo.GetValue(component, null);
                            }
                            else
                            {
                                FieldInfo fieldInfo = componentType.GetField(memberData.PropertyName);
                                fieldInfo.SetValue(component, value);
                            }
                        }
                    }
                }
            }
        }

        internal void Reset()
        {
            //foreach (MemberClipCurveData memberData in CurveData)
            //{
            //    memberData.Reset(Actor);
            //}
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Editor.
        /// </summary>
        public RevertMode EditorRevertMode
        {
            get { return editorRevertMode; }
            set { editorRevertMode = value; }
        }

        /// <summary>
        /// Option for choosing when this curve clip will Revert to initial state in Runtime.
        /// </summary>
        public RevertMode RuntimeRevertMode
        {
            get { return runtimeRevertMode; }
            set { runtimeRevertMode = value; }
        }
    }
}