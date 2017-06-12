/*
 Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

using System;
using System.Diagnostics;
using SCG = System.Collections.Generic;
namespace C5
{
  /// <summary>
  /// An entry in a dictionary from K to V.
  /// </summary>
  [Serializable]
  public struct KeyValuePair<K, V> : IEquatable<KeyValuePair<K, V>>, IShowable
  {
    /// <summary>
    /// The key field of the entry
    /// </summary>
    public K Key;

    /// <summary>
    /// The value field of the entry
    /// </summary>
    public V Value;

    /// <summary>
    /// Create an entry with specified key and value
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    public KeyValuePair(K key, V value) { Key = key; Value = value; }


    /// <summary>
    /// Create an entry with a specified key. The value will be the default value of type <code>V</code>.
    /// </summary>
    /// <param name="key">The key</param>
    public KeyValuePair(K key) { Key = key; Value = default(V); }


    /// <summary>
    /// Pretty print an entry
    /// </summary>
    /// <returns>(key, value)</returns>
    [Tested]
    public override string ToString() { return "(" + Key + ", " + Value + ")"; }


    /// <summary>
    /// Check equality of entries. 
    /// </summary>
    /// <param name="obj">The other object</param>
    /// <returns>True if obj is an entry of the same type and has the same key and value</returns>
    [Tested]
    public override bool Equals(object obj)
    {
      if (!(obj is KeyValuePair<K, V>))
        return false;
      KeyValuePair<K, V> other = (KeyValuePair<K, V>)obj;
      return Equals(other);
    }


    /// <summary>
    /// Get the hash code of the pair.
    /// </summary>
    /// <returns>The hash code</returns>
    [Tested]
    public override int GetHashCode() { return EqualityComparer<K>.Default.GetHashCode(Key) + 13984681 * EqualityComparer<V>.Default.GetHashCode(Value); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(KeyValuePair<K, V> other)
    {
      return EqualityComparer<K>.Default.Equals(Key, other.Key) && EqualityComparer<V>.Default.Equals(Value, other.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair1"></param>
    /// <param name="pair2"></param>
    /// <returns></returns>
    public static bool operator ==(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2) { return pair1.Equals(pair2); }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pair1"></param>
    /// <param name="pair2"></param>
    /// <returns></returns>
    public static bool operator !=(KeyValuePair<K, V> pair1, KeyValuePair<K, V> pair2) { return !pair1.Equals(pair2); }

    #region IShowable Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stringbuilder"></param>
    /// <param name="formatProvider"></param>
    /// <param name="rest"></param>
    /// <returns></returns>
    public bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
    {
      if (rest < 0)
        return false;
      if (!Showing.Show(Key, stringbuilder, ref rest, formatProvider))
        return false;
      stringbuilder.Append(" => ");
      rest -= 4;
      if (!Showing.Show(Value, stringbuilder, ref rest, formatProvider))
        return false;
      return rest >= 0;
    }
    #endregion

    #region IFormattable Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="formatProvider"></param>
    /// <returns></returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Showing.ShowString(this, format, formatProvider);
    }

    #endregion
  }

}