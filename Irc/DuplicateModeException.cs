/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Represents an error that occurs when a client or channel mode is duplicated in a mode string.
    /// </summary>
    public class DuplicateModeException : Exception
    {
        /// <summary>
        /// Gets the duplicated mode.
        /// </summary>
        public Mode DuplicateMode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.DuplicateModeException"/> class.
        /// </summary>
        public DuplicateModeException(Mode mode) : this(string.Format(Properties.Resources.DuplicateModeExceptionMessage, mode), mode) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.DuplicateModeException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="mode">The duplicated mode.</param>
        public DuplicateModeException(string message, Mode mode)
            : base(message)
        {
            DuplicateMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.DuplicateModeException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="mode">The duplicated mode.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public DuplicateModeException(string message, Mode mode, Exception innerException)
            : base(message, innerException)
        {
            DuplicateMode = mode;
        }
    }
}
