using System;
using System.Collections.Generic;
using System.Text;

namespace CLRSharp
{

    public class CLRSharp_Environment : ICLRSharp_Environment
    {
        public string version
        {
            get
            {
                return "0.47Beta";
            }
        }
        public ICLRSharp_Logger logger
        {
            get;
            private set;
        }
        public CLRSharp_Environment(ICLRSharp_Logger logger)
        {
            this.logger = logger;
            logger.Log_Warning("CLR# Ver:" + version + " Inited.");

            this.RegCrossBind(new CrossBind_IEnumerable());
            this.RegCrossBind(new CrossBind_IEnumerator());
            this.RegCrossBind(new CrossBind_IDisposable());
        }
        Dictionary<string, ICLRType> mapType = new Dictionary<string, ICLRType>();
        //public Dictionary<string, Mono.Cecil.ModuleDefinition> mapModule = new Dictionary<string, Mono.Cecil.ModuleDefinition>();
        public void LoadModule(System.IO.Stream dllStream)
        {
            LoadModule(dllStream, null, null);
        }
        public void LoadModule(System.IO.Stream dllStream, System.IO.Stream pdbStream, Mono.Cecil.Cil.ISymbolReaderProvider debugInfoLoader)
        {
            var module = Mono.Cecil.ModuleDefinition.ReadModule(dllStream);
            if (debugInfoLoader != null && pdbStream != null)
            {
                module.ReadSymbols(debugInfoLoader.GetSymbolReader(module, pdbStream));
            }
            if (module.HasAssemblyReferences)
            {
                {
                    // foreach(var ar in module.AssemblyReferences)
                    var __enumerator1 = (module.AssemblyReferences).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var ar = __enumerator1.Current;
                        {
                            if (moduleref.Contains(ar.Name) == false)
                                moduleref.Add(ar.Name);
                            if (moduleref.Contains(ar.FullName) == false)
                                moduleref.Add(ar.FullName);
                        }
                    }
                }
            }
            //mapModule[module.Name] = module;
            if (module.HasTypes)
            {
                {
                    // foreach(var t in module.Types)
                    var __enumerator2 = (module.Types).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var t = __enumerator2.Current;
                        {

                            mapType[t.FullName] = new Type_Common_CLRSharp(this, t);

                        }
                    }
                }
            }

        }
        public List<System.Reflection.Assembly> assemblylist;
        public void AddSerachAssembly(System.Reflection.Assembly assembly)
        {
            if (assemblylist == null)
                assemblylist = new List<System.Reflection.Assembly>();
            assemblylist.Add(assembly);
        }
        public void LoadModule_OnlyName(System.IO.Stream dllStream)
        {
            var module = Mono.Cecil.ModuleDefinition.ReadModule(dllStream);
            if (moduleref.Contains(module.Name) == false)
                moduleref.Add(module.Name);
            if (module.HasAssemblyReferences)
            {
                {
                    // foreach(var ar in module.AssemblyReferences)
                    var __enumerator3 = (module.AssemblyReferences).GetEnumerator();
                    while (__enumerator3.MoveNext())
                    {
                        var ar = __enumerator3.Current;
                        {
                            if (moduleref.Contains(ar.Name) == false)
                                moduleref.Add(ar.Name);
                        }
                    }
                }
            }
        }

        List<string> moduleref = new List<string>();
        public string[] GetAllTypes()
        {
            string[] array = new string[mapType.Count];
            mapType.Keys.CopyTo(array, 0);
            return array;
        }
        public string[] GetModuleRefNames()
        {
            return moduleref.ToArray();
        }
        //得到类型的时候应该得到模块内Type或者真实Type
        //一个统一的Type,然后根据具体情况调用两边

        public ICLRType GetType(string fullname)
        {
            try
            {
                ICLRType type = null;
                bool b = mapType.TryGetValue(fullname, out type);
                if (!b)
                {
                    List<ICLRType> subTypes = new List<ICLRType>();
                    if (fullname.Contains("<>") || fullname.Contains("/"))//匿名类型
                    {
                        string[] subts = fullname.Split('/');
                        ICLRType ft = GetType(subts[0]);
                        if (ft is ICLRType_Sharp)
                        {
                            for (int i = 1; i < subts.Length; i++)
                            {
                                ft = ft.GetNestType(this, subts[i]);
                            }
                            return ft;
                        }
                    }

                    string fullnameT = fullname;//.Replace('/', '+');

                    if (fullnameT.Contains("<"))
                    {
                        string outname = "";
                        int depth = 0;
                        int lastsplitpos = 0;
                        for (int i = 0; i < fullname.Length; i++)
                        {
                            string checkname = null;
                            if (fullname[i] == '/')
                            {

                            }
                            else if (fullname[i] == '<')
                            {
                                if (i != 0)
                                    depth++;
                                if (depth == 1)//
                                {
                                    lastsplitpos = i;
                                    outname += "[";
                                    continue;
                                }

                            }
                            else if (fullname[i] == '>')
                            {
                                if (depth == 1)
                                {
                                    checkname = fullnameT.Substring(lastsplitpos + 1, i - lastsplitpos - 1);
                                    var subtype = GetType(checkname);
                                    if (subtype == null)
                                    {
                                        continue;
                                    }
                                    subTypes.Add(subtype);
                                    if (!subtype.IsEnum() && subtype is ICLRType_Sharp)
                                    {
                                        subtype = GetType(typeof(CLRSharp_Instance));
                                    }
                                    outname += "[" + subtype.FullNameWithAssembly + "]";
                                    lastsplitpos = i;
                                }
                                //if(depth>0)
                                depth--;
                                if (depth == 0)
                                {
                                    outname += "]";
                                    continue;
                                }
                                else if (depth < 0)
                                {
                                    depth = 0;
                                }
                            }
                            else if (fullname[i] == ',')
                            {
                                if (depth == 1)
                                {
                                    checkname = fullnameT.Substring(lastsplitpos + 1, i - lastsplitpos - 1);
                                    if (checkname.StartsWith("System.Action`"))
                                    {
                                        checkname = checkname.Substring(0, 15) + "<" +
                                                    checkname.Substring(16, checkname.Length - 17) + ">";
                                    }
                                    else if (checkname.StartsWith("System.Func`"))
                                    {
                                        checkname = checkname.Substring(0, 13) + "<" +
                                                    checkname.Substring(14, checkname.Length - 15) + ">";
                                    }
                                    var subtype = GetType(checkname);
                                    subTypes.Add(subtype);
                                    if (!subtype.IsEnum() && subtype is ICLRType_Sharp)
                                    {

                                        subtype = GetType(typeof(CLRSharp_Instance));
                                    }

                                    outname += "[" + subtype.FullNameWithAssembly + "],";
                                    lastsplitpos = i;
                                }
                            }
                            if (depth == 0)
                            {
                                outname += fullnameT[i];
                            }
                        }
                        fullnameT = outname;
                        //    fullnameT = fullnameT.Replace('<', '[');
                        //fullnameT = fullnameT.Replace('>', ']');


                    }
                    fullnameT = fullnameT.Replace('/', '+');
                    System.Type t = System.Type.GetType(fullnameT);

                    if (t == null)
                    {
                        if (assemblylist != null)
                        {
                            {
                                var __list4 = assemblylist;
                                var __listCount4 = __list4.Count;
                                for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                                {
                                    var i = __list4[__i4];
                                    {
                                        t = i.GetType(fullnameT);
                                        if (t != null)
                                            break;
                                    }
                                }
                            }
                        }
                        if (t == null)
                        {
                            {
                                var __list5 = moduleref;
                                var __listCount5 = __list5.Count;
                                for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                                {
                                    var rm = __list5[__i5];
                                    {
                                        t = System.Type.GetType(fullnameT + "," + rm);
                                        if (t != null)
                                        {
                                            fullnameT = fullnameT + "," + rm;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (t != null)
                    {
                        //之所以做这么扭曲的设计，是因为Unity的Type.Fullname 实现错误，导致在Unity环境Type.FullName不一致
                        if (t.FullName.Contains("CLRSharp.CLRSharp_Instance") == false)
                        {
                            b = mapType.TryGetValue(t.FullName, out type);
                            if (b)
                            {
                                //mapType[fullname] = type;
                                return type;
                            }
                            type = new Type_Common_System(this, t, subTypes.ToArray());
                            mapType[t.FullName] = type;
                            return type;
                        }
                        else
                        {

                        }
                        type = new Type_Common_System(this, t, subTypes.ToArray());
                        mapType[fullname] = type;
                        //mapType[t.FullName] = type;
                    }

                }
                return type;
            }
            catch (Exception err)
            {
                throw new Exception("Error in getType:" + fullname, err);
            }
        }


        public ICLRType GetType(System.Type systemType)
        {
            ICLRType type = null;
            bool b = mapType.TryGetValue(systemType.FullName, out type);
            if (!b)
            {
                type = new Type_Common_System(this, systemType, null);
                mapType[systemType.FullName] = type;
            }
            return type;
        }
        public void RegType(ICLRType type)
        {
            mapType[type.FullName] = type;
        }

        /// <summary>
        /// 交叉绑定工具，让脚本继承程序类型用的
        /// </summary>
        Dictionary<Type, ICrossBind> crossBind = new Dictionary<Type, ICrossBind>();
        public void RegCrossBind(ICrossBind bind)
        {
            crossBind[bind.Type] = bind;
        }

        public ICrossBind GetCrossBind(Type type)
        {
            ICrossBind bind = null;

            crossBind.TryGetValue(type, out bind);
            return bind;
        }

    }
}
