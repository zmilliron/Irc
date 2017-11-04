/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/

namespace Irc.Ctcp
{
    /// <summary>
    /// Provides data for an event raised after receiving a Client-to-Client Protocol (CTCP) message.
    /// </summary>
    public class CtcpEventArgs : PrivateMessageEventArgs
    {
        /// <summary>
        /// Gets or sets the CTCP command raising the event.
        /// </summary>
        public string CtcpCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the Irc.Ctcp.CtcpEventArgs class.
        /// </summary>
        /// <param name="sender">The nickname of the user sending the CTCP message.</param>
        /// <param name="target">The target of the CTCP message.</param>
        public CtcpEventArgs(IrcNickname sender, IrcNameBase target) : base(sender, target)
        {
        }
    }
}
