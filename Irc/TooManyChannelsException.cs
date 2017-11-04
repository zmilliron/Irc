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
    /// Represents the error that occurs when a client attempts to join a channel after they
    /// have reached the maximum number of channels allowed.
    /// </summary>
    public class TooManyChannelsException : Exception
    {
        /// <summary>
        /// Gets or sets the maximum number of channels allowed by the server.
        /// </summary>
        public int MaximumChannelsAllowed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TooManyChannelsException"/> class.
        /// </summary>
        public TooManyChannelsException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TooManyChannelsException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public TooManyChannelsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.TooManyChannelsException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public TooManyChannelsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
