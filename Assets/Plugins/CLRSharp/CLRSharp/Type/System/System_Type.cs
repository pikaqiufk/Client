using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using TB.ComponentModel;
using UnityEngine;

namespace CLRSharp
{
    public static class CustomMethod

    {
        public static int GetTwoDimesionArray(this int[,] arr, int i, int j)
        {
            return arr[i, j];
        }

        public static object CompareExchange(ref object a, object b, object c)
        {
            var t = a;
            a = b;
            return t;
        }
    }

    public class Type_Common_System : ICLRType_System
    {
        public System.Type TypeForSystem
        {
            get;
            private set;
        }
        public ICLRSharp_Environment env
        {
            get;
            private set;
        }
        public ICLRType[] SubTypes
        {
            get;
            private set;
        }
        public Type_Common_System(ICLRSharp_Environment env, System.Type type, ICLRType[] subtype)
        {
            this.env = env;
            this.TypeForSystem = type;
            FullNameWithAssembly = type.AssemblyQualifiedName;
            this.SubTypes = subtype;
        }
        public string Name
        {
            get { return TypeForSystem.Name; }
        }

        public string FullName
        {
            get { return TypeForSystem.FullName; }
        }
        public string FullNameWithAssembly
        {
            get;
            private set;

            //{
            //    string aname = TypeForSystem.AssemblyQualifiedName;
            //    int i = aname.IndexOf(',');
            //    i = aname.IndexOf(',', i + 1);
            //    return aname.Substring(0, i);
            //}
        }

        public static Dictionary<MethodBase, Func<object, object[], object>> FastCalls =
            new Dictionary<MethodBase, Func<object, object[], object>>();

        public virtual IMethod GetMethod(string funcname, MethodParamList types)
        {
            if (funcname == ".ctor")
            {
                var con = TypeForSystem.GetConstructor(types.ToArraySystem());
                return new Method_Common_System(this, con);
            }
            else if (funcname == "Get" && TypeForSystem == typeof(int[,]) && types.Count == 2 && types[0].TypeForSystem == typeof(int) && types[1].TypeForSystem == typeof(int))
            {
                return new Method_Common_System(this, typeof(CustomMethod).GetMethod("GetTwoDimesionArray"));
            }
            var method = TypeForSystem.GetMethod(funcname, types.ToArraySystem());
            var ret = new Method_Common_System(this, method);

            Func<object, object[], object> fastCall;
            if (FastCalls.TryGetValue(method, out fastCall))
            {
                ret.SetFastCall(fastCall);
            }

            return ret;
        }
        public virtual IMethod[] GetMethods(string funcname)
        {
            List<IMethod> methods = new List<IMethod>();
            if (funcname == ".ctor")
            {
                var cons = TypeForSystem.GetConstructors();
                {
                    var __array1 = cons;
                    var __arrayLength1 = __array1.Length;
                    for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var c = __array1[__i1];
                        {
                            methods.Add(new Method_Common_System(this, c));
                        }
                    }
                }
            }
            else
            {
                var __methods = TypeForSystem.GetMethods();
                {
                    var __array2 = __methods;
                    var __arrayLength2 = __array2.Length;
                    for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var m = __array2[__i2];
                        {
                            if (m.Name == funcname)
                            {
                                methods.Add(new Method_Common_System(this, m));
                            }
                        }
                    }
                }
            }

            return methods.ToArray();
        }
        public virtual IMethod[] GetAllMethods()
        {
            List<IMethod> methods = new List<IMethod>();
            {
                var __methods = TypeForSystem.GetMethods();
                {
                    var __array3 = __methods;
                    var __arrayLength3 = __array3.Length;
                    for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                    {
                        var m = __array3[__i3];
                        {
                            //if (m.Name == funcname)
                            {
                                methods.Add(new Method_Common_System(this, m));
                            }
                        }
                    }
                }
            }

            return methods.ToArray();
        }
        public object InitObj()
        {
            return Activator.CreateInstance(TypeForSystem);
        }

        public virtual IMethod GetMethodT(string funcname, MethodParamList ttypes, MethodParamList types)
        {
            if (funcname == "CompareExchange")
            {
                return new Method_Common_System(this, typeof(CustomMethod).GetMethod("CompareExchange"));
            }

            //这个实现还不完全
            //有个别重构下，判定比这个要复杂
            System.Reflection.MethodInfo _method = null;
            var ms = TypeForSystem.GetMethods();
            {
                var __array4 = ms;
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var m = __array4[__i4];
                    {
                        if (m.Name == funcname && m.IsGenericMethodDefinition)
                        {
                            var ts = m.GetGenericArguments();
                            var ps = m.GetParameters();
                            if (ts.Length == ttypes.Count && ps.Length == types.Count)
                            {
                                _method = m;
                                break;
                            }

                        }
                    }
                }
            }
            // _method = TypeForSystem.GetMethod(funcname, types.ToArraySystem());

            return new Method_Common_System(this, _method.MakeGenericMethod(ttypes.ToArraySystem()));
        }
        public virtual IField GetField(string name)
        {
            return new Field_Common_System(env, TypeForSystem.GetField(name));
        }
        public bool IsInst(object obj)
        {
            if (obj is VBox)
            {
                return TypeForSystem.IsInstanceOfType((obj as VBox).BoxDefine());
            }

            return TypeForSystem.IsInstanceOfType(obj);

        }


        public ICLRType GetNestType(ICLRSharp_Environment env, string fullname)
        {
            throw new NotImplementedException();
        }

        public Delegate CreateDelegate(Type deletype, object _this, IMethod_System _method)
        {
            return Delegate.CreateDelegate(deletype, _this, _method.method_System as System.Reflection.MethodInfo);
        }

        public string[] GetFieldNames()
        {
            var fs = TypeForSystem.GetFields();
            string[] names = new string[fs.Length];
            for (int i = 0; i < fs.Length; i++)
            {
                names[i] = fs[i].Name;
            }
            return names;
        }
        public bool IsEnum()
        {
            return TypeForSystem.IsEnum;
        }
    }
    class Field_Common_System : IField
    {
        public System.Reflection.FieldInfo info;
        public Field_Common_System(ICLRSharp_Environment env, System.Reflection.FieldInfo field)
        {
            info = field;

            FieldType = env.GetType(field.FieldType);
            DeclaringType = env.GetType(field.DeclaringType);
        }
        public ICLRType FieldType
        {
            get;
            private set;
        }
        public ICLRType DeclaringType
        {
            get;
            private set;
        }
        public void Set(object _this, object value)
        {
            info.SetValue(_this, value);
        }

        public object Get(object _this)
        {
            return info.GetValue(_this);
        }

        public bool isStatic
        {
            get { return info.IsStatic; }
        }
    }

    public class Method_Common_System : IMethod_System
    {
        public Method_Common_System(ICLRType DeclaringType, System.Reflection.MethodBase method)
        {
            if (method == null)
                throw new Exception("not allow null method.");
            method_System = method;
            this.DeclaringType = DeclaringType;
            if (method is System.Reflection.MethodInfo)
            {
                System.Reflection.MethodInfo info = method as System.Reflection.MethodInfo;
                ReturnType = DeclaringType.env.GetType(info.ReturnType);
            }
            ParamList = new MethodParamList(DeclaringType.env, method);
        }
        public bool isStatic
        {
            get { return method_System.IsStatic; }
        }
        public string Name
        {
            get
            {
                return method_System.Name;
            }
        }

        public ICLRType DeclaringType
        {
            get;
            private set;

        }
        public ICLRType ReturnType
        {
            get;
            private set;
        }
        public MethodParamList ParamList
        {
            get;
            private set;
        }
        public System.Reflection.MethodBase method_System
        {
            get;
            private set;
        }

        private Func<object, object[], object> fastCall;

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
        {
            if (fastCall != null)
                return fastCall(_this, _params);

            //对程序类型，其实我们做不到区分虚实调用。。。没办法
            if (this.Name == "Concat" && this.DeclaringType.TypeForSystem == typeof(string))
            {//这里有一个IL2CPP的问题
                if (_params.Length == 1)
                {
                    if (_params[0] == null)
                        return "null";
                    if (_params[0] is string[])
                    {
                        return string.Concat(_params[0] as string[]);
                    }
                    else if (_params[0] is object[])
                    {
                        return string.Concat(_params[0] as object[]);
                    }
                    else
                    {
                        return _params[0].ToString();
                    }
                }
                else
                {
                    string outstr = "null";
                    if (_params[0] != null) outstr = _params[0].ToString();

                    for (int i = 1; i < _params.Length; i++)
                    {
                        if (_params[i] != null)
                            outstr += _params[i];
                        else
                            outstr += "null";
                    }
                    return outstr;
                }

            }
            return Invoke(context, _this, _params);
        }
        public object Invoke(ThreadContext context, object _this, object[] _params)
        {
            if (_this is CLRSharp_Instance)
            {
                CLRSharp_Instance inst = _this as CLRSharp_Instance;
                if (inst.type.HasSysBase)
                {
                    var btype = inst.type.ContainBase(method_System.DeclaringType);
                    if (btype)
                    {
                        var CrossBind = context.environment.GetCrossBind(method_System.DeclaringType);
                        if (CrossBind != null)
                        {
                            _this = CrossBind.CreateBind(inst);
                        }
                        else
                        {
                            _this = (_this as CLRSharp_Instance).system_base;
                            //如果没有绑定器，尝试直接使用System_base;
                        }
                        //context.environment.logger.Log("这里有一个需要映射的类型");
                    }
                }
            }
            //委托是很特殊的存在
            //if(this.DeclaringType.IsDelegate)
            //{

            //}
            if (method_System is System.Reflection.ConstructorInfo)
            {
                if (method_System.DeclaringType.IsSubclassOf(typeof(Delegate)))
                {//创建委托
                    object src = _params[0];
                    RefFunc fun = _params[1] as RefFunc;
                    IMethod method = fun._method;
                    MemoryPool.ReleaseRefFunc(fun);
                    ICLRType_Sharp clrtype = method.DeclaringType as ICLRType_Sharp;
                    if (clrtype != null)//onclr
                    {

                        CLRSharp_Instance inst = src as CLRSharp_Instance;
                        if (method.isStatic && clrtype != null)
                            inst = clrtype.staticInstance;
                        return inst.GetDelegate(context, method_System.DeclaringType, method);
                    }
                    else//onsystem
                    {
                        ICLRType_System stype = method.DeclaringType as ICLRType_System;
                        return stype.CreateDelegate(method_System.DeclaringType, src, method as IMethod_System);
                    }
                }
                object[] _outp = null;
                if (_params != null && _params.Length > 0)
                {
                    _outp = new object[_params.Length];
                    var _paramsdef = ParamList;
                    for (int i = 0; i < _params.Length; i++)
                    {
                        if (_params[i] == null)
                        {
                            _outp[i] = null;
                            continue;
                        }
                        Type tsrc = _params[i].GetType();
                        Type ttarget = _paramsdef[i].TypeForSystem;
                        if (tsrc == ttarget)
                        {
                            _outp[i] = _params[i];
                        }
                        else if (tsrc.IsSubclassOf(ttarget))
                        {
                            _outp[i] = _params[i];
                        }
                        else if (ttarget.IsEnum)//特殊处理枚举
                        {
                            _outp[i] = Enum.ToObject(ttarget, _params[i]);
                        }
						else if (tsrc.IsEnum) {
							_outp[i] = Enum.ToObject(ttarget, _params[i]);
						}
                        else
                        {
                            if (ttarget == typeof(byte))
                                _outp[i] = (byte)Convert.ToDecimal(_params[i]);
                            else
                            {
                                _outp[i] = _params[i];
                            }
                            //var ms =_params[i].GetType().GetMethods();
                        }
                    }
                }
                var newobj = (method_System as System.Reflection.ConstructorInfo).Invoke(_outp);
                return newobj;
            }
            else
            {
                object[] hasref = MemoryPool.GetArray(_params.Length);
                long flag = 0;
                object[] _outp = null;
                if (_params != null && _params.Length > 0)
                {
                    _outp = new object[_params.Length];
                    var _paramsdef = ParamList;
                    for (int i = 0; i < _params.Length; i++)
                    {
                        var _targetType = _paramsdef[i].TypeForSystem;
                        if (_targetType.GetElementType() != null)
                        {
                            _targetType = _targetType.GetElementType();
                        }
                        if (_params[i] is CLRSharp.StackFrame.RefObj) //特殊处理outparam
                        {
                            object v = (_params[i] as CLRSharp.StackFrame.RefObj).Get();
                            MemoryPool.ReleaseRefObj(_params[i]);
                            if (v is VBox)
                            {
                                v = (v as VBox).BoxDefine();
                            }
                            else if (v == null)
                            {
                                v = null;
                            }
                            else if (v.GetType() != _targetType &&
                                     _targetType != typeof(object))
                            {
                                v = v.Convert(_targetType);
                            }
                            hasref[i] = v;
                            flag |= (1 << i);
                            _outp[i] = v;
                        }
                        else if (_targetType.IsEnum || _params[i] is Enum) //特殊处理枚举
                        {
                            _outp[i] = Enum.ToObject(_targetType, _params[i]);
                        }
                        else
                        {
                            var v = _params[i];
                            if (v != null && v.GetType() != _targetType &&
                                _targetType != typeof(object))
                            {
                                v = v.Convert(_targetType);
                            }
                            _outp[i] = v;
                        }
                    }
                }
                //if (method_System.DeclaringType.IsSubclassOf(typeof(Delegate)))//直接用Delegate.Invoke,会导致转到本机代码再回来
                ////会导致错误堆栈不方便观察,但是也没办法直接调用，只能手写一些常用类型
                //{
                //    //需要从Delegate转换成实际类型执行的帮助类
                //    Action<int> abc = _this as Action<int>;
                //    abc((int)_params[0]);
                //    return null;
                //}
                //else
                {
                    try
                    {
                        if (_this != null && _this.GetType() != method_System.DeclaringType)
                        {
                            _this = _this.Convert(method_System.DeclaringType);
                        }

                        var _out = method_System.Invoke(_this, _outp);
                        {
                            for (int i = 0; i < hasref.Length; i++)
                            {
                                var _ref = hasref[i];
                                if ((flag & (1 << i)) == 0)
                                {
                                    continue;
                                }

                                if (_ref is VBox)
                                {
                                    (_ref as VBox).SetDirect(_outp[i]);
                                }
                                else
                                {
                                    (_params[i] as CLRSharp.StackFrame.RefObj).Set(_outp[i]);
                                }
                            }
                        }
                        return _out;
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (_outp != null)
                        {
                            sb.Append("real args: ");
                            {
                                var __array6 = _outp;
                                var __arrayLength6 = __array6.Length;
                                for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
                                {
                                    var o = __array6[__i6];
                                    {
                                        sb.Append(o.GetType().FullName);
                                        sb.Append("=");
										sb.Append(o);
										sb.Append(",");
                                    }
                                }
                            }
                        }

						context.environment.logger.Log_Error(string.Format("name:{0} type:{1} ret:{2} {3} ex:{4}", method_System.ToString(),
                            DeclaringType, ReturnType, sb, ex));
                        throw;
                    }
                    finally
                    {
                        MemoryPool.ReleaseArray(hasref);
                    }

                }
            }

        }

        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
        {
            try
            {
                return Invoke(context, _this, _params);
            }
            catch (Exception err)
            {
                if (context == null) context = ThreadContext.activeContext;
                if (context == null)
                    throw new Exception("当前线程没有创建ThreadContext,无法Dump", err);
                else
                {
                    context.environment.logger.Log_Error("Error InSystemCall:" + this.DeclaringType.FullName + "::" + this.Name);
                    throw err;
                }
            }
        }

        public void SetFastCall(Func<object, object[], object> call)
        {
            if (call == null)
                return;

            Type_Common_System.FastCalls[method_System] = call;
            fastCall = call;
        }
    }
}
