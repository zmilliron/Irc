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
    /// Represents the error that occurs when a connection has already been registered on an Internet Relay Chat (IRC) network.
    /// </summary>
    public class AlreadyRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.AlreadyRegisteredException"/> class.
        /// </summary>
        public AlreadyRegisteredException() : this(Properties.Resources.AlreadyRegisteredExceptionMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.AlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public AlreadyRegisteredException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.AlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public AlreadyRegisteredException(string message, Exception innerException) : base(message, innerException) { }
    }
}
