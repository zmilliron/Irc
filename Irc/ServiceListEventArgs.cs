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
    /// Provides data for an event raised after an entry from an 
    /// Internet Relay Chat (IRC) network's service list has been received.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_SERVLIST"/>
    public class ServiceListEventArgs : IrcEventArgs
    {
        /// <summary>
        /// Gets the name of a service.
        /// </summary>
        public IrcNickname ServiceName { get; private set; }

        /// <summary>
        /// Gets or sets the server name a service is connected to.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the mask of a service.
        /// </summary>
        public string Mask { get; set; }

        /// <summary>
        /// Gets or sets a service type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the server hop count to a service.
        /// </summary>
        public int HopCount { get; set; }

        /// <summary>
        /// Gets or sets information about a service.
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ServiceListEventArgs"/> class.
        /// </summary>
        /// <param name="serviceName">The name of a service in the network service list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if serviceName is null.</exception>
        public ServiceListEventArgs(IrcNickname serviceName)
        {
            if (serviceName == null) { throw new ArgumentNullException(nameof(serviceName)); }

            ServiceName = serviceName;
        }
    }
}
