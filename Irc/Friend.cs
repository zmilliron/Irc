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
    /// Represents a user on an Internet Relay Chat (IRC) network that the client would
    /// like to monitor.
    /// </summary>
    public sealed class Friend : User, IComparable<Friend>
    {
        private string _network;
        private bool _online;

        /// <summary>
        /// Gets the default string reprsentation to display in a UI.
        /// </summary>
        public string DisplayName { get { return (string.Format("{0} ({1})", Nickname, IsOnline ? "Online" : "Offline")); } }

        /// <summary>
        /// Gets the online status of the user.
        /// </summary>
        public bool IsOnline
        {
            get { return (_online); }
            internal set
            {
                if (_online != value)
                {
                    _online = value;
                    OnIsOnlineChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the IRC network the user belongs to.
        /// </summary>
        public string NetworkName
        {
            get { return (_network); }
            internal set
            {
                if (_network != value)
                {
                    _network = value;
                    OnPropertyChanged(nameof(NetworkName));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Friend"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        /// <param name="client">A client connection to an IRC network.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or connection is null.</exception>
        public Friend(IrcNickname nickname, IIrcxProtocol client) : base(nickname, client)
        {
            Client.FriendSignedOn += new EventHandler<WatchEventArgs>(OnUserSignedOn);
            Client.FriendSignedOff += new EventHandler<WatchEventArgs>(OnUserSignedOff);
            Client.ConnectionStateChanged += new EventHandler(OnConnectionStateChanged);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="System.Object"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort 
        /// order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="other">An object that evalutes to a <see cref="Irc.Friend"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedoes, follow or occurs 
        /// in the same position in the sort order as the specified <see cref="Irc.Friend"/></returns>
        public int CompareTo(Friend other)
        {
            int retVal = 1;
            if (other != null)
            {
                //if the other user is offline and this user is online, this user comes first
                if (!other.IsOnline && IsOnline)
                {
                    retVal = -1;
                }
                //if the online statuses are equal, compare names
                else if (other.IsOnline == IsOnline)
                {
                    base.CompareTo(other);
                }
            }
            return (retVal);
        }

        /// <summary>
        /// Releases resources used by the object.
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Client.FriendSignedOn -= new EventHandler<WatchEventArgs>(OnUserSignedOn);
                Client.FriendSignedOff -= new EventHandler<WatchEventArgs>(OnUserSignedOff);
                Client.ConnectionStateChanged -= new EventHandler(OnConnectionStateChanged);
            }

            base.Dispose();
        }

        /// <summary>
        /// Event handler for <see cref="Irc.IRfc2812.ConnectionStateChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnConnectionStateChanged(object sender, EventArgs e)
        {
            if (Client.ConnectionState == ConnectionStates.Disconnected)
            {
                IsOnline = false;
            }
        }

        private void OnIsOnlineChanged()
        {
            OnPropertyChanged(nameof(IsOnline));
            OnPropertyChanged(nameof(DisplayName));
        }

        /// <summary>
        /// Event handler for <see cref="Irc.IIrcxProtocol.FriendSignedOn"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if e is null.</exception>
        private void OnUserSignedOn(object sender, WatchEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.Nickname == Nickname)
            {
                IsOnline = true;
            }
        }

        /// <summary>
        /// Event handler for <see cref="Irc.IIrcxProtocol.FriendSignedOff"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if e is null.</exception>
        private void OnUserSignedOff(object sender, WatchEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.Nickname == Nickname)
            {
                IsOnline = false;
            }
        }

        /// <summary>
        /// Removes this friend from the client's friend list.
        /// </summary>
        public void Remove()
        {
            Client.RemoveFriend(Nickname);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Irc.Friend"/> instance.
        /// </summary>
        /// <returns>A string representation of the current <see cref="Irc.Friend"/> instance.</returns>
        public override string ToString()
        {
            return (string.Format("{0} ({1})", Nickname, IsOnline ? "Online" : "Offline"));
        }
    }
}
