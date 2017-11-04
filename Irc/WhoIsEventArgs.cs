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
    /// Provides data for an event raised when a WHOIS command reply is received from
    /// an IRC server.
    /// </summary>
    /// <seealso cref="Irc.Commands.WHOIS"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISREGNICK"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
    /// <seealso cref="Irc.Numerics.RPL_WHOISSPECIAL"/>
    public class WhoIsEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets or sets whether a user is a network operator.
        /// </summary>
        public bool IsIRCOperator { get; set; }

        /// <summary>
        /// Gets or sets the time, in seconds, a user has been idle on the network.
        /// </summary>
        public int IdleTime { get; set; }

        /// <summary>
        /// Gets or sets information about the server a user is connected to on the IRC network.
        /// </summary>
        public string ServerInfo { get; set; }

        /// <summary>
        /// Gets or sets a list of channels a user has joined.
        /// </summary>
        public IrcChannelName[] ChannelList { get; set; }

        /// <summary>
        /// Gets or sets the date and time the user signed on to the network.
        /// </summary>
        public DateTime? SignOnTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the user is connected using a Secure Sockets Layer (SSL) connection.
        /// </summary>
        public bool IsSecureConnection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the user's nickname is registered.
        /// </summary>
        public bool IsNicknameRegistered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the user is available as a network help operator.
        /// </summary>
        public bool IsHelpOperator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.WhoIsEventArgs"/> class.
        /// </summary>
        /// <param name="searchedUser">The nickname of the user searched for with a WHOIS command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if searchedUser is null.</exception>
        public WhoIsEventArgs(IrcNickname searchedUser) : base(searchedUser)
        {
        }
    }
}
