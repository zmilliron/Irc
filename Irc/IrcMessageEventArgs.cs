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
    /// Provides data for an event raised when a message from an Internet Relay Chat (IRC) 
    /// network is received.
    /// </summary>
    public class IrcMessageEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        public IrcMessage Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.IrcMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">A message received from an IRC network.</param>
        public IrcMessageEventArgs(IrcMessage message)
        {
            Message = message;
        }
    }
}
