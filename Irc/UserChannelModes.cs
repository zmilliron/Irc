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
    /// Defines the values that specify the channel status of a user.
    /// </summary>
    [Flags]
    public enum UserChannelModes
    {
        /// <summary>
        /// A public user.
        /// </summary>
        None = 0,

        /// <summary>
        /// The user may speak in a moderated channel.
        /// </summary>
        Voice = 1,

        /// <summary>
        /// The user may change the channel topic, kick users with half-operator status or 
        /// lower, ban users with half-operator status or lower, and give Voice status to 
        /// any user.
        /// </summary>
        HalfOperator = 2,

        /// <summary>
        /// The user may change all channel and user modes.
        /// </summary>
        Operator = 4,

        /// <summary>
        /// The user may not be removed from the channel.
        /// </summary>
        Protected = 8,

        /// <summary>
        /// The user owns channel.
        /// </summary>
        Owner = 16
    }
}
