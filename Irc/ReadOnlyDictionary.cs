/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;

namespace Irc
{
    /// <summary>
    /// Represents a read-only collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">TKey is TKey.</typeparam>
    /// <typeparam name="TValue">TValue is TValue.</typeparam>
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _dictionary;
        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/> class that 
        /// serves as a wrapper around the specified <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="dictionary">The <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> with which 
        /// to create this instance of the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/> class.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if dictionary is null.</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }

            _dictionary = dictionary;
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.NotSupportedException">Modifying the collection is not supported.</exception>
        void System.Collections.Generic.IDictionary<TKey,TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/> contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if key is null.</exception>
        public bool ContainsKey(TKey key)
        {
            return(_dictionary.ContainsKey(key));
        }

        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/> containing the keys of the 
        /// <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/>
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return (_dictionary.Keys); }
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="System.NotSupportedException">Modifying the collection is not supported.</exception>
        bool System.Collections.Generic.IDictionary<TKey,TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if 
        /// they key exists; otherwise, the default value for the type of value parameter.  This 
        /// parameter is passed uninitialized/</param>
        /// <returns>True if the value is found, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return (_dictionary.TryGetValue(key, out value));
        }

        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/> containing the values in 
        /// the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return (_dictionary.Values); }
        }

        /// <summary>
        /// Gets the element with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if an attempt to set a value in the collection is made.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return (_dictionary[key]);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.NotSupportedException">Modifying the collection is not supported.</exception>
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Modifying the collection is not supported.</exception>
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/> contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="item">The object to locate.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return (_dictionary.Contains(item));
        }

        /// <summary>
        /// Copies the elements of the <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/> to 
        /// a <see cref="System.Array"/>, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of 
        /// the elements copies from <see cref="System.Collections.Generic.ICollection&lt;T&gt;"/>.  The 
        /// array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if arrayIndex is outside the bounds of the collection.</exception>
        /// <exception cref="System.ArgumentException">Thrown if array is a not a one-dimensional <see cref="System.Array"/></exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public int Count
        {
            get { return (_dictionary.Count); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Irc.ReadOnlyDictionary&lt;TKey, TValue&gt;"/> is read-only. 
        /// This property always returns true.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (true); }
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.NotSupportedException">Modifying the collection is not supported.</exception>
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator that can iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (_dictionary.GetEnumerator());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator that can iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return(_dictionary.GetEnumerator());
        }
    }
}
