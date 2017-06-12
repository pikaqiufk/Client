using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CLRSharp
{
    public class Type_Common_CLRSharp : ICLRType_Sharp
    {
        public System.Type TypeForSystem
        {
            get
            {
                if (_isenum)
                {
                    return typeof(int);
                }
                return typeof(CLRSharp_Instance);
            }
        }
        public Mono.Cecil.TypeDefinition type_CLRSharp
        {
            get;
            private set;
        }

        public Dictionary<string, List<KeyValuePair<MethodDefinition, Method_Common_CLRSharp>>> methodCache;

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
        public ICLRType BaseType
        {
            get;
            private set;
        }
        public List<ICLRType> _Interfaces = null;

        public bool ContainBase(Type t)
        {
            if (BaseType != null && BaseType.TypeForSystem == t) return true;
            if (_Interfaces == null) return false;
            {
                var __list1 = _Interfaces;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var i = __list1[__i1];
                    {
                        if (i.TypeForSystem == t) return true;
                    }
                }
            }
            return false;
        }
        public bool HasSysBase
        {
            get;
            private set;
        }
        public string[] GetMethodNames()
        {
            string[] t = new string[type_CLRSharp.Methods.Count];
            for (int i = 0; i < type_CLRSharp.Methods.Count; i++)
            {
                t[i] = type_CLRSharp.Methods[i].Name;
            }
            return t;
        }
        public Type_Common_CLRSharp(ICLRSharp_Environment env, Mono.Cecil.TypeDefinition type)
        {
            this.env = env;
            this.type_CLRSharp = type;
            if (type.IsEnum)
            {
                _isenum = true;
            }
            if (type_CLRSharp.BaseType != null)
            {
                BaseType = env.GetType(type_CLRSharp.BaseType.FullName);
                if (BaseType is ICLRType_System)
                {
                    if (BaseType.TypeForSystem == typeof(Enum) || BaseType.TypeForSystem == typeof(object) || BaseType.TypeForSystem == typeof(ValueType) || BaseType.TypeForSystem == typeof(System.Enum))
                    {//都是这样，无所谓
                        BaseType = null;
                    }
                    else
                    {//继承了其他系统类型
                        env.logger.Log_Error("ScriptType:" + Name + " Based On a SystemType:" + BaseType.Name);
                        HasSysBase = true;
                        throw new Exception("不得继承系统类型，脚本类型系统和脚本类型系统是隔离的");

                    }
                }
                if (type_CLRSharp.HasInterfaces)
                {
                    _Interfaces = new List<ICLRType>();
                    bool bWarning = true;
                    {
                        // foreach(var i in type_CLRSharp.Interfaces)
                        var __enumerator2 = (type_CLRSharp.Interfaces).GetEnumerator();
                        while (__enumerator2.MoveNext())
                        {
                            var i = __enumerator2.Current;
                            {
                                var itype = env.GetType(i.FullName);
                                if (itype is ICLRType_System)
                                {
                                    //继承了其他系统类型
                                    Type ts = (itype as ICLRType_System).TypeForSystem;

                                    if (bWarning & env.GetCrossBind(ts) == null)
                                    {

                                        if (ts.IsInterface)
                                        {
                                            {
                                                var __array12 = ts.GetInterfaces();
                                                var __arrayLength12 = __array12.Length;
                                                for (int __i12 = 0; __i12 < __arrayLength12; ++__i12)
                                                {
                                                    var t = __array12[__i12];
                                                    {
                                                        if (env.GetCrossBind(t) != null)
                                                        {
                                                            bWarning = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (bWarning)
                                        {
                                            env.logger.Log_Warning("警告:没有CrossBind的情况下直接继承\nScriptType:" + Name + " Based On a SystemInterface:" + itype.Name);
                                        }
                                    }
                                    HasSysBase = true;
                                }
                                _Interfaces.Add(itype);
                            }
                        }
                    }
                }
            }
            {
                // foreach(var m in this.type_CLRSharp.Methods)
                var __enumerator3 = (this.type_CLRSharp.Methods).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var m = __enumerator3.Current;
                    {
                        if (m.Name == ".cctor")
                        {
                            NeedCCtor = true;
                            break;
                        }
                    }
                }
            }

            methodCache = new Dictionary<string, List<KeyValuePair<MethodDefinition, Method_Common_CLRSharp>>>();

        }
        public IMethod GetVMethod(IMethod _base)
        {
            IMethod _method = null;
            ICLRType_Sharp type = this;
            while (type != _base.DeclaringType && type != null)
            {
                _method = type.GetMethod(_base.Name, _base.ParamList);
                if (_method != null)
                    return _method;
                type = env.GetType(type.type_CLRSharp.BaseType.FullName) as ICLRType_Sharp;
            }
            return _base;

        }
        public void ResetStaticInstace()
        {
            this._staticInstance = null;
            {
                // foreach(var m in this.type_CLRSharp.Methods)
                var __enumerator4 = (this.type_CLRSharp.Methods).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var m = __enumerator4.Current;
                    {
                        if (m.Name == ".cctor")
                        {
                            NeedCCtor = true;
                            break;
                        }
                    }
                }
            }

        }
        public string Name
        {
            get { return type_CLRSharp.Name; }
        }

        public string FullName
        {
            get { return type_CLRSharp.FullName; }
        }
        public string FullNameWithAssembly
        {
            get
            {
                if (_isenum)
                {
                    return env.GetType(typeof(int)).FullNameWithAssembly;
                }
                return type_CLRSharp.FullName;// +"," + type_CLRSharp.Module.Name;
            }
        }

        public IMethod GetMethod(string funcname, MethodParamList types)
        {
            List<KeyValuePair<MethodDefinition, Method_Common_CLRSharp>> methodDefinitions;
            if (methodCache.TryGetValue(funcname, out methodDefinitions))
            {
                {
                    var __list5 = methodDefinitions;
                    var __listCount5 = __list5.Count;
                    for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                    {
                        var mm = __list5[__i5];
                        {
                            var m = mm.Key;
                            if ((types == null) ? !m.HasParameters : (m.Parameters.Count == types.Count))
                            {
                                bool match = true;
                                for (int i = 0; i < ((types == null) ? 0 : types.Count); i++)
                                {
                                    var envtype = env.GetType(m.Parameters[i].ParameterType.FullName);
                                    if (envtype.IsEnum())
                                    {
                                        if (envtype.TypeForSystem != types[i].TypeForSystem)
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (envtype != types[i])
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                }
                                if (match)
                                {
                                    return mm.Value;
                                }
                            }
                        }
                    }
                }
            }

            if (type_CLRSharp.HasMethods)
            {
                {
                    // foreach(var m in type_CLRSharp.Methods)
                    var __enumerator6 = (type_CLRSharp.Methods).GetEnumerator();
                    while (__enumerator6.MoveNext())
                    {
                        var m = __enumerator6.Current;
                        {
                            if (m.Name != funcname) continue;
                            if ((types == null) ? !m.HasParameters : (m.Parameters.Count == types.Count))
                            {
                                bool match = true;
                                for (int i = 0; i < ((types == null) ? 0 : types.Count); i++)
                                {
                                    var envtype = env.GetType(m.Parameters[i].ParameterType.FullName);
                                    if (envtype.IsEnum())
                                    {
                                        if (envtype.TypeForSystem != types[i].TypeForSystem)
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (envtype != types[i])
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                }
                                if (match)
                                {
                                    List<KeyValuePair<MethodDefinition, Method_Common_CLRSharp>> l;
                                    if (!methodCache.TryGetValue(funcname, out l))
                                    {
                                        l = new List<KeyValuePair<MethodDefinition, Method_Common_CLRSharp>>();
                                        methodCache.Add(funcname, l);
                                    }

                                    var method = new Method_Common_CLRSharp(this, m); ;
                                    l.Add(new KeyValuePair<MethodDefinition, Method_Common_CLRSharp>(m, method));

                                    return method;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public IMethod GetMethodOverloaded(string funcname, MethodParamList types)
        {
            Mono.Cecil.MethodDefinition minDistanceMethod = null;
            List<int> minDistanceParameters = null;

            if (type_CLRSharp.HasMethods)
            {
                {
                    // foreach(var m in type_CLRSharp.Methods)
                    var __enumerator7 = (type_CLRSharp.Methods).GetEnumerator();
                    while (__enumerator7.MoveNext())
                    {
                        var m = __enumerator7.Current;
                        {
                            if (m.Name != funcname) continue;
                            if ((types == null) ? !m.HasParameters : (m.Parameters.Count == types.Count))
                            {
                                bool match = true;
                                List<int> currentDistanceParameters = new List<int>();

                                for (int i = 0; i < ((types == null) ? 0 : types.Count); i++)
                                {
                                    var envtype = env.GetType(m.Parameters[i].ParameterType.FullName);
                                    if (envtype.IsEnum())
                                    {
                                        if (envtype.TypeForSystem != types[i].TypeForSystem)
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (!(envtype.TypeForSystem.IsAssignableFrom(types[i].TypeForSystem)))
                                        {
                                            match = false;
                                            break;
                                        }

                                        currentDistanceParameters.Add(GetInheritanceDistance(envtype.TypeForSystem, types[i].TypeForSystem));

                                    }
                                }
                                if (match)
                                {
                                    if (minDistanceParameters == null)
                                    {
                                        minDistanceMethod = m;
                                        minDistanceParameters = currentDistanceParameters;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < currentDistanceParameters.Count; i++)
                                        {
                                            if (currentDistanceParameters[i] < minDistanceParameters[i])
                                            {
                                                minDistanceMethod = m;
                                                minDistanceParameters = currentDistanceParameters;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                if (minDistanceMethod == null)
                {
                    return null;
                }
                return new Method_Common_CLRSharp(this, minDistanceMethod);
            }

            return null;
        }

        public int GetInheritanceDistance(Type baseClass, Type subClass)
        {
            if (baseClass == subClass)
            {
                return 0;
            }
            if (!baseClass.IsAssignableFrom(subClass))
            {
                return int.MaxValue;
            }

            int distance = 0;
            while ((subClass = subClass.BaseType) != baseClass)
            {
                distance++;
            }
            return ++distance;
        }

        public IMethod[] GetMethods(string funcname)
        {
            List<IMethod> methods = new List<IMethod>();
            if (type_CLRSharp.HasMethods)
            {
                {
                    // foreach(var m in type_CLRSharp.Methods)
                    var __enumerator8 = (type_CLRSharp.Methods).GetEnumerator();
                    while (__enumerator8.MoveNext())
                    {
                        var m = __enumerator8.Current;
                        {
                            if (m.Name != funcname) continue;
                            methods.Add(new Method_Common_CLRSharp(this, m));
                        }
                    }
                }
            }
            return methods.ToArray();
        }
        public IMethod[] GetAllMethods()
        {
            List<IMethod> methods = new List<IMethod>();
            if (type_CLRSharp.HasMethods)
            {
                {
                    // foreach(var m in type_CLRSharp.Methods)
                    var __enumerator9 = (type_CLRSharp.Methods).GetEnumerator();
                    while (__enumerator9.MoveNext())
                    {
                        var m = __enumerator9.Current;
                        {
                            methods.Add(new Method_Common_CLRSharp(this, m));
                        }
                    }
                }
            }
            return methods.ToArray();
        }
        public object InitObj()
        {
            return new CLRSharp_Instance(this);
        }
        public IMethod GetMethodT(string funcname, MethodParamList ttypes, MethodParamList types)
        {
            return null;
        }
        public IField GetField(string name)
        {
            {
                // foreach(var f in type_CLRSharp.Fields)
                var __enumerator10 = (type_CLRSharp.Fields).GetEnumerator();
                while (__enumerator10.MoveNext())
                {
                    var f = __enumerator10.Current;
                    {
                        if (f.Name == name)
                        {
                            return new Field_Common_CLRSharp(this, f);
                        }
                    }
                }
            }
            return null;
        }
        public bool IsInst(object obj)
        {
            if (obj is CLRSharp_Instance)
            {
                CLRSharp_Instance ins = obj as CLRSharp_Instance;
                if (ins.type == this)
                {
                    return true;
                }
                //这里还要实现继承关系
            }
            return false;

        }

        public ICLRType GetNestType(ICLRSharp_Environment env, string fullname)
        {
            {
                // foreach(var stype in type_CLRSharp.NestedTypes)
                var __enumerator11 = (type_CLRSharp.NestedTypes).GetEnumerator();
                while (__enumerator11.MoveNext())
                {
                    var stype = __enumerator11.Current;
                    {
                        if (stype.Name == fullname)
                        {
                            var itype = new Type_Common_CLRSharp(env, stype);
                            env.RegType(itype);
                            return itype;
                        }
                    }
                }
            }
            return null;
        }

        CLRSharp_Instance _staticInstance = null;
        public CLRSharp_Instance staticInstance
        {
            get
            {
                if (_staticInstance == null)
                    _staticInstance = new CLRSharp_Instance(this);
                return _staticInstance;
            }
        }

        public bool NeedCCtor
        {
            get;
            private set;
        }
        public void InvokeCCtor(ThreadContext context)
        {
            NeedCCtor = false;
            this.GetMethod(".cctor", null).Invoke(context, this.staticInstance, new object[] { });

        }


        public string[] GetFieldNames()
        {
            string[] abc = new string[type_CLRSharp.Fields.Count];
            for (int i = 0; i < type_CLRSharp.Fields.Count; i++)
            {
                abc[i] = type_CLRSharp.Fields[i].Name;
            }
            return abc;
        }
        bool _isenum = false;
        public bool IsEnum()
        {
            return _isenum;
        }
    }
    public class Method_Common_CLRSharp : IMethod_Sharp
    {
        Type_Common_CLRSharp _DeclaringType;

        public Method_Common_CLRSharp(Type_Common_CLRSharp type, Mono.Cecil.MethodDefinition method)
        {

            if (method == null)
                throw new Exception("not allow null method.");
            this._DeclaringType = type;

            method_CLRSharp = method;
            ReturnType = type.env.GetType(method.ReturnType.FullName);

            ParamList = new MethodParamList(type.env, method);
        }
        public string Name
        {
            get
            {
                return method_CLRSharp.Name;

            }
        }

        public bool isStatic
        {
            get
            {
                return method_CLRSharp.IsStatic;
            }
        }
        public ICLRType DeclaringType
        {
            get
            {
                return _DeclaringType;
            }
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
        public Mono.Cecil.MethodDefinition method_CLRSharp;
        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
        {
            if (EmptyBody)
            {
                return null;
            }

            if (context == null)
                context = ThreadContext.activeContext;
            if (context == null)
                throw new Exception("这个线程上没有CLRSharp:ThreadContext");
            if (bVisual && method_CLRSharp.IsVirtual)
            {
                CLRSharp_Instance inst = _this as CLRSharp_Instance;
                if (inst.type != this.DeclaringType)
                {
                    IMethod impl = inst.type.GetVMethod(this);// .GetMethod(this.Name, this.ParamList);
                    if (impl != this)
                    {
                        return impl.Invoke(context, _this, _params);
                    }
                }
            }
            if (method_CLRSharp.Name == ".ctor")
            {
                CLRSharp_Instance inst = _this as CLRSharp_Instance;
                if (inst == null)
                    inst = new CLRSharp_Instance(_DeclaringType);

                //if (_DeclaringType.BaseType is ICLRType_System)
                context.ExecuteFunc(this, inst, _params);
                return inst;
            }
            var obj = context.ExecuteFunc(this, _this, _params);
            if (obj is CLRSharp_Instance && ReturnType is ICLRType_System)
            {
                var bind = context.environment.GetCrossBind((ReturnType as ICLRType_System).TypeForSystem);
                if (bind != null)
                {
                    obj = bind.CreateBind(obj as CLRSharp_Instance);
                }
            }
            return obj;
        }
        public object Invoke(ThreadContext context, object _this, object[] _params)
        {
            if (EmptyBody)
            {
                return null;
            }

            return Invoke(context, _this, _params, true);
        }
        public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
        {
            try
            {
                if (EmptyBody)
                {
                    return null;
                }

                return Invoke(context, _this, _params, bVisual);
            }
            catch (Exception err)
            {
                if (context == null) context = ThreadContext.activeContext;
                if (context == null)
                    throw new Exception("当前线程没有创建ThreadContext,无法Dump", err);
                else
                {
                    context.environment.logger.Log_Error(DeclaringType.FullName + "." + Name + " has error. " + context.Dump());
                    throw err;
                }
            }
        }

        private bool? _emptyBody = null;
        bool EmptyBody
        {
            get
            {
                if (_emptyBody != null)
                {
                    return _emptyBody.Value;
                }

                if (method_CLRSharp.Body == null)
                    return true;

                var ins = method_CLRSharp.Body.Instructions;
                if (ins.Count == 0)
                {
                    _emptyBody = true;
                    return true;
                }
                else if(ins.Count > 0)
                {
                    int i = 0;
                    while (ins[i].OpCode.Code == Code.Nop)
                    {
                        i++;
                    }

                    if (ins[i].OpCode.Code == Code.Ret)
                    {
                        _emptyBody = true;
                        return true;
                    }
                }

                _emptyBody = false;
                return false;
            }
        }

        CodeBody _body = null;
        public CodeBody body
        {
            get
            {
                if (_body == null)
                {
                    if (!method_CLRSharp.HasBody)
                        return null;
                    _body = new CodeBody(this._DeclaringType.env, this.method_CLRSharp);
                    //(this.DeclaringType.env as CLRSharp_Environment).CreateCodeBody(this);
                }
                return _body;
            }

        }
    }

    public class Field_Common_CLRSharp : IField
    {
        public Type_Common_CLRSharp _DeclaringType;
        public Mono.Cecil.FieldDefinition field;
        public Field_Common_CLRSharp(Type_Common_CLRSharp type, Mono.Cecil.FieldDefinition field)
        {
            this.field = field;
            this.FieldType = type.env.GetType(field.FieldType.FullName);
            this._DeclaringType = type;

        }
        public ICLRType FieldType
        {
            get;
            private set;
        }
        public ICLRType DeclaringType
        {
            get
            {
                return _DeclaringType;
            }
        }
        public void Set(object _this, object value)
        {
            CLRSharp_Instance sins = null;
            if (_this == null)
            {
                sins = _DeclaringType.staticInstance;
            }
            else
            {
                sins = _this as CLRSharp_Instance;
            }


            sins.Fields[field.Name] = value;
        }

        public object Get(object _this)
        {
            CLRSharp_Instance sins = null;
            if (_this == null)
            {
                sins = _DeclaringType.staticInstance;
            }
            else
            {
                sins = _this as CLRSharp_Instance;
            }

            object v = null;
            if (sins.Fields.TryGetValue(field.Name, out v))
            {
                return v;
            }

            v = field.FieldType.IsValueType ? (object)0 : null;
            sins.Fields.Add(field.Name, v);

            return v;
        }

        public bool isStatic
        {
            get { return this.field.IsStatic; }
        }
    }
}
