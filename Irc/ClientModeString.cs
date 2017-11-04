/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Represents a string of client modes and their respective parameters.
    /// </summary>
    public sealed class ClientModeString : IEnumerable, IEnumerable<Mode>
    {
        private Mode[] _modes;
        private string _stringRepresentation;

        /// <summary>
        /// Returns the total number of elements in the <see cref="Irc.ClientModeString"/>
        /// </summary>
        public int Length { get { return (_modes.Length); } }

        /// <summary>
        /// Returns the <see cref="Irc.Mode"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="Irc.Mode"/> to return.</param>
        /// <returns>A <see cref="Irc.Mode"/>.</returns>
        public Mode this[int index]
        {
            get
            {
                return (_modes[index]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ClientModeString"/> class to 
        /// the value indicated by the specified <see cref="Irc.Mode"/>.
        /// </summary>
        /// <param name="mode">A <see cref="Irc.Mode"/>.</param>
        public ClientModeString(Mode mode)
        {
            _modes = new Mode[] { mode };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ClientModeString"/> class to 
        /// the value indicated by the specified <see cref="Irc.Mode"/> collection.
        /// </summary>
        /// <param name="modes">A <see cref="Irc.Mode"/> collection.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if modes is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if modes contains no elements.</exception>
        /// <exception cref="Irc.DuplicateModeException">Thrown if any modes appear more than once in the list.</exception>
        public ClientModeString(IEnumerable<Mode> modes)
        {
            if (modes == null) { throw new ArgumentNullException(nameof(modes)); }
            int count = modes.Count();
            if (count == 0) { throw new ArgumentException(nameof(modes)); }

            _modes = new Mode[count];
            
            int i = 0;
            foreach (Mode m in modes)
            {
                if (Contains(m)) { throw new DuplicateModeException(m); }
                _modes[i] = m;
                i++;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="Irc.Mode"/> occurs in this 
        /// <see cref=" Irc.ClientModeString"/>.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>True if the mode occurs, false otherwise.</returns>
        public bool Contains(Mode mode)
        {
            return (Contains((char)mode));
        }

        /// <summary>
        /// Returns a value indicating whether the specified mode character occurs in this 
        /// <see cref=" Irc.ClientModeString"/>.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>True if the mode occurs, false otherwise.</returns>
        public bool Contains(char mode)
        {
            return (IndexOf(mode) > -1);
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ClientModeString"/>.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ClientModeString"/>.</returns>
        public IEnumerator GetEnumerator()
        {
            return (_modes.GetEnumerator());
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ClientModeString"/>.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ClientModeString"/>.</returns>
        IEnumerator<Mode> IEnumerable<Mode>.GetEnumerator()
        {
            return ((_modes as IEnumerable<Mode>).GetEnumerator());
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified mode.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>The zero-based index of the specified mode, or -1 if the mode was not found.</returns>
        public int IndexOf(char mode)
        {
            for (int i = 0; i < _modes.Length; i++)
            {
                if (_modes[i] == mode)
                {
                    return (i);
                }
            }

            return (-1);
        }

        /// <summary>
        /// Deletes all <see cref="Irc.Mode">Irc.Modes</see> from this <see cref="Irc.ClientModeString"/> 
        /// starting from the specified index and continuing until the last position.
        /// </summary>
        /// <param name="startIndex">The zero-based index from which 
        /// to start deleting <see cref="Irc.Mode">Irc.Modes</see>.</param>
        /// <returns>A new <see cref="Irc.ClientModeString"/> with characters after the 
        /// specified index removed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if startIndex not in the bounds of the array.</exception>
        public ClientModeString Remove(int startIndex)
        {
            return (Remove(startIndex, Length - startIndex));
        }

        /// <summary>
        /// Deletes the specified number of <see cref="Irc.Mode">Irc.Modes</see> from this 
        /// <see cref="Irc.ChannelModeString"/> starting from the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based index from which 
        /// to start deleting <see cref="Irc.Mode">Irc.Modes</see>.</param>
        /// <param name="count">The number of <see cref="Irc.Mode">Irc.Modes</see> to delete.</param>
        /// <returns>A new <see cref="Irc.ChannelModeString"/> with the specified number of modes removed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if startIndex or count is not in the bounds of the array.</exception>
        public ClientModeString Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex >= Length) { throw new ArgumentOutOfRangeException(nameof(startIndex)); }
            if (count < 0 || count > Length - startIndex) { throw new ArgumentOutOfRangeException(nameof(count)); }

            ClientModeString retVal = null;

            if (startIndex > 0)
            {
                Mode[] newModeArray = new Mode[Length - count];
                Array.Copy(_modes, 0, newModeArray, 0, startIndex);
                Array.Copy(_modes, startIndex + count, newModeArray, startIndex, Length - startIndex - count);

                retVal = new ClientModeString(newModeArray);
            }

            return (retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeString"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ClientModeString operator +(ClientModeString modeString, Mode mode)
        {
            ClientModeString retVal = null;
            if (modeString == null)
            {
                retVal = new ClientModeString(mode);
            }
            else
            {
                Mode[] newModeArray = new Mode[modeString.Length + 1];
                modeString._modes.CopyTo(newModeArray, 0);
                newModeArray[newModeArray.Length - 1] = mode;
                
                retVal = new ClientModeString(newModeArray);
            }

            return (retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="modeString"></param>
        /// <returns></returns>
        public static ClientModeString operator +(Mode mode, ClientModeString modeString)
        {
            return (modeString + mode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static ClientModeString operator +(ClientModeString first, ClientModeString second)
        {
            ClientModeString retVal = first ?? second;

            if (first != null && second != null)
            {
                Mode[] newModeArray = new Mode[first.Length + second.Length];
                first._modes.CopyTo(newModeArray, 0);
                second._modes.CopyTo(newModeArray, first.Length);

                retVal = new ClientModeString(newModeArray);

            }

            return (retVal);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Irc.ClientModeString"/>.
        /// </summary>
        /// <returns>A string containing user modes formatted in accordance with the IRC protocol.</returns>
        public override string ToString()
        {
            if (_stringRepresentation == null)
            {
                StringBuilder adds = new StringBuilder("+");
                StringBuilder removes = new StringBuilder("-");
                StringBuilder final = new StringBuilder();

                foreach (Mode m in _modes)
                {
                    if (m.IsAdded)
                    {
                        adds.Append(m);
                    }
                    else
                    {
                        removes.Append(m);
                    }
                }

                if (adds.Length > 1)
                {
                    final.Append(adds.ToString());
                }
                if (removes.Length > 1)
                {
                    final.Append(removes.ToString());
                }

                _stringRepresentation = final.ToString();

            }

            return (_stringRepresentation);
        }
    }
}
