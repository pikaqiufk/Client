// file     : ObjectExtension.cs
// project  : UniversalTypeConverter
// author   : Thorsten Bruning
// date     : 28.07.2011

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TB.ComponentModel {

    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ObjectExtension {

        #region [ CanConvertTo<T> ]
        /// <summary>
        /// Determines whether the value can be converted to the specified type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <typeparam name="T">The Type to test.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
        public static bool CanConvertTo<T>(this object value) {
            return UniversalTypeConverter.CanConvertTo<T>(value);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <typeparam name="T">The Type to test.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
        public static bool CanConvertTo<T>(this object value, CultureInfo culture) {
            return UniversalTypeConverter.CanConvertTo<T>(value, culture);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <typeparam name="T">The Type to test.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
        public static bool CanConvertTo<T>(this object value, ConversionOptions options) {
            return UniversalTypeConverter.CanConvertTo<T>(value, options);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <typeparam name="T">The Type to test.</typeparam>
        /// <param name="value">The value to test.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <typeparamref name="T"/>; otherwise, false.</returns>
        public static bool CanConvertTo<T>(this object value, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.CanConvertTo<T>(value, culture, options);
        }
        #endregion

        #region [ TryConvertTo<T> ]
        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvertTo<T>(this object value, out T result) {
            return UniversalTypeConverter.TryConvertTo(value, out result);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvertTo<T>(this object value, out T result, CultureInfo culture) {
            return UniversalTypeConverter.TryConvertTo(value, out result, culture);
        }

        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvertTo<T>(this object value, out T result, ConversionOptions options) {
            return UniversalTypeConverter.TryConvertTo(value, out result, options);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="result">An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvertTo<T>(this object value, out T result, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.TryConvertTo(value, out result, culture, options);
        }
        #endregion

        #region [ ConvertTo<T> ]
        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static T ConvertTo<T>(this object value) {
            return UniversalTypeConverter.ConvertTo<T>(value);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static T ConvertTo<T>(this object value, CultureInfo culture) {
            return UniversalTypeConverter.ConvertTo<T>(value, culture);
        }

        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static T ConvertTo<T>(this object value, ConversionOptions options) {
            return UniversalTypeConverter.ConvertTo<T>(value, options);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <typeparam name="T">The Type to which the given value is converted.</typeparam>
        /// <param name="value">The value which is converted.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>An Object instance of type <typeparamref name="T">T</typeparamref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static T ConvertTo<T>(this object value, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.ConvertTo<T>(value, culture, options);
        }
        #endregion

        #region [ CanConvert ]
        /// <summary>
        /// Determines whether the value can be converted to the specified type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="destinationType">The Type to test.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <paramref name="destinationType"/>; otherwise, false.</returns>
        public static bool CanConvert(this object value, Type destinationType) {
            return UniversalTypeConverter.CanConvert(value, destinationType);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="destinationType">The Type to test.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <paramref name="destinationType"/>; otherwise, false.</returns>
        public static bool CanConvert(this object value, Type destinationType, CultureInfo culture) {
            return UniversalTypeConverter.CanConvert(value, destinationType, culture);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="destinationType">The Type to test.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <paramref name="destinationType"/>; otherwise, false.</returns>
        public static bool CanConvert(this object value, Type destinationType, ConversionOptions options) {
            return UniversalTypeConverter.CanConvert(value, destinationType, options);
        }

        /// <summary>
        /// Determines whether the value can be converted to the specified type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="destinationType">The Type to test.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> can be converted to <paramref name="destinationType"/>; otherwise, false.</returns>
        public static bool CanConvert(this object value, Type destinationType, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.CanConvert(value, destinationType, culture, options);
        }
        #endregion

        #region [ TryConvert ]
        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(this object value, Type destinationType, out object result) {
            return UniversalTypeConverter.TryConvert(value, destinationType, out result);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(this object value, Type destinationType, out object result, CultureInfo culture) {
            return UniversalTypeConverter.TryConvert(value, destinationType, out result, culture);
        }

        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(this object value, Type destinationType, out object result, ConversionOptions options) {
            return UniversalTypeConverter.TryConvert(value, destinationType, out result, options);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(this object value, Type destinationType, out object result, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.TryConvert(value, destinationType, out result, culture, options);
        }
        #endregion

        #region [ Convert ]


        private static int Length;
        private static List<Func<object, object>> Converter;

//         static ObjectExtension()
//         {
//             Length = Enum.GetValues(typeof (TypeCode)).Length;
//             var l = Length * Length;
//             Converter = new List<Func<object, object>>(l);
//             for (int i = 0; i < l; i++)
//             {
//                 Converter.Add(null);
//             }
// 
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) ((Boolean) value ? 1 : 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Boolean)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) ((Boolean) value ? 1 : 0);
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => ((Byte) value != 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Byte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Byte)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Byte) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => ((Char) value != 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Char) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Char)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Char) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => ((DateTime) value).ToBinary();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(DateTime)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) ((DateTime) value).ToBinary();
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => ((Decimal) value != 0);                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Decimal) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Decimal)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Decimal) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (Double) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Double) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Double)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Double) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (Int16)value != 0;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32)(Int16)value;
//             Converter[((int)Type.GetTypeCode(typeof(Int16)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64)(Int16)value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (Int32) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Int32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int32)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Int32) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (Int64) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => DateTime.FromBinary((Int64) value);                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Int64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Int64)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Int64) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (SByte) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (SByte) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(SByte)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (SByte) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (Single) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (Single) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(Single)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (Single) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (UInt16) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (UInt16) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt16)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (UInt16) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (UInt32) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => new InvalidCastException();                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (UInt32) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt32)) * Length + (int)Type.GetTypeCode(typeof(UInt64)))] = value => (UInt64) (UInt32) value;
// 
// 
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Boolean)))] = value => (UInt64) value != 0;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Byte)))] = value => (Byte) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Char)))] = value => (Char) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(DateTime)))] = value => DateTime.FromBinary((Int64) (UInt64) value);                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Decimal)))] = value => (Decimal) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Double)))] = value => (Double) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Int16)))] = value => (Int16) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Int32)))] = value => (Int32) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Int64)))] = value => (Int64) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(SByte)))] = value => (SByte) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(Single)))] = value => (Single) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(UInt16)))] = value => (UInt16) (UInt64) value;                    
//             Converter[((int)Type.GetTypeCode(typeof(UInt64)) * Length + (int)Type.GetTypeCode(typeof(UInt32)))] = value => (UInt32) (UInt64) value;                    
//         }

        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <returns>An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static object Convert(this object value, Type destinationType)
        {
            if (value == null)
            {
                return null;
            }

            var t = value.GetType();

            if (t == destinationType)
            {
                return value;
            }

            if (destinationType.IsEnum)
            {
                return Enum.ToObject(destinationType, value);
            }

            var ltype = Type.GetTypeCode(t);
            var rtype = Type.GetTypeCode(destinationType);

            var v = (int)ltype * 20 + (int)rtype;

            switch (v)
            {
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Byte): return (Byte)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Char): return (Char)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Decimal): return (Decimal)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Double): return (Double)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Int16): return (Int16)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Int32): return (Int32)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Int64): return (Int64)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.SByte): return (SByte)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.Single): return (Single)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.UInt16): return (UInt16)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.UInt32): return (UInt32)((Boolean)value ? 1 : 0);
                case ((int)TypeCode.Boolean * 20 + (int)TypeCode.UInt64): return (UInt64)((Boolean)value ? 1 : 0);


                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Boolean): return ((Byte)value != 0);
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Char): return (Char)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Decimal): return (Decimal)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Double): return (Double)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Int16): return (Int16)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Int32): return (Int32)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Int64): return (Int64)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.SByte): return (SByte)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.Single): return (Single)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.UInt16): return (UInt16)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.UInt32): return (UInt32)(Byte)value;
                case ((int)TypeCode.Byte * 20 + (int)TypeCode.UInt64): return (UInt64)(Byte)value;


                case ((int)TypeCode.Char * 20 + (int)TypeCode.Boolean): return ((Char)value != 0);
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Byte): return (Byte)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Decimal): return (Decimal)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Double): return (Double)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Int16): return (Int16)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Int32): return (Int32)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Int64): return (Int64)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.SByte): return (SByte)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.Single): return (Single)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.UInt16): return (UInt16)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.UInt32): return (UInt32)(Char)value;
                case ((int)TypeCode.Char * 20 + (int)TypeCode.UInt64): return (UInt64)(Char)value;


                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Boolean): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Byte): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Char): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Decimal): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Double): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Int16): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Int32): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Int64): return ((DateTime)value).ToBinary();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.SByte): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.Single): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.UInt16): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.UInt32): return new InvalidCastException();
                case ((int)TypeCode.DateTime * 20 + (int)TypeCode.UInt64): return (UInt64)((DateTime)value).ToBinary();


                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Boolean): return ((Decimal)value != 0);
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Byte): return (Byte)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Char): return (Char)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Double): return (Double)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Int16): return (Int16)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Int32): return (Int32)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Int64): return (Int64)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.SByte): return (SByte)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.Single): return (Single)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.UInt16): return (UInt16)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.UInt32): return (UInt32)(Decimal)value;
                case ((int)TypeCode.Decimal * 20 + (int)TypeCode.UInt64): return (UInt64)(Decimal)value;


                case ((int)TypeCode.Double * 20 + (int)TypeCode.Boolean): return (Double)value != 0;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Byte): return (Byte)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Char): return (Char)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Decimal): return (Decimal)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Int16): return (Int16)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Int32): return (Int32)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Int64): return (Int64)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.SByte): return (SByte)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.Single): return (Single)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.UInt16): return (UInt16)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.UInt32): return (UInt32)(Double)value;
                case ((int)TypeCode.Double * 20 + (int)TypeCode.UInt64): return (UInt64)(Double)value;


                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Boolean): return (Int16)value != 0;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Byte): return (Byte)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Char): return (Char)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Decimal): return (Decimal)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Double): return (Double)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Int32): return (Int32)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Int64): return (Int64)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.SByte): return (SByte)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.Single): return (Single)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.UInt16): return (UInt16)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.UInt32): return (UInt32)(Int16)value;
                case ((int)TypeCode.Int16 * 20 + (int)TypeCode.UInt64): return (UInt64)(Int16)value;


                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Boolean): return (Int32)value != 0;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Byte): return (Byte)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Char): return (Char)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Decimal): return (Decimal)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Double): return (Double)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Int16): return (Int16)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Int64): return (Int64)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.SByte): return (SByte)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.Single): return (Single)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.UInt16): return (UInt16)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.UInt32): return (UInt32)(Int32)value;
                case ((int)TypeCode.Int32 * 20 + (int)TypeCode.UInt64): return (UInt64)(Int32)value;


                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Boolean): return (Int64)value != 0;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Byte): return (Byte)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Char): return (Char)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.DateTime): return DateTime.FromBinary((Int64)value);
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Decimal): return (Decimal)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Double): return (Double)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Int16): return (Int16)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Int32): return (Int32)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.SByte): return (SByte)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.Single): return (Single)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.UInt16): return (UInt16)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.UInt32): return (UInt32)(Int64)value;
                case ((int)TypeCode.Int64 * 20 + (int)TypeCode.UInt64): return (UInt64)(Int64)value;


                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Boolean): return (SByte)value != 0;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Byte): return (Byte)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Char): return (Char)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Decimal): return (Decimal)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Double): return (Double)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Int16): return (Int16)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Int32): return (Int32)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Int64): return (Int64)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.Single): return (Single)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.UInt16): return (UInt16)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.UInt32): return (UInt32)(SByte)value;
                case ((int)TypeCode.SByte * 20 + (int)TypeCode.UInt64): return (UInt64)(SByte)value;


                case ((int)TypeCode.Single * 20 + (int)TypeCode.Boolean): return (Single)value != 0;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Byte): return (Byte)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Char): return (Char)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Decimal): return (Decimal)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Double): return (Double)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Int16): return (Int16)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Int32): return (Int32)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.Int64): return (Int64)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.SByte): return (SByte)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.UInt16): return (UInt16)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.UInt32): return (UInt32)(Single)value;
                case ((int)TypeCode.Single * 20 + (int)TypeCode.UInt64): return (UInt64)(Single)value;


                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Boolean): return (UInt16)value != 0;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Byte): return (Byte)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Char): return (Char)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Decimal): return (Decimal)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Double): return (Double)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Int16): return (Int16)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Int32): return (Int32)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Int64): return (Int64)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.SByte): return (SByte)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.Single): return (Single)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.UInt32): return (UInt32)(UInt16)value;
                case ((int)TypeCode.UInt16 * 20 + (int)TypeCode.UInt64): return (UInt64)(UInt16)value;


                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Boolean): return (UInt32)value != 0;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Byte): return (Byte)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Char): return (Char)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.DateTime): return new InvalidCastException();
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Decimal): return (Decimal)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Double): return (Double)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Int16): return (Int16)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Int32): return (Int32)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Int64): return (Int64)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.SByte): return (SByte)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.Single): return (Single)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.UInt16): return (UInt16)(UInt32)value;
                case ((int)TypeCode.UInt32 * 20 + (int)TypeCode.UInt64): return (UInt64)(UInt32)value;


                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Boolean): return (UInt64)value != 0;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Byte): return (Byte)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Char): return (Char)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.DateTime): return DateTime.FromBinary((Int64)(UInt64)value);
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Decimal): return (Decimal)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Double): return (Double)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Int16): return (Int16)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Int32): return (Int32)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Int64): return (Int64)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.SByte): return (SByte)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.Single): return (Single)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.UInt16): return (UInt16)(UInt64)value;
                case ((int)TypeCode.UInt64 * 20 + (int)TypeCode.UInt32): return (UInt32)(UInt64)value;   
 
                default:
                    try
                    {
                        return UniversalTypeConverter.Convert(value, destinationType);
                    }
                    catch (Exception)
                    {
                        return value;
                    }
            }
        }


        public static object Convert<T>(this object value)
        {
            Type destinationType = typeof (T);
            return value.Convert(destinationType);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the <see cref="ConversionOptions">ConversionOptions</see>.<see cref="ConversionOptions.EnhancedTypicalValues">ConvertSpecialValues</see>.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static object Convert(this object value, Type destinationType, CultureInfo culture) {
            return UniversalTypeConverter.Convert(value, destinationType, culture);
        }

        /// <summary>
        /// Converts the value to the given Type using the current CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static object Convert(this object value, Type destinationType, ConversionOptions options) {
            return UniversalTypeConverter.Convert(value, destinationType, options);
        }

        /// <summary>
        /// Converts the value to the given Type using the given CultureInfo and the given <see cref="ConversionOptions">ConversionOptions</see>.
        /// </summary>
        /// <param name="value">The value which is converted.</param>
        /// <param name="destinationType">The Type to which the given value is converted.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="options">The options which are used for conversion.</param>
        /// <returns>An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref>.</returns>
        public static object Convert(this object value, Type destinationType, CultureInfo culture, ConversionOptions options) {
            return UniversalTypeConverter.Convert(value, destinationType, culture, options);
        }
        #endregion

        #region [ ConvertToEnumerable<T> ]
        /// <summary>
        /// Converts all elements of the list to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <typeparam name="T">The type to which the values are converted.</typeparam>
        /// <param name="values">The list of values which are converted.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableConversion<T> ConvertToEnumerable<T>(this IEnumerable values) {
            return UniversalTypeConverter.ConvertToEnumerable<T>(values);
        }

        /// <summary>
        /// Splits the string by using the semicolon (;) as a seperator and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <typeparam name="T">The type to which the values are converted.</typeparam>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<T> ConvertToEnumerable<T>(this string valueList) {
            return UniversalTypeConverter.ConvertToEnumerable<T>(valueList);
        }

        /// <summary>
        /// Splits the string by using the given seperator and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <typeparam name="T">The type to which the values are converted.</typeparam>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <param name="seperator">The value seperator which is used in <paramref name="valueList">valueList</paramref>.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<T> ConvertToEnumerable<T>(this string valueList, string seperator) {
            return UniversalTypeConverter.ConvertToEnumerable<T>(valueList, seperator);
        }

        /// <summary>
        /// Splits the string by using the given splitter and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <typeparam name="T">The type to which the values are converted.</typeparam>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <param name="stringSplitter">The splitter to use.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<T> ConvertToEnumerable<T>(this string valueList, IStringSplitter stringSplitter) {
            return UniversalTypeConverter.ConvertToEnumerable<T>(valueList, stringSplitter);
        }
        #endregion

        #region [ ConvertToEnumerable ]
        /// <summary>
        /// Converts all elements of the list to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <param name="values">The list of values which are converted.</param>
        /// <param name="destinationType">The type to which the values are converted.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableConversion<object> ConvertToEnumerable(this IEnumerable values, Type destinationType) {
            return UniversalTypeConverter.ConvertToEnumerable(values, destinationType);
        }

        /// <summary>
        /// Splits the string by using the semicolon (;) as a seperator and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <param name="destinationType">The type to which the values are converted.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<object> ConvertToEnumerable(this string valueList, Type destinationType) {
            return UniversalTypeConverter.ConvertToEnumerable(valueList, destinationType);
        }

        /// <summary>
        /// Splits the string by using the given seperator and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <param name="destinationType">The type to which the values are converted.</param>
        /// <param name="seperator">The value seperator which is used in <paramref name="valueList">valueList</paramref>.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<object> ConvertToEnumerable(this string valueList, Type destinationType, string seperator) {
            return UniversalTypeConverter.ConvertToEnumerable(valueList, destinationType, seperator);
        }

        /// <summary>
        /// Splits the string by using the given splitter and converts all elements of the result to the given type.
        /// The result is configurable further more before first iteration.
        /// </summary>
        /// <param name="valueList">The string representation of the list of values to convert.</param>
        /// <param name="destinationType">The type to which the values are converted.</param>
        /// <param name="stringSplitter">The splitter to use.</param>
        /// <returns>List of converted values.</returns>
        public static UniversalTypeConverter.EnumerableStringConversion<object> ConvertToEnumerable(this string valueList, Type destinationType, IStringSplitter stringSplitter) {
            return UniversalTypeConverter.ConvertToEnumerable(valueList, destinationType, stringSplitter);
        }
        #endregion

        #region [ ConvertToStringRepresentation ]
        /// <summary>
        /// Converts the list to a semicolon seperated string.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values);
        }

        /// <summary>
        /// Converts the list to a string where all values a seperated by the given seperator.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <param name="seperator">Seperator.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values, string seperator) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values, seperator);
        }

        /// <summary>
        /// Converts the list to a string where all values a seperated by the given seperator.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <param name="seperator">Seperator.</param>
        /// <param name="nullValue">The string which is used for null values.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values, string seperator, string nullValue) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values, seperator, nullValue);
        }

        /// <summary>
        /// Converts the list to a semicolon seperated string.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values, CultureInfo culture) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values, culture);
        }

        /// <summary>
        /// Converts the list to a string.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <param name="stringConcatenator">The concatenator which is used to build the string.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values, IStringConcatenator stringConcatenator) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values, stringConcatenator);
        }

        /// <summary>
        /// Converts the list to a string.
        /// </summary>
        /// <param name="values">Values to convert to string.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <param name="stringConcatenator">The concatenator which is used to build the string.</param>
        /// <returns>String representation of the given value list.</returns>
        public static string ConvertToStringRepresentation(this IEnumerable values, CultureInfo culture, IStringConcatenator stringConcatenator) {
            return UniversalTypeConverter.ConvertToStringRepresentation(values, culture, stringConcatenator);
        }
        #endregion
    }
}
