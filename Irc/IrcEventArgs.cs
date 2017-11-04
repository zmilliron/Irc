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
    /// Provides data for events on an Internet Relay Chat (IRC) network.
    /// </summary>
    public abstract class IrcEventArgs : EventArgs
    {
        private DateTime _timestamp = DateTime.Now;

        /// <summary>
        /// Gets or sets the TimeStamp of when the event occurred.
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                return (_timestamp);
            }
            set
            {
                _timestamp = value;
            }
        }
    }
}
