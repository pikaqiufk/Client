#region using

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DataTable;
using NCalc;

#endregion

public class ExpressionHelper
{
    public static Dictionary<int, string> AttrDiffNameAndValue = new Dictionary<int, string>();
    public static Dictionary<int, string> AttrName = new Dictionary<int, string>();
    public static Dictionary<int, string> AttrNameAndValue = new Dictionary<int, string>();
    public static Dictionary<int, string> AttrNameById = new Dictionary<int, string>();
    //注册表达式容器
    private static readonly Dictionary<string, Func<Expression[], object>> GlobalFunctions =
        new Dictionary<string, Func<Expression[], object>>();

    private ExpressionHelper()
    {
    }

    private EvaluateFunctionHandler mEvaluateFunction;
    private EvaluateParameterHandler mEvaluateParameter;
    private Expression mExpression;
    //根据条件，修改字符颜色
    public static object ChangeColor(Expression[] args)
    {
        if (args.Length != 4)
        {
            var result = string.Format("args error !!  in ChangeColor Expression !!");
            Logger.Error(result);
            return result;
        }
        try
        {
            var condition = args[0].Evaluate();
            var desc = args[1].Evaluate();
            var trueColor = args[2].Evaluate();
            var falseColor = args[3].Evaluate();
            if (desc == null)
            {
                return null;
            }
            if (desc is string)
            {
                if (string.IsNullOrEmpty(desc as string))
                {
                    return "";
                }
            }
            if ((int) condition <= 0)
            {
                return string.Format("[{0}]{1}[-]", falseColor, desc);
            }
            return string.Format("[{0}]{1}[-]", trueColor, desc);
        }
        catch (Exception ex)
        {
            Logger.Info("ChangeColor Not arg Evaluate {0}", ex);
        }

        return "";
    }

    //根据条件，修改拼接字符串
    public static object ChangeString(Expression[] args)
    {
        if (args.Length != 4)
        {
            var result = string.Format("args error !!  in ChangeString Expression !!");
            Logger.Error(result);
            return result;
        }
        try
        {
            var condition = args[0].Evaluate();
            var desc = args[1].Evaluate();
            var falseString = args[2].Evaluate();
            var trueString = args[3].Evaluate();
            if (desc == null)
            {
                return null;
            }
            if (desc is string)
            {
                if (string.IsNullOrEmpty(desc as string))
                {
                    return "";
                }
            }
            if ((int) condition <= 0)
            {
                return string.Format((string) falseString, desc);
            }
            return string.Format((string) trueString, desc);
        }
        catch (Exception ex)
        {
            Logger.Info("ChangeColor Not arg Evaluate {0}", ex);
        }

        return "";
    }

    private static bool Compare(object param1, object param2, string condition)
    {
        if (condition == ">")
        {
            if (Convert.ToDouble(param1) > Convert.ToDouble(param2))
            {
                return true;
            }
        }
        else if (condition == ">=")
        {
            if (Convert.ToDouble(param1) >= Convert.ToDouble(param2))
            {
                return true;
            }
        }
        else if (condition == "<")
        {
            if (Convert.ToDouble(param1) < Convert.ToDouble(param2))
            {
                return true;
            }
        }
        else if (condition == "<=")
        {
            if (Convert.ToDouble(param1) <= Convert.ToDouble(param2))
            {
                return true;
            }
        }
        else if (condition == "==")
        {
            if (param1.Equals(param2))
            {
                return true;
            }
        }
        else if (condition == "!=")
        {
            if (!param1.Equals(param2))
            {
                return true;
            }
        }
        return false;
    }

    private static object Condition(Expression[] args)
    {
        if (args.Length != 3)
        {
            var result = string.Format("args error !!  in Condition Expression !!");
            Logger.Error(result);
            return result;
        }
        try
        {
            var param1 = args[0].Evaluate();
            var condition = args[1].Evaluate() as string;
            var param2 = args[2].Evaluate();
            if (Compare(param1, param2, condition))
            {
                return "1";
            }
            return "0";
        }
        catch (Exception ex)
        {
            Logger.Info("Condition Not arg Evaluate {0}", ex);
        }
        return "0";
    }

    //设置条件控制控件的color的值，前三个是数值和添加，后两个是相应的颜色
    private static object ConditionForColor(Expression[] args)
    {
        if (args.Length != 5)
        {
            var result = string.Format("args error !!  in ConditionForColor Expression !!");
            Logger.Error(result);
            return result;
        }

        try
        {
            var param1 = args[0].Evaluate();
            var condition = args[1].Evaluate() as string;
            var param2 = args[2].Evaluate();
            var trueColor = args[3].Evaluate() as string;
            var falseColor = args[4].Evaluate() as string;
            if (Compare(param1, param2, condition))
            {
                return trueColor;
            }
            return falseColor;
        }
        catch (Exception ex)
        {
            Logger.Info("Condition Not arg Evaluate {0}", ex);
        }
        return null;
    }

    //设置sprite的Atlas成灰度图，前三个是条件，如果有第四个参数则添加grey，如果没有则改动编辑的sprite
    private static object ConditionForGrey(Expression[] args)
    {
        if (args.Length != 3 && args.Length != 4)
        {
            var result = string.Format("args error !!  in ConditionForGrey Expression !!");
            Logger.Error(result);
            return result;
        }
        try
        {
            var param1 = args[0].Evaluate();
            var condition = args[1].Evaluate() as string;
            var param2 = args[2].Evaluate();
            var atlasName = "";
            if (args.Length == 4)
            {
                atlasName = args[3].Evaluate() as string;
            }
            if (Compare(param1, param2, condition))
            {
                return atlasName + "Grey";
            }
            if (string.IsNullOrEmpty(atlasName))
            {
                return "NotGrey";
            }
            return atlasName;
        }
        catch (Exception ex)
        {
            Logger.Info("ConditionForGrey Not arg Evaluate {0}", ex);
        }
        return null;
    }

    //根据条件选择一个值，前三个是数值和添加，后两个是相应的颜色
    private static object ConditionSelect(Expression[] args)
    {
        if (args.Length != 5)
        {
            var result = string.Format("args error !!  in ConditionSelect Expression !!");
            Logger.Error(result);
            return result;
        }

        try
        {
            var param1 = args[0].Evaluate();
            var condition = args[1].Evaluate() as string;
            var param2 = args[2].Evaluate();
            var value1 = args[3].Evaluate();
            var value2 = args[4].Evaluate();
            if (Compare(param1, param2, condition))
            {
                return value1;
            }
            return value2;
        }
        catch (Exception ex)
        {
            Logger.Info("Condition Not arg Evaluate {0}", ex);
        }
        return null;
    }

    /// <summary>
    ///     表达式求值,支持嵌套
    ///     表达式例子 '向前方'+[SkillLv]*GetTable('SkillUpgrading',5,'Type')+'米处冲锋，对途经敌人造成80%攻击附加100点的物理伤害并将敌人推至目标点。'
    /// </summary>
    /// <param name="dataSource">当前环境的数据源</param>
    /// <param name="expression">表达式string</param>
    /// <returns></returns>
    public static object Evaluate(object dataSource, string expression)
    {
        var helper = new ExpressionHelper
        {
            mExpression = new Expression(expression),
            mEvaluateFunction = (s, args) =>
            {
                Func<Expression[], object> func;
                if (GlobalFunctions.TryGetValue(s, out func))
                {
                    object result = null;
                    try
                    {
                        result = func(args.Parameters);
                    }
                    finally
                    {
                        if (result != null)
                        {
                            args.Result = result;
                        }
                    }
                }
            },
            mEvaluateParameter =
                (s, args) =>
                {
                    object result = null;
                    try
                    {
                        result = UIClassBinding.UIClassBindingItemInner.GetDataFromSource(dataSource, s);
                    }
                    finally
                    {
                        if (result != null)
                        {
                            args.Result = result;
                        }
                    }
                }
        };

        helper.mExpression.EvaluateFunction += helper.mEvaluateFunction;
        helper.mExpression.EvaluateParameter += helper.mEvaluateParameter;
        try
        {
            return helper.mExpression.Evaluate();
        }
        catch (Exception ex)
        {
            //Logger.Warn("Evaluate {0} get exception : {1}", helper.mExpression, ex.ToString());
            return null;
        }
    }

    ~ExpressionHelper()
    {
        if (mExpression != null)
        {
            mExpression.EvaluateParameter -= mEvaluateParameter;
            mExpression.EvaluateFunction -= mEvaluateFunction;
        }
    }

    private static string GetAttrDiffNameAndValue(Expression[] args)
    {
        if (args.Length != 2)
        {
            var result = string.Format("args error !!  in GetAttrDiffNameAndValue Expression !!");
            Logger.Error(result);
            return result;
        }
        var args0 = args[0].Evaluate();
        if (args0 == null)
        {
            return "";
        }
        var attrid = 0;
        if (!Int32.TryParse(args0.ToString(), out attrid))
        {
            return "args error !!  in GetAttrDiffNameAndValue Expression !! args0 not int";
        }
        if (attrid == -1)
        {
            return "";
        }
        string attrName;
        if (!AttrDiffNameAndValue.TryGetValue(attrid, out attrName))
        {
            var result = string.Format("args error !!  in GetAttrDiffNameAndValue Expression !! args0={0} overflow",
                attrid);
            Logger.Error(result);
            return result;
        }
        var args1 = args[1].Evaluate();

        var attrValue = 0;
        if (!Int32.TryParse(args1.ToString(), out attrValue))
        {
            return "args error !!  in GetAttrDiffNameAndValue Expression !! args0 not int";
        }
        if (attrValue > 0)
        {
            return "[00FF0C]" + string.Format(attrName, "+" + args1) + "[-]";
        }
        return "[FF0000]" + string.Format(attrName, args1) + "[-]";
    }

    public static string GetAttrName(int AttrId)
    {
        string attrName;
        if (!AttrNameById.TryGetValue(AttrId, out attrName))
        {
            var result = string.Format("args error !!  in GetAttrName Expression !! args0={0} overflow", AttrId);
            Logger.Error(result);
            return result;
        }
        return attrName;
    }

    private static string GetAttrName(Expression[] args)
    {
        if (args.Length != 1)
        {
            var result = string.Format("args error !!  in GetAttrName Expression !!");
            Logger.Error(result);
            return result;
        }
        var args0 = args[0].Evaluate();
        if (args0 == null)
        {
            return "";
        }
        var attrid = 0;
        if (!Int32.TryParse(args0.ToString(), out attrid))
        {
            return "args error !!  in GetAttrName Expression !! args0 not int";
        }
        if (attrid == -1)
        {
            return "";
        }
        string attrName;
        if (!AttrNameById.TryGetValue(attrid, out attrName))
        {
            var result = string.Format("args error !!  in GetAttrName Expression !! args0={0} overflow", attrid);
            Logger.Error(result);
            return result;
        }
        return attrName;
    }

    private static string GetAttrNameAndValue(Expression[] args)
    {
        if (args.Length != 3)
        {
            var result = string.Format("args error !!  in GetAttrName Expression !!");
            Logger.Error(result);
            return result;
        }
        var args0 = args[0].Evaluate();
        if (args0 == null)
        {
            return "";
        }
        var attrid = 0;
        if (!Int32.TryParse(args0.ToString(), out attrid))
        {
            return "args error !!  in GetAttrName Expression !! args0 not int";
        }
        if (attrid == -1)
        {
            return "";
        }
        string attrName;
        if (!AttrNameAndValue.TryGetValue(attrid, out attrName))
        {
            var result = string.Format("args error !!  in GetAttrName Expression !! args0={0} overflow", attrid);
            Logger.Error(result);
            return result;
        }
        var args1 = args[1].Evaluate();
        switch (attrid)
        {
            case 5:
            case 7:
            {
                var args2 = args[2].Evaluate();
                return string.Format(attrName, args1, args2);
            }
        }
        return string.Format(attrName, args1);
    }

    //获取某个通用颜色的返回值
    public static string GetColorByIndex(int nColor)
    {
        switch (nColor)
        {
            case 0: //白
                return "FFFFFF";
                break;
            case 1: //绿
                return "4FC012";
                break;
            case 2: //蓝
                return "2866E1";
                break;
            case 3: //紫
                return "EB5CE3";
                break;
            case 4: //橙
                return "FFC000";
                break;
        }
        return "C0C0C0";
    }

    //技能参数获得
    public static object GetDictionary(Expression[] args)
    {
        if (args.Length != 1)
        {
            Logger.Error(string.Format("args error !!  in GetDictionary Expression !!"));
            return "args error !!  in GetDictionary Expression !!";
        }

        try
        {
            var args0 = args[0].Evaluate().ToString();
            int DictionaryId;
            if (!Int32.TryParse(args0, out DictionaryId))
            {
                return "args error !!  in GetDictionary Expression !!";
            }
            return GameUtils.GetDictionaryText(DictionaryId);
                //Table.GetDictionary(DictionaryId).Desc[GameUtils.LanguageIndex];
        }
        catch (Exception ex)
        {
            Logger.Info("GetDictionary Not arg Evaluate {0}", ex);
        }
        return "";
    }

    //获得强化概率 物品颜色，概率
    private static object GetEnchancePro(Expression[] args)
    {
        if (args.Length != 2)
        {
            var result = string.Format("args error !!  in GetEnchancePro Expression !!");
            Logger.Error(result);
            return result;
        }
        var itemIndex = args[0].Evaluate();
        //if (itemId == null)
        //{
        //    return null;
        //}
        //int nItemId;
        //if (itemId is Int32)
        //{
        //    nItemId = (int)itemId;
        //    if (nItemId == -1) return null;
        //}
        //else
        //{
        //    return null;
        //}

        //var tbItem = Table.GetItemBase(nItemId);
        //if (tbItem == null) return nItemId;
        //string color = GetColorByIndex(tbItem.Color);
        string color;
        if ((int) itemIndex == -1)
        {
            color = GetColorByIndex(-1);
        }
        else
        {
            color = "00FF00"; //GetColorByIndex(4);
        }
        var fPro = args[1].Evaluate();
        return string.Format("[{0}]+{1:F2}%[-]", color, fPro);
    }

    //拆解字符串，看是否有需要特殊运算的
    public static string GetExpressionString(object dataSource, string desc)
    {
        var nend = 0;
        var nbegin = 0;
        var token1 = "#{";
        var token2 = "}";
        var result_str = "";
        while (nend != -1)
        {
            nend = desc.IndexOf(token1, nbegin, StringComparison.Ordinal);
            if (nend == -1)
            {
                var temp1 = desc.Substring(nbegin, desc.Length - nbegin);
                result_str = result_str + temp1;
            }
            else
            {
                //拼接井号前的字符
                var temp3 = desc.Substring(nbegin, nend - nbegin);
                result_str = result_str + temp3;
                nbegin = nend + token1.Length;
                //查询结束符
                var findend = true;
                var midstr = "";
                var dianCount = 0;
                while (findend)
                {
                    nend = desc.IndexOf(token2, nbegin, StringComparison.Ordinal);
                    if (nend == -1)
                    {
//没有找到
                        var temp1 = desc.Substring(nbegin, desc.Length - nbegin);
                        result_str = result_str + midstr + temp1;
                        return result_str;
                    }
                    var temp2 = desc.Substring(nbegin, nend - nbegin);
                    dianCount += Regex.Matches(temp2, @"\'").Count;
                    if (dianCount%2 == 1)
                    {
                        midstr = midstr + temp2 + token2;
                        nbegin = nend + token2.Length;
                    }
                    else
                    {
                        midstr = midstr + temp2;
                        var EvaluateStr = Evaluate(dataSource, midstr);
                        if (EvaluateStr == null)
                        {
                            return null;
                        }
                        result_str = result_str + EvaluateStr;
                        findend = false;
                        nbegin = nend + token2.Length;
                    }
                }
            }
        }
        return result_str;
    }

    //获得带有颜色 及强化等级的物品名称
    private static object GetItemName(Expression[] args)
    {
        if (args.Length <= 0 || args.Length >= 3)
        {
            var result = string.Format("args error !!  in GetItemName Expression !!");
            Logger.Error(result);
            return result;
        }
        var itemId = args[0].Evaluate();
        if (itemId == null)
        {
            return null;
        }
        int nItemId;
        if (itemId is Int32)
        {
            nItemId = (int) itemId;
            if (nItemId == -1)
            {
                return string.Empty;
            }
        }
        else
        {
            return null;
        }

        var tbItem = Table.GetItemBase(nItemId);
        if (tbItem == null)
        {
            return nItemId;
        }
        var color = GameUtils.GetTableColorString(tbItem.Quality);

        if (args.Length == 2)
        {
            var level = args[1].Evaluate();
            if (level.ToString() == "0")
            {
#if UNITY_EDITOR
                return string.Format("[{0}]{1}[-] ID: {2}", color, tbItem.Name, itemId);
#endif
                return string.Format("[{0}]{1}[-]", color, tbItem.Name);
            }
#if UNITY_EDITOR
            return string.Format("[{0}]{1}+{2}[-] ID: {3}", color, tbItem.Name, level, itemId);
#endif
            return string.Format("[{0}]{1}+{2}[-]", color, tbItem.Name, level);
        }

#if UNITY_EDITOR
        return string.Format("[{0}]{1}[-] ID: {2}", color, tbItem.Name, itemId);
#endif
        return string.Format("[{0}]{1}[-]", color, tbItem.Name);
    }

    //技能参数获得
    public static object GetMinMaxValue(Expression[] args)
    {
        if (args.Length != 3)
        {
            Logger.Error(string.Format("args error !!  in SkillParam Expression !!"));
            return "args error !!  in SkillParam Expression !!";
        }

        try
        {
            var nowValue = args[0].Evaluate();
            var minValue = args[1].Evaluate();
            var maxValue = args[2].Evaluate();
            return Math.Min(Math.Max((int) nowValue, (int) minValue), (int) maxValue);
        }
        catch (Exception ex)
        {
            Logger.Info("SkillParam Not arg Evaluate {0}", ex);
        }
        return -1;
    }

    /// <summary>
    ///     注册一个读取表中数据的表达式
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static object GetTable(Expression[] args)
    {
        if (args.Length < 3)
        {
            Logger.Error(string.Format("args error !!  in GetTable Expression !!"));
        }
        try
        {
            var table = args[0].Evaluate();
            var row = args[1].Evaluate();
            var column = args[2].Evaluate();
            var ret = Table.GetTableData(table.ToString(), (int) row, column.ToString());
            if (null == ret)
            {
                Logger.Error(string.Format("Could not find Table Data in TableName: {0} column: {1} row: {2}", table,
                    column, row));
            }
            return ret;
        }
        catch (Exception ex)
        {
            Logger.Info("GetTable Not arg Evaluate {0}", ex);
        }
        return null;
    }

    //设置sprite的Atlas成圆图，第一个条件绑定值， 第2个条件为是否Grey
    // #{IconCircleOrSquare([myIconAtlas],'Circle')}
    // #{IconCircleOrSquare([myIconAtlas],'Square','Grey')}

    private static object IconCircleOrSquare(Expression[] args)
    {
        if (args.Length != 2 && args.Length != 3)
        {
            var result = string.Format("args error !!  in IconForCircleOrSquare Expression !!");
            Logger.Error(result);
            return result;
        }
        try
        {
            var atlasName = args[0].Evaluate().ToString();
            atlasName += args[1].Evaluate().ToString();
            if (args.Length == 3)
            {
                atlasName += args[2].Evaluate().ToString();
            }
            return atlasName;
        }
        catch (Exception ex)
        {
            //Logger.Info("IconForCircleOrSquare Not arg Evaluate {0}", ex);
        }
        return null;
    }

    public static void initializeStaticString()
    {
        AttrName.Clear();
        AttrName = new Dictionary<int, string>
        {
            //221000
            {0, GameUtils.GetDictionaryText(221000)}, //"等级"
            {1, GameUtils.GetDictionaryText(221001)}, //"力量"
            {2, GameUtils.GetDictionaryText(221002)}, //"敏捷"
            {3, GameUtils.GetDictionaryText(221003)}, //"智力"
            {4, GameUtils.GetDictionaryText(221004)}, //"体力"
            {5, GameUtils.GetDictionaryText(221005)}, //"物攻最小值"
            {6, GameUtils.GetDictionaryText(221006)}, //"物攻最大值"
            {7, GameUtils.GetDictionaryText(221007)}, //"魔攻最小值"
            {8, GameUtils.GetDictionaryText(221008)}, //"魔攻最大值"
            {9, GameUtils.GetDictionaryText(221009)}, //"附加伤害"
            {10, GameUtils.GetDictionaryText(221010)}, //"物理防御"
            {11, GameUtils.GetDictionaryText(221011)}, //"魔法防御"
            {12, GameUtils.GetDictionaryText(221012)}, //"伤害抵挡"
            {13, GameUtils.GetDictionaryText(221013)}, //"生命上限"
            {14, GameUtils.GetDictionaryText(221014)}, //"魔法上限"
            {15, GameUtils.GetDictionaryText(221015)}, //"幸运一击率"
            {16, GameUtils.GetDictionaryText(221016)}, //"幸运一击伤害率"
            {17, GameUtils.GetDictionaryText(221017)}, //"卓越一击率"
            {18, GameUtils.GetDictionaryText(221018)}, //"卓越一击伤害率"
            {19, GameUtils.GetDictionaryText(221019)}, //"命中"
            {20, GameUtils.GetDictionaryText(221020)}, //"闪避"
            {21, GameUtils.GetDictionaryText(221021)}, //"伤害加成率"
            {22, GameUtils.GetDictionaryText(221022)}, //"伤害减少率"
            {23, GameUtils.GetDictionaryText(221023)}, //"伤害反弹率"
            {24, GameUtils.GetDictionaryText(221024)}, //"无视防御率"
            {25, GameUtils.GetDictionaryText(221025)}, //"移动速度"
            {26, GameUtils.GetDictionaryText(221026)}, //"击中回复"
            {(int)eAttributeType.FireAttack, GameUtils.GetDictionaryText(221031)}, //"火属性攻击"
            {(int)eAttributeType.IceAttack, GameUtils.GetDictionaryText(221032)}, //"冰属性攻击"
            {(int)eAttributeType.PoisonAttack, GameUtils.GetDictionaryText(221033)}, //"毒属性攻击"
            {(int)eAttributeType.FireResistance, GameUtils.GetDictionaryText(221034)}, //"火属性抗性"
            {(int)eAttributeType.IceResistance, GameUtils.GetDictionaryText(221035)}, //"冰属性抗性"
            {(int)eAttributeType.PoisonResistance, GameUtils.GetDictionaryText(221036)}, //"毒属性抗性"
            {(int)eAttributeType.HpNow, GameUtils.GetDictionaryText(221027)}, //"生命"
            {(int)eAttributeType.MpNow, GameUtils.GetDictionaryText(221028)}, //"魔法"
            {98, GameUtils.GetDictionaryText(221029)}, //"攻击增加等级" 
            {99, GameUtils.GetDictionaryText(221000)}, //"防御增加等级" 
            {105, GameUtils.GetDictionaryText(221098)}, //"攻击"
            {106, GameUtils.GetDictionaryText(221106)}, //"攻击百分比"
            {110, GameUtils.GetDictionaryText(221110)}, //"防御"
            {111, GameUtils.GetDictionaryText(221111)}, //"防御百分比"
            {113, GameUtils.GetDictionaryText(221113)}, //"生命百分比"
            {114, GameUtils.GetDictionaryText(221114)}, //"魔法百分比"
            {119, GameUtils.GetDictionaryText(221119)}, //"命中百分比"
            {120, GameUtils.GetDictionaryText(221120)} //"闪避百分比"}
        };

        AttrNameById = new Dictionary<int, string>
        {
            {0, GameUtils.GetDictionaryText(221000)}, //"等级"},
            {1, GameUtils.GetDictionaryText(221001) + ":"}, //"需求力量:"
            {2, GameUtils.GetDictionaryText(221002) + ":"}, //"需求敏捷:"
            {3, GameUtils.GetDictionaryText(221003) + ":"}, //"需求智力:"
            {4, GameUtils.GetDictionaryText(221004) + ":"}, //"需求体力:"
            {5, GameUtils.GetDictionaryText(221005) + ":+"}, //"物理攻击:+"
            {6, GameUtils.GetDictionaryText(221006) + ":"}, //"物攻最大值:"
            {7, GameUtils.GetDictionaryText(221007) + ":+"}, //"魔法攻击:+"
            {8, GameUtils.GetDictionaryText(221008) + ":"}, //"魔攻最大值:"
            {9, GameUtils.GetDictionaryText(221009) + ":"}, //"附加伤害:"
            {10, GameUtils.GetDictionaryText(221010) + ":+"}, //"物理防御:+"
            {11, GameUtils.GetDictionaryText(221011) + ":+"}, //"魔法防御:+"
            {12, GameUtils.GetDictionaryText(221012) + ":"}, //"伤害抵挡:"
            {13, GameUtils.GetDictionaryText(221013) + ":"}, //"生命上限:"
            {14, GameUtils.GetDictionaryText(221014) + ":"}, //"魔法上限:"
            {15, GameUtils.GetDictionaryText(221015) + ":"}, //"幸运一击率:"
            {16, GameUtils.GetDictionaryText(221016) + ":+"}, //"幸运一击伤害率:+"
            {17, GameUtils.GetDictionaryText(221017) + ":+"}, //"卓越一击率:+"
            {18, GameUtils.GetDictionaryText(221018) + ":+"}, //"卓越一击伤害率:+"
            {19, GameUtils.GetDictionaryText(221019) + ":+"}, //"命中:+"
            {20, GameUtils.GetDictionaryText(221020) + ":+"}, //"闪避:+"
            {21, GameUtils.GetDictionaryText(221021) + ":+"}, //"伤害加成率:+"
            {22, GameUtils.GetDictionaryText(221022) + ":+"}, //"伤害减少率:+"
            {23, GameUtils.GetDictionaryText(221023) + ":+"}, //"伤害反弹率:+"
            {24, GameUtils.GetDictionaryText(221024) + ":+"}, //"无视防御率:+"
            {25, GameUtils.GetDictionaryText(221025) + ":"}, //"移动速度:"
            {26, GameUtils.GetDictionaryText(221026) + ":+"}, //"击中回复:+"
            {(int)eAttributeType.FireAttack, GameUtils.GetDictionaryText(221031) + ":+"}, //"火属性攻击:+"
            {(int)eAttributeType.IceAttack, GameUtils.GetDictionaryText(221032) + ":+"}, //"冰属性攻击:+"
            {(int)eAttributeType.PoisonAttack, GameUtils.GetDictionaryText(221033) + ":+"}, //"毒属性攻击:+"
            {(int)eAttributeType.FireResistance, GameUtils.GetDictionaryText(221034) + ":+"}, //"火属性抗性:+"
            {(int)eAttributeType.IceResistance, GameUtils.GetDictionaryText(221035) + ":+"}, //"冰属性抗性:+"
            {(int)eAttributeType.PoisonResistance, GameUtils.GetDictionaryText(221036) + ":+"}, //"毒属性抗性:+"
            {(int)eAttributeType.HpNow, GameUtils.GetDictionaryText(221027) + ":"}, //"生命:"
            {(int)eAttributeType.MpNow, GameUtils.GetDictionaryText(221028) + ":"}, //"魔法:"
            {98, GameUtils.GetDictionaryText(270133)}, //"攻击:人物等级/" 
            {99, GameUtils.GetDictionaryText(270134)}, //"防御:人物等级/" 
            {105, GameUtils.GetDictionaryText(221098)}, //,"攻击"
            {106, GameUtils.GetDictionaryText(221106) + ":+"}, //,"攻击百分比:+"
            {110, GameUtils.GetDictionaryText(221110)}, //,"防御"
            {111, GameUtils.GetDictionaryText(221111) + ":+"}, //,"防御百分比:+"
            {113, GameUtils.GetDictionaryText(221113) + ":+"}, //,"生命百分比:+"
            {114, GameUtils.GetDictionaryText(221114) + ":+"}, //,"魔法百分比:+"
            {119, GameUtils.GetDictionaryText(221119) + ":+"}, //,"命中百分比:+"
            {120, GameUtils.GetDictionaryText(221120) + ":+"} //,"闪避百分比:+"
        };
        AttrNameAndValue = new Dictionary<int, string>
        {
            {0, GameUtils.GetDictionaryText(270135)}, //"等级+{0}"
            {1, GameUtils.GetDictionaryText(270136)}, //"需求力量:{0}"
            {2, GameUtils.GetDictionaryText(270137)}, //"需求敏捷:{0}"
            {3, GameUtils.GetDictionaryText(270138)}, //"需求智力:{0}"
            {4, GameUtils.GetDictionaryText(270139)}, //"需求体力:{0}"
            {5, GameUtils.GetDictionaryText(270140)}, //"物理攻击:+{0}~{1}"
            {6, GameUtils.GetDictionaryText(270141)}, //"物攻最大值:+{0}"
            {7, GameUtils.GetDictionaryText(270142)}, //"魔法攻击:+{0}~{1}"
            {8, GameUtils.GetDictionaryText(270143)}, //"魔攻最大值:+{0}"
            {9, GameUtils.GetDictionaryText(270144)}, //"附加伤害:+{0}"
            {10, GameUtils.GetDictionaryText(270145)}, //"物理防御:+{0}"
            {11, GameUtils.GetDictionaryText(270146)}, //"魔法防御:+{0}"
            {12, GameUtils.GetDictionaryText(270147)}, //"伤害抵挡:+{0}"
            {13, GameUtils.GetDictionaryText(270148)}, //"生命上限:+{0}"
            {14, GameUtils.GetDictionaryText(270149)}, //"魔法上限:+{0}"
            {15, GameUtils.GetDictionaryText(270150)}, //"幸运一击率:+{0}%"
            {16, GameUtils.GetDictionaryText(270151)}, //"幸运一击伤害率:+{0}%"
            {17, GameUtils.GetDictionaryText(270152)}, //"卓越一击率:+{0}%"
            {18, GameUtils.GetDictionaryText(270153)}, //"卓越一击伤害率:+{0}%"
            {19, GameUtils.GetDictionaryText(270154)}, //"命中:+{0}"
            {20, GameUtils.GetDictionaryText(270155)}, //"闪避:+{0}"
            {21, GameUtils.GetDictionaryText(270156)}, //"伤害加成:+{0}%"
            {22, GameUtils.GetDictionaryText(270157)}, //"伤害减少:+{0}%"
            {23, GameUtils.GetDictionaryText(270158)}, //"伤害反弹:+{0}%"
            {24, GameUtils.GetDictionaryText(270159)}, //"无视防御:+{0}%"
            {25, GameUtils.GetDictionaryText(270160)}, //"移动速度:+{0}"
            {26, GameUtils.GetDictionaryText(270161)}, //"击中回复:+{0}"
            {(int)eAttributeType.FireAttack, GameUtils.GetDictionaryText(222101)}, //"火属性攻击:+{0}"
            {(int)eAttributeType.IceAttack, GameUtils.GetDictionaryText(222102)}, //"冰属性攻击:+{0}"
            {(int)eAttributeType.PoisonAttack, GameUtils.GetDictionaryText(222103)}, //"毒属性攻击:+{0}"
            {(int)eAttributeType.FireResistance, GameUtils.GetDictionaryText(222104)}, //"火属性抗性:+{0}"
            {(int)eAttributeType.IceResistance, GameUtils.GetDictionaryText(222105)}, //"冰属性抗性:+{0}"
            {(int)eAttributeType.PoisonResistance, GameUtils.GetDictionaryText(222106)}, //"毒属性抗性:+{0}"
            {(int)eAttributeType.HpNow, GameUtils.GetDictionaryText(270162)}, //"生命:+{0}"
            {(int)eAttributeType.MpNow, GameUtils.GetDictionaryText(270163)}, //"魔法:+{0}"
            {98, GameUtils.GetDictionaryText(270164)}, //"攻击:+人物等级/{0}"
            {99, GameUtils.GetDictionaryText(270165)}, //"防御:+人物等级/{0}"
            {105, GameUtils.GetDictionaryText(270166)}, //"攻击:+{0}"
            {106, GameUtils.GetDictionaryText(270167)}, //"攻击:+{0}%"
            {110, GameUtils.GetDictionaryText(270168)}, //"防御:+{0}"
            {111, GameUtils.GetDictionaryText(270169)}, //"防御:+{0}%"
            {113, GameUtils.GetDictionaryText(270170)}, //"生命上限:+{0}%"
            {114, GameUtils.GetDictionaryText(270171)}, //"魔法上限:+{0}%"
            {119, GameUtils.GetDictionaryText(270172)}, //"命中:+{0}%"
            {120, GameUtils.GetDictionaryText(270173)} //"闪避:+{0}%"}
        };
        AttrDiffNameAndValue = new Dictionary<int, string>
        {
            {0, GameUtils.GetDictionaryText(270174)}, //"等级{0}"
            {1, GameUtils.GetDictionaryText(270175)}, //"需求力量:{0}"
            {2, GameUtils.GetDictionaryText(270176)}, //"需求敏捷:{0}"
            {3, GameUtils.GetDictionaryText(270177)}, //"需求智力:{0}"
            {4, GameUtils.GetDictionaryText(270178)}, //"体力{0}"
            {5, GameUtils.GetDictionaryText(270179)}, //"物攻最小值:{0}"
            {6, GameUtils.GetDictionaryText(270180)}, //"物攻最大值:{0}"
            {7, GameUtils.GetDictionaryText(270181)}, //"魔攻最小值:{0}"
            {8, GameUtils.GetDictionaryText(270182)}, //"魔攻最大值:{0}"
            {9, GameUtils.GetDictionaryText(270183)}, //"附加伤害:{0}"
            {10, GameUtils.GetDictionaryText(270184)}, //"物理防御:{0}"
            {11, GameUtils.GetDictionaryText(270185)}, //"魔法防御:{0}"
            {12, GameUtils.GetDictionaryText(270186)}, //"伤害抵挡:{0}"
            {13, GameUtils.GetDictionaryText(270187)}, //"生命上限:{0}"
            {14, GameUtils.GetDictionaryText(270188)}, //"魔法上限:{0}"
            {15, GameUtils.GetDictionaryText(270189)}, //"幸运一击率:{0}%"
            {16, GameUtils.GetDictionaryText(270190)}, //"幸运一击伤害率:{0}%"
            {17, GameUtils.GetDictionaryText(270191)}, //"卓越一击率:{0}%"
            {18, GameUtils.GetDictionaryText(270192)}, //"卓越一击伤害率:{0}%"
            {19, GameUtils.GetDictionaryText(270193)}, //"命中{0}"
            {20, GameUtils.GetDictionaryText(270194)}, //"闪避{0}"
            {21, GameUtils.GetDictionaryText(270195)}, //"伤害加成:{0}%"
            {22, GameUtils.GetDictionaryText(270196)}, //"伤害减少:{0}%"
            {23, GameUtils.GetDictionaryText(270197)}, //"伤害反弹:{0}%"
            {24, GameUtils.GetDictionaryText(270198)}, //"无视防御:{0}%"
            {25, GameUtils.GetDictionaryText(270199)}, //"移动速度:{0}"
            {26, GameUtils.GetDictionaryText(270200)}, //"击中回复:{0}"
            {(int)eAttributeType.FireAttack, GameUtils.GetDictionaryText(222201)}, //"火属性攻击:{0}"
            {(int)eAttributeType.IceAttack, GameUtils.GetDictionaryText(222202)}, //"冰属性攻击:{0}"
            {(int)eAttributeType.PoisonAttack, GameUtils.GetDictionaryText(222203)}, //"毒属性攻击:{0}"
            {(int)eAttributeType.FireResistance, GameUtils.GetDictionaryText(222204)}, //"火属性抗性:{0}"
            {(int)eAttributeType.IceResistance, GameUtils.GetDictionaryText(222205)}, //"冰属性抗性:{0}"
            {(int)eAttributeType.PoisonResistance, GameUtils.GetDictionaryText(222206)}, //"毒属性抗性:{0}"
            {(int)eAttributeType.HpNow, GameUtils.GetDictionaryText(270201)}, //"生命:{0}"
            {(int)eAttributeType.MpNow, GameUtils.GetDictionaryText(270202)}, //"魔法:{0}"
            {98, GameUtils.GetDictionaryText(270203)}, //"攻击:人物等级/{0}"
            {99, GameUtils.GetDictionaryText(270204)}, //"防御:人物等级/{0}"
            {105, GameUtils.GetDictionaryText(270205)}, //"攻击:{0}"
            {106, GameUtils.GetDictionaryText(270206)}, //"攻击:{0}%"
            {110, GameUtils.GetDictionaryText(270207)}, //"防御:{0}"
            {111, GameUtils.GetDictionaryText(270208)}, //"防御:{0}%"
            {113, GameUtils.GetDictionaryText(270209)}, //"生命上限:{0}%"
            {114, GameUtils.GetDictionaryText(270210)}, //"魔法上限:{0}%"
            {119, GameUtils.GetDictionaryText(270211)}, //"命中:{0}%"
            {120, GameUtils.GetDictionaryText(270212)} //"闪避:{0}%"}
        };
    }

    public static string PlayerName(Expression[] args)
    {
        try
        {
            var name = string.Empty;
            if (null != PlayerDataManager.Instance)
            {
                name = PlayerDataManager.Instance.GetName();
            }
            return name;
        }
        catch (Exception ex)
        {
            Logger.Info("PlayerName Not arg Evaluate {0}", ex);
        }
        return "";
    }

    public static void RegisterAllFunction()
    {
        RegisterFunction("GetTable", GetTable);
        RegisterFunction("SkillParam", SkillParam);
        RegisterFunction("SkillParamWithPower", SkillParamWithPower);
        RegisterFunction("ChangeColor", ChangeColor);
        RegisterFunction("GetItemName", GetItemName);
        RegisterFunction("GetEnchancePro", GetEnchancePro);
        RegisterFunction("ChangeString", ChangeString);
        RegisterFunction("Dictionary", GetDictionary);
        RegisterFunction("StringFormat", StringFormat);
        RegisterFunction("GetAttrName", GetAttrName);
        RegisterFunction("GetAttrNameAndValue", GetAttrNameAndValue);
        RegisterFunction("GetAttrDiffNameAndValue", GetAttrDiffNameAndValue);
        RegisterFunction("Condition", Condition);
        RegisterFunction("PlayerName", PlayerName);
        RegisterFunction("ConditionForColor", ConditionForColor);
        RegisterFunction("StringFormatLevel", StringFormatLevel);
        RegisterFunction("ConditionForGrey", ConditionForGrey);
        RegisterFunction("ConditionSelect", ConditionSelect);
        RegisterFunction("IconCircleOrSquare", IconCircleOrSquare);
        RegisterFunction("GetMinMaxValue", GetMinMaxValue);
        RegisterFunction("GetLevelColor", GetLevelColor);
    }

    /// <summary>
    ///     注册表达式函数
    /// </summary>
    /// <param name="funcName"></param>
    /// <param name="func"></param>
    private static void RegisterFunction(string funcName, Func<Expression[], object> func)
    {
        if (!GlobalFunctions.ContainsKey(funcName))
        {
            GlobalFunctions.Add(funcName, func);
        }
    }

    //技能参数获得
    public static object SkillParam(Expression[] args)
    {
        if (args.Length != 2)
        {
            Logger.Error(string.Format("args error !!  in SkillParam Expression !!"));
            return "args error !!  in SkillParam Expression !!";
        }

        try
        {
            var tableId = args[0].Evaluate();
            var SkillLevel = Table_Tamplet.Convert_Int(args[1].Evaluate().ToString());
            var tbUpgrade = Table.GetSkillUpgrading(Table_Tamplet.Convert_Int(tableId.ToString()));
            if (tbUpgrade == null)
            {
                return string.Format("!ERROR_Table Not find {0}!", tableId);
            }
            if (SkillLevel <= 0)
            {
                return tbUpgrade.GetSkillUpgradingValue(SkillLevel);
            }
            return tbUpgrade.GetSkillUpgradingValue(SkillLevel - 1);
        }
        catch (Exception ex)
        {
            Logger.Info("SkillParam Not arg Evaluate {0}", ex);
        }
        return -1;
    }

    public static object SkillParamWithPower(Expression[] args)
    {
        if (args.Length != 2)
        {
            Logger.Error(string.Format("args error !!  in SkillParamWithPower Expression !!"));
            return "args error !!  in SkillParamWithPower Expression !!";
        }

        try
        {
            var tableId = args[0].Evaluate();
            var SkillLevel = Table_Tamplet.Convert_Int(args[1].Evaluate().ToString());
            var tbUpgrade = Table.GetSkillUpgrading(Table_Tamplet.Convert_Int(tableId.ToString()));
            if (tbUpgrade == null)
            {
                return string.Format("!ERROR_Table Not find {0}!", tableId);
            }
            if (SkillLevel > 0)
            {
                SkillLevel -= 1;
            }

            var value = tbUpgrade.GetSkillUpgradingValue(SkillLevel)/100f;
//             int power;
//             if (PlayerDataManager.Instance.GetRoleId() == 1)
//             {
//                 power = PlayerDataManager.Instance.PlayerDataModel.Attributes.MagPowerMin;
//             }
//             else
//             {
//                 power = PlayerDataManager.Instance.PlayerDataModel.Attributes.PhyPowerMin;
//             }
//             var addpro = PlayerDataManager.Instance.PlayerDataModel.Attributes.DamageAddPro/10000f;
//             var damage = value*power/100f;
//             damage = damage + damage*addpro;
            var str = string.Format("{0}%", value.ToString("f1")/*, damage.ToString("f0")*/);
            return str;
        }
        catch (Exception ex)
        {
            Logger.Info("SkillParam Not arg Evaluate {0}", ex);
        }
        return -1;
    }

    private static string StringFormat(Expression[] args)
    {
        if (args.Length < 2)
        {
            var result = string.Format("args error !!  in StringFromat Expression !!");
            Logger.Error(result);
            return result;
        }
        string formater;
        try
        {
            formater = args[0].Evaluate().ToString();
        }
        catch (Exception ex)
        {
            var result = string.Format("args error !!  in StringFromat Expression !! {0}", ex);
            Logger.Error(result);
            return "";
        }
        var argList = new List<object>();

        var argsLength0 = args.Length;
        for (var i = 1; i < argsLength0; i++)
        {
            try
            {
                if (args[i] == null)
                {
                    return "";
                }

                var o = args[i].Evaluate();
                if (o == null)
                {
                    return "";
                }
                argList.Add(o);
            }
            catch (Exception)
            {
                return "";
            }
        }

        return string.Format(formater, argList.ToArray());
    }

    //通过等级和转生值，生成合适的字符
    private static object StringFormatLevel(Expression[] args)
    {
        if (args.Length != 2)
        {
            var result = string.Format("args error !!  in StringFormatLevel Expression !!");
            Logger.Error(result);
            return result;
        }
        var ret = "";
        try
        {
            var ladder = (int) args[0].Evaluate();
            var level = (int) args[1].Evaluate();
            if (ladder == 0)
            {
//{0}级
                var str = GameUtils.GetDictionaryText(1039);
                ret = string.Format(str, level);
            }
            else
            {
//{0}转{1}级
                var str = GameUtils.GetDictionaryText(1038);
                ret = string.Format(str, ladder, level);
            }
        }
        catch (Exception ex)
        {
            Logger.Info("StringFormatLevel Not arg Evaluate {0}", ex);
        }
        return ret;
    }

    private static object GetLevelColor(Expression[] args)
    {
        if (args.Length <= 0 || args.Length >= 3)
        {
            var result = string.Format("args error !!  in GetItemName Expression !!");
            Logger.Error(result);
            return MColor.green;
        }
        var itemId = args[0].Evaluate();
        if (itemId == null)
        {
            return MColor.green;
        }
        int nItemId;
        if (itemId is Int32)
        {
            nItemId = (int)itemId;
            if (nItemId == -1)
            {
                return MColor.green;
            }
        }
        else
        {
            return MColor.green;
        }

        var tbItem = Table.GetItemBase(nItemId);
        if (tbItem == null)
        {
            return MColor.green;
        }

        if (tbItem.UseLevel > PlayerDataManager.Instance.GetLevel())
        {
            return MColor.red;
        }
        else
        {
            return MColor.green;
        }
    }
}