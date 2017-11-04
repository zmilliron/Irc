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
    /// Represents a valid Internet Relay Chat (IRC) nickname.
    /// </summary>
    public sealed class IrcNickname : IrcNameBase, IComparable<IrcNickname>
    {
        /// <summary>
        /// The regular expression used to validate a nickname.
        /// </summary>
        public const string REGEXSTRING = @"^[a-zA-Z\x5B-\x60\x7B-\x7D][a-zA-Z0-9\x5B-\x60\x7B-\x7D\-]*$";
        private static readonly Regex _nicknameRegex = new Regex(REGEXSTRING);

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.IrcNickname"/> class.
        /// </summary>
        /// <param name="nickname">The nickname the structure will represent.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        /// <exception cref="System.FormatException">Thrown if nickname is not a valid nickname as defined by RFC2812.</exception>
        public IrcNickname(string nickname) : base(nickname)
        {
            if (!_nicknameRegex.IsMatch(nickname)) { throw new FormatException(Properties.Resources.NicknameFormatError); }
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Irc.IrcNickname"/> and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the specified Irc.NameBase.
        /// </summary>
        /// <param name="other">The <see cref="Irc.IrcNickname"/> to compare to this instance.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the specified Irc.NameBase.</returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.IrcNickname"/></exception>
        public int CompareTo(IrcNickname other)
        {
            return (base.CompareTo(other));
        }

        /// <summary>
        /// Determines whether the specified string is a valid nickname.
        /// </summary>
        /// <param name="nickname">The string to validate.</param>
        /// <returns>True if the string is a valid nickname, false otherwise.</returns>
        public static bool IsValid(string nickname)
        {
            return (nickname != null && _nicknameRegex.IsMatch(nickname));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static explicit operator IrcNickname(string s)
        {
            if (s != null && !IsValid(s)) { throw new InvalidCastException(string.Format(Properties.Resources.InvalidCastNickname, s)); }

            return (s == null ? null : new IrcNickname(s));
        }

        
    }
}
