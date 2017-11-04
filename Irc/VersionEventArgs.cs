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
    /// Provides data for an event raised after an IRC server version is received.
    /// </summary>
    /// <seealso cref="Irc.Commands.VERSION"/>
    public class VersionEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets or sets the version number of a server.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a server name.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets revision comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.VersionEventArgs"/> class.
        /// </summary>
        public VersionEventArgs() { }
    }
}
