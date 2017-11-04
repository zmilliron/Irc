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
    /// Represents the error that occurs when a mode character is not supported.
    /// </summary>
    public class UnsupportedModeException : Exception
    {
        /// <summary>
        /// Gets or sets the mode character that is the cause of the exception.
        /// </summary>
        public char Mode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnsupportedModeException"/> class.
        /// </summary>
        public UnsupportedModeException() : this(Properties.Resources.UnsupportedModeExceptionMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnsupportedModeException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public UnsupportedModeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnsupportedModeException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public UnsupportedModeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
