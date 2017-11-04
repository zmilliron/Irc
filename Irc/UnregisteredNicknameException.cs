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
    /// Represents the error that occurs when the client user cannot perform an action because 
    /// their nickname is not registered on an Internet Relay Chat (IRC) network.
    /// </summary>
    public class UnregisteredNicknameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnregisteredNicknameException"/> class.
        /// </summary>
        public UnregisteredNicknameException() : this(Properties.Resources.UnregisteredNicknameError) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnregisteredNicknameException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public UnregisteredNicknameException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UnregisteredNicknameException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public UnregisteredNicknameException(string message, Exception innerException) : base(message, innerException) { }
    }
}
