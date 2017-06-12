using System;
using TB.ComponentModel;

namespace CLRSharp
{
    //栈上值类型，拆箱，装箱转换非常频繁,需要处理一下。
    //
    public class ValueOnStack
    {
        public static VBox MakeVBox(object value, ICLRType type)
        {
            if (type == null) return null;
            return new VBox(value.Convert(type.TypeForSystem));
        }

        public static VBox MakeVBox(object value, System.Type type)
        {
            return new VBox(value.Convert(type));
        }

        public static VBox MakeVBox(object value)
        {
            return new VBox(value);
        }

        public static VBox Convert(VBox box, Type type)
        {
            try
            {
                return new VBox(box.BoxDefine().Convert(type));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class VBox
    {
        public VBox(object value)
        {
            mValue = value;
        }

        public VBox Clone()
        {
            return new VBox(mValue);
        }

        private object mValue;

        public object BoxDefine()
        {
            return mValue;
        }

        //SetValue
        public void SetDirect(object value)
        {
            mValue = value;
        }


        internal void Set(VBox vBox)
        {
            mValue = vBox.mValue;
        }
    }
    //public interface IBox
    //{
    //    object BoxStack();
    //    object BoxDefine();

    //    void Add(IBox right);
    //    void Sub(IBox right);
    //    void Mul(IBox right);
    //    void Div(IBox right);
    //    void Mod(IBox right);

    //    IBox Mod_New(IBox right);
    //    void SetDirect(object value);
    //    void Set(IBox value);

    //    NumberType type
    //    {
    //        get;
    //        set;
    //    }

    //    NumberOnStack typeStack
    //    {
    //        get;
    //    }

    //    bool logic_eq(IBox right);//=
    //    bool logic_ne(IBox right);//!=
    //    bool logic_ne_Un(IBox right);//!=
    //    bool logic_ge(IBox right);//>=
    //    bool logic_ge_Un(IBox right);//>=
    //    bool logic_le(IBox right);//<=
    //    bool logic_le_Un(IBox right);
    //    bool logic_gt(IBox right);//>
    //    bool logic_gt_Un(IBox right);//>
    //    bool logic_lt(IBox right);//<
    //    bool logic_lt_Un(IBox right);//<

    //    bool ToBool();
    //    int ToInt();
    //    uint ToUint();

    //    Int64 ToInt64();
    //    float ToFloat();

    //    double ToDouble();

    //    int refcount
    //    {
    //        get;
    //        set;
    //    }
    //}
    //public class BoxInt32 : IBox
    //{
    //    public int refcount
    //    {
    //        get;
    //        set;
    //    }
    //    public BoxInt32(NumberType type)
    //    {
    //        this.type = type;
    //    }
    //    public NumberType type
    //    {
    //        get;
    //        set;
    //    }
    //    public NumberOnStack typeStack
    //    {
    //        get
    //        {
    //            return NumberOnStack.Int32;
    //        }
    //    }
    //    public Int32 value;
    //    public object BoxStack()
    //    {
    //        return value;
    //    }

    //    public object BoxDefine()
    //    {
    //        switch (type)
    //        {
    //            case NumberType.BOOL:
    //                return (value > 0);
    //            case NumberType.SBYTE:
    //                return (sbyte)value;
    //            case NumberType.BYTE:
    //                return (byte)value;
    //            case NumberType.CHAR:
    //                return (char)value;
    //            case NumberType.INT16:
    //                return (Int16)value;
    //            case NumberType.UINT16:
    //                return (UInt16)value;
    //            case NumberType.INT32:
    //                return (Int32)value;
    //            case NumberType.UINT32:
    //                return (UInt32)value;
    //            case NumberType.INT64:
    //                return (Int64)value;
    //            case NumberType.UINT64:
    //                return (UInt64)value;
    //            case NumberType.FLOAT:
    //                return (float)value;
    //            case NumberType.DOUBLE:
    //                return (double)value;
    //            default:
    //                return null;
    //        }

    //    }

    //    public void Set(IBox value)
    //    {

    //        this.value = (value as BoxInt32).value;
    //    }
    //    public void SetDirect(object value)
    //    {
    //        if (value is bool)
    //        {
    //            this.value = (bool)value ? 1 : 0;
    //        }
    //        else
    //        {
    //            this.value = (int)value;
    //        }
    //    }
    //    public void Add(IBox right)
    //    {
    //        this.value += (right as BoxInt32).value;
    //    }

    //    public void Sub(IBox right)
    //    {
    //        this.value -= (right as BoxInt32).value;
    //    }

    //    public void Mul(IBox right)
    //    {
    //        this.value *= (right as BoxInt32).value;
    //    }

    //    public void Div(IBox right)
    //    {
    //        this.value /= (right as BoxInt32).value;
    //    }
    //    public void Mod(IBox right)
    //    {
    //        this.value %= (right as BoxInt32).value;
    //    }

    //    public IBox Mod_New(IBox right)
    //    {
    //        BoxInt32 b = ValueOnStack.Make(this.type) as BoxInt32;
    //        b.value = this.value % (right as BoxInt32).value;
    //        return b;
    //    }

    //    public bool logic_eq(IBox right)
    //    {
    //        return value == (right as BoxInt32).value;
    //    }


    //    public bool logic_ne(IBox right)
    //    {
    //        return value != (right as BoxInt32).value;
    //    }

    //    public bool logic_ne_Un(IBox right)
    //    {
    //        return (UInt32)value != (UInt32)(right as BoxInt32).value;
    //    }

    //    public bool logic_ge(IBox right)
    //    {
    //        return value >= (right as BoxInt32).value;
    //    }

    //    public bool logic_ge_Un(IBox right)
    //    {
    //        return (UInt32)value >= (UInt32)(right as BoxInt32).value;
    //    }

    //    public bool logic_le(IBox right)
    //    {
    //        return value <= (right as BoxInt32).value;
    //    }

    //    public bool logic_le_Un(IBox right)
    //    {
    //        return (UInt32)value <= (UInt32)(right as BoxInt32).value;
    //    }

    //    public bool logic_gt(IBox right)
    //    {
    //        return value > (right as BoxInt32).value;
    //    }

    //    public bool logic_gt_Un(IBox right)
    //    {
    //        return (UInt32)value > (UInt32)(right as BoxInt32).value;
    //    }

    //    public bool logic_lt(IBox right)
    //    {
    //        return value < (right as BoxInt32).value;
    //    }

    //    public bool logic_lt_Un(IBox right)
    //    {
    //        return (UInt32)value < (UInt32)(right as BoxInt32).value;
    //    }

    //    public bool ToBool()
    //    {
    //        return value > 0;
    //    }
    //    public int ToInt()
    //    {
    //        return (int)value;
    //    }
    //    public uint ToUint()
    //    {
    //        return (uint)value;
    //    }
    //    public Int64 ToInt64()
    //    {
    //        return (Int64)value;
    //    }
    //    public float ToFloat()
    //    {
    //        return (float)value;
    //    }

    //    public double ToDouble()
    //    {
    //        return (double)value;
    //    }
    //}
    //public class BoxInt64 : IBox
    //{
    //    public int refcount
    //    {
    //        get;
    //        set;
    //    }
    //    public BoxInt64(NumberType type)
    //    {
    //        this.type = type;
    //    }
    //    public NumberType type
    //    {
    //        get;
    //        set;
    //    }
    //    public NumberOnStack typeStack
    //    {
    //        get
    //        {
    //            return NumberOnStack.Int64;
    //        }
    //    }
    //    public Int64 value;
    //    public object BoxStack()
    //    {
    //        return value;
    //    }

    //    public object BoxDefine()
    //    {
    //        switch (type)
    //        {
    //            case NumberType.BOOL:
    //                return (value > 0);
    //            case NumberType.SBYTE:
    //                return (sbyte)value;
    //            case NumberType.BYTE:
    //                return (byte)value;
    //            case NumberType.CHAR:
    //                return (char)value;
    //            case NumberType.INT16:
    //                return (Int16)value;
    //            case NumberType.UINT16:
    //                return (UInt16)value;
    //            case NumberType.INT32:
    //                return (Int32)value;
    //            case NumberType.UINT32:
    //                return (UInt32)value;
    //            case NumberType.INT64:
    //                return (Int64)value;
    //            case NumberType.UINT64:
    //                return (UInt64)value;
    //            case NumberType.FLOAT:
    //                return (float)value;
    //            case NumberType.DOUBLE:
    //                return (double)value;
    //            default:
    //                return null;
    //        }

    //    }
    //    public void Set(IBox value)
    //    {
    //        this.value = (value as BoxInt64).value;
    //    }
    //    public void SetDirect(object value)
    //    {
    //        this.value = (Int64)value;
    //    }
    //    public void Add(IBox right)
    //    {
    //        this.value += (right as BoxInt64).value;
    //    }

    //    public void Sub(IBox right)
    //    {
    //        this.value -= (right as BoxInt64).value;
    //    }

    //    public void Mul(IBox right)
    //    {
    //        this.value *= (right as BoxInt64).value;
    //    }

    //    public void Div(IBox right)
    //    {
    //        this.value /= (right as BoxInt64).value;
    //    }
    //    public void Mod(IBox right)
    //    {
    //        this.value %= (right as BoxInt64).value;
    //    }
    //    public IBox Mod_New(IBox right)
    //    {
    //        BoxInt64 b = ValueOnStack.Make(this.type) as BoxInt64;
    //        b.value = this.value % (right as BoxInt64).value;
    //        return b;
    //    }

    //    public bool logic_eq(IBox right)
    //    {
    //        return value == (right as BoxInt64).value;
    //    }


    //    public bool logic_ne(IBox right)
    //    {
    //        return value != (right as BoxInt64).value;
    //    }

    //    public bool logic_ne_Un(IBox right)
    //    {
    //        return (UInt64)value != (UInt64)(right as BoxInt64).value;
    //    }

    //    public bool logic_ge(IBox right)
    //    {
    //        return value >= (right as BoxInt64).value;
    //    }

    //    public bool logic_ge_Un(IBox right)
    //    {
    //        return (UInt64)value >= (UInt64)(right as BoxInt64).value;
    //    }

    //    public bool logic_le(IBox right)
    //    {
    //        return value <= (right as BoxInt64).value;
    //    }

    //    public bool logic_le_Un(IBox right)
    //    {
    //        return (UInt64)value <= (UInt64)(right as BoxInt64).value;
    //    }

    //    public bool logic_gt(IBox right)
    //    {
    //        return value > (right as BoxInt64).value;
    //    }

    //    public bool logic_gt_Un(IBox right)
    //    {
    //        return (UInt64)value > (UInt64)(right as BoxInt64).value;
    //    }

    //    public bool logic_lt(IBox right)
    //    {
    //        return value < (right as BoxInt64).value;
    //    }

    //    public bool logic_lt_Un(IBox right)
    //    {
    //        return (UInt64)value < (UInt64)(right as BoxInt64).value;
    //    }
    //    public bool ToBool()
    //    {
    //        return value > 0;
    //    }
    //    public int ToInt()
    //    {
    //        return (int)value;
    //    }
    //    public uint ToUint()
    //    {
    //        return (uint)value;
    //    }
    //    public Int64 ToInt64()
    //    {
    //        return (Int64)value;
    //    }
    //    public float ToFloat()
    //    {
    //        return (float)value;
    //    }

    //    public double ToDouble()
    //    {
    //        return (double)value;
    //    }
    //}
    //public class BoxDouble : IBox
    //{
    //    public int refcount
    //    {
    //        get;
    //        set;
    //    }
    //    public BoxDouble(NumberType type)
    //    {
    //        this.type = type;
    //    }
    //    public NumberType type
    //    {
    //        get;
    //        set;
    //    }
    //    public NumberOnStack typeStack
    //    {
    //        get
    //        {
    //            return NumberOnStack.Double;
    //        }
    //    }
    //    public double value;
    //    public object BoxStack()
    //    {
    //        return value;
    //    }

    //    public object BoxDefine()
    //    {
    //        switch (type)
    //        {
    //            case NumberType.BOOL:
    //                return (value > 0);
    //            case NumberType.SBYTE:
    //                return (sbyte)value;
    //            case NumberType.BYTE:
    //                return (byte)value;
    //            case NumberType.CHAR:
    //                return (char)value;
    //            case NumberType.INT16:
    //                return (Int16)value;
    //            case NumberType.UINT16:
    //                return (UInt16)value;
    //            case NumberType.INT32:
    //                return (Int32)value;
    //            case NumberType.UINT32:
    //                return (UInt32)value;
    //            case NumberType.INT64:
    //                return (Int64)value;
    //            case NumberType.UINT64:
    //                return (UInt64)value;
    //            case NumberType.FLOAT:
    //                return (float)value;
    //            case NumberType.DOUBLE:
    //                return (double)value;
    //            default:
    //                return null;
    //        }

    //    }

    //    public void Set(IBox value)
    //    {
    //        this.value = (value as BoxDouble).value;
    //    }
    //    public void SetDirect(object value)
    //    {

    //        this.value = (double)Convert.ToDecimal(value);
    //    }
    //    public void Add(IBox right)
    //    {
    //        this.value += (right as BoxDouble).value;
    //    }

    //    public void Sub(IBox right)
    //    {
    //        this.value -= (right as BoxDouble).value;
    //    }

    //    public void Mul(IBox right)
    //    {
    //        this.value *= (right as BoxDouble).value;
    //    }

    //    public void Div(IBox right)
    //    {
    //        this.value /= (right as BoxDouble).value;
    //    }
    //    public void Mod(IBox right)
    //    {
    //        this.value %= (right as BoxDouble).value;
    //    }

    //    public IBox Mod_New(IBox right)
    //    {
    //        BoxDouble b = new BoxDouble(this.type);
    //        b.value = this.value % (right as BoxDouble).value;
    //        return b;
    //    }

    //    public bool logic_eq(IBox right)
    //    {
    //        return value == (right as BoxDouble).value;
    //    }

    //    public bool logic_ne(IBox right)
    //    {
    //        return value != (right as BoxDouble).value;
    //    }

    //    public bool logic_ne_Un(IBox right)
    //    {
    //        return value != (right as BoxDouble).value;
    //    }

    //    public bool logic_ge(IBox right)
    //    {
    //        return value >= (right as BoxDouble).value;
    //    }

    //    public bool logic_ge_Un(IBox right)
    //    {
    //        return value >= (right as BoxDouble).value;
    //    }

    //    public bool logic_le(IBox right)
    //    {
    //        return value <= (right as BoxDouble).value;
    //    }

    //    public bool logic_le_Un(IBox right)
    //    {
    //        return value <= (right as BoxDouble).value;
    //    }

    //    public bool logic_gt(IBox right)
    //    {
    //        return value > (right as BoxDouble).value;
    //    }

    //    public bool logic_gt_Un(IBox right)
    //    {
    //        return value > (right as BoxDouble).value;
    //    }

    //    public bool logic_lt(IBox right)
    //    {
    //        return value < (right as BoxDouble).value;
    //    }

    //    public bool logic_lt_Un(IBox right)
    //    {
    //        return value < (right as BoxDouble).value;
    //    }
    //    public bool ToBool()
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public int ToInt()
    //    {
    //        return (int)value;
    //    }
    //    public uint ToUint()
    //    {
    //        return (uint)value;
    //    }
    //    public Int64 ToInt64()
    //    {
    //        return (Int64)value;
    //    }
    //    public float ToFloat()
    //    {
    //        return (float)value;
    //    }

    //    public double ToDouble()
    //    {
    //        return (double)value;
    //    }
    //}

}
