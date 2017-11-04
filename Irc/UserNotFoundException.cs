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
    /// Represents the error that occurs when a user is not found when performing an 
    /// action on a user.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>
        /// Gets the nickname of the missing user.
        /// </summary>
        public IrcNickname Nickname { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        public UserNotFoundException() : base(Properties.Resources.UserNotFound) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user that could not be found.</param>
        public UserNotFoundException(IrcNickname nickname)
            : base(Properties.Resources.UserNotFound)
        {
            Nickname = nickname;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public UserNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user that could not be found.</param>
        /// <param name="message">A message that describes the error.</param>
        public UserNotFoundException(IrcNickname nickname, string message)
            : base(message)
        {
            Nickname = nickname;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserNotFoundException"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user that could not be found.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference is no inner exception is specified.</param>
        public UserNotFoundException(IrcNickname nickname, string message, Exception innerException)
            : base(message, innerException)
        {
            Nickname = nickname;
        }
    }
}
