#if UNITY_EDITOR || !UNITY_FLASH
#define REFLECTION_SUPPORT
#endif

#region using

//namespace Assets
using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
//{
//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------
#if REFLECTION_SUPPORT
using System.Reflection;
using System.Diagnostics;

#endif

#endregion

/// <summary>
///     Reference to a specific field or property that can be set via inspector.
/// </summary>
[Serializable]
public class TargetBindingProperty
{
    private static readonly Dictionary<PropertyInfo, Action<object, object>> mPropertySetters =
        new Dictionary<PropertyInfo, Action<object, object>>();

    private static readonly Dictionary<FieldInfo, Action<object, object>> mFieldSetters =
        new Dictionary<FieldInfo, Action<object, object>>();

    private static void RegisterPropertySetter(PropertyInfo info, Action<object, object> act)
    {
        mPropertySetters[info] = act;
    }

    private static void RegisterFieldSetter(FieldInfo info, Action<object, object> act)
    {
        mFieldSetters[info] = act;
    }

    public static void SetLableText(object obj, object value)
    {
        ((UILabel) obj).text = (string) value;
    }

    public static void SetInputValue(object obj, object value)
    {
        ((UIInput) obj).value = (string) value;
    }

    public static void SetInputText(object obj, object value)
    {
        ((UIInput) obj).value = (string) value;
    }

    public static void SetSpriteEnabled(object obj, object value)
    {
        ((UISprite) obj).enabled = (bool) value;
    }

    public static void SetTransformActive(object obj, object value)
    {
        ((Transform) obj).active = (bool) value;
    }

    public static void SetSpriteActive(object obj, object value)
    {
        ((UISprite) obj).active = (bool) value;
    }

    public static void SetBindDataRootSource(object obj, object value)
    {
        ((BindDataRoot) obj).Source = value;
    }

    public static void SetLabelText(object obj, object value)
    {
        ((UILabel) obj).text = (string) value;
    }

    public static void SetGridSimpleSource(object obj, object value)
    {
        ((UIGridSimple) obj).Source = value;
    }

    public static void SetSpriteAtlas(object obj, object value)
    {
        ((UISprite) obj).atlas = (UIAtlas) value;
    }

    public static void SetButtonNormalSprite(object obj, object value)
    {
        ((UIButton) obj).normalSprite = (string) value;
    }

    public static void SetButtonIsEnabled(object obj, object value)
    {
        ((UIButton) obj).isEnabled = (bool) value;
    }

    public static void SetTransformLocalPosition(object obj, object value)
    {
        ((Transform) obj).localPosition = (Vector3) value;
    }

    public static void SetTextureWidth(object obj, object value)
    {
        ((UITexture) obj).width = (int) value;
    }

    public static void SetTextureHeight(object obj, object value)
    {
        ((UITexture) obj).height = (int) value;
    }

    public static void SetTextureMainTexture(object obj, object value)
    {
        ((UITexture) obj).mainTexture = (Texture) value;
    }

    public static void SetTransformEulerAngles(object obj, object value)
    {
        ((Transform) obj).eulerAngles = (Vector3) value;
    }

    public static void SetSliderValue(object obj, object value)
    {
        ((UISlider) obj).value = (float) value;
    }

    public static void SetSpriteSpriteName(object obj, object value)
    {
        ((UISprite) obj).spriteName = (string) value;
    }

    public static void SetSliderDoubleMaxValue(object obj, object value)
    {
        ((UISliderDouble) obj).MaxValue = (int) value;
    }

    public static void SetSliderDoubleTargetValue(object obj, object value)
    {
        ((UISliderDouble) obj).TargetValue = (float) value;
    }

    public static void SetSliderDoubleIsReset(object obj, object value)
    {
        ((UISliderDouble) obj).IsReset = (int) value;
    }

    public static void SetSliderNormalTargetValue(object obj, object value)
    {
        ((UISliderNormal) obj).TargetValue = (float) value;
    }

    public static void SetSliderNormalMaxValue(object obj, object value)
    {
        ((UISliderNormal) obj).MaxValue = (int) value;
    }

    public static void SetMultiSliderLogicLayerCount(object obj, object value)
    {
        ((MultiSliderLogic) obj).LayerCount = (int) value;
    }

    public static void SetMultiSliderLogicMaxValue(object obj, object value)
    {
        ((MultiSliderLogic) obj).MaxValue = (int) value;
    }

    public static void SetMultiSliderLogicTargetValue(object obj, object value)
    {
        ((MultiSliderLogic) obj).TargetValue = (int) value;
    }

    public static void SetMultiSliderLogicIsReset(object obj, object value)
    {
        ((MultiSliderLogic) obj).IsReset = (int) value;
    }

    public static void SetListBindItemSource(object obj, object value)
    {
        ((UIListBindItem) obj).Source = value;
    }

    public static void SetChatMessageLogicChannelId(object obj, object value)
    {
        ((ChatMessageLogic) obj).ChannelId = (int) value;
    }

    public static void SetChatMessageLogicCharName(object obj, object value)
    {
        ((ChatMessageLogic) obj).CharName = (string) value;
    }

    public static void SetChatMessageLogicText(object obj, object value)
    {
        ((ChatMessageLogic) obj).Text = (string) value;
    }

    public static void SetTimerLogicTargetTime(object obj, object value)
    {
        ((TimerLogic) obj).TargetTime = (DateTime) value;
    }

    public static void SetSpriteColor(object obj, object value)
    {
        ((UISprite) obj).color = (Color) value;
    }

    public static void SetProgressBarValue(object obj, object value)
    {
        ((UIProgressBar) obj).value = (float) value;
    }

    public static void SetLabelColor(object obj, object value)
    {
        ((UILabel) obj).color = (Color) value;
    }

    public static void SetToggleEnabled(object obj, object value)
    {
        ((UIToggle) obj).enabled = (bool) value;
    }

    public static void SetSpriteWidth(object obj, object value)
    {
        ((UISprite) obj).width = (int) value;
    }

    public static void SetSpriteHeight(object obj, object value)
    {
        ((UISprite) obj).height = (int) value;
    }

    public static void SetListDataBindSource(object obj, object value)
    {
        ((ListDataBind) obj).Source = value;
    }

    public static void SetClassBindingEnabled(object obj, object value)
    {
        ((UIClassBinding) obj).enabled = (bool) value;
    }

    public static void Register()
    {
        mPropertySetters.Clear();
        mFieldSetters.Clear();

        var property = typeof (UILabel).GetProperty("text");
        RegisterPropertySetter(property, SetLableText);

        property = typeof (UIInput).GetProperty("value");
        RegisterPropertySetter(property, SetInputValue);

        property = typeof (UIInput).GetProperty("text");
        RegisterPropertySetter(property, SetInputText);

        property = typeof (UISprite).GetProperty("enabled");
        RegisterPropertySetter(property, SetSpriteEnabled);

        property = typeof (Transform).GetProperty("active");
        RegisterPropertySetter(property, SetTransformActive);


        property = typeof (UISprite).GetProperty("active");
        RegisterPropertySetter(property, SetSpriteActive);

        property = typeof (BindDataRoot).GetProperty("Source");
        RegisterPropertySetter(property, SetBindDataRootSource);

        property = typeof (UILabel).GetProperty("text");
        RegisterPropertySetter(property, SetLabelText);

        property = typeof (UIGridSimple).GetProperty("Source");
        RegisterPropertySetter(property, SetGridSimpleSource);

//         property = typeof(SelectRoleLogic).GetProperty("LastSelectIndex");
//         RegisterPropertySetter(property, (obj, value) =>
//         {
//             ((SelectRoleLogic)obj).LastSelectIndex = (int)value;
//         });

//         property = typeof(SelectRoleLogic).GetProperty("SelectedRoleId");
//         RegisterPropertySetter(property, (obj, value) =>
//         {
//             ((SelectRoleLogic)obj).SelectedRoleId = (ulong)value;
// 
//         });      

        property = typeof (UISprite).GetProperty("atlas");
        RegisterPropertySetter(property, SetSpriteAtlas);

        property = typeof (UIButton).GetProperty("normalSprite");
        RegisterPropertySetter(property, SetButtonNormalSprite);

        property = typeof (UIButton).GetProperty("isEnabled");
        RegisterPropertySetter(property, SetButtonIsEnabled);

        property = typeof (Transform).GetProperty("localPosition");
        RegisterPropertySetter(property, SetTransformLocalPosition);

        property = typeof (UITexture).GetProperty("width");
        RegisterPropertySetter(property, SetTextureWidth);

        property = typeof (UITexture).GetProperty("height");
        RegisterPropertySetter(property, SetTextureHeight);

        property = typeof (UITexture).GetProperty("mainTexture");
        RegisterPropertySetter(property, SetTextureMainTexture);

        property = typeof (Transform).GetProperty("eulerAngles");
        RegisterPropertySetter(property, SetTransformEulerAngles);

        property = typeof (UISlider).GetProperty("value");
        RegisterPropertySetter(property, SetSliderValue);

        property = typeof (UISprite).GetProperty("spriteName");
        RegisterPropertySetter(property, SetSpriteSpriteName);

        property = typeof (UISliderDouble).GetProperty("MaxValue");
        RegisterPropertySetter(property, SetSliderDoubleMaxValue);

        property = typeof (UISliderDouble).GetProperty("TargetValue");
        RegisterPropertySetter(property, SetSliderDoubleTargetValue);

        property = typeof (UISliderDouble).GetProperty("IsReset");
        RegisterPropertySetter(property, SetSliderDoubleIsReset);

        property = typeof (UISliderNormal).GetProperty("TargetValue");
        RegisterPropertySetter(property, SetSliderNormalTargetValue);

        property = typeof (UISliderNormal).GetProperty("MaxValue");
        RegisterPropertySetter(property, SetSliderNormalMaxValue);


        property = typeof (MultiSliderLogic).GetProperty("LayerCount");
        RegisterPropertySetter(property, SetMultiSliderLogicLayerCount);

        property = typeof (MultiSliderLogic).GetProperty("MaxValue");
        RegisterPropertySetter(property, SetMultiSliderLogicMaxValue);

        property = typeof (MultiSliderLogic).GetProperty("TargetValue");
        RegisterPropertySetter(property, SetMultiSliderLogicTargetValue);

        property = typeof (MultiSliderLogic).GetProperty("IsReset");
        RegisterPropertySetter(property, SetMultiSliderLogicIsReset);


        property = typeof (UIListBindItem).GetProperty("Source");
        RegisterPropertySetter(property, SetListBindItemSource);


//         property = typeof(SkillBarItemLogic).GetProperty("SkillItemDataModel");
//         RegisterPropertySetter(property, (obj, value) =>
//         {
//             ((SkillBarItemLogic)obj).SkillItemDataModel =(SkillItemDataModel)value;
//         });     

//         property = typeof(ChatMessageLogic).GetProperty("ChatMessageData");
//         RegisterPropertySetter(property, (obj, value) =>
//         {
//             ((ChatMessageLogic)obj).ChatMessageData = (ChatMessageDataModel)value;
//         });

        property = typeof (ChatMessageLogic).GetProperty("ChannelId");
        RegisterPropertySetter(property, SetChatMessageLogicChannelId);

        property = typeof (ChatMessageLogic).GetProperty("CharName");
        RegisterPropertySetter(property, SetChatMessageLogicCharName);
        property = typeof (ChatMessageLogic).GetProperty("Text");
        RegisterPropertySetter(property, SetChatMessageLogicText);

        property = typeof (TimerLogic).GetProperty("TargetTime");
        RegisterPropertySetter(property, SetTimerLogicTargetTime);

        property = typeof (UISprite).GetProperty("color");
        RegisterPropertySetter(property, SetSpriteColor);

        property = typeof (UIProgressBar).GetProperty("value");
        RegisterPropertySetter(property, SetProgressBarValue);

        property = typeof (UILabel).GetProperty("color");
        RegisterPropertySetter(property, SetLabelColor);

        property = typeof (UIToggle).GetProperty("enabled");
        RegisterPropertySetter(property, SetToggleEnabled);


        property = typeof (UISprite).GetProperty("width");
        RegisterPropertySetter(property, SetSpriteWidth);
        property = typeof (UISprite).GetProperty("height");
        RegisterPropertySetter(property, SetSpriteHeight);

        property = typeof (ListDataBind).GetProperty("Source");
        RegisterPropertySetter(property, SetListDataBindSource);

        property = typeof (UIClassBinding).GetProperty("enabled");
        RegisterPropertySetter(property, SetClassBindingEnabled);
    }


    [SerializeField] private Component mTarget;
    [SerializeField] private string mName;

#if REFLECTION_SUPPORT
    private FieldInfo mField;
    private PropertyInfo mProperty;

    private Action<object, object> mSetAction;
#endif

    public string AtlasValue { get; set; }

    /// <summary>
    ///     Event delegate's target object.
    /// </summary>
    public Component target
    {
        get { return mTarget; }
        set
        {
            mTarget = value;
#if REFLECTION_SUPPORT
            mProperty = null;
            mField = null;
#endif
        }
    }

    /// <summary>
    ///     Event delegate's method name.
    /// </summary>
    public string name
    {
        get { return mName; }
        set
        {
            mName = value;
#if REFLECTION_SUPPORT
            mProperty = null;
            mField = null;
#endif
        }
    }

    /// <summary>
    ///     Whether this delegate's values have been set.
    /// </summary>
    public bool isValid
    {
        get { return (mTarget != null && !string.IsNullOrEmpty(mName)); }
    }

    /// <summary>
    ///     Whether the target script is actually enabled.
    /// </summary>
    public bool isEnabled
    {
        get
        {
            if (mTarget == null)
            {
                return false;
            }
            var mb = (mTarget as MonoBehaviour);
            return (mb == null || mb.enabled);
        }
    }

    public TargetBindingProperty()
    {
    }

    public TargetBindingProperty(Component target, string fieldName)
    {
        mTarget = target;
        mName = fieldName;
    }

    /// <summary>
    ///     Helper function that returns the property type.
    /// </summary>
    public Type GetPropertyType()
    {
#if REFLECTION_SUPPORT
        if (mProperty == null && mField == null && isValid)
        {
            Cache();
        }
        if (mProperty != null)
        {
            return mProperty.PropertyType;
        }
        if (mField != null)
        {
            return mField.FieldType;
        }
#endif
#if UNITY_EDITOR || !UNITY_FLASH
        return typeof (void);
#else
		return null;
#endif
    }

    /// <summary>
    ///     Equality operator.
    /// </summary>
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return !isValid;
        }

        if (obj is TargetBindingProperty)
        {
            var pb = obj as TargetBindingProperty;
            return (mTarget == pb.mTarget && string.Equals(mName, pb.mName));
        }
        return false;
    }

    private static readonly int s_Hash = "PropertyBinding".GetHashCode();

    /// <summary>
    ///     Used in equality operators.
    /// </summary>
    public override int GetHashCode()
    {
        return s_Hash;
    }

    /// <summary>
    ///     Set the delegate callback using the target and method names.
    /// </summary>
    public void Set(Component target, string methodName)
    {
        mTarget = target;
        mName = methodName;
    }

    /// <summary>
    ///     Clear the event delegate.
    /// </summary>
    public void Clear()
    {
        mTarget = null;
        mName = null;
    }

    /// <summary>
    ///     Reset the cached references.
    /// </summary>
    public void Reset()
    {
#if REFLECTION_SUPPORT
        mField = null;
        mProperty = null;
#endif
    }

    /// <summary>
    ///     Convert the delegate to its string representation.
    /// </summary>
    public override string ToString()
    {
        return ToString(mTarget, name);
    }

    /// <summary>
    ///     Convenience function that converts the specified component + property pair into its string representation.
    /// </summary>
    public static string ToString(Component comp, string property)
    {
        if (comp != null)
        {
            var typeName = comp.GetType().ToString();
            var period = typeName.LastIndexOf('.');
            if (period > 0)
            {
                typeName = typeName.Substring(period + 1);
            }

            if (!string.IsNullOrEmpty(property))
            {
                return typeName + "." + property;
            }
            return typeName + ".[property]";
        }
        return null;
    }

#if REFLECTION_SUPPORT
    /// <summary>
    ///     Retrieve the property's value.
    /// </summary>
    [DebuggerHidden]
    [DebuggerStepThrough]
    public object Get()
    {
        if (mProperty == null && mField == null && isValid)
        {
            Cache();
        }

        if (mProperty != null)
        {
            if (mProperty.CanRead)
            {
                return mProperty.GetValue(mTarget, null);
            }
        }
        else if (mField != null)
        {
            return mField.GetValue(mTarget);
        }
        return null;
    }

    /// <summary>
    ///     Assign the bound property's value.
    /// </summary>
    [DebuggerHidden]
    [DebuggerStepThrough]
    public bool Set(object value)
    {
        if (mProperty == null && mField == null && isValid)
        {
            Cache();
        }
        if (mProperty == null && mField == null)
        {
            return false;
        }

        if (mTarget == null)
        {
            Logger.Error("TargetBindingProperty ... mTarget .. null");
            return false;
        }
        if (value == null)
        {
            try
            {
                if (mProperty != null)
                {
                    if (mProperty.CanWrite)
                    {
                        if (mSetAction != null)
                        {
                            mSetAction(mTarget, null);
                        }
                        else
                        {
                            mProperty.SetValue(mTarget, null, null);
                        }

                        return true;
                    }
                }
                else
                {
                    mField.SetValue(mTarget, null);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Can we set the value?
        if (!Convert(ref value))
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Target:[" + mTarget + "],Property:[" + mProperty + "],Unable to convert " +
                               value.GetType() + " to " + GetPropertyType());
            }
        }
        else if (mField != null)
        {
            mField.SetValue(mTarget, value);
            return true;
        }
        else if (mProperty != null && mProperty.CanWrite)
        {
            if (mSetAction != null)
            {
                mSetAction(mTarget, value);
            }
            else
            {
                mProperty.SetValue(mTarget, value, null);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Cache the field or property.
    /// </summary>
    [DebuggerHidden]
    [DebuggerStepThrough]
    private bool Cache()
    {
        if (mTarget != null && !string.IsNullOrEmpty(mName))
        {
            var type = mTarget.GetType();
#if NETFX_CORE
			mField = type.GetRuntimeField(mName);
			mProperty = type.GetRuntimeProperty(mName);
#else
            mField = type.GetField(mName);
            mProperty = type.GetProperty(mName);

//             if (mField != null)
//             {
//                 Logger.Error("Field TargetBind Type = {0} Field = {1} Name = {2}", type.Name, mField.Name, mName);
//             }
// 
            if (mProperty != null)
            {
                mPropertySetters.TryGetValue(mProperty, out mSetAction);
                //Logger.Error("Property TargetBind Type = {0} Property = {1} Name = {2}", type.Name, mProperty.Name, mName);
            }
#endif
        }
        else
        {
            mField = null;
            mProperty = null;
        }
        return (mField != null || mProperty != null);
    }

    /// <summary>
    ///     Whether we can assign the property using the specified value.
    /// </summary>
    private bool Convert(ref object value)
    {
        if (mTarget == null)
        {
            return false;
        }

        var to = GetPropertyType();
        Type from;

        if (value == null)
        {
#if NETFX_CORE
			if (!to.GetTypeInfo().IsClass) return false;
#else
            if (!to.IsClass)
            {
                return false;
            }
#endif
            from = to;
        }
        else
        {
            from = value.GetType();
        }
        return Convert(ref value, from, to);
    }
#else // Everything below = no reflection support
	public object Get ()
	{
		Debug.LogError("Reflection is not supported on this platform");
		return null;
	}

	public bool Set (object value)
	{
		Debug.LogError("Reflection is not supported on this platform");
		return false;
	}

	bool Cache () { return false; }
	bool Convert (ref object value) { return false; }
#endif

    /// <summary>
    ///     Whether we can convert one type to another for assignment purposes.
    /// </summary>
    public static bool Convert(Type from, Type to)
    {
        object temp = null;
        return Convert(ref temp, from, to);
    }

    /// <summary>
    ///     Whether we can convert one type to another for assignment purposes.
    /// </summary>
    public static bool Convert(object value, Type to)
    {
        if (value == null)
        {
            value = null;
            return Convert(ref value, to, to);
        }
        return Convert(ref value, value.GetType(), to);
    }

    /// <summary>
    ///     Whether we can convert one type to another for assignment purposes.
    /// </summary>
    public static bool Convert(ref object value, Type from, Type to)
    {
#if REFLECTION_SUPPORT
        // If the value can be assigned as-is, we're done
#if NETFX_CORE
		if (to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo())) return true;
#else
        if (to.IsAssignableFrom(from))
        {
            return true;
        }
#endif

#else
		if (from == to) return true;
#endif
        // If the target type is a string, just convert the value
        if (to == typeof (string))
        {
            value = (value != null) ? value.ToString() : "null";
            return true;
        }

        // If the value is null we should not proceed further
        if (value == null)
        {
            return false;
        }

        if (to == typeof (int))
        {
            if (from == typeof (string))
            {
                int val;

                if (int.TryParse((string) value, out val))
                {
                    value = val;
                    return true;
                }
            }
            else if (from == typeof (float))
            {
                value = Mathf.RoundToInt((float) value);
                return true;
            }
        }
        else if (to == typeof (float))
        {
            if (from == typeof (string))
            {
                float val;

                if (float.TryParse((string) value, out val))
                {
                    value = val;
                    return true;
                }
            }
        }
        else if (to.IsEnum)
        {
            return from == typeof (int);
        }
        else if (to == typeof (bool))
        {
            if (from == typeof (string))
            {
                int val;
                if (int.TryParse((string) value, out val))
                {
                    value = val != 0;
                    return true;
                }

                bool bVal;
                if (bool.TryParse((string) value, out bVal))
                {
                    value = bVal;
                    return true;
                }
            }
        }
        return false;
    }
}

//}