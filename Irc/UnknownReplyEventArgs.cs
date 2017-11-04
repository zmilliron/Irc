using System;

namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after an unknown reply numeric is received from a server.
    /// </summary>
    /// <see cref="Irc.Numerics"/>
    public class UnknownReplyEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets an unknown reply numeric.
        /// </summary>
        public int ReplyCode { get; set; }

        /// <summary>
        /// Gets or sets a message associated with the reply.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnknownReplyEventArgs"/> class.
        /// </summary>
        public UnknownReplyEventArgs() { }
    }
}
