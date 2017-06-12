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

#define IMPROVED_COLLECTION_HASHFUNCTION

using System;
using System.Diagnostics;
using SCG = System.Collections.Generic;
namespace C5
{
    /// <summary>
    /// A base class for implementing an IEnumerable&lt;T&gt;
    /// </summary>
    [Serializable]
    public abstract class EnumerableBase<T> : SCG.IEnumerable<T>
    {
        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public abstract SCG.IEnumerator<T> GetEnumerator();

        /// <summary>
        /// Count the number of items in an enumerable by enumeration
        /// </summary>
        /// <param name="items">The enumerable to count</param>
        /// <returns>The size of the enumerable</returns>
        [Tested]
        protected static int countItems(SCG.IEnumerable<T> items)
        {
            ICollectionValue<T> jtems = items as ICollectionValue<T>;

            if (jtems != null)
                return jtems.Count;

            int count = 0;

            using (SCG.IEnumerator<T> e = items.GetEnumerator())
                while (e.MoveNext()) count++;

            return count;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }


    /// <summary>
    /// Base class for classes implementing ICollectionValue[T]
    /// </summary>
    [Serializable]
    public abstract class CollectionValueBase<T> : EnumerableBase<T>, ICollectionValue<T>, IShowable
    {
        #region Event handling
        EventBlock<T> eventBlock;
        /// <summary>
        ///
        /// </summary>
        /// <value></value>
        public virtual EventTypeEnum ListenableEvents { get { return 0; } }

        /// <summary>
        /// A flag bitmap of the events currently subscribed to by this collection.
        /// </summary>
        /// <value></value>
        public virtual EventTypeEnum ActiveEvents { get { return eventBlock == null ? 0 : eventBlock.events; } }

        private void checkWillListen(EventTypeEnum eventType)
        {
            if ((ListenableEvents & eventType) == 0)
                throw new UnlistenableEventException();
        }

        /// <summary>
        /// The change event. Will be raised for every change operation on the collection.
        /// </summary>
        public virtual event CollectionChangedHandler<T> CollectionChanged
        {
            add { checkWillListen(EventTypeEnum.Changed); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionChanged += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Changed);
                if (eventBlock != null)
                {
                    eventBlock.CollectionChanged -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the CollectionChanged event
        /// </summary>
        protected virtual void raiseCollectionChanged()
        { if (eventBlock != null) eventBlock.raiseCollectionChanged(this); }

        /// <summary>
        /// The clear event. Will be raised for every Clear operation on the collection.
        /// </summary>
        public virtual event CollectionClearedHandler<T> CollectionCleared
        {
            add { checkWillListen(EventTypeEnum.Cleared); (eventBlock ?? (eventBlock = new EventBlock<T>())).CollectionCleared += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Cleared);
                if (eventBlock != null)
                {
                    eventBlock.CollectionCleared -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void raiseCollectionCleared(bool full, int count)
        { if (eventBlock != null) eventBlock.raiseCollectionCleared(this, full, count); }

        /// <summary>
        /// Fire the CollectionCleared event
        /// </summary>
        protected virtual void raiseCollectionCleared(bool full, int count, int? offset)
        { if (eventBlock != null) eventBlock.raiseCollectionCleared(this, full, count, offset); }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemsAddedHandler<T> ItemsAdded
        {
            add { checkWillListen(EventTypeEnum.Added); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsAdded += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Added);
                if (eventBlock != null)
                {
                    eventBlock.ItemsAdded -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemsAdded event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="count"></param>
        protected virtual void raiseItemsAdded(T item, int count)
        { if (eventBlock != null) eventBlock.raiseItemsAdded(this, item, count); }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemsRemovedHandler<T> ItemsRemoved
        {
            add { checkWillListen(EventTypeEnum.Removed); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemsRemoved += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Removed);
                if (eventBlock != null)
                {
                    eventBlock.ItemsRemoved -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemsRemoved event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="count"></param>
        protected virtual void raiseItemsRemoved(T item, int count)
        { if (eventBlock != null) eventBlock.raiseItemsRemoved(this, item, count); }

        /// <summary>
        /// The item added  event. Will be raised for every individual addition to the collection.
        /// </summary>
        public virtual event ItemInsertedHandler<T> ItemInserted
        {
            add { checkWillListen(EventTypeEnum.Inserted); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemInserted += value; }
            remove
            {
                checkWillListen(EventTypeEnum.Inserted);
                if (eventBlock != null)
                {
                    eventBlock.ItemInserted -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemInserted event
        /// </summary>
        /// <param name="item">The item that was added</param>
        /// <param name="index"></param>
        protected virtual void raiseItemInserted(T item, int index)
        { if (eventBlock != null) eventBlock.raiseItemInserted(this, item, index); }

        /// <summary>
        /// The item removed event. Will be raised for every individual removal from the collection.
        /// </summary>
        public virtual event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add { checkWillListen(EventTypeEnum.RemovedAt); (eventBlock ?? (eventBlock = new EventBlock<T>())).ItemRemovedAt += value; }
            remove
            {
                checkWillListen(EventTypeEnum.RemovedAt);
                if (eventBlock != null)
                {
                    eventBlock.ItemRemovedAt -= value;
                    if (eventBlock.events == 0) eventBlock = null;
                }
            }
        }
        /// <summary>
        /// Fire the ItemRemovedAt event
        /// </summary>
        /// <param name="item">The item that was removed</param>
        /// <param name="index"></param>
        protected virtual void raiseItemRemovedAt(T item, int index)
        { if (eventBlock != null) eventBlock.raiseItemRemovedAt(this, item, index); }

        #region Event support for IList
        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        protected virtual void raiseForSetThis(int index, T value, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, 1);
                raiseItemRemovedAt(item, index);
                raiseItemsAdded(value, 1);
                raiseItemInserted(value, index);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <param name="item"></param>
        protected virtual void raiseForInsert(int i, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemInserted(item, i);
                raiseItemsAdded(item, 1);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        protected void raiseForRemove(T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, 1);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        protected void raiseForRemove(T item, int count)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(item, count);
                raiseCollectionChanged();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void raiseForRemoveAt(int index, T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemRemovedAt(item, index);
                raiseItemsRemoved(item, 1);
                raiseCollectionChanged();
            }
        }

        #endregion

        #region Event  Support for ICollection
        /// <summary>
        ///
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        protected virtual void raiseForUpdate(T newitem, T olditem)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(olditem, 1);
                raiseItemsAdded(newitem, 1);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="newitem"></param>
        /// <param name="olditem"></param>
        /// <param name="count"></param>
        protected virtual void raiseForUpdate(T newitem, T olditem, int count)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsRemoved(olditem, count);
                raiseItemsAdded(newitem, count);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        protected virtual void raiseForAdd(T item)
        {
            if (ActiveEvents != 0)
            {
                raiseItemsAdded(item, 1);
                raiseCollectionChanged();
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="wasRemoved"></param>
        protected virtual void raiseForRemoveAll(ICollectionValue<T> wasRemoved)
        {
            if ((ActiveEvents & EventTypeEnum.Removed) != 0)
            {
                // foreach(var item in wasRemoved)
                var __enumerator1 = (wasRemoved).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var item = (T)__enumerator1.Current;
                    raiseItemsRemoved(item, 1);
                }
            }
            if (wasRemoved != null && wasRemoved.Count > 0)
                raiseCollectionChanged();
        }

        /// <summary>
        ///
        /// </summary>
        #endregion

        #endregion

        /// <summary>
        /// Check if collection is empty.
        /// </summary>
        /// <value>True if empty</value>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// The number of items in this collection.
        /// </summary>
        /// <value></value>
        public abstract int Count { get; }

        /// <summary>
        /// The value is symbolic indicating the type of asymptotic complexity
        /// in terms of the size of this collection (worst-case or amortized as
        /// relevant).
        /// </summary>
        /// <value>A characterization of the speed of the
        /// <code>Count</code> property in this collection.</value>
        public abstract Speed CountSpeed { get; }

        /// <summary>
        /// Copy the items of this collection to part of an array.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if <code>index</code>
        /// is not a valid index
        /// into the array (i.e. negative or greater than the size of the array)
        /// or the array does not have room for the items.</exception>
        /// <param name="array">The array to copy to.</param>
        /// <param name="index">The starting index.</param>
        [Tested]
        public virtual void CopyTo(T[] array, int index)
        {
            if (index < 0 || index + Count > array.Length)
                throw new ArgumentOutOfRangeException();
            {
                // foreach(var item in this)
                var __enumerator2 = (this).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var item = (T)__enumerator2.Current;
                    array[index++] = item;
                }
            }
        }

        /// <summary>
        /// Create an array with the items of this collection (in the same order as an
        /// enumerator would output them).
        /// </summary>
        /// <returns>The array</returns>
        //[Tested]
        public virtual T[] ToArray()
        {
            T[] res = new T[Count];
            int i = 0;
            {
                // foreach(var item in this)
                var __enumerator3 = (this).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var item = (T)__enumerator3.Current;
                    res[i++] = item;
                }
            }

            return res;
        }

        /// <summary>
        /// Apply an single argument action, <see cref="T:C5.Act`1"/> to this enumerable
        /// </summary>
        /// <param name="action">The action delegate</param>
        [Tested]
        public virtual void Apply(Act<T> action)
        {
            {
                // foreach(var item in this)
                var __enumerator4 = (this).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var item = (T)__enumerator4.Current;
                    action(item);
                }
            }
        }


        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection.
        /// </summary>
        /// <param name="predicate">A delegate
        /// (<see cref="T:C5.Fun`2"/> with <code>R = bool</code>)
        /// defining the predicate</param>
        /// <returns>True if such an item exists</returns>
        [Tested]
        public virtual bool Exists(Fun<T, bool> predicate)
        {
            {
                // foreach(var item in this)
                var __enumerator5 = (this).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var item = (T)__enumerator5.Current;
                    if (predicate(item))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if there exists an item  that satisfies a
        /// specific predicate in this collection and return the first one in enumeration order.
        /// </summary>
        /// <param name="predicate">A delegate
        /// (<see cref="T:C5.Fun`2"/> with <code>R == bool</code>) defining the predicate</param>
        /// <param name="item"></param>
        /// <returns>True is such an item exists</returns>
        public virtual bool Find(Fun<T, bool> predicate, out T item)
        {
            {
                // foreach(var jtem in this)
                var __enumerator6 = (this).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    var jtem = (T)__enumerator6.Current;
                    if (predicate(jtem))
                    {
                        item = jtem;
                        return true;
                    }
                }
            }
            item = default(T);
            return false;
        }

        /// <summary>
        /// Check if all items in this collection satisfies a specific predicate.
        /// </summary>
        /// <param name="predicate">A delegate
        /// (<see cref="T:C5.Fun`2"/> with <code>R = bool</code>)
        /// defining the predicate</param>
        /// <returns>True if all items satisfies the predicate</returns>
        [Tested]
        public virtual bool All(Fun<T, bool> predicate)
        {
            {
                // foreach(var item in this)
                var __enumerator7 = (this).GetEnumerator();
                while (__enumerator7.MoveNext())
                {
                    var item = (T)__enumerator7.Current;
                    if (!predicate(item))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create an enumerable, enumerating the items of this collection that satisfies
        /// a certain condition.
        /// </summary>
        /// <param name="predicate">A delegate
        /// (<see cref="T:C5.Fun`2"/> with <code>R = bool</code>)
        /// defining the predicate</param>
        /// <returns>The filtered enumerable</returns>
        public virtual SCG.IEnumerable<T> Filter(Fun<T, bool> predicate)
        {
            {
                // foreach(var item in this)
                var __enumerator8 = (this).GetEnumerator();
                while (__enumerator8.MoveNext())
                {
                    var item = (T)__enumerator8.Current;
                    if (predicate(item))
                        yield return item;
                }
            }
        }

        /// <summary>
        /// Choose some item of this collection.
        /// </summary>
        /// <exception cref="NoSuchItemException">if collection is empty.</exception>
        /// <returns></returns>
        public abstract T Choose();


        /// <summary>
        /// Create an enumerator for this collection.
        /// </summary>
        /// <returns>The enumerator</returns>
        public override abstract SCG.IEnumerator<T> GetEnumerator();

        #region IShowable Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            return Showing.ShowCollectionValue<T>(this, stringbuilder, ref rest, formatProvider);
        }
        #endregion

        #region IFormattable Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return Showing.ShowString(this, format, formatProvider);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

    }
}
