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
    /// Provides data for an event raised by a user action.
    /// </summary>
    public class UserEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the nickname of a user.
        /// </summary>
        public IrcNickname Nickname { get; protected set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public IrcUsername UserName { get; set; }

        /// <summary>
        /// Gets or sets the hostname or IP of the user.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the real name of the user.
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.UserEventArgs"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of a user.</param>
        public UserEventArgs(IrcNickname nickname)
        {
            Nickname = nickname;
        }
    }
}
