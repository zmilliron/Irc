/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;

namespace Irc
{
    /// <summary>
    /// Provides a base class from which to derive names used when communicating over the Internet Relay Chat (IRC) protocol.
    /// </summary>
    public abstract class IrcNameBase : IComparable, IComparable<IrcNameBase>, IEquatable<IrcNameBase>
    {
        private readonly string _name;

        private readonly string _nameUpperCase;

        /// <summary>
        /// Gets the number of characters in the current <see cref="Irc.IrcNameBase"/> object.
        /// </summary>
        public int Length
        {
            get { return (_name.Length); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrcNameBase"/> class.
        /// </summary>
        /// <param name="name">The name this object represents.</param>
        /// <exception cref="ArgumentNullException"/>
        protected IrcNameBase(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }

            _name = name;
            _nameUpperCase = name.ToUpperInvariant();
        }

        /// <summary>
        /// Returns the hash code for this System.Object.
        /// </summary>
        /// <returns>The hash code for this System.Object.</returns>
        public override int GetHashCode()
        {
            return (_nameUpperCase.GetHashCode());
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare.</param>
        /// <returns>True if the System.Object is equal to the current instance, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (this == (obj as IrcNameBase));
        }

        /// <summary>
        /// Determines value equality between two <see cref="Irc.IrcNameBase"/> objects.
        /// </summary>
        /// <param name="name1">The first <see cref="Irc.IrcNameBase"/> to compare.</param>
        /// <param name="name2">The second <see cref="Irc.IrcNameBase"/> to compare.</param>
        /// <returns>True if IrcNameBases are equal independent of casing, false otherwise.</returns>
        public static bool operator ==(IrcNameBase name1, IrcNameBase name2)
        {
            return(Object.ReferenceEquals(name1, name2) ||
                  (!Object.ReferenceEquals(name1, null) && !Object.ReferenceEquals(name2, null) && name1._nameUpperCase == name2._nameUpperCase));
        }

        /// <summary>
        /// Determines value inequality between two <see cref="Irc.IrcNameBase"/> objects.
        /// </summary>
        /// <param name="name1">The first <see cref="Irc.IrcNameBase"/> to compare.</param>
        /// <param name="name2">The second <see cref="Irc.IrcNameBase"/> to compare.</param>
        /// <returns>True if the Usernames are not equal independent of casing, false otherwise.</returns>
        public static bool operator !=(IrcNameBase name1, IrcNameBase name2)
        {
            return (!(name1 == name2));
        }


        /// <summary>
        /// Returns the string representation of this <see cref="Irc.IrcNameBase"/> instance.
        /// </summary>
        /// <returns>A string representation of this <see cref="Irc.IrcNameBase"/> instance.</returns>
        public override string ToString()
        {
            return (_name);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Irc.IrcNameBase"/> and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the specified  <see cref="Irc.IrcNameBase"/>.
        /// </summary>
        /// <param name="obj">An object that evaluates to an <see cref="Irc.IrcNameBase"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the specified <see cref="Irc.IrcNameBase"/>.</returns>
        public int CompareTo(object obj)
        {
            return (CompareTo(obj as IrcNameBase));
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Irc.IrcNameBase"/> and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the specified Irc.NameBase.
        /// </summary>
        /// <param name="other">The <see cref="Irc.IrcNameBase"/> to compare to this instance.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the specified Irc.NameBase.</returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.IrcNameBase"/></exception>
        public int CompareTo(IrcNameBase other)
        {
            int retVal = 1;
            if (other != null)
            {
                retVal = _name.CompareTo(other._name);
            }
            return (retVal);
            
        }

        /// <summary>
        /// Determines whether the specified <see cref="Irc.IrcNameBase"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="Irc.IrcNameBase"/> to compare.</param>
        /// <returns>True if the <see cref="Irc.IrcNameBase"/> is equal to the current instance, false otherwise.</returns>
        public bool Equals(IrcNameBase other)
        {
            return (this == other);
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="System.String"/> object occurs 
        /// within the name represented by this <see cref="Irc.IrcNameBase"/>, independent of casing.
        /// </summary>
        /// <param name="value">A System.String to seek.</param>
        /// <returns>True if the specified System.String exists, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if value is null.</exception>
        public bool Contains(string value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            return (_nameUpperCase.Contains(value.ToUpperInvariant()));
        }

        /// <summary>
        /// Returns a value indicating whether the beginning of this <see cref="Irc.IrcNameBase"/>
        /// matches the specified <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">A System.String to seek.</param>
        /// <returns>True if the specified System.String exists, false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if value is null.</exception>
        public bool StartsWith(string value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            return (_nameUpperCase.StartsWith(value.ToUpperInvariant()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static implicit operator string(IrcNameBase n)
        {
            return (Convert.ToString(n));
        }
    }
}
