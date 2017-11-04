/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Text.RegularExpressions;

namespace Irc
{
    /// <summary>
    /// Represents a valid Internet Relay Chat (IRC) username.
    /// </summary>
    public sealed class IrcUsername : IrcNameBase, IComparable<IrcUsername>
    {
        /// <summary>
        /// The regular expression used to validate a username.
        /// </summary>
        public const string REGEXSTRING = @"^[^\x00\r\n\s@]*$";
        private static readonly Regex _usernameRegex = new Regex(REGEXSTRING);

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.IrcUsername"/> class.
        /// </summary>
        /// <param name="username">The username the structure will represent.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if username is null or empty.</exception>
        /// <exception cref="System.FormatException">Thrown if username is not a valid username as defined by RFC2812.</exception>
        public IrcUsername(string username) : base(username)
        {
            if (!_usernameRegex.IsMatch(username)) { throw new FormatException(Properties.Resources.UserNameFormatError); }
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Irc.IrcUsername"/> and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the specified Irc.NameBase.
        /// </summary>
        /// <param name="other">The <see cref="Irc.IrcUsername"/> to compare to this instance.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the specified Irc.NameBase.</returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.IrcUsername"/></exception>
        public int CompareTo(IrcUsername other)
        {
            return (base.CompareTo(other));
        }

        /// <summary>
        /// Determines whether the specified string is a valid username.
        /// </summary>
        /// <param name="username">The string to validate.</param>
        /// <returns>True if the string is a valid username, false otherwise.</returns>
        public static bool IsValid(string username)
        {
            return (_usernameRegex.IsMatch(username));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static explicit operator IrcUsername(string s)
        {
            return (s == null ? null : new IrcUsername(s));
        }

        
    }
}
