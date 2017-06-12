#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ClientDataModel;
using DataTable;
using SignalChain;
using UnityEngine;

#endregion

[Serializable]
//[DisallowMultipleComponent]
public class UIClassBinding : MonoBehaviour
{
    private static readonly object[] args = {};
    public List<UIClassBindingItemInner> BindingDatas = new List<UIClassBindingItemInner>();
    //public int Index;
    [SerializeField] public BindingClassName BindingName;
    private readonly List<UIClassBindingItemInner> mBindings = new List<UIClassBindingItemInner>();
    private BubbleSignal<IChainRoot> mBubbleSignal;
    private object mDataSource;

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
                    Unbinding();
                    InitBinding();
                }
            }
        }
    }

    public void AddBinds(UIClassBindingItemInner item)
    {
        if (item.Target.target == null)
        {
            Debug.LogError(string.Format("UIClassBind ....................Error..........TargetName....{0}",
                transform.name));
            return;
        }
        //UIClassBindingItemInner inner = new UIClassBindingItemInner();
        //inner.ExpressionList = item.ExpressionList;
        item.Analyse(mDataSource);

        mBindings.Add(item);
        item.CreateBinding();
    }

    public void Awake()
    {
        if (mBubbleSignal == null)
        {
            mBubbleSignal = new BubbleSignal<IChainRoot>(gameObject.transform);
        }
    }

    public void InitBinding()
    {
        {
            var __list6 = BindingDatas;
            var __listCount6 = __list6.Count;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var item = __list6[__i6];
                {
                    AddBinds(item);
                }
            }
        }
    }

    public void SignalActiveChanged()
    {
        if (mBubbleSignal == null)
        {
            mBubbleSignal = new BubbleSignal<IChainRoot>(gameObject.transform);
            //return;
        }
        mBubbleSignal.TargetedDispatch("ActiveChanged");
    }

    public void Unbinding()
    {
        {
            var __list5 = mBindings;
            var __listCount5 = __list5.Count;
            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
            {
                var inner = __list5[__i5];
                {
                    inner.UpdataBinding();
                }
            }
        }
        mBindings.Clear();
    }

    [Serializable]
    public class UIClassBindingItemInner
    {
        private static readonly Dictionary<string, PropertyInfo> mCacheProp = new Dictionary<string, PropertyInfo>();

        private static Dictionary<string, INotifyPropertyChanged> mDictionary =
            new Dictionary<string, INotifyPropertyChanged>();

        public UIClassBindingItemInner()
        {
            SourceList = new List<BindSourceData>();
            FromatId = -1;
        }

        //public BindingDataExpression Expression ;
        public List<BindingDataExpression> ExpressionList;
        public int FromatId = -1;
        public string FromatString;
        public string InvisibleValue;
        private string[] mInvisibleListStr;
        public List<BindSourceData> SourceList;
        public TargetBindingProperty Target;
        public object DefaultValue { get; set; }

        public void Analyse(object source)
        {
            SetInvisibleStrList();
            SetFormatString();
            SetDefault();
            SourceList.Clear();
            {
                var __list1 = ExpressionList;
                var __listCount1 = __list1.Count;
                for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var expression = __list1[__i1];
                    {
                        try
                        {
                            AnalyseEx(source, expression);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(string.Format("UIClasssBinding:name={0}\n{1}",
                                GetFullPath(Target.target.transform), ex));
                        }
                    }
                }
            }
        }

        public void AnalyseEx(object source, BindingDataExpression expression)
        {
            var data = new BindSourceData();

            var path = expression.DataExpressionName.Split('.');
            var flags = BindingFlags.Instance | BindingFlags.Public;
            INotifyPropertyChanged notify = null;
            var bindSource = source;

            if (path.Length == 1 && path[0] == "this")
            {
                data.RootData = source;
                data.Source = bindSource;
                data.PropertyName = path[0];
                SourceList.Add(data);
                return;
            }

            for (var i = 0; i < path.Length; i++)
            {
                if (bindSource == null)
                {
                    return;
                }
                var prop = bindSource.GetType().GetProperty(path[i]);

                if (prop.Name == path[i] && i == path.Length - 1)
                {
                    data.PropertyName = path[i];
                    break;
                }

                var value = prop.GetGetMethod().Invoke(bindSource, args);
                if (prop.IsDefined(typeof (ListSize), false) && prop.IsDefined(typeof (TableBinding), false))
                {
                    var objList = value as IReadonlyList;
                    //int listIndex = int.Parse(path[i + 1]);
                    //value = objList[listIndex];

                    var obj = prop.GetCustomAttributes(typeof (TableBinding), false);
                    var attr = obj as TableBinding[];

                    data.TableColumns = new List<string>();
                    data.PropertyName = path[i + 1];
                    data.TableName = attr[0].TableName;
                    for (var j = i + 2; j < path.Length; j++)
                    {
                        data.TableColumns.Add(path[j]);
                    }

                    data.ListFlag = true;
                    bindSource = objList;
                    break;
                }
                if (prop.IsDefined(typeof (ListSize), false))
                {
                    var objList = value as IReadonlyList;

                    var listIndex = int.Parse(path[i + 1]);

                    value = objList[listIndex];
                    i++;

                    if (i == path.Length - 1)
                    {
                        data.ListFlag = true;
                        bindSource = objList;
                        data.PropertyName = path[i];
                        break;
                    }
                }
                else if (prop.IsDefined(typeof (TableBinding), false))
                {
                    var obj = prop.GetCustomAttributes(typeof (TableBinding), false);
                    var attr = obj as TableBinding[];

                    data.TableColumns = new List<string>();
                    data.PropertyName = path[i];
                    data.TableName = attr[0].TableName;
                    for (var j = i + 1; j < path.Length; j++)
                    {
                        data.TableColumns.Add(path[j]);
                    }

                    break;
                }

                if (prop.Name == path[i] && i == path.Length - 1)
                {
                    data.PropertyName = path[i];
                    break;
                }
                bindSource = value;
            }

            notify = bindSource as INotifyPropertyChanged;
            if (notify != null)
            {
                data.RootData = source;
                data.Source = bindSource;
                SourceList.Add(data);
            }
        }

        public void CreateBinding()
        {
            {
                var __list3 = SourceList;
                var __listCount3 = __list3.Count;
                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var data = __list3[__i3];
                    {
                        var notofy = data.Source as INotifyPropertyChanged;
                        if (notofy != null)
                        {
                            notofy.PropertyChanged += UpdataBindingData;
                            UpdataBindingData(data.Source, new PropertyChangedEventArgs(data.PropertyName));
                        }
                    }
                }
            }
        }

        public static object GetDataFromSource(object dataSource, string s)
        {
            object retObj = null;

            var path = s.Split('.');
            var flags = BindingFlags.Instance | BindingFlags.Public;
            INotifyPropertyChanged notify = null;

            var mDataSource = dataSource;
            for (var i = 0; i < path.Length; i++)
            {
                var prop = mDataSource.GetType().GetProperty(path[i]);
                if (prop == null)
                {
                    Debug.LogError(string.Format("UIClasssBinding:Property is null. name={0},Property={1}",
                        dataSource.GetType().Name, path[i]));
                    return null;
                }
                var value = prop.GetGetMethod().Invoke(mDataSource, args);

                if (prop.IsDefined(typeof (ListSize), false) && prop.IsDefined(typeof (TableBinding), false))
                {
                    var objList = value as IReadonlyList;
                    var listIndex = int.Parse(path[i + 1]);
                    value = objList[listIndex];

                    var obj = prop.GetCustomAttributes(typeof (TableBinding), false);
                    var attr = obj as TableBinding[];

                    var mTableName = attr[0].TableName;

                    i++;

                    return GetTableValue(value, mTableName, path, i + 1);
                }
                if (prop.IsDefined(typeof (ListSize), false))
                {
                    var objList = value as IReadonlyList;
                    var listIndex = int.Parse(path[i + 1]);

                    value = objList[listIndex];
                    i++;
                }
                else if (prop.IsDefined(typeof (TableBinding), false))
                {
                    var obj = prop.GetCustomAttributes(typeof (TableBinding), false);
                    var attr = obj as TableBinding[];
                    var mTableName = attr[0].TableName;

                    return GetTableValue(value, mTableName, path, i + 1);
                }

                mDataSource = value;

                if (i == path.Length - 1)
                {
                    retObj = value;
                }
            }

            if (retObj == null)
            {
                Debug.LogWarning(string.Format("------getDataFromSource NULL! source:{0} ParameterName:{1}------",
                    dataSource.GetType(), s));
            }
            return retObj;
        }

        public static string GetFullPath(Transform o)
        {
            if (null != o.parent)
            {
                return GetFullPath(o.parent) + "." + o.name;
            }
            return o.name;
        }

        public object GetPropertyByName(object obj, string name)
        {
            if (name == null || name == "")
            {
                Logger.Info("........name == null............");
            }
            var type = obj.GetType();
            if (type == null)
            {
                Logger.Info("........type == null............");
            }
            var prpInfo = type.GetProperty(name);
            if (prpInfo == null)
            {
                Logger.Info("........prpInfo == null............");
            }
            var method = prpInfo.GetGetMethod();
            return method.Invoke(obj, args);
        }

        public static IRecord GetRecord(string TableName, int Key)
        {
            var tables = Table.GetTableByFileName(TableName) as IDictionary;

            if (tables == null)
            {
                //  Debug.LogWarning(string.Format("UIClasssBinding:table is null. name={0},tableName={1}", GetFullPath(Target.target.transform), TableName));
                return null;
            }
            if (!tables.Contains(Key))
            {
                //Debug.LogWarning(string.Format("UIClasssBinding: table do not contains value:{2} name={0},tableName={1}",GetFullPath(Target.target.transform), mTableName, value));
                return null;
            }
            return tables[Key] as IRecord;
        }

        public static object GetRecordValue(IRecord record, string Attr, ref PropertyInfo prop, string tableName = "")
        {
            var type = record.GetType();
            var key = type.FullName + Attr;

            if (!mCacheProp.TryGetValue(key, out prop))
            {
                prop = type.GetProperty(Attr);
                mCacheProp[key] = prop;
            }
            return record.GetField(Attr);
        }

        /// <summary>
        ///     根据字符串重数据中得到相应的表中配置文件
        /// </summary>
        /// <param name="dataSource">当前的值</param>
        /// <param name="property">当前值的属性</param>
        /// <param name="tableName">当前表名</param>
        /// <param name="argList">需要解析的字符串数组</param>
        /// <param name="index">数组起始索引</param>
        /// <returns></returns>
        public static object GetTableValue(object dataSource, string tableName, string[] argList, int index)
        {
            IRecord record = null;
            var mTableName = tableName;
            var value = dataSource;
            PropertyInfo prop = null;
            for (var i = index; i < argList.Length; i++)
            {
                if (prop != null && prop.IsDefined(typeof (ListSize), false))
                {
                    if (prop.IsDefined(typeof (TableBinding), false))
                    {
                        var objNext1 = prop.GetCustomAttributes(typeof (TableBinding), false);
                        var attrNext1 = objNext1 as TableBinding[];
                        mTableName = attrNext1[0].TableName;
                    }
                    var objList1 = value as IReadonlyList;
                    var listIndex1 = int.Parse(argList[i]);
                    value = objList1[listIndex1];

                    i++;
                    if (i == argList.Length)
                    {
                        return value;
                    }
                }

                record = GetRecord(mTableName, (int) value);
                if (record == null)
                {
                    return null;
                }
                value = GetRecordValue(record, argList[i], ref prop, mTableName);

                if (!prop.IsDefined(typeof (TableBinding), false))
                {
                    continue;
                }

                var objNext = prop.GetCustomAttributes(typeof (TableBinding), false);
                var attrNext = objNext as TableBinding[];
                mTableName = attrNext[0].TableName;
            }

            return value;
        }

        public object GetValue(int index)
        {
            object ret = null;
            if (index < 0 || index >= SourceList.Count)
            {
                return ret;
            }
            var currentSource = SourceList[index];
            if (currentSource.TableColumns != null)
            {
                object value = null;
                var tableName = string.Empty;
                IRecord record = null;
                PropertyInfo prop = null;
                try
                {
                    if (currentSource.ListFlag)
                    {
                        var objList = currentSource.Source as IReadonlyList;
                        var listIndex = int.Parse(currentSource.PropertyName);
                        value = objList[listIndex];
                    }
                    else
                    {
                        value = GetPropertyByName(currentSource.Source, currentSource.PropertyName);
                    }

                    tableName = currentSource.TableName;
                }
                catch (Exception ex)
                {
                    Debug.LogError(string.Format("UIClasssBinding:name={0}\n{1}",
                        GetFullPath(Target.target.transform), ex));
                }


                return GetTableValue(value, tableName, currentSource.TableColumns.ToArray(), 0);
            }
            try
            {
                if (SourceList[index].ListFlag)
                {
                    var objList = SourceList[index].Source as IReadonlyList;
                    var listIndex = int.Parse(SourceList[index].PropertyName);
                    var objCell = objList[listIndex];
                    return objCell;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("UIClasssBinding:name={0}\n{1}",
                    GetFullPath(Target.target.transform), ex));
            }

            try
            {
                var objTar = GetPropertyByName(SourceList[index].Source, SourceList[index].PropertyName);
                return objTar;
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("UIClasssBinding:name={0}\n{1}",
                    GetFullPath(Target.target.transform), ex));
            }

            Debug.LogWarning(string.Format("UIClasssBinding: can not get value. name={0}",
                GetFullPath(Target.target.transform)));
            return null;
        }

        public void SetDefault()
        {
            if (DefaultValue == null)
            {
                DefaultValue = Target.Get();
            }
        }

        public void SetFormatString()
        {
            if (FromatId != -1 && FromatId != 0)
            {
                FromatString = GameUtils.GetDictionaryText(FromatId);
            }
        }

        public void SetInvisibleStrList()
        {
            if (string.IsNullOrEmpty(InvisibleValue))
            {
                return;
            }
            if (InvisibleValue.IndexOf('#') == 0)
            {
                return;
            }
            mInvisibleListStr = InvisibleValue.Split(',');
        }

        public void SetValue(object obj)
        {
            if (obj != null)
            {
                if (obj != null && mInvisibleListStr != null && mInvisibleListStr.Length > 0)
                {
                    for (var i = 0; i < mInvisibleListStr.Length; i++)
                    {
                        if (obj.ToString() == mInvisibleListStr[i])
                        {
                            if (obj.ToString() == mInvisibleListStr[i])
                            {
                                Target.target.gameObject.SetActive(false);
                                Target.target.gameObject.GetComponent<UIClassBinding>().SignalActiveChanged();
                                return;
                            }
                        }
                    }
                }
            }

            if (obj == null)
            {
                if (Target.target == null)
                {
                    Debug.LogError("........SetValue(object obj)  Target.target == null............");
                    return;
                }
                if (Target.target.gameObject == null)
                {
                    Debug.LogError("........SetValue(object obj)  Target.target.gameObject == null............");
                    return;
                }
                Target.Set(DefaultValue);
                return;
            }

            if (Target.target == null)
            {
                Debug.LogError("........SetValue(object obj)  Target.target == null............");
                return;
            }
            if (Target.target.gameObject == null)
            {
                Debug.LogError("........SetValue(object obj)  Target.target.gameObject == null............");
                return;
            }

            if (!string.IsNullOrEmpty(InvisibleValue) || Target.target is UILabel)
            {
                if (Target.target.gameObject.activeSelf == false)
                {
                    Target.target.gameObject.SetActive(true);
                    Target.target.gameObject.GetComponent<UIClassBinding>().SignalActiveChanged();
                }
            }

            if (!string.IsNullOrEmpty(InvisibleValue) && InvisibleValue.IndexOf('#') == 0)
            {
                if (SourceList == null || SourceList.Count == 0)
                {
                    SetValue(null);
                    return;
                }
//                 var visiRet = ExpressionHelper.GetExpressionString(
//                     Target.target.gameObject.GetComponent<UIClassBinding>().DataSource, InvisibleValue);
                var visiRet = ExpressionHelper.GetExpressionString(SourceList[0].RootData, InvisibleValue);
                Target.target.gameObject.SetActive(!(visiRet == "1"));
                Target.Set(obj);
                return;
            }

            if (string.IsNullOrEmpty(Target.name) || Target.name == "Null")
            {
                return;
            }

            if (Target.target is UILabel && Target.name == "text" && string.IsNullOrEmpty(obj.ToString()))
            {
                Target.target.gameObject.SetActive(false);
                Target.target.gameObject.GetComponent<UIClassBinding>().SignalActiveChanged();
            }

            if (TypeConvert(obj, Target))
            {
                return;
            }

            Target.Set(obj);
        }

        public bool TypeConvert(object o, TargetBindingProperty target)
        {
            if (string.IsNullOrEmpty(target.name))
            {
                return false;
            }
            if (target.name == "atlas")
            {
                var sprite = target.target.GetComponent<UISprite>();
                if (sprite == null)
                {
                    return true;
                }
                var currentValue = "";
                if (sprite.atlas != null)
                {
                    currentValue = sprite.atlas.name;
                    if (currentValue.Equals(o.ToString()))
                    {
                        return true;
                    }
                }
                var targetValue = "";
                var value = o.ToString();
                if (value == "Grey")
                {
//需要把原本的设为灰度
                    if (currentValue.Contains("Grey"))
                    {
                        return true;
                    }
                    //原来不是灰度
                    targetValue = currentValue + "Grey";
                }
                else if (value == "NotGrey")
                {
//需要把原本的设为彩色
                    if (currentValue.Contains("Grey"))
                    {
                        targetValue = currentValue.Remove(currentValue.Length - 4, 4);
                    }
                    else
                    {
//原来不是灰度
                        return true;
                    }
                }
                else
                {
                    targetValue = value;
                }

                if (currentValue.Equals(targetValue))
                {
                    return true;
                }
                target.AtlasValue = targetValue;

                if (targetValue.Contains("Circle"))
                {
                    var i = 1;
                }
                ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + target.AtlasValue + ".prefab",
                    res =>
                    {
                        if (res == null)
                        {
                            return;
                        }
                        //异步加载找不到原控件
                        if (target.target == null)
                        {
                            return;
                        }
                        if (targetValue == target.AtlasValue)
                        {
                            target.Set(res.GetComponent<UIAtlas>());
                        }
                    }, true, true, true, false, true);
                return true;
            }
            if (target.name == "mainTexture")
            {
                target.AtlasValue = o.ToString();
                ResourceManager.PrepareResource<Texture>(o.ToString(), res =>
                {
                    if (o.ToString() == target.AtlasValue)
                    {
                        target.Set(res);
                    }
                }, true, true, false, false, true);
                return true;
            }
            if (target.name == "color")
            {
                if (o is string)
                {
                    var c = GameUtils.StringToColor(o.ToString());
                    target.Set(c);
                    return true;
                }
            }
            return false;
        }

        public void UpdataBinding()
        {
            {
                var __list4 = SourceList;
                var __listCount4 = __list4.Count;
                for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var data = __list4[__i4];
                    {
                        var notofy = data.Source as INotifyPropertyChanged;
                        if (notofy != null)
                        {
                            notofy.PropertyChanged -= UpdataBindingData;
                        }
                    }
                }
            }
        }

        public void UpdataBindingData(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var bFind = false;
                {
                    var __list2 = SourceList;
                    var __listCount2 = __list2.Count;
                    for (var __i2 = 0; __i2 < __listCount2; ++__i2)
                    {
                        var data = __list2[__i2];
                        {
                            if (data.PropertyName == e.PropertyName)
                            {
                                bFind = true;
                                break;
                            }
                        }
                    }
                }
                if (e.PropertyName == "this")
                {
                    SetValue(SourceList[0].Source);
                    return;
                }
                if (!bFind)
                {
                    return;
                }
                if (FromatString != "")
                {
                    if (FromatString == "#E#") //{0} 
                    {
                        var value = GetValue(0);
                        if (value == null)
                        {
                            SetValue(null);
                            return;
                        }
//                         value = ExpressionHelper.GetExpressionString(
//                              Target.target.gameObject.GetComponent<UIClassBinding>().DataSource, value.ToString());
                        if (SourceList == null || SourceList.Count == 0)
                        {
                            SetValue(null);
                            return;
                        }
                        value = ExpressionHelper.GetExpressionString(SourceList[0].RootData, value.ToString());
                        SetValue(value);
                    }
                    else if (FromatString.IndexOf('#') == 0) //#{'aaa' + bbb} #{+'/'+}  
                    {
                        if (SourceList == null || SourceList.Count == 0)
                        {
                            SetValue(null);
                            return;
                        }
//                         var strRet = ExpressionHelper.GetExpressionString(
//                            Target.target.gameObject.GetComponent<UIClassBinding>().DataSource, FromatString);
                        var strRet = ExpressionHelper.GetExpressionString(SourceList[0].RootData, FromatString);
                        SetValue(strRet);
                    }
                    else
                    {
                        var args = new List<object>();

                        for (var i = 0; i < SourceList.Count; i++)
                        {
                            var value = GetValue(i);
                            if (value == null)
                            {
                                SetValue(null);
                                return;
                            }
                            //args.Add(value);
                            //args.Add(ExpressionHelper.GetExpressionString(Target.target.gameObject.GetComponent<UIClassBinding>().DataSource, value.ToString()));
                            args.Add(ExpressionHelper.GetExpressionString(SourceList[i].RootData, value.ToString()));
                        }
                        var strRet = string.Format(FromatString, args.ToArray());
                        SetValue(strRet);
                    }
                }
                else
                {
                    var objTar = GetValue(0);
                    SetValue(objTar);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("UpdataBindingData......Error{0}----/n{1}", ex,
                    GetFullPath(Target.target.transform)));
            }
        }
    }
}

[Serializable]
public class BindingClassName
{
    [SerializeField] public string ClassName;
}

[Serializable]
public class BindingDataExpression
{
    [SerializeField] public string DataExpressionName;
}

public class BindSourceData
{
    public bool ListFlag;
    //public INotifyPropertyChanged Source;
    public string PropertyName;
    public object RootData;
    //public object SourceObj;
    public object Source;
    public List<string> TableColumns;
    public string TableName = "";
}