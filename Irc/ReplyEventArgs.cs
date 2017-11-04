/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after a reply numeric is received from a server.
    /// </summary>
    /// <see cref="Irc.Numerics"/>
    public class ReplyEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets a reply numeric.
        /// </summary>
        public int ReplyCode { get; set; }

        /// <summary>
        /// Gets or sets the parameters in the reply.
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        /// Gets or sets a message associated with the reply.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ReplyEventArgs"/> class.
        /// </summary>
        public ReplyEventArgs() { }
    }
}
