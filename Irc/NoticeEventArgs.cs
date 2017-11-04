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
    /// Provides data for an event raised after a notice has been received or generated.
    /// </summary>
    public class NoticeEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the notice is a CTCP reply.
        /// </summary>
        public bool IsCtcpReply { get; set; }

        /// <summary>
        /// Gets or sets a notice mesage.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the sender of the notice.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the recipient of the notice.
        /// </summary>
        public IrcNameBase Target { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.NoticeEventArgs"/> class.
        /// </summary>
        public NoticeEventArgs()
        {
        }
    }
}
