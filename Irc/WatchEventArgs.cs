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
    /// Provides data for an event raised after a WATCH event is received from a server.
    /// </summary>
    /// <seealso cref="Irc.Commands.WATCH"/>
    /// <seealso cref="Irc.SupportedOptions.WATCH"/>
    public class WatchEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating if the user is online.
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.WatchEventArgs"/> class.
        /// </summary>
        /// <param name="watchedUser">The nickname of the user added or removed from
        /// the client's watch list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if watchedUser is null.</exception>
        public WatchEventArgs(IrcNickname watchedUser) : base(watchedUser) { }
    }
}
