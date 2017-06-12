using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Mono.Cecil.Cil;
using TB.ComponentModel;

namespace CLRSharp
{
    public class RefFunc
    {
        public IMethod _method;
        public object _this;
        public RefFunc(IMethod _method, object _this)
        {
            this._method = _method;
            this._this = _this;
        }
    }
    /// <summary>
    /// 堆栈帧
    /// 一个堆栈帧，包含一个计算栈，一个临时变量槽，一个参数槽
    /// 模拟虚拟机上的堆栈帧
    /// </summary>
    public class StackFrame
    {
        public string Name
        {
            get;
            private set;
        }
        public bool IsStatic
        {
            get;
            private set;
        }
        public StackFrame(string name, bool isStatic)
        {
            this.Name = name;
            this.IsStatic = isStatic;
        }
        Mono.Cecil.Cil.Instruction _posold;
        public void SetCodePos(int offset)
        {
            _codepos = this._body.addr[offset];
        }
        public Mono.Cecil.Cil.Instruction GetCode(int pos = -1)
        {
            if (_body == null) return null;
            if (pos == -1)
                pos = _codepos;
            int nowaddr = 0;
            if (pos >= _body.opCodes.Count)
            {
                nowaddr = this._body.opCodes[_body.opCodes.Count - 1].addr;
            }
            else
            {
                nowaddr = this._body.opCodes[pos].addr;
            }
            {
                // foreach(var c in this._body.bodyNative.Instructions)
                var __enumerator1 = (this._body.bodyNative.Instructions).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var c = __enumerator1.Current;
                    {
                        if (nowaddr == c.Offset)
                            return c;
                    }
                }
            }
            return null;
        }

        private Stack<int> leavePos = new Stack<int>();

        public int _codepos = 0;
        public class MyCalcStack : Stack<object>
        {
            public void Push(object box)
            {
                base.Push(box);
            }
            public new object Pop()
            {
                var ob = base.Pop();
                return ob;

            }
            public void ClearVBox()
            {
                this.Clear();
            }
        }
        public MyCalcStack stackCalc = new MyCalcStack();
        public class MySlotVar : List<object>
        {
            public new void Add(object obj)
            {
                base.Add(obj);
            }
            public void Add(VBox box)
            {
                base.Add(box);
            }

            public void ClearVBox()
            {
                this.Clear();
            }
        }

        public MySlotVar slotVar = new MySlotVar();
        public object[] _params = null;
        public void SetParams(object[] _p)
        {
            if (_p == null)
            {
                _params = null;
                return;
            }
            _params = MemoryPool.GetArray(_p.Length);
            for (int i = 0; i < _p.Length; i++)
            {
                _params[i] = _p[i];
            }
        }
        CodeBody _body = null;
        public CodeBody codebody
        {
            get
            {
                return _body;
            }
        }
        public void Init(CodeBody body)
        {
            _body = body;
            if (body.typelistForLoc != null)
            {
                for (int i = 0; i < body.typelistForLoc.Count; i++)
                {
                    slotVar.Add(null);
                }
            }
            this.leavePos.Clear();
        }
        public object Return()
        {
            MemoryPool.ReleaseArray(_params);
            this.slotVar.ClearVBox();
            if (this.stackCalc.Count == 0) return null;
            object ret = stackCalc.Pop();
            this.stackCalc.ClearVBox();
            this.leavePos.Clear();
            return ret;
        }
        void FillArray(object array, byte[] bytes)
        {
            if (array is byte[])
            {
                byte[] arr = array as byte[];
                for (int i = 0; i < bytes.Length; i++)
                {
                    arr[i] = bytes[i];
                }
            }
            else if (array is sbyte[])
            {
                sbyte[] arr = array as sbyte[];
                for (int i = 0; i < bytes.Length; i++)
                {
                    arr[i] = (sbyte)bytes[i];
                }
            }
            else if (array is Int16[])
            {
                int step = 2;
                Int16[] arr = array as Int16[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToInt16(bytes, i * step);
                }
            }
            else if (array is UInt16[])
            {
                int step = 2;
                UInt16[] arr = array as UInt16[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToUInt16(bytes, i * step);
                }
            }
            else if (array is char[])
            {
                int step = 2;
                char[] arr = array as char[];
                for (int i = 0; i < Math.Min(bytes.Length / step, arr.Length); i++)
                {
                    arr[i] = (char)BitConverter.ToUInt16(bytes, i * step);
                }
            }
            else if (array is int[])
            {
                int step = 4;
                int[] arr = array as int[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToInt32(bytes, i * step);
                }
            }
            else if (array is uint[])
            {
                int step = 4;
                uint[] arr = array as uint[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToUInt32(bytes, i * step);
                }
            }
            else if (array is Int64[])
            {
                int step = 8;
                Int64[] arr = array as Int64[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToInt64(bytes, i * step);
                }
            }
            else if (array is UInt64[])
            {
                int step = 8;
                UInt64[] arr = array as UInt64[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToUInt64(bytes, i * step);
                }
            }
            else if (array is float[])
            {
                int step = 4;
                float[] arr = array as float[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToSingle(bytes, i * step);
                }
            }
            else if (array is double[])
            {
                int step = 8;
                double[] arr = array as double[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToDouble(bytes, i * step);
                }
            }
            else if (array is bool[])
            {
                int step = 1;
                bool[] arr = array as bool[];
                for (int i = 0; i < bytes.Length / step; i++)
                {
                    arr[i] = BitConverter.ToBoolean(bytes, i * step);
                }
            }
            else
            {
                throw new NotImplementedException("array=" + array.GetType());
            }
        }
        //流程控制
        public void Call(ThreadContext context, IMethod _clrmethod, bool bVisual)
        {

            if (_clrmethod == null)//不想被执行的函数
            {
                _codepos++;
                return;
            }

            object[] _pp = null;
            object _this = null;
            bool bCLR = _clrmethod is IMethod_Sharp;
            if (_clrmethod.ParamList != null)
            {
                _pp = MemoryPool.GetArray(_clrmethod.ParamList.Count);
                for (int i = 0; i < _pp.Length; i++)
                {
                    int iCallPPos = _pp.Length - 1 - i;
                    ICLRType pType = _clrmethod.ParamList[iCallPPos];
                    var pp = stackCalc.Pop();
                    if (pp is CLRSharp_Instance && pType.TypeForSystem != typeof(CLRSharp_Instance))
                    {
                        var inst = pp as CLRSharp_Instance;

                        var btype = inst.type.ContainBase(pType.TypeForSystem);
                        if (btype)
                        {
                            var CrossBind = context.environment.GetCrossBind(pType.TypeForSystem);
                            if (CrossBind != null)
                            {
                                pp = CrossBind.CreateBind(inst);
                            }
                            else
                            {
                                pp = inst.system_base;
                                //如果没有绑定器，尝试直接使用System_base;
                            }
                            //context.environment.logger.Log("这里有一个需要映射的类型");
                        }

                    }
                    if (pp is VBox && !bCLR)
                    {
                        var v = pp as VBox;
                        var p = v.BoxDefine();
                        if (p == null)
                        {
                            pp = p;
                        }
                        else if (p.GetType() != pType.TypeForSystem && pType.TypeForSystem != typeof(object))
                        {
                            var vbox = ValueOnStack.Convert(v, pType.TypeForSystem);
                            pp = vbox != null ? vbox.BoxDefine() : v;
                        }
                        else
                        {
                            pp = p;
                        }
                    }
                    else if (pp is ICLRType_System)
                    {
                        pp = (pp as ICLRType_System).TypeForSystem;
                    }
                    else if (pType.TypeForSystem.IsByRef)
                    {
                        if (pp is VBox)
                        {
                            pp = (pp as VBox).BoxDefine();
                        }
                    }
                    else if (pp != null && pp.GetType() != pType.TypeForSystem && pType.TypeForSystem != typeof(object))
                    {
                        pp = pp.Convert(pType.TypeForSystem);
                    }
                    else if (pp is Array)
                    {
                        var array = pp as Array;
                        if (array.Rank == 1)
                        {
                            for (int j = 0; j < array.Length; j++)
                            {
                                if (array.GetValue(j) is VBox)
                                {
                                    array.SetValue((array.GetValue(j) as VBox).BoxDefine(), j);
                                }
                            }
                        }
                    }

                    _pp[iCallPPos] = pp;
                }
            }


            //if (method.HasThis)
            if (!_clrmethod.isStatic)
            {
                _this = stackCalc.Pop();
            }
            if (_clrmethod.DeclaringType.FullName.Contains("System.Runtime.CompilerServices.RuntimeHelpers") && _clrmethod.Name.Contains("InitializeArray"))
            {
                FillArray(_pp[0], _pp[1] as byte[]);
                _codepos++;
                return;
            }
            if (_clrmethod.DeclaringType.FullName.Contains("System.Type") && _clrmethod.Name.Contains("GetTypeFromHandle"))
            {
                stackCalc.Push(_pp[0]);
                _codepos++;
                return;
            }
            if (_clrmethod.DeclaringType.FullName.Contains("System.Object") && _clrmethod.Name.Contains(".ctor"))
            {//跳过这个没意义的构造
                _codepos++;
                return;
            }
            if (_this is RefObj && _clrmethod.Name != ".ctor")
            {
                _this = (_this as RefObj).Get();

            }
            if (_this is VBox)
            {
                _this = (_this as VBox).BoxDefine();
            }
            bool bCross = (_this is CLRSharp_Instance && _clrmethod is IMethod_System);
            object returnvar = _clrmethod.Invoke(context, _this, _pp, bVisual);
            if (bCross)
            {
                //这里究竟如何处理还需要再考虑
                //returnvar = _clrmethod.Invoke(context, (_this as CLRSharp_Instance).system_base, _pp, bVisual);
                if (_clrmethod.Name.Contains(".ctor"))
                {
                    (_this as CLRSharp_Instance).system_base = returnvar;
                    returnvar = (_this);
                }
            }
            else
            {
                //returnvar = _clrmethod.Invoke(context, _this, _pp, bVisual);
            }

            // bool breturn = false;
            if (_clrmethod.ReturnType != null && _clrmethod.ReturnType.FullName != "System.Void")
            {
                if (_clrmethod.ReturnType.FullName == "System.Object")
                {
                    if (returnvar == null)
                    {
                        stackCalc.Push(null);
                    }
                    else if (returnvar.GetType().IsValueType)
                    {
                        stackCalc.Push(ValueOnStack.MakeVBox(returnvar));
                    }
                    else
                    {
                        stackCalc.Push(returnvar);
                    }
                }
                else
                {
                    stackCalc.Push(returnvar);
                }
            }

            else if (_this is RefObj && _clrmethod.Name == ".ctor")
            {
                //如果这里有发生程序类型，脚本类型的cross，就需要特别处理
                (_this as RefObj).Set(returnvar);
            }

            MemoryPool.ReleaseArray(_pp);

            _codepos++;
            return;

        }
        //栈操作
        public void Nop()
        {
            _codepos++;
        }
        public void Dup()
        {
            var v = stackCalc.Peek();
            if (v is VBox)
            {
                v = (v as VBox).Clone();
            }
            stackCalc.Push(v);
            _codepos++;
        }
        public void Pop()
        {
            stackCalc.Pop();
            _codepos++;
        }
        //流程控制
        public void Ret()
        {
            _codepos++;
        }
        public void Box(ICLRType type)
        {
            object obj = stackCalc.Pop();

            if (obj == null)
            {
                stackCalc.Push(obj);
                _codepos++;
                return;
            }

            VBox box = obj as VBox;
            if (type.TypeForSystem.IsEnum)
            {
                int ev = 0;
                if (box != null)
                {
                    if (box.BoxDefine() != null)
                    {
                        ev = (int)box.BoxDefine().Convert<int>();
                    }
                    else
                    {
                        throw new Exception("how to do with this.");
                    }
                }
                else
                    ev = (int)obj;

                obj = ValueOnStack.MakeVBox(Enum.ToObject(type.TypeForSystem, ev));
            }
            else
            {
                if (box != null)
                {
                    var value = box.BoxDefine();

                    if (value != null &&
                        (type.TypeForSystem != value.GetType() &&
                         type.TypeForSystem != typeof(object)))
                    {
                        obj = ValueOnStack.MakeVBox(value.Convert(type.TypeForSystem));
                    }
                }
                else
                {
                    if (obj != null &&
                        (type.TypeForSystem != obj.GetType() &&
                         type.TypeForSystem != typeof(object)))
                    {
                        obj = ValueOnStack.MakeVBox(obj.Convert(type.TypeForSystem));
                    }
                    else
                    {
                        obj = ValueOnStack.MakeVBox(obj);
                    }
                }
            }
            stackCalc.Push(obj);
            _codepos++;
        }
        public void Unbox()
        {
            object obj = stackCalc.Pop();
            if (obj is VBox)
            {
                stackCalc.Push((obj as VBox).BoxDefine());
            }
            else
            {
                stackCalc.Push(obj);
            }
            _codepos++;
        }
        public void Unbox_Any()
        {
            object obj = stackCalc.Pop();
            if (obj is VBox)
            {
                stackCalc.Push((obj as VBox).BoxDefine());
            }
            else
            {
                stackCalc.Push(obj);
            }

            _codepos++;
        }
        public void Br(int addr_index)
        {
            _codepos = addr_index;// _body.addr[pos.Offset];
        }
        public void Leave(int addr_index, CodeBody body)
        {
            //找到对应的try块
            if (body.method.Body.HasExceptionHandlers)
            {
                var eh = body.method.Body.ExceptionHandlers;
                {
                    // foreach(var exceptionHandler in eh)
                    var __enumerator2 = (eh).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var exceptionHandler = __enumerator2.Current;
                        {
                            if (exceptionHandler.HandlerType == ExceptionHandlerType.Finally)
                            {
                                var codenow = GetCode();
                                var codetarget = GetCode(addr_index);
                                if (codenow.Offset >= exceptionHandler.TryStart.Offset && codenow.Offset < exceptionHandler.TryEnd.Offset
                                    && !(codetarget.Offset >= exceptionHandler.TryStart.Offset && codetarget.Offset < exceptionHandler.TryEnd.Offset))
                                {
                                    _codepos = body.addr[exceptionHandler.HandlerStart.Offset];
                                    leavePos.Push(addr_index);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            _codepos = addr_index;// _body.addr[addr];
        }
        //public void Leave_AddrIndex(int addr_index)
        //{
        //    stackCalc.Clear();
        //    _codepos = addr_index;
        //}
        public void Brtrue(int addr_index)
        {
            object obj = stackCalc.Pop();
            bool b = false;
            if (obj != null)
            {
                if (obj is VBox)
                {
                    b = true;
                }
                else if (obj.GetType().IsClass)
                {
                    b = true;
                }
                else if (obj is DateTime)
                {
                    b = ((DateTime)obj).ToBinary() != 0;
                }
                else if (obj is bool)
                {
                    b = (bool)obj;
                }
                else
                {
                    b = (bool)obj.Convert<bool>();
                }
            }

            if (b)
            {
                _codepos = addr_index;// _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }
        }
        public void Brfalse(int addr_index)
        {
            object obj = stackCalc.Pop();
            bool b = false;
            if (obj != null)
            {
                if (obj is VBox)
                {
                    b = true;
                }
                else if (obj.GetType().IsClass)
                {
                    b = true;
                }
                else if (obj is bool)
                {
                    b = (bool)obj;
                }
                else if (obj is DateTime)
                {
                    b = ((DateTime)obj).ToBinary() != 0;
                }
                else
                {
                    try
                    {
                        b = (bool)obj.Convert<bool>();
                    }
                    catch
                    {
                        // obj is not null
                        b = true;
                    }
                }
            }
            if (!b)
            {
                _codepos = addr_index;// _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }
        }
        //条件跳转
        public void Beq(int addr_index)
        {
            object o2 = stackCalc.Pop();
            object o1 = stackCalc.Pop();
            if (Operator.Equal(o1, o2))
            {
                _codepos = addr_index; // _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }

        }

        public void Bne(int addr_index)
        {
            object o2 = stackCalc.Pop();
            object o1 = stackCalc.Pop();
            if (!Operator.Equal(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }
        }
        public void Bne_Un(int addr_index)
        {
            object o2 = stackCalc.Pop();
            object o1 = stackCalc.Pop();
            if (!Operator.Equal(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }
        }
        public void Bge(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.GreaterEqual(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Bge_Un(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.GreaterEqual(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Bgt(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.Greater(o1, o2))
            {
                _codepos = addr_index;//_body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Bgt_Un(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.Greater(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Ble(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.LessEqual(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Ble_Un(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.LessEqual(o1, o2))
            {
                _codepos = addr_index;//_body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Blt(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.Less(o1, o2))
            {
                _codepos = addr_index;//_body.addr[pos.Offset];

            }
            else
            {
                _codepos++;
            }
        }
        public void Blt_Un(int addr_index)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (Operator.Less(o1, o2))
            {
                _codepos = addr_index;// _body.addr[pos.Offset];
            }
            else
            {
                _codepos++;
            }
        }
        //加载常量

        public void Ldc_I4(int v)//int32
        {
            stackCalc.Push(v);
            _codepos++;
        }
        public void Ldc_I8(Int64 v)//int64
        {
            stackCalc.Push(v);
            _codepos++;
        }
        public void Ldc_R4(float v)
        {
            stackCalc.Push(v);
            _codepos++;
        }
        public void Ldc_R8(double v)
        {
            stackCalc.Push(v);
            _codepos++;
        }
        //放进变量槽
        public void Stloc(int pos)
        {
            object v = stackCalc.Pop();
            while (slotVar.Count <= pos)
            {
                slotVar.Add(null);
            }

            slotVar[pos] = v;
            _codepos++;
        }
        //拿出变量槽
        public void Ldloc(int pos)
        {
            var obj = slotVar[pos];
            stackCalc.Push(obj);
            _codepos++;
        }
        public enum RefType
        {
            loc,//本地变量槽
            arg,//参数槽
            field,//成员变量
            Array,
        }
        public class RefObj
        {
            public StackFrame frame;
            public int pos;
            public RefType type;
            //public ICLRType _clrtype;
            public IField _field;
            public object _this;
            public Array _array;
            public RefObj(StackFrame frame, int pos, RefType type)
            {
                this.frame = frame;
                this.pos = pos;
                this.type = type;
            }
            public RefObj(IField field, object _this)
            {
                this.type = RefType.field;
                //this._clrtype = type;
                this._field = field;
                this._this = _this;
            }
            public RefObj(Array array, int index)
            {
                this.type = RefType.Array;
                this._array = array;
                this.pos = index;
            }
            public void Set(object obj)
            {
                if (type == RefType.arg)
                {
                    frame._params[pos] = obj;
                }
                else if (type == RefType.loc)
                {
                    while (frame.slotVar.Count <= pos)
                    {
                        frame.slotVar.Add(null);
                    }
                    frame.slotVar[pos] = obj;
                }
                else if (type == RefType.field)
                {
                    _field.Set(_this, obj);
                }
                else if (type == RefType.Array)
                {
                    _array.SetValue(obj, pos);
                }

            }
            public object Get()
            {
                if (type == RefType.arg)
                {
                    return frame._params[pos];
                }
                else if (type == RefType.loc)
                {
                    while (frame.slotVar.Count <= pos)
                    {
                        frame.slotVar.Add(null);
                    }
                    return frame.slotVar[pos];
                }
                else if (type == RefType.field)
                {
                    return _field.Get(_this);
                }
                else if (type == RefType.Array)
                {
                    return _array.GetValue(pos);
                }
                return null;
            }

        }

        //拿出变量槽的引用

        public void Ldloca(int pos)
        {
            var obj = MemoryPool.GetRefObj(this, pos, RefType.loc);
            stackCalc.Push(obj);
            _codepos++;
        }

        public void Ldstr(string text)
        {
            stackCalc.Push(text);
            _codepos++;
        }

        //加载参数(还得处理static，il静态非静态不一样，成员参数0是this)
        public void Ldarg(int pos)
        {
            object obj = null;
            if (_params != null)
                obj = _params[pos];
            VBox b = obj as VBox;
            if (b != null)
            {
                obj = b.Clone();
            }
            stackCalc.Push(obj);
            _codepos++;
        }
        public void Ldarga(int pos)
        {
            var obj = MemoryPool.GetRefObj(this, pos, RefType.arg);
            stackCalc.Push(obj);
            _codepos++;
        }
        //逻辑计算

        public void Ceq()
        {
            object o2 = stackCalc.Pop();
            object o1 = stackCalc.Pop();
            stackCalc.Push(Operator.Equal(o1, o2));
            _codepos++;
        }
        public void Cgt()
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            stackCalc.Push(Operator.Greater(o1, o2));
            _codepos++;
        }
        public void Cgt_Un()
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            stackCalc.Push(Operator.Greater(o1, o2));
            _codepos++;
        }
        public void Clt()
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            stackCalc.Push(Operator.Less(o1, o2));

            _codepos++;
        }
        public void Clt_Un()
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            stackCalc.Push(Operator.Less(o1, o2));
            _codepos++;
        }
        public void Ckfinite()
        {
            object n1 = stackCalc.Pop();
            if (n1 is float)
            {
                float v = (float)n1;
                stackCalc.Push(float.IsInfinity(v) || float.IsNaN(v) ? 1 : 0);
            }
            else
            {
                double v = (double)n1;
                stackCalc.Push(double.IsInfinity(v) || double.IsNaN(v) ? 1 : 0);
            }
            _codepos++;
        }
        public object PopAndCheckBox()
        {
            object o = stackCalc.Pop();
            if (o is VBox)
            {
                return (o as VBox).BoxDefine();
            }

            return o;
        }

        //算术操作
        public void Add(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Add(o1, o2, op));
            }
            _codepos++;
        }

        public void Sub(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Sub(o1, o2, op));
            }
            _codepos++;
        }

        public void Mul(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Mul(o1, o2, op));
            }
            _codepos++;
        }


        public void Div(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Div(o1, o2, op));
            }
            _codepos++;
        }
        public void Div_Un(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Div(o1, o2, op));
            }
            _codepos++;
        }
        public void Rem(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Rem(o1, o2, op));
            }
            _codepos++; ;
        }
        public void Rem_Un(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Rem(o1, o2, op));
            }
            _codepos++;
        }

        public void Neg()
        {
            object n1 = PopAndCheckBox();
            if (n1 is byte)
            {
                stackCalc.Push(-(byte)n1);
            }
            else if (n1 is short)
            {
                stackCalc.Push(-(short)n1);
            }
            else if (n1 is int)
            {
                stackCalc.Push(-(int)n1);
            }
            else if (n1 is float)
            {
                stackCalc.Push(-(float)n1);
            }
            else if (n1 is double)
            {
                stackCalc.Push(-(double)n1);
            }
            else
            {
                throw new ArithmeticException();
            }

            _codepos++;
        }

        //转换
        public void Conv_I1()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<sbyte>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<sbyte>());
            }
            _codepos++;
        }
        public void Conv_U1()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<byte>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<byte>());
            }
            _codepos++;
        }
        public void Conv_I2()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int16>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int16>());
            }
            _codepos++;
        }
        public void Conv_U2()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt16>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt16>());
            }
            _codepos++;
        }
        public void Conv_I4()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int32>());
            }
            _codepos++;
        }
        public void Conv_U4()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt32>());
            }
            _codepos++;
        }
        public void Conv_I8()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int64>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int64>());
            }
            _codepos++;
        }
        public void Conv_U8()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt64>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt64>());
            }
            _codepos++;
        }
        public void Conv_I()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int32>());
            }
            _codepos++;
        }
        public void Conv_U()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt32>());
            }
            _codepos++;
        }
        public void Conv_R4()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<float>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<float>());
            }
            _codepos++;
        }
        public void Conv_R8()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<double>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<double>());
            }
            _codepos++;
        }
        public void Conv_R_Un()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<float>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<float>());
            }
            _codepos++;
        }

        ////数组
        public void NewArr(ThreadContext context, Type type)
        {
            var objv = stackCalc.Pop();
            if (objv is VBox) objv = (objv as VBox).BoxDefine();
            var array = Array.CreateInstance(type, (int)objv);
            stackCalc.Push(array);
            _codepos++;
        }

        public void LdLen()
        {
            var obj = stackCalc.Pop();
            Array a = obj as Array;
            stackCalc.Push(a.Length);
            _codepos++;
        }
        public void Ldelema(object obj)
        {
            int index = PopValue<int>();
            Array array = stackCalc.Pop() as Array;
            var o = MemoryPool.GetRefObj(array, index);
            stackCalc.Push(o);
            _codepos++;
            //_codepos++;
        }
        public void Ldelem_I1()
        {
            int index = PopValue<int>();
            var _array = stackCalc.Pop();
            if (_array is sbyte[])
            {
                sbyte[] array = _array as sbyte[];
                stackCalc.Push(array[index]);
            }
            else if (_array is bool[])
            {
                bool[] array = _array as bool[];
                stackCalc.Push(array[index]);
            }
            else
            {
                throw new Exception("not support.this array i1");
            }

            _codepos++;
        }
        public void Ldelem_U1()
        {
            int index = PopValue<int>();
            object obj = stackCalc.Pop();
            if (obj is byte[])
            {
                byte[] array = obj as byte[];
                stackCalc.Push(array[index]);
                _codepos++;
            }
            else if (obj is bool[])
            {
                bool[] array = obj as bool[];
                stackCalc.Push(array[index]);
                _codepos++;
            }
        }

        public void Ldelem_I2()
        {
            int index = PopValue<int>();
            Int16[] array = stackCalc.Pop() as Int16[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Ldelem_U2()
        {
            int index = PopValue<int>();
            var _array = stackCalc.Pop();
            if (_array is UInt16[])
            {
                UInt16[] array = _array as UInt16[];
                stackCalc.Push(array[index]);
            }
            else
            {
                char[] array = _array as char[];
                stackCalc.Push(array[index]);
            }

            _codepos++;
        }
        public void Ldelem_I4()
        {
            int index = PopValue<int>();
            int[] array = stackCalc.Pop() as int[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Ldelem_U4()
        {
            int index = PopValue<int>();
            uint[] array = stackCalc.Pop() as uint[];
            stackCalc.Push(array[index]);
            _codepos++;
        }

        public void Ldelem_I8()
        {
            int index = PopValue<int>();
            var obj = stackCalc.Pop();
            if (obj is Int64[])
            {
                Int64[] array = obj as Int64[];
                stackCalc.Push(array[index]);
            }
            else
            {
                UInt64[] array = obj as UInt64[];
                stackCalc.Push(array[index]);
            }
            _codepos++;
        }
        public void Ldelem_I()
        {
            int index = PopValue<int>();
            int[] array = stackCalc.Pop() as int[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Ldelem_R4()
        {
            int index = PopValue<int>();
            float[] array = stackCalc.Pop() as float[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Ldelem_R8()
        {
            int index = PopValue<int>();
            double[] array = stackCalc.Pop() as double[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Ldelem_Ref()
        {
            int index = PopValue<int>();
            Array array = stackCalc.Pop() as Array;
            stackCalc.Push(array.GetValue(index));
            _codepos++;
        }
        public void Ldelem_Any(object obj)
        {
            int index = PopValue<int>();
            Object[] array = stackCalc.Pop() as Object[];
            stackCalc.Push(array[index]);
            _codepos++;
        }
        public void Stelem_I()
        {
            Stelem_I4();
        }
        public void Stelem_I1()
        {
            var value = PopValue<byte>();
            int index = PopValue<int>();
            var array = stackCalc.Pop();
            if (array is sbyte[])
            {
                (array as sbyte[])[index] = (sbyte)value;
            }
            else if (array is byte[])
            {
                (array as byte[])[index] = (byte)value;
            }
            else if (array is bool[])
            {
                (array as bool[])[index] = value != 0;

            }

            _codepos++;
        }

        private T PopValue<T>()
        {
            var obj = stackCalc.Pop();
            if (obj is VBox)
            {
                return (T)(obj as VBox).BoxDefine().Convert<T>();
            }
            else
            {
                return (T)obj.Convert<T>();
            }
        }

        public void Stelem_I2()
        {
            var value = PopValue<Int16>();
            int index = PopValue<int>();
            var array = stackCalc.Pop();
            if (array is char[])
            {
                (array as char[])[index] = (char)value;
            }
            else if (array is Int16[])
            {
                (array as Int16[])[index] = (Int16)value;
            }
            else if (array is UInt16[])
            {
                (array as UInt16[])[index] = (UInt16)value;
            }

            _codepos++;
        }
        public void Stelem_I4()
        {
            var value = PopValue<Int32>();
            int index = PopValue<int>();
            var _array = stackCalc.Pop();
            if (_array is Int32[])
            {
                var array = _array as Int32[];
                array[index] = (Int32)value;

            }
            else if (_array is UInt32[])
            {
                var array = _array as UInt32[];
                array[index] = (UInt32)value;
            }
            _codepos++;
        }
        public void Stelem_I8()
        {
            var value = PopValue<Int64>();
            int index = PopValue<int>();
            var _array = stackCalc.Pop();
            if (_array is Int64[])
            {
                var array = _array as Int64[];
                array[index] = (Int64)value;
            }
            else if (_array is UInt64[])
            {
                var array = _array as UInt64[];
                array[index] = (UInt64)value;
            }
            _codepos++;
        }
        public void Stelem_R4()
        {
            var value = PopValue<float>();
            int index = PopValue<int>();
            var array = stackCalc.Pop() as float[];
            array[index] = value;
            _codepos++;
        }
        public void Stelem_R8()
        {
            var value = PopValue<double>();
            int index = PopValue<int>();
            var array = stackCalc.Pop() as double[];
            array[index] = value;
            _codepos++;
        }
        public void Stelem_Ref()
        {
            var value = stackCalc.Pop();
            int index = PopValue<int>();
            var array = stackCalc.Pop() as Object[];
            array[index] = value;
            _codepos++;
        }

        public void Stelem_Any()
        {
            var value = stackCalc.Pop();
            int index = PopValue<int>();
            var array = stackCalc.Pop() as Object[];
            array[index] = value;
            _codepos++;
        }

        //寻址类
        public void NewObj(ThreadContext context, IMethod _clrmethod)
        {
            object[] _pp = null;
            bool bCLR = _clrmethod is IMethod_Sharp;
            if (_clrmethod.ParamList != null)
            {
                _pp = MemoryPool.GetArray(_clrmethod.ParamList.Count);
                for (int i = 0; i < _pp.Length; i++)
                {
                    int iCallPPos = _pp.Length - 1 - i;
                    ICLRType pType = _clrmethod.ParamList[iCallPPos];
                    var pp = stackCalc.Pop();
                    if (pp is VBox && !bCLR)
                    {
                        pp = (pp as VBox).BoxDefine();
                    }
                    if (pp != null && pType.TypeForSystem != pp.GetType() && pType.TypeForSystem != typeof(object))
                    {
                        pp = pp.Convert(pType.TypeForSystem);
                    }
                    _pp[iCallPPos] = pp;
                }
            }

            //var typesys = context.environment.GetType(method.DeclaringType.FullName, method.Module);
            object returnvar = _clrmethod.Invoke(context, null, _pp);
            MemoryPool.ReleaseArray(_pp);
            stackCalc.Push(returnvar);

            _codepos++;
        }

        public void Ldfld(ThreadContext context, IField field)
        {
            var obj = stackCalc.Pop();

            if (obj is RefObj)
            {
                obj = (obj as RefObj).Get();
            }

            //0	chocolateEN	 0x00124ec8	 m_CLRSharp_Field_Common_CLRSharp_Get_object + 148
            //1	chocolateEN	 0x0011be48	 m_CLRSharp_StackFrame_Ldfld_CLRSharp_ThreadContext_CLRSharp_IField + 268
            if (obj == null)
            {
                throw new Exception("Ldfld failed from " + field.DeclaringType.FullName + " obj == null.");
            }
            var value = field.Get(obj);

            if (value != null &&
                (field.FieldType.TypeForSystem != value.GetType() &&
                 field.FieldType.TypeForSystem != typeof(object)))
            {
                value = value.Convert(field.FieldType.TypeForSystem);
            }

            stackCalc.Push(value);
            _codepos++;
        }
        public void Ldflda(ThreadContext context, IField field)
        {
            var obj = stackCalc.Pop();
            var o = MemoryPool.GetRefObj(field, obj);
            stackCalc.Push(o);
            _codepos++;
        }

        public void Ldsfld(ThreadContext context, IField field)
        {
            var value = field.Get(null);
            if (value != null &&
                (field.FieldType.TypeForSystem != value.GetType() &&
                 field.FieldType.TypeForSystem != typeof(object)))
            {
                value = value.Convert(field.FieldType.TypeForSystem);
            }
            stackCalc.Push(value);
            _codepos++;
        }
        public void Ldsflda(ThreadContext context, IField field)
        {
            var obj = MemoryPool.GetRefObj(field, null);
            stackCalc.Push(obj);
            _codepos++;
        }
        public void Stfld(ThreadContext context, IField field)
        {
            var value = stackCalc.Pop();
            var obj = stackCalc.Pop();
            if (obj is RefObj)
            {
                var _this = (obj as RefObj).Get();
                if (_this == null && !field.isStatic)
                {
                    (obj as RefObj).Set(field.DeclaringType.InitObj());
                }
                obj = (obj as RefObj).Get();
            }
            else if (obj is VBox)
            {
                obj = (obj as VBox).BoxDefine();
            }

            if (value is VBox)
            {
                value = (value as VBox).BoxDefine();
            }
            else if (value == null)
            {
                value = null;
            }
            if (value != null &&
                (field.FieldType.TypeForSystem != value.GetType() &&
                 field.FieldType.TypeForSystem != typeof(object)))
            {
                value = value.Convert(field.FieldType.TypeForSystem);
            }

            field.Set(obj, value);
            _codepos++;
        }
        public void Stsfld(ThreadContext context, IField field)
        {
            var value = stackCalc.Pop();
            if (value is VBox)
            {
                value = (value as VBox).BoxDefine();
            }

            if (value != null &&
                (field.FieldType.TypeForSystem != value.GetType() &&
                 field.FieldType.TypeForSystem != typeof(object)))
            {
                value = value.Convert(field.FieldType.TypeForSystem);
            }

            field.Set(null, value);

            _codepos++;
        }
        public void Constrained(ThreadContext context, ICLRType obj)
        {

            _codepos++;
        }
        public void Isinst(ThreadContext context, ICLRType _type)
        {
            var value = stackCalc.Pop();
            object v = value;
            if (value is VBox)
            {
                v = (value as VBox).BoxDefine();
            }

            if (_type.IsInst(v))
                stackCalc.Push(value);
            else
                stackCalc.Push(null);

            _codepos++;
        }
        public void Ldtoken(ThreadContext context, object token)
        {
            stackCalc.Push(token);
            _codepos++;
        }

        public void Conv_Ovf_I1()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<sbyte>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<sbyte>());
            }
            _codepos++;
        }
        public void Conv_Ovf_U1()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<byte>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<byte>());
            }
            _codepos++;
        }
        public void Conv_Ovf_I2()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int16>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int16>());
            }
            _codepos++;
        }
        public void Conv_Ovf_U2()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt16>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt16>());
            }
            _codepos++;
        }
        public void Conv_Ovf_I4()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int32>());
            }
            _codepos++;
        }
        public void Conv_Ovf_U4()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt32>());
            }
            _codepos++;
        }
        public void Conv_Ovf_I8()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int64>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int64>());
            }
            _codepos++;
        }
        public void Conv_Ovf_U8()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt64>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt64>());
            }
            _codepos++;
        }
        public void Conv_Ovf_I()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<Int32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<Int32>());
            }
            _codepos++;
        }
        public void Conv_Ovf_U()
        {
            object num1 = stackCalc.Pop();
            VBox b = num1 as VBox;
            if (b != null)
            {
                stackCalc.Push(ValueOnStack.MakeVBox(b.BoxDefine().Convert<UInt32>()));
            }
            else
            {
                stackCalc.Push(num1.Convert<UInt32>());
            }
            _codepos++;
        }
        public void Conv_Ovf_I1_Un()
        {
            throw new NotImplementedException();
        }

        public void Conv_Ovf_U1_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_I2_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_U2_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_I4_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_U4_Un()
        {
            throw new NotImplementedException();
        }

        public void Conv_Ovf_I8_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_U8_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_I_Un()
        {
            throw new NotImplementedException();
        }
        public void Conv_Ovf_U_Un()
        {
            throw new NotImplementedException();
        }

        public void Ldftn(ThreadContext context, IMethod method)
        {
            var func = MemoryPool.GetRefFunc(method, null);
            stackCalc.Push(func);
            _codepos++;
        }
        public void Ldvirtftn(ThreadContext context, IMethod method)
        {
            object _this = stackCalc.Pop();
            var func = MemoryPool.GetRefFunc(method, _this);
            stackCalc.Push(func);

            _codepos++;
        }

        public void Starg(ThreadContext context, int p)
        {
            object _this = stackCalc.Pop();
            if (_this is VBox)
            {
                _this = (_this as VBox).Clone();
            }
            this._params[p] = _this;
            _codepos++;
        }

        public void Calli(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }

        public void Break(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }

        public void Ldnull()
        {
            stackCalc.Push(null);
            _codepos++;
        }
        public void Jmp(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }

        public void Switch(ThreadContext context, int[] index)
        {
            var indexobj = stackCalc.Pop();
            uint pos = 0;
            if (indexobj is VBox)
            {
                pos = (uint)(indexobj as VBox).BoxDefine().Convert<uint>();
            }
            else if (indexobj is int)
            {
                pos = (uint)(int)indexobj;
            }
            else
            {
                pos = (uint)indexobj.Convert<uint>();
            }

            if (pos >= index.Length)
            {
                _codepos++;
            }
            else
            {
                _codepos = index[pos];
            }
        }
        public void Ldind_I1()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_I1:");
            //_codepos++;
        }
        public void Ldind_U1()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_U1:");
            //_codepos++;
        }
        public void Ldind_I2()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_I2:");
            //_codepos++;
        }
        public void Ldind_U2()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_U2:");
            //_codepos++;
        }
        public void Ldind_I4()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_I4:");
            //_codepos++;
        }
        public void Ldind_U4()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_U4:");
            //_codepos++;
        }
        public void Ldind_I8()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_I8:");
            //_codepos++;
        }
        public void Ldind_I()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_I:");
            //_codepos++;
        }
        public void Ldind_R4()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_R4:");
            //_codepos++;
        }
        public void Ldind_R8()
        {
            object obje = PopAndCheckBox();
            if (obje is RefObj)
            {
                RefObj _ref = obje as RefObj;
                object value = _ref.Get();
                stackCalc.Push(value);
                _codepos++;
                return;
            }
            throw new Exception("not impl Ldind_R8:");
            //_codepos++;
        }
        public void Ldind_Ref()
        {

            throw new Exception("not impl Ldind_Ref:");
            //_codepos++;
        }
        public void Stind_Ref(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_I1(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_I2(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_I4(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_I8(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_R4(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void Stind_R8(ThreadContext context, object obj)
        {
            var o1 = stackCalc.Pop();
            var o2 = stackCalc.Pop();
            if (o2 is RefObj)
            {
                (o2 as RefObj).Set(o1 is VBox ? (o1 as VBox).BoxDefine() : o1);
            }

            _codepos++;
        }
        public void And(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.And(o1, o2, op));
            }
            _codepos++;
        }
        public void Or(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Or(o1, o2, op));
            }
            _codepos++;
        }
        public void Xor(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Xor(o1, o2, op));
            }
            _codepos++;
        }
        public void Shl(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Shl(o1, o2, op));
            }
            _codepos++;
            //_codepos++;
        }
        public void Shr(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Shr(o1, o2, op));
            }
            _codepos++;
        }
        public void Shr_Un(CodeBody.OpCode op)
        {
            object o2 = PopAndCheckBox();
            object o1 = PopAndCheckBox();
            if (op.binaryOperation != null)
            {
                stackCalc.Push(op.binaryOperation(o1, o2));
            }
            else
            {
                stackCalc.Push(Operator.Shr(o1, o2, op));
            }
            _codepos++;
        }
        public void Not()
        {
            object o1 = PopAndCheckBox();
            stackCalc.Push(Operator.Not(o1));
            _codepos++;
        }
        public void Cpobj(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Ldobj(ThreadContext context, object obj)
        {
            var pos = stackCalc.Pop() as RefObj;
            stackCalc.Push(pos.Get());
            _codepos++;
        }
        public void Castclass(ThreadContext context, ICLRType _type)
        {
            if (_type is ICLRType_System)
            {
                var obj = PopAndCheckBox();
                stackCalc.Push(obj.Convert(_type.TypeForSystem));
            }

            _codepos++;
        }
        public void Throw(ThreadContext context, object obj)
        {
            Exception exc = stackCalc.Pop() as Exception;
            throw exc;
            //_codepos++;
        }
        public void Stobj(ThreadContext context, object obj)
        {
            var v = stackCalc.Pop();
            var addr = stackCalc.Pop() as RefObj;
            addr.Set(v);
            _codepos++;
        }
        public void Refanyval(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Mkrefany(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }

        public void Add_Ovf(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Add_Ovf_Un(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Mul_Ovf(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Mul_Ovf_Un(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Sub_Ovf(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Sub_Ovf_Un(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Endfinally(ThreadContext context, object obj)
        {
            //Type t = obj.GetType();
            //throw new NotImplementedException(t.ToString());

            if (leavePos.Count > 0)
            {
                _codepos = leavePos.Pop();
                return;
            }

            _codepos++;
        }
        public void Stind_I(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Arglist(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }

        public void Localloc(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Endfilter(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Unaligned(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Volatile()
        {
            _codepos++;
        }
        //ThreadContext context, object obj)
        //{
        //    Type t = obj.GetType();
        //    throw new NotImplementedException(t.ToString());
        //    //_codepos++;
        //}
        public void Tail(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Initobj(ThreadContext context, ICLRType _type)
        {
            RefObj _this = stackCalc.Pop() as RefObj;
            var _object = _type.InitObj();
            _this.Set(_object);

            _codepos++;
        }
        public void Cpblk(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Initblk(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void No(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Rethrow(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Sizeof(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Refanytype(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
        public void Readonly(ThreadContext context, object obj)
        {
            Type t = obj.GetType();
            throw new NotImplementedException(t.ToString());
            //_codepos++;
        }
    }
}
