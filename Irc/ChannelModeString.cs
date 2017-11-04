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
    /// Represents a string of channel modes and their respective parameters.
    /// </summary>
    /// <remarks>
    /// A ChannelModeString possesses similar behavior to the <see cref="System.String"/> class but differs in 
    /// several key ways.
    /// 
    /// First, a ChannelModeString is constructed of individual <see cref="Irc.Mode">Irc.Modes</see> which 
    /// contain additional information to the mode character itself.
    /// 
    /// Second, a ChannelModeString validates its contents and throws an exception if an error is detected.  
    /// An exception is thrown if an attempt is made to add a <see cref="Irc.Mode"/> that requires a parameter 
    /// but is missing one.  An exception is also thrown if an attempt is made to add an existing mode 
    /// for which duplicates are not allowed.  Any modes that do not belong to the 
    /// <see cref="Irc.ChannelModes.UserModes"/> collection cannot be duplicated in a mode string.
    /// 
    /// Third, many <see cref="System.String"/> methods have been omitted since they have no analog in a ChannelModeString.
    /// </remarks>
    public sealed class ChannelModeString : IEnumerable, IEnumerable<Mode>
    {
        private Mode[] _modes;
        private string _stringRepresentation;

        /// <summary>
        /// Returns the total number of elements in the <see cref="Irc.ChannelModeString"/>
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
        /// Initializes a new instance of the <see cref="Irc.ChannelModeString"/> class to 
        /// the value indicated by the specified <see cref="Irc.Mode"/>.
        /// </summary>
        /// <param name="mode">A <see cref="Irc.Mode"/>.</param>
        public ChannelModeString(Mode mode)
        {
            _modes = new Mode[] { mode };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelModeString"/> class to 
        /// the value indicated by the specified <see cref="Irc.Mode"/> collection.
        /// </summary>
        /// <param name="modes">A <see cref="Irc.Mode"/> collection.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if modes is null.</exception>
        /// <exception cref="Irc.DuplicateModeException">Thrown if any modes appear more than once in the list that are not user-related modes.</exception>
        public ChannelModeString(IEnumerable<Mode> modes)
        {
            if (modes == null) { throw new ArgumentNullException(nameof(modes)); }
            int count = modes.Count();
            if (count == 0) { throw new ArgumentException(nameof(modes)); }

            _modes = new Mode[count];
            int i = 0;
            foreach (Mode m in modes)
            {
                if (IsDuplicate(m)) { throw new DuplicateModeException(m); }
                _modes[i] = m;
                i++;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="Irc.Mode"/> occurs in this 
        /// <see cref=" Irc.ChannelModeString"/>.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>True if the mode occurs, false otherwise.</returns>
        public bool Contains(Mode mode)
        {
            return (Contains(mode.ModeChar));
        }

        /// <summary>
        /// Returns a value indicating whether the specified mode character occurs in this 
        /// <see cref=" Irc.ChannelModeString"/>.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>True if the mode occurs, false otherwise.</returns>
        public bool Contains(char mode)
        {
            return (IndexOf(mode) > -1);
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ChannelModeString"/>.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ChannelModeString"/>.</returns>
        public IEnumerator GetEnumerator()
        {
            return (_modes.GetEnumerator());
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ChannelModeString"/>.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> for the <see cref="Irc.ChannelModeString"/>.</returns>
        IEnumerator<Mode> IEnumerable<Mode>.GetEnumerator()
        {
            return ((_modes as IEnumerable<Mode>).GetEnumerator());
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified <see cref="Irc.Mode"/>.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>The zero-based index of the specified mode, or -1 if the mode was not found.</returns>
        public int IndexOf(Mode mode)
        {
            return (IndexOf((char)mode));
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified mode.
        /// </summary>
        /// <param name="mode">The value to locate.</param>
        /// <returns>The zero-based index of the specified mode, or -1 if the mode was not found.</returns>
        public int IndexOf(char mode)
        {
            for(int i = 0; i < _modes.Length; i++)
            {
                if (_modes[i] == mode)
                {
                    return (i);
                }
            }

            return (-1);
        }

        private bool IsDuplicate(Mode mode)
        {
            bool retVal = !ChannelModes.UserModes.Contains(mode.ModeChar) && _modes.Contains(mode);

            return (retVal);
        }

        /// <summary>
        /// Converts
        /// </summary>
        /// <param name="s">A string containing channel modes to convert.</param>
        /// <param name="modesAlwaysParameters">A list of modes that always require a parameter.</param>
        /// <param name="modesAddParameters">A list of modes that only require a parameter when adding.</param>
        /// <returns>A ChannelModeString</returns>
        public static ChannelModeString Parse(string s, string modesAlwaysParameters, string modesAddParameters)
        {
            if (s == null) { throw new ArgumentNullException(nameof(s)); }
            if (s == string.Empty) { throw new FormatException("Parameter s was not in the correct format."); }

            try
            {
                List<Mode> modes = new List<Mode>();
                string[] parts = s.Split(' ');
                bool modeAdded = true;
                int modeParameterIndex = 1;
                foreach (char c in parts[0])
                {
                    switch (c)
                    {
                        case '+':
                            modeAdded = true;
                            break;
                        case '-':
                            modeAdded = false;
                            break;
                        default:
                            if (modeAdded)
                            {
                                string addParam = null;
                                string allparams = string.Format("{0}{1}", modesAlwaysParameters, modesAddParameters);
                                if (allparams != null)
                                {
                                    addParam = allparams.Contains(c) ? parts[modeParameterIndex++] : null;
                                }
                                modes.Add(new Mode(c, modeAdded, addParam));
                            }
                            else
                            {
                                string removeParam = null;
                                if (modesAlwaysParameters != null)
                                {
                                    removeParam = modesAlwaysParameters.Contains(c) ? parts[modeParameterIndex++] : null;
                                }
                                modes.Add(new Mode(c, modeAdded, removeParam));
                            }

                            break;
                    }
                }
                return (new ChannelModeString(modes));
            }
            catch (Exception ex)
            {
                throw new FormatException("Input string not in the correct format.", ex);
            }
        }

        /// <summary>
        /// Deletes all <see cref="Irc.Mode">Irc.Modes</see> from this <see cref="Irc.ChannelModeString"/> 
        /// starting from the specified index and continuing until the last position.
        /// </summary>
        /// <param name="startIndex">The zero-based index from which 
        /// to start deleting <see cref="Irc.Mode">Irc.Modes</see>.</param>
        /// <returns>A new <see cref="Irc.ChannelModeString"/> with characters after the 
        /// specified index removed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if startIndex not in the bounds of the array.</exception>
        public ChannelModeString Remove(int startIndex)
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
        public ChannelModeString Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex >= Length) { throw new ArgumentOutOfRangeException(nameof(startIndex)); }
            if (count < 0 || count > Length - startIndex) { throw new ArgumentOutOfRangeException(nameof(count)); }

            ChannelModeString retVal = null;

            if (startIndex > 0)
            {
                Mode[] newModeArray = new Mode[Length - count];
                Array.Copy(_modes, 0, newModeArray, 0, startIndex);
                Array.Copy(_modes, startIndex + count, newModeArray, startIndex, Length - startIndex - count);

                retVal = new ChannelModeString(newModeArray);
            }

            return (retVal);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Irc.ChannelModeString"/>.
        /// </summary>
        /// <returns>A string containing channel modes and their respective parameters 
        /// formatted in accordance with the IRC protocol.</returns>
        public override string ToString()
        {
            if (_stringRepresentation == null)
            {
                StringBuilder adds = new StringBuilder("+");
                StringBuilder addParams = new StringBuilder();
                StringBuilder removes = new StringBuilder("-");
                StringBuilder removeParms = new StringBuilder();
                StringBuilder final = new StringBuilder();

                foreach (Mode m in _modes)
                {
                    if (m.IsAdded)
                    {
                        adds.Append(m);
                        if (m.Parameter != null)
                        {
                            addParams.Append(" ");
                            addParams.Append(m.Parameter);
                        }
                    }
                    else
                    {
                        removes.Append(m);
                        if (m.Parameter != null)
                        {
                            removeParms.Append(" ");
                            removeParms.Append(m.Parameter);
                        }
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

                final.Append(addParams.ToString());
                final.Append(removeParms.ToString());

                _stringRepresentation = final.ToString();

            }

            return (_stringRepresentation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modeString"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static ChannelModeString operator +(ChannelModeString modeString, Mode mode)
        {
            ChannelModeString retVal = null;
            if (modeString == null)
            {
                retVal = new ChannelModeString(mode);
            }
            else
            {
                Mode[] newModeArray = new Mode[modeString.Length + 1];
                modeString._modes.CopyTo(newModeArray, 0);
                newModeArray[newModeArray.Length - 1] = mode;

                retVal = new ChannelModeString(newModeArray);
            }

            return (retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="modeString"></param>
        /// <returns></returns>
        public static ChannelModeString operator +(Mode mode, ChannelModeString modeString)
        {
            return (modeString + mode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static ChannelModeString operator +(ChannelModeString first, ChannelModeString second)
        {
            ChannelModeString retVal = null;

            if (first != null && second == null)
            {
                retVal = first;
            }
            else if (first == null && second != null)
            {
                retVal = second;
            }
            else if (first != null && second != null)
            {
                Mode[] newModeArray = new Mode[first.Length + second.Length];
                first._modes.CopyTo(newModeArray, 0);
                second._modes.CopyTo(newModeArray, first.Length);

                retVal = new ChannelModeString(newModeArray);

            }

            return (retVal);
        }
    }
}
