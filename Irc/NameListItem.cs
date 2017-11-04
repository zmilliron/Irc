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
    /// Represents a user in the name list of a channel.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_NAMEREPLY"/>
    /// <seealso cref="Irc.SupportedOptions.NAMESX"/>
    /// <seealso cref="Irc.SupportedOptions.UHNAMES"/>
    public struct NameListItem
    {
        /// <summary>
        /// Gets the nickname of a user.
        /// </summary>
        public IrcNickname Nickname { get; set; }

        /// <summary>
        /// Gets the username of a user.
        /// </summary>
        public IrcUsername Username { get; set; }

        /// <summary>
        /// Gets the hostname of a user.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets sets the channel mode set for a user.
        /// </summary>
        public string Modes { get; set; }

        /// <summary>
        /// Converts a string of user data into a <see cref="Irc.NameListItem"/>.
        /// </summary>
        /// <param name="nameString">A string containing user data to convert.</param>
        /// <returns>An object containing the parsed user data.</returns>
        /// <remarks>A name string may arrive from a server in four possible formats, depending 
        /// on the options set by the server.  By default, names only arrive as nicknames with the 
        /// highest channel status set for that user inserted at the front.
        /// 
        /// If the NAMESX option has been set by the server, names arrive with all channel 
        /// statuses appended to the front.
        /// 
        /// If the UHNAMES option has been set by the server, names arrive in the format
        /// nickname!username@host.
        /// 
        /// Finally, both the NAMESX and UHNAMES may be set at the same time combining the 
        /// formats for each.
        /// </remarks>
        /// <example>Default: User1, @User2</example>
        /// <example>NAMESX: User1, @+User2</example>
        /// <example>UHNAMES: User1!User@123.abc.com, +User2!OtherUser@123.abc.com</example>
        /// <example>NAMES + UHNAMES: User1!User@123.abc.com, @+User2!OtherUser@123.abc.com</example>
        /// <exception cref="System.ArgumentNullException">Thrown if nameString is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if nameString is in an invalid format.</exception>
        /// <exception cref="System.FormatException">Thrown if nameString is in an invalid format.</exception>
        public static NameListItem Parse(string nameString)
        {
            if (string.IsNullOrEmpty(nameString)) { throw new ArgumentNullException(nameof(nameString)); }
           
            int nickIndex = Regex.Match(nameString, @"[a-zA-Z\[\]\\`_\^{}\|-]").Index;
            NameListItem retVal = new NameListItem();

            int bashIndex = nameString.IndexOf('!');
            retVal.Nickname = bashIndex > 0 ? new IrcNickname(nameString.Substring(nickIndex, bashIndex - nickIndex)) :  new IrcNickname(nameString.Substring(nickIndex));
            
            //NAMESX format
            if (nickIndex > 0)
            {
                retVal.Modes = nameString.Substring(0, nickIndex);
                nameString = nameString.Substring(nickIndex);
            }

            //If  UHNAMES format
            if (nameString.Contains("!"))
            {
                retVal.Username = new IrcUsername(nameString.Substring(nameString.IndexOf("!") + 1, nameString.IndexOf("@") - nameString.IndexOf("!") - 1));
                retVal.HostName = nameString.Substring(nameString.IndexOf("@") + 1);
            }

            return (retVal);
        }
    }
}
