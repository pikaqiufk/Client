#region using

using System;
using ClientDataModel;
using UnityEngine;

#endregion

[Serializable]
public class InverseBinding : MonoBehaviour
{
    private static readonly object[] args = {};
    public BindingClassName BindingName;
    private object BindSource;
    public BindingDataExpression Expression;
    private bool ListFlag;
    private object mDataSource;
    private string PropertyName;
    public TargetBindingProperty Target;

    public object DataSource
    {
        get { return mDataSource; }
        set
        {
            if (mDataSource != value)
            {
                mDataSource = value;
                if (mDataSource != null)
                {
                    Analyse();
                    UpdateSourceData();
                }
            }
        }
    }

    public EventDelegate EventDelegate { get; set; }

    public void Analyse()
    {
        var path = Expression.DataExpressionName.Split('.');
        var bindSource = mDataSource;
        for (var i = 0; i < path.Length; i++)
        {
            if (i == path.Length - 1)
            {
                PropertyName = path[i];
                BindSource = bindSource;
                return;
            }
            var prop = bindSource.GetType().GetProperty(path[i]);
            var value = prop.GetGetMethod().Invoke(bindSource, args);
            if (prop.IsDefined(typeof (ListSize), false))
            {
                if (i == path.Length - 2)
                {
                    ListFlag = true;
                    PropertyName = path[i + 1];
                    BindSource = value;
                    return;
                }
                var objList = value as IReadonlyList;
                var listIndex = int.Parse(path[i + 1]);
                value = objList[listIndex];
                i++;
            }
            bindSource = value;
        }
    }

    public void SetValue(object value)
    {
        if (BindSource != null)
        {
            if (ListFlag)
            {
                var objList = BindSource as IWriteableList;
                var listIndex = int.Parse(PropertyName);
                objList[listIndex] = value;
            }
            else
            {
                var prop = BindSource.GetType().GetProperty(PropertyName);
                if (prop != null)
                {
                    var method = prop.GetSetMethod();
                    if (method != null)
                    {
                        method.Invoke(BindSource, new[] {value});
                    }
                }
            }
        }
    }

    private void Start()
    {
    }

    public void UpdateSourceData()
    {
        try
        {
            var value = Target.Get();
            SetValue(value);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
}