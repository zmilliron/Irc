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
    /// Provides data for an event raised when a user has quit an IRC network.
    /// </summary>
    /// <seealso cref="Irc.Commands.QUIT"/>
    public class QuitEventArgs : UserEventArgs
    {
        /// <summary>
        /// Gets or sets a farewell message sent by the user quitting.
        /// </summary>
        public string FarewellMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.QuitEventArgs"/> class.
        /// </summary>
        /// <param name="quittingUser">The nickname of the user that has quit the network.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if quittingUser is null.</exception>
        public QuitEventArgs(IrcNickname quittingUser) :base (quittingUser){ }
    }
}
