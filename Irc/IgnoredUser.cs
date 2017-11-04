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
    /// Represents a user on an Internet Relay Chat (IRC) network that the client is ignoring 
    /// messages from.
    /// </summary>
    public sealed class IgnoredUser
    {
        /// <summary>
        /// Gets or sets the nickname or host mask of the user.
        /// </summary>
        public string Mask { get; private set; }

        /// <summary>
        /// Gets the name of the network the user is on.
        /// </summary>
        public string NetworkName { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Irc.IIrcxProtocol"/> object that the ignored user is connected to.
        /// </summary>
        private IIrcxProtocol Connection { get; set; }

        /// <summary>
        /// Initializes a new instance of the Relirc.IgnoredUser class.
        /// </summary>
        public IgnoredUser(string mask, string networkName, IIrcxProtocol connection)
        {
            if (string.IsNullOrWhiteSpace(mask)) { throw new ArgumentNullException(nameof(mask)); }
            if (string.IsNullOrWhiteSpace(networkName)) { throw new ArgumentNullException(nameof(networkName)); }
            if (connection == null) { throw new ArgumentNullException(nameof(connection)); }

            Mask = mask;
            NetworkName = networkName;
            Connection = connection;
        }

        /// <summary>
        /// Removes the ignore from the client's ignore list.
        /// </summary>
        public void Remove()
        {
            Connection.Unignore(Mask);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Irc.IgnoredUser"/>
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return (string.Format("{0} ({1})", Mask, NetworkName));
        }
    }
}
