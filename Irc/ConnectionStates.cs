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
    /// Specifies the possible states of an Internet Relay Chat (IRC) connection.
    /// </summary>
    public enum ConnectionStates
    {
        /// <summary>
        /// The client is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// The client is currently connecting to a server.
        /// </summary>
        Connecting,
        /// <summary>
        /// The client is connected with a server but has not yet completed the registration process.
        /// </summary>
        Connected,
        /// <summary>
        /// The client is connected to and registered with a server and may now begin communicating.
        /// </summary>
        Registered
    }
}
