/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Defines a user or channel mode with an optional parameter value.
    /// </summary>
    public struct Mode : IEquatable<Mode>, IComparable, IComparable<Mode>
    {
        /// <summary>
        /// True if the mode is being set, false otherwise.
        /// </summary>
        public bool IsAdded { get; private set; }

        /// <summary>
        /// A user or channel mode character.
        /// </summary>
        public char ModeChar { get; private set; }

        /// <summary>
        /// An optional parameter value.
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Mode"/> class.
        /// </summary>
        /// <param name="modeChar">The user or channel mode character this mode represents.</param>
        /// <param name="isAdded">True if the mode is being set, false otherwise.</param>
        public Mode(char modeChar, bool isAdded) : this()
        {
            ModeChar = modeChar;
            IsAdded = isAdded;
            Parameter = null;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="Irc.Mode"/> class.
        /// </summary>
        /// <param name="modeChar">The user or channel mode character this mode represents.</param>
        /// <param name="isAdded">True if the mode is being set, false otherwise.</param>
        /// <param name="parameter">An optional parameter for supported channel modes.</param>
        public Mode(char modeChar, bool isAdded, string parameter) : this(modeChar, isAdded)
        {
            Parameter = parameter;
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="System.Object"/> and indicates whether this instance
        /// precedes, follows, or appears in the same sort order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">A <see cref="System.Object"/> to compare.</param>
        /// <returns>A -1 if this instance precedes the specified object, 1 if it follows, or 0 if it appears in the same position.</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is Mode)) { throw new ArgumentException("Object must be of type Irc.Mode"); }

            int retVal = 1;
            if (obj != null)
            {
                retVal = ModeChar.CompareTo(((Mode)obj).ModeChar);
            }
            return (retVal);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Irc.Mode"/> and indicates whether this instance
        /// precedes, follows, or appears in the same sort order as the specified <see cref="Irc.Mode"/>.
        /// </summary>
        /// <param name="other">A <see cref="Irc.Mode"/> to compare.</param>
        /// <returns>A -1 if this instance precedes the specified object, 1 if it follows, or 0 if it appears in the same position.</returns>
        public int CompareTo(Mode other)
        {
            return (CompareTo((object)other));
        }

        /// <summary>
        /// Returns whether this instance is equal to the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">A <see cref="System.Object"/> to compare this instance to.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool retVal = false;

            if (!Object.ReferenceEquals(obj, null))
            {
                if (obj is Mode)
                {
                    retVal = this.ModeChar == ((Mode)obj).ModeChar;
                }
                else if (obj is char)
                {
                    retVal = this.ModeChar == ((char)obj);
                }
            }
            return (retVal);
        }

        /// <summary>
        /// Returns whether this instance is equal to the specified <see cref="Irc.Mode"/>.
        /// </summary>
        /// <param name="other">A <see cref="Irc.Mode"/> to compare this instance to.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(Mode other)
        {
            return (Equals((object)other));
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            int retVal = ModeChar.GetHashCode();
            retVal ^= IsAdded.GetHashCode();
            retVal ^= Parameter == null ? 0 : Parameter.GetHashCode();

            return (retVal);
        }

        /// <summary>
        /// Converts the value of this instance to its string representation.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        public override string ToString()
        {
            return (ModeChar.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode1"></param>
        /// <param name="mode2"></param>
        /// <returns></returns>
        public static bool operator ==(Mode mode1, Mode mode2)
        {
            bool retVal = false;

            if (Object.ReferenceEquals(mode1, null))
            {
                retVal = Object.ReferenceEquals(mode2, null);
            }
            else
            {
                retVal = mode1.Equals(mode2);
            }

            return (retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode1"></param>
        /// <param name="char1"></param>
        /// <returns></returns>
        public static bool operator ==(Mode mode1, char char1)
        {
            bool retVal = false;

            if (Object.ReferenceEquals(mode1, null))
            {
                retVal = Object.ReferenceEquals(char1, null);
            }
            else
            {
                retVal = mode1.Equals(char1);
            }

            return (retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="mode1"></param>
        /// <returns></returns>
        public static bool operator ==(char char1, Mode mode1)
        {
            return (mode1 == char1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode1"></param>
        /// <param name="mode2"></param>
        /// <returns></returns>
        public static bool operator !=(Mode mode1, Mode mode2)
        {
            return (!(mode1 == mode2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode1"></param>
        /// <param name="char1"></param>
        /// <returns></returns>
        public static bool operator !=(Mode mode1, char char1)
        {
            return (!(mode1 == char1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="mode1"></param>
        /// <returns></returns>
        public static bool operator !=(char char1, Mode mode1)
        {
            return (!(mode1 == char1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static implicit operator char(Mode mode)
        {
            return (mode.ModeChar);
        }
    }
}
