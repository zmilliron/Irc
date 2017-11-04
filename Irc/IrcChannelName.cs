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
    /// Represents a valid Internet Relay Chat (IRC) channel name.
    /// </summary>
    public sealed class IrcChannelName : IrcNameBase, IComparable<IrcChannelName>
    {
        /// <summary>
        /// The regular expression used to validate a channel name.
        /// </summary>
        public const string REGEXSTRING = @"^([#&\+])([^\x07\s,]{1,50})$";

        private static readonly Regex _channelNameRegex = new Regex(REGEXSTRING);

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.IrcChannelName"/> class.
        /// </summary>
        /// <param name="channelName">The channel name the structure will represent.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null or empty.</exception>
        /// <exception cref="System.FormatException">Thrown if channelName is not a valid channel name as defined by RFC2812.</exception>
        public IrcChannelName(string channelName) : base(PrependChannelSymbol(channelName))
        {
            if (!_channelNameRegex.IsMatch(channelName)) { throw new FormatException(Properties.Resources.ChannelNameFormatError); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        private static string PrependChannelSymbol(string channelName)
        {
            if (Regex.IsMatch(channelName, @"^[^#&!\+]")) { channelName = channelName.Insert(0, "#"); }
            return (channelName);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="Irc.IrcChannelName"/> and returns an integer 
        /// that indicates whether the current instance precedes, follows, or occurs in the same 
        /// position in the sort order as the specified Irc.NameBase.
        /// </summary>
        /// <param name="other">The <see cref="Irc.IrcChannelName"/> to compare to this instance.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows,
        /// or occurs in the same position in the sort order as the specified Irc.NameBase.</returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.IrcChannelName"/></exception>
        public int CompareTo(IrcChannelName other)
        {
            return (base.CompareTo(other));
        }

        /// <summary>
        /// Determines whether the specified string is a valid channel name.
        /// </summary>
        /// <param name="channelName">The string to validate.</param>
        /// <returns>True if the string is a valid channel name, false otherwise.</returns>
        public static bool IsValid(string channelName)
        {
            return (channelName != null && _channelNameRegex.IsMatch(channelName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static explicit operator IrcChannelName(string s)
        {
            return (s == null ? null : new IrcChannelName(s));
        }
    }
}
