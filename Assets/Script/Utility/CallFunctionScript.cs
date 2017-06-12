#region using

using System;
using System.Collections.Generic;
using NCalc;
using UnityEngine;

#endregion

//解析字符串表达式执行函数

public class CallFunctionScript
{
    //缓存函数表
    private readonly Dictionary<string, Func<Expression[], object>> mRegisterFunctions =
        new Dictionary<string, Func<Expression[], object>>();

    //执行函数
    public object Execute(string expression)
    {
        var exp = new Expression(expression);

        //函数代理
        exp.EvaluateFunction += delegate(string name, FunctionArgs args)
        {
            Func<Expression[], object> func;
            if (mRegisterFunctions.TryGetValue(name, out func))
            {
                object result = null;
                try
                {
                    result = func(args.Parameters);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Execute " + name + "() Error:" + ex);
                }
                finally
                {
                    args.Result = result;
                }
            }
            else
            {
                Debug.LogError("Can't find function [" + name + "]");
            }
        };

        try
        {
            return exp.Evaluate(); //执行函数
        }
        catch (Exception ex)
        {
            Debug.LogError("Evaluate \"" + expression + "\" Error: " + ex);
            return null;
        }
    }

    //注册可被执行的函数
    //注册的函数格式满足 public static object Func(Expression[] args)
    //函数返回值不能是null，如果不需要返回值可以返回1
    public void RegisterFunction(string funcName, Func<Expression[], object> func)
    {
        if (!mRegisterFunctions.ContainsKey(funcName))
        {
            mRegisterFunctions.Add(funcName, func);
        }
        else
        {
            Debug.LogError("duplicate func " + funcName);
        }
    }

    //反注册所有函数
    public void UnRgisterAllFunction()
    {
        mRegisterFunctions.Clear();
    }
}