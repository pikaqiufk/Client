using System;
using System.Collections.Generic;
using CLRSharp;
using TB.ComponentModel;

namespace CLRSharp
{
    public class Operator
    {
        public static List<Func<object, object, object>> RemOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> DivOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> MulOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> SubOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> AddOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> AndOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> OrOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> XorOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> ShlOperators = new List<Func<object, object, object>>(18 * 18);
        public static List<Func<object, object, object>> ShrOperators = new List<Func<object, object, object>>(18 * 18);

        static Operator()
        {
            for (int i = 0; i < 18 * 18; ++i)
            {
                RemOperators.Add(null);
                DivOperators.Add(null);
                MulOperators.Add(null);
                SubOperators.Add(null);
                AddOperators.Add(null);
                AndOperators.Add(null);
                OrOperators.Add(null);
                XorOperators.Add(null);
                ShlOperators.Add(null);
                ShrOperators.Add(null);
            }

            #region RemOperator

            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Byte)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Byte)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Byte)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Byte)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Byte)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Byte)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Char)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Char)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Char)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Char)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Char)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Char)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Decimal)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Decimal)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Decimal)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Decimal)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Decimal)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Decimal)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Decimal)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Decimal)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Decimal)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Decimal)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Double)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Double)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Double)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Double)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Double)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Double)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Double)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Double)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Double)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Double)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Double)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int16)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int16)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int16)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int16)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int16)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int16)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int16)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int32)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int32)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int32)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int32)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int32)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int32)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int32)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int64)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int64)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int64)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int64)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int64)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int64)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int64)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (SByte)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (SByte)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (SByte)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (SByte)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (SByte)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (SByte)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(SByte)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Single)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Single)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Single)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Single)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Single)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Single)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Single)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Single)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Single)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Single)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Single)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt16)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt16)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt16)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt16)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt16)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt16)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt16)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt16)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt16)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt16)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt32)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt32)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt32)lhs % (Int16)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt32)lhs % (Int32)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt32)lhs % (Int64)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt32)lhs % (SByte)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt32)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt32)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt32)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt32)lhs % (UInt64)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs % (Byte)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs % (Char)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt64)lhs % (Decimal)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt64)lhs % (Double)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs % (UInt64)(Int16)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs % (UInt64)(Int32)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs % (UInt64)(Int64)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs % (UInt64)(SByte)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt64)lhs % (Single)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt64)lhs % (UInt16)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt64)lhs % (UInt32)rhs);
            RemOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt64)lhs % (UInt64)rhs);

            #endregion

            #region DivOperator

            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Byte)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Byte)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Byte)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Byte)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Byte)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Byte)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Char)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Char)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Char)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Char)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Char)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Char)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Decimal)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Decimal)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Decimal)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Decimal)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Decimal)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Decimal)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Decimal)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Decimal)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Decimal)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Decimal)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Double)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Double)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Double)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Double)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Double)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Double)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Double)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Double)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Double)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Double)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Double)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int16)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int16)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int16)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int16)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int16)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int16)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int16)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int32)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int32)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int32)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int32)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int32)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int32)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int32)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int64)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int64)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int64)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int64)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int64)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int64)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int64)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (SByte)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (SByte)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (SByte)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (SByte)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (SByte)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (SByte)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(SByte)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Single)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Single)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Single)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Single)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Single)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Single)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Single)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Single)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Single)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Single)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Single)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt16)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt16)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt16)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt16)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt16)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt16)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt16)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt16)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt16)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt16)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt32)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt32)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt32)lhs / (Int16)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt32)lhs / (Int32)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt32)lhs / (Int64)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt32)lhs / (SByte)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt32)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt32)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt32)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt32)lhs / (UInt64)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs / (Byte)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs / (Char)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt64)lhs / (Decimal)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt64)lhs / (Double)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs / (UInt64)(Int16)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs / (UInt64)(Int32)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs / (UInt64)(Int64)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs / (UInt64)(SByte)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt64)lhs / (Single)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt64)lhs / (UInt16)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt64)lhs / (UInt32)rhs);
            DivOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt64)lhs / (UInt64)rhs);

            #endregion

            #region MulOperator

            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Byte)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Byte)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Byte)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Byte)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Byte)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Byte)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Char)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Char)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Char)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Char)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Char)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Char)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Decimal)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Decimal)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Decimal)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Decimal)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Decimal)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Decimal)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Decimal)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Decimal)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Decimal)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Decimal)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Double)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Double)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Double)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Double)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Double)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Double)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Double)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Double)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Double)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Double)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Double)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int16)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int16)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int16)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int16)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int16)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int16)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int16)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int32)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int32)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int32)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int32)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int32)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int32)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int32)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Int64)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int64)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int64)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int64)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int64)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int64)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int64)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (SByte)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (SByte)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (SByte)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (SByte)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (SByte)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (SByte)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(SByte)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Single)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Single)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Single)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Single)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Single)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Single)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Single)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Single)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Single)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Single)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Single)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt16)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt16)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt16)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt16)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt16)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt16)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt16)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt16)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt16)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt16)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs * (Char)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt32)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt32)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt32)lhs * (Int16)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt32)lhs * (Int32)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt32)lhs * (Int64)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt32)lhs * (SByte)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt32)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt32)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt32)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt32)lhs * (UInt64)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs * (Byte)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs * (Char)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt64)lhs * (Decimal)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (UInt64)lhs * (Double)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs * (UInt64)(Int16)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs * (UInt64)(Int32)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs * (UInt64)(Int64)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs * (UInt64)(SByte)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (UInt64)lhs * (Single)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (UInt64)lhs * (UInt16)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (UInt64)lhs * (UInt32)rhs);
            MulOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (UInt64)lhs * (UInt64)rhs);

            #endregion

            #region SubOperator

            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Byte)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Byte)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Byte)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Byte)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Byte)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Byte)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Char)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Char)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Char)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Char)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Char)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Char)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Decimal)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Decimal)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Decimal)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (Decimal)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (Decimal)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Decimal)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (Decimal)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Decimal)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Decimal)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Decimal)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Double)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Double)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (Double)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Double)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Double)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Double)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Double)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (Double)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Double)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Double)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Double)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int16)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int16)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int16)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int16)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int16)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int16)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int16)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int32)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int32)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int32)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int32)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int32)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int32)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int32)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int64)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int64)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int64)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int64)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int64)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int64)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int64)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (SByte)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (SByte)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (SByte)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (SByte)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (SByte)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (SByte)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(SByte)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Single)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Single)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (Single)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Single)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Single)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Single)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Single)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (Single)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Single)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Single)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Single)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt16)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt16)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt16)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt16)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt16)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt16)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt16)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt16)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt16)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt16)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs - (Char)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt32)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt32)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt32)lhs - (Int16)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt32)lhs - (Int32)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt32)lhs - (Int64)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt32)lhs - (SByte)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt32)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt32)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt32)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt32)lhs - (UInt64)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs - (Byte)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs - (Char)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt64)lhs - (Decimal)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt64)lhs - (Double)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt64)(Int16)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt64)(Int32)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt64)(Int64)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt64)(SByte)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt64)lhs - (Single)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt16)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt32)rhs);
            SubOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)lhs - (UInt64)rhs);

            #endregion

            #region AddOperator

            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Byte)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Byte)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Byte)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Byte)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Byte)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Byte)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Decimal)] = ((lhs, rhs) => (Char)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Char)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Char)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Char)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Char)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = ((lhs, rhs) => (Char)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Decimal)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Decimal)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Decimal)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (Decimal)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (Decimal)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Decimal)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (Decimal)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Decimal)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Decimal)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Decimal * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Decimal)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Double)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Double)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (Double)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Double)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Double)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Double)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Double)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (Double)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Double)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Double)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Double * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Double)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int16)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int16)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int16)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int16)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int16)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int16)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int16)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int32)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int32)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int32)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int32)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int32)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int32)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int32)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (Int64)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (Int64)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Int64)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (Int64)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Int64)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (Int64)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(Int64)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (SByte)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Double)] = ((lhs, rhs) => (SByte)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (SByte)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Single)] = ((lhs, rhs) => (SByte)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (SByte)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = ((lhs, rhs) => (SByte)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)(SByte)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Single)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Single)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.Single * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (Single)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Single)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Single)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (Single)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Single)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (Single)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Single)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Single)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.Single * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Single)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt16)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt16)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt16)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt16)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt16)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt16)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt16)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt16)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt16)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt16)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs + (Char)rhs);
            DivOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.DateTime)] =
                ((lhs, rhs) => new InvalidCastException());
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt32)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt32)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (UInt32)lhs + (Int16)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (UInt32)lhs + (Int32)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = ((lhs, rhs) => (UInt32)lhs + (Int64)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (UInt32)lhs + (SByte)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt32)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt32)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt32)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt32)lhs + (UInt64)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs + (Byte)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs + (Char)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Decimal)] =
                ((lhs, rhs) => (UInt64)lhs + (Decimal)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Double)] =
                ((lhs, rhs) => (UInt64)lhs + (Double)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt64)(Int16)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt64)(Int32)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt64)(Int64)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt64)(SByte)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Single)] =
                ((lhs, rhs) => (UInt64)lhs + (Single)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt16)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt32)rhs);
            AddOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)lhs + (UInt64)rhs);

            #endregion

            #region AndOperator

            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Byte)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Byte)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Byte)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Byte)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Byte)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Byte)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Byte)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Byte)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Byte)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Char)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Char)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Char)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Char)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Char)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Char)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Char)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Char)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Char)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int16)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int16)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int16)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int16)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int16)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int16)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int16)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int16)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int16)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int32)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int32)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int32)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int32)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int32)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int32)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int32)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int32)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int32)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int64)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int64)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int64)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int64)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int64)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int64)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int64)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int64)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int64)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (SByte)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (SByte)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (SByte)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (SByte)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (SByte)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (SByte)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (SByte)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (SByte)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(SByte)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt16)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt16)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt16)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt16)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt16)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt16)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt16)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt16)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt16)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt32)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt32)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt32)lhs & (Int16)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt32)lhs & (Int32)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt32)lhs & (Int64)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt32)lhs & (SByte)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt32)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt32)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt32)lhs & (UInt64)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt64)lhs & (Byte)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt64)lhs & (Char)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                (lhs, rhs) => (UInt64)lhs & (UInt64)(Int16)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                (lhs, rhs) => (UInt64)lhs & (UInt64)(Int32)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                (lhs, rhs) => (UInt64)lhs & (UInt64)(Int64)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                (lhs, rhs) => (UInt64)lhs & (UInt64)(SByte)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt64)lhs & (UInt16)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt64)lhs & (UInt32)rhs;
            AndOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt64)lhs & (UInt64)rhs;

            #endregion

            #region OrOperator

            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Byte)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Byte)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Byte)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Byte)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Byte)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Byte)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Byte)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Byte)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Byte)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Char)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Char)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Char)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Char)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Char)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Char)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Char)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Char)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Char)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int16)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int16)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int16)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int16)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int16)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int16)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int16)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int16)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int16)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int32)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int32)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int32)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int32)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int32)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int32)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int32)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int32)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int32)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int64)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int64)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int64)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int64)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int64)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int64)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int64)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int64)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int64)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (SByte)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (SByte)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (SByte)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (SByte)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (SByte)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (SByte)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (SByte)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (SByte)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(SByte)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt16)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt16)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt16)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt16)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt16)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt16)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt16)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt16)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt16)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt32)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt32)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt32)lhs | (Int16)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt32)lhs | (Int32)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt32)lhs | (Int64)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt32)lhs | (SByte)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt32)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt32)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt32)lhs | (UInt64)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt64)lhs | (Byte)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt64)lhs | (Char)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                (lhs, rhs) => (UInt64)lhs | (UInt64)(Int16)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                (lhs, rhs) => (UInt64)lhs | (UInt64)(Int32)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                (lhs, rhs) => (UInt64)lhs | (UInt64)(Int64)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                (lhs, rhs) => (UInt64)lhs | (UInt64)(SByte)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt64)lhs | (UInt16)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt64)lhs | (UInt32)rhs;
            OrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt64)lhs | (UInt64)rhs;

            #endregion

            #region XorOperator

            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Byte)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Byte)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Byte)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Byte)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Byte)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Byte)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Byte)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Byte)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Byte)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Char)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Char)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Char)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Char)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Char)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Char)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Char)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Char)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (Char)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int16)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int16)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int16)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int16)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int16)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int16)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int16)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int16)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int16)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int32)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int32)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int32)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int32)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int32)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int32)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int32)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int32)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int32)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (Int64)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (Int64)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (Int64)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (Int64)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (Int64)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (Int64)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (Int64)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (Int64)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(Int64)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (SByte)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (SByte)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (SByte)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (SByte)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (SByte)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (SByte)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (SByte)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (SByte)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                (lhs, rhs) => (UInt64)(SByte)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt16)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt16)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt16)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt16)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt16)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt16)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt16)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt16)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt16)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt32)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt32)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] = (lhs, rhs) => (UInt32)lhs ^ (Int16)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] = (lhs, rhs) => (UInt32)lhs ^ (Int32)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] = (lhs, rhs) => (UInt32)lhs ^ (Int64)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] = (lhs, rhs) => (UInt32)lhs ^ (SByte)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt32)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt32)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt32)lhs ^ (UInt64)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = (lhs, rhs) => (UInt64)lhs ^ (Byte)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = (lhs, rhs) => (UInt64)lhs ^ (Char)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                (lhs, rhs) => (UInt64)lhs ^ (UInt64)(Int16)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                (lhs, rhs) => (UInt64)lhs ^ (UInt64)(Int32)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                (lhs, rhs) => (UInt64)lhs ^ (UInt64)(Int64)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                (lhs, rhs) => (UInt64)lhs ^ (UInt64)(SByte)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] = (lhs, rhs) => (UInt64)lhs ^ (UInt16)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] = (lhs, rhs) => (UInt64)lhs ^ (UInt32)rhs;
            XorOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] = (lhs, rhs) => (UInt64)lhs ^ (UInt64)rhs;

            #endregion

            #region ShlOperator

            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int16)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int32)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int64)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (SByte)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt16)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt16)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt16)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt32)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt32)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt32)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(UInt64)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs << (Byte)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs << (Char)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs << (Int16)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(Int64)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs << (SByte)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt64)lhs << (UInt16)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(UInt32)rhs);
            ShlOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(UInt64)rhs);

            #endregion

            #region ShrOperator

            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Byte)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Byte)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Byte)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Byte)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Byte)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Byte)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.Byte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Byte)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Char)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Char)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Char)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Char)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Char)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt16)] = ((lhs, rhs) => (Char)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.Char * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Char)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int16)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int16)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int16)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int16)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int16)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int16)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.Int16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int16)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int32)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int32)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int32)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int32)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int32)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int32)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.Int32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int32)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (Int64)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (Int64)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (Int64)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (Int64)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (Int64)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (Int64)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.Int64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (Int64)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (SByte)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (SByte)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int16)] = ((lhs, rhs) => (SByte)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int32)] = ((lhs, rhs) => (SByte)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.SByte)] = ((lhs, rhs) => (SByte)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (SByte)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.SByte * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (SByte)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt16)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt16)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt16)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt16)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt16)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.UInt16 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt16)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt32)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt32)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt32)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt32)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt32)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.UInt32 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt32)lhs << (Int32)(UInt64)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Byte)] = ((lhs, rhs) => (UInt64)lhs << (Byte)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Char)] = ((lhs, rhs) => (UInt64)lhs << (Char)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int16)] =
                ((lhs, rhs) => (UInt64)lhs << (Int16)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int32)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.Int64)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(Int64)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.SByte)] =
                ((lhs, rhs) => (UInt64)lhs << (SByte)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt16)] =
                ((lhs, rhs) => (UInt64)lhs << (UInt16)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt32)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(UInt32)rhs);
            ShrOperators[((int)TypeCode.UInt64 * 18 + (int)TypeCode.UInt64)] =
                ((lhs, rhs) => (UInt64)lhs << (Int32)(UInt64)rhs);

            #endregion
        }

        public static bool Equal(object lhs, object rhs)
        {
            if (lhs is VBox)
            {
                if (rhs == null)
                {
                    return false;
                }

                lhs = (lhs as VBox).BoxDefine();
            }

            if (rhs is VBox)
            {
                if (lhs == null)
                {
                    return false;
                }

                rhs = (rhs as VBox).BoxDefine();
            }

            if (lhs is Enum || rhs is Enum)
            {
                return (int)lhs == (int)rhs;
            }

            if (lhs is IComparable)
            {
                if (rhs == null)
                {
                    return false;
                }

                return (lhs as IComparable).CompareTo(rhs.Convert(lhs.GetType())) == 0;
            }
            else if (rhs is IComparable)
            {
                if (lhs == null)
                {
                    return false;
                }

                return (rhs as IComparable).CompareTo(lhs.Convert(rhs.GetType())) == 0;
            }

            if (lhs != null)
            {
                return lhs.Equals(rhs);
            }

            if (rhs != null)
            {
                return rhs.Equals(lhs);
            }

            // (lhs == null && rhs == null)
            return true;
        }

        public static bool Less(object lhs, object rhs)
        {
            if (lhs is IComparable)
            {
                if (rhs == null)
                {
                    return false;
                }

                return (lhs as IComparable).CompareTo(rhs.Convert(lhs.GetType())) < 0;
            }

            if (lhs == null)
            {
                if (rhs != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            throw new ArithmeticException();
        }

        public static bool Greater(object lhs, object rhs)
        {
            if (lhs is IComparable)
            {
                if (rhs == null)
                {
                    return true;
                }

                return (lhs as IComparable).CompareTo(rhs.Convert(lhs.GetType())) > 0;
            }

            if (lhs == null)
            {
                return false;
            }

            throw new ArithmeticException();
        }

        public static bool LessEqual(object lhs, object rhs)
        {
            if (lhs is IComparable)
            {
                if (rhs == null)
                {
                    return false;
                }

                return (lhs as IComparable).CompareTo(rhs.Convert(lhs.GetType())) <= 0;
            }

            if (lhs == null && rhs == null)
            {
                return true;
            }

            throw new ArithmeticException();
        }

        public static bool GreaterEqual(object lhs, object rhs)
        {
            if (lhs is IComparable)
            {
                if (rhs == null)
                {
                    return false;
                }

                return (lhs as IComparable).CompareTo(rhs.Convert(lhs.GetType())) >= 0;
            }

            if (lhs == null && rhs == null)
            {
                return true;
            }

            throw new ArithmeticException();
        }

        public static object Rem(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = RemOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();
        }

        public static object Div(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = DivOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Mul(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = MulOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Sub(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = SubOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Add(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = AddOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object And(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = AndOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Or(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;

            var op = OrOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();
        }

        public static object Xor(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = XorOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Shl(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = ShlOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Shr(object lhs, object rhs, CodeBody.OpCode opcode)
        {
            var ltype = (int)Type.GetTypeCode(lhs.GetType());
            var rtype = (int)Type.GetTypeCode(rhs.GetType());

            var v = (int)ltype * 18 + (int)rtype;
            var op = ShrOperators[v];
            if (op != null)
            {
                opcode.binaryOperation = op;
                return op(lhs, rhs);
            }

            throw new InvalidCastException();

        }

        public static object Not(object lhs)
        {
            var ltype = Type.GetTypeCode(lhs.GetType());
            switch (ltype)
            {
                case TypeCode.Char:
                    return ~(Char)lhs;
                case TypeCode.SByte:
                    return ~(SByte)lhs;
                case TypeCode.Byte:
                    return ~(Byte)lhs;
                case TypeCode.Int16:
                    return ~(Int16)lhs;
                case TypeCode.UInt16:
                    return ~(UInt16)lhs;
                case TypeCode.Int32:
                    return ~(Int32)lhs;
                case TypeCode.UInt32:
                    return ~(UInt32)lhs;
                case TypeCode.Int64:
                    return ~(Int64)lhs;
                case TypeCode.UInt64:
                    return ~(UInt64)lhs;
                default:
                    throw new InvalidCastException();
            }
        }
    }
}