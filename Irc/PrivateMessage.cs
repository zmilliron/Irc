/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using Irc.Ctcp;
using System;

namespace Irc
{
    /// <summary>
    /// Represents a private message session with another user on an Internet Relay Chat (IRC) server.
    /// </summary>
    public sealed class PrivateMessage : PrivateMessageBase
    {
        #region Properties...

        /// <summary>
        /// Gets the message that opened the private message session.
        /// </summary>
        public PrivateMessageEventArgs OpeningMessage { get; private set; }

        /// <summary>
        /// Gets the nickname of the remote user that messages are being sent to.
        /// </summary>
        public IrcNickname Name
        {
            get { return (base.TargetName as IrcNickname); }
            private set { base.TargetName = value; }
        }

        /// <summary>
        /// Gets the remote user private messages are being sent to.
        /// </summary>
        public User RemoteUser { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.PrivateMessage"/> class.
        /// </summary>
        /// <param name="user">The remote user the message is open with.</param>
        /// <param name="connection">The server connection the message belongs to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if user or connection are null.</exception>
        public PrivateMessage(User user, IIrcxProtocol connection)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            if (connection == null) { throw new ArgumentNullException(nameof(connection)); }

            Name = user.Nickname;
            RemoteUser = user;
            Client = connection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.PrivateMessage"/> class.
        /// </summary>
        /// <param name="user">The remote user the message is open with.</param>
        /// <param name="connection">The server connection the message belongs to.</param>
        /// <param name="incomingMessage">The incoming message that initiated the private message conversation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if user or connection are null.</exception>
        public PrivateMessage(User user, IIrcxProtocol connection, PrivateMessageEventArgs incomingMessage)
            : this(user, connection)
        {
            OpeningMessage = incomingMessage;
        }

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                RemoteUser.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IrcClient.CtcpReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected override void OnCtcpReceived(object sender, CtcpEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.MessageTarget is IrcNickname && e.CtcpCommand == CtcpCommands.ACTION)
            {
                if ((IrcNickname)e.MessageTarget == Client.Nickname && RemoteUser.Nickname == e.Nickname)
                {
                    OnNewEmote(e);
                }
            }
        }

        /// <summary>
        /// Event handling method for <see cref="Irc.IrcClient.PrivateMessageReceived"/>event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected override void OnPrivateMessageReceived(object sender, PrivateMessageEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.MessageTarget is IrcNickname)
            {
                if ((IrcNickname)e.MessageTarget == Client.Nickname && RemoteUser.Nickname == e.Nickname)
                {
                    OnNewMessage(e);
                }
            }
        }
    }
}
