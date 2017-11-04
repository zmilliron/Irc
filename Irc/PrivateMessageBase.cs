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
    /// Provides an implementation base for sending an receiving private messages 
    /// on an Internet Relay Chat (IRC) network.
    /// </summary>
    public abstract class PrivateMessageBase : IDisposable
    {

        private bool _isDisposed;
        private IIrcxProtocol _client;
        private string _network;

        #region Properties...

        /// <summary>
        /// Gets the connection to the IRC network the private message belongs to.
        /// </summary>
        protected IIrcxProtocol Client
        {
            get { return (_client); }
            set
            {
                _client = value;
                OnConnectionChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating if this object instance has been disposed of.
        /// </summary>
        protected bool IsDisposed
        {
            get { return (_isDisposed); }
        }

        /// <summary>
        /// Gets the name of the remote target of private messages.
        /// </summary>
        public IrcNameBase TargetName { get; protected set; }

        /// <summary>
        /// Gets the name of the network the private message belongs to.
        /// </summary>
        public string NetworkName
        {
            get { return (_network); }
            set
            {
                if (_network != value)
                {
                    _network = value;
                }
            }
        }

        #endregion

        #region Events...

        /// <summary>
        /// Occurs when a new chat message has arrived.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> NewMessage;

        /// <summary>
        /// Occurs when a new user emote has arrived.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> NewEmote;

        #endregion

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_isDisposed)
            {
                NewEmote = null;
                NewMessage = null;
                Client.PrivateMessageReceived -= new EventHandler<PrivateMessageEventArgs>(OnPrivateMessageReceived);
                Client.CtcpReceived -= new EventHandler<Ctcp.CtcpEventArgs>(OnCtcpReceived);

                _isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Emotes to the remote user.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if message is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the object has been disposed of.</exception>
        public virtual void Emote(string message)
        {
            if (_isDisposed) { throw new ObjectDisposedException(this.GetType().ToString()); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }         

            Client.CtcpAction(TargetName, message);

            CtcpEventArgs pmea = new CtcpEventArgs(Client.Nickname, TargetName)
            {
                UserName = Client.Username,
                RealName = Client.RealName,
                Message = message,
                IsLocalEcho = true
            };

            OnNewEmote(pmea);
        }

        /// <summary>
        /// Processes internal changes when the Connection property has changed.
        /// </summary>
        private void OnConnectionChanged()
        {
            if (Client != null)
            {
                Client.PrivateMessageReceived += new EventHandler<PrivateMessageEventArgs>(OnPrivateMessageReceived);
                Client.CtcpReceived += new EventHandler<Ctcp.CtcpEventArgs>(OnCtcpReceived);
            }
        }

        /// <summary>
        /// Event handling method for <see cref="Irc.Ctcp.ICtcp.CtcpReceived"/> event.  To be
        /// overridden in derived classes.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.Ctcp.ICtcp"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected abstract void OnCtcpReceived(object sender, CtcpEventArgs e);

        /// <summary>
        /// Raises the NewEmote event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnNewEmote(CtcpEventArgs e)
        {
            if (e != null)
            {
                NewEmote?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Raises the NewMessage event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnNewMessage(PrivateMessageEventArgs e)
        {
            if (e != null)
            {
                NewMessage?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for <see cref="Irc.IRfc2812.PrivateMessageReceived"/> event to be
        /// overridden in derived classes.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected abstract void OnPrivateMessageReceived(object sender, PrivateMessageEventArgs e);

        /// <summary>
        /// Sends a message to the remote user.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if message is null.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the object has been disposed of.</exception>
        public virtual void SendMessage(string message)
        {
            if (_isDisposed) { throw new ObjectDisposedException(this.GetType().ToString()); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }

            Client.SendMessage(TargetName, message);

            PrivateMessageEventArgs pmea = new PrivateMessageEventArgs(Client.Nickname, TargetName)
            {
                UserName = Client.Username,
                RealName = Client.RealName,
                Message = message,
                IsLocalEcho = true
            };

            OnNewMessage(pmea);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="PrivateMessageBase"/> instance.
        /// </summary>
        /// <returns>A string representation of this Channel instance.</returns>
        public override string ToString()
        {
            return (TargetName.ToString());
        }
    }
}
