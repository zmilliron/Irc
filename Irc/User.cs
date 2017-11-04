/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace Irc
{
    /// <summary>
    /// Represents a user on an Internet Relay Chat (IRC) network.
    /// </summary>
    public class User : INotifyPropertyChanged, IDisposable, IComparable, IComparable<User>
    {

        #region Fields...

        private bool _away;
        private string _awayMessage;
        private string _host;
        private IrcUsername _name;
        private IrcNickname _nick;
        private string _realName;
        private int _idleTime;
        private DateTime? _signOnTime;
        private bool _isNickRegistered;
        private bool _secureConnection;
        private DateTime? _lastWhoIsUpdate;
        private string _ircServer;
        private bool _disposed;
        private IIrcxProtocol _client;
        private ReadOnlyCollection<IrcChannelName> _channels;
        private bool _ircOp;
        private bool _helpOp;

        #endregion

        #region Properties...

        /// <summary>
        /// Gets or sets a method for handling cross-thread property updates.
        /// </summary>
        public Action<Action> CrossThreadInvoke { get; set; }

        /// <summary>
        /// Gets the IP or host address of the user.
        /// </summary>
        public string HostName
        {
            get { return (_host); }
            set
            {
                if (_host != value)
                {
                    _host = value;
                    OnPropertyChanged(nameof(HostName));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is an network help operator.
        /// </summary>
        public bool IsHelpOperator
        {
            get { return (_helpOp); }
            private set
            {
                if (_helpOp != value)
                {
                    _helpOp = value;
                    OnPropertyChanged(nameof(IsHelpOperator));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is a network operator.
        /// </summary>
        public bool IsIRCOperator
        {
            get { return (_ircOp); }
            private set
            {
                if (_ircOp != value)
                {
                    _ircOp = value;
                    OnPropertyChanged(nameof(IsIRCOperator));
                }
            }
        }

        /// <summary>
        /// Gets a message displayed when the user is away.
        /// </summary>
        public string AwayMessage
        {
            get { return (_awayMessage); }
            protected set
            {
                if (_awayMessage != value)
                {
                    _awayMessage = value;
                    OnPropertyChanged(nameof(AwayMessage));
                }
            }
        }

        /// <summary>
        /// Gets a list of channels the user is currently on.
        /// </summary>
        public ReadOnlyCollection<IrcChannelName> Channels
        {
            get
            {
                return (_channels);
            }
            private set
            {
                if (_channels != value)
                {
                    _channels = value;
                    OnPropertyChanged(nameof(Channels));
                }
            }
        }

        /// <summary>
        /// Gets the connection to the IRC network the user belongs to.
        /// </summary>
        public IIrcxProtocol Client
        {
            get { return (_client); }
            private set
            {
                _client = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is currently away from the keyboard.
        /// </summary>
        public bool IsAway
        {
            get { return (_away); }
            protected set
            {
                if (_away != value)
                {
                    _away = value;
                    OnPropertyChanged(nameof(IsAway));
                    OnAwayChanged();                   
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object instance has been disposed of.
        /// </summary>
        protected bool IsDisposed
        {
            get { return (_disposed); }
        }

        /// <summary>
        /// Gets the user's nickname.
        /// </summary>
        public IrcNickname Nickname
        {
            get { return (_nick); }
            private set
            {
                if (_nick != value)
                {
                    _nick = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }

        /// <summary>
        /// Gets the "real" name of the user.
        /// </summary>
        public string RealName
        {
            get { return (_realName); }
            set
            {
                if (_realName != value)
                {
                    _realName = value;
                    OnPropertyChanged(nameof(RealName));
                }
            }
        }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public IrcUsername UserName
        {
            get { return (_name); }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        /// <summary>
        /// Gets the amount of time, in seconds, the user has been idle on the network.
        /// </summary>
        public int IdleTime
        {
            get { return (_idleTime); }
            private set
            {
                if (_idleTime != value)
                {
                    _idleTime = value;
                    OnPropertyChanged(nameof(IdleTime));
                }
            }
        }

        /// <summary>
        /// Gets the name of the IRC server the user is connected to.
        /// </summary>
        public string IrcServerName
        {
            get { return (_ircServer); }
            private set
            {
                if (_ircServer != value)
                {
                    _ircServer = value;
                    OnPropertyChanged(nameof(IrcServerName));
                }
            }
        }

        /// <summary>
        /// Gets the date and time of when the last WHOIS command 
        /// was performed on the user.
        /// </summary>
        public DateTime? LastWhoIsUpdate
        {
            get
            {
                return (_lastWhoIsUpdate);
            }
            private set
            {
                if (_lastWhoIsUpdate != value)
                {
                    _lastWhoIsUpdate = value;
                    OnPropertyChanged(nameof(LastWhoIsUpdate));
                }
            }
        }

        /// <summary>
        /// Gets a timestamp of when the user signed on.
        /// </summary>
        public DateTime? SignOnTime
        {
            get { return (_signOnTime); }
            private set
            {
                if (_signOnTime != value)
                {
                    _signOnTime = value;
                    OnPropertyChanged(nameof(SignOnTime));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the user's nickname is registered 
        /// with the IRC network.
        /// </summary>
        public bool IsNicknameRegistered
        {
            get { return (_isNickRegistered); }
            private set
            {
                if (_isNickRegistered != value)
                {
                    _isNickRegistered = value;
                    OnPropertyChanged(nameof(IsNicknameRegistered));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the user is connected through a 
        /// Secure Sockets Layer (SSL) connection.
        /// </summary>
        public bool IsSecureConnection
        {
            get { return (_secureConnection); }
            private set
            {
                if (_secureConnection != value)
                {
                    _secureConnection = value;
                    OnPropertyChanged(nameof(IsSecureConnection));
                }
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.User"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        /// <param name="client">The connection to the network on which the user exists.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or connection is null.</exception>
        public User(IrcNickname nickname, IIrcxProtocol client)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (client == null) { throw new ArgumentNullException(nameof(client)); }

            Nickname = nickname;
            Client = client;
            Channels = new ReadOnlyCollection<IrcChannelName>(new IrcChannelName[0]);

            Client.WhoIsReceived += new EventHandler<WhoIsEventArgs>(OnWhoIsReceived);
            Client.NicknameChanged += new EventHandler<NickChangeEventArgs>(OnNicknameChanged);
            Client.UserIsAway += new EventHandler<AwayEventArgs>(OnUserIsAway);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="System.Object"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort 
        /// order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">An object that evalutes to a <see cref="Irc.User"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedoes, follow or occurs 
        /// in the same position in the sort order as the specified <see cref="Irc.User"/></returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.User"/>.</exception>
        public virtual int CompareTo(object obj)
        {
            if (!(obj is User)) { throw new ArgumentException("Object must be of type Irc.User."); }


            /**
             * If objects are equal: return 0
             * If this object occurs before the parameter: return less than 0
             * If this object occurs after the parameter: return greater than 0
             * 
             */

            int retVal = 1;
            if (obj != null)
            {
                User other = (User)obj;
                retVal = Nickname.CompareTo(other.Nickname);
            }

            return (retVal);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="System.Object"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort 
        /// order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="other">An object that evalutes to a <see cref="Irc.User"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedoes, follow or occurs 
        /// in the same position in the sort order as the specified <see cref="Irc.User"/></returns>
        public int CompareTo(User other)
        {
            return (CompareTo((object)other));
        }

        /// <summary>
        /// Releases resources used by the object.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                PropertyChanged = null;
                Client.WhoIsReceived -= new EventHandler<WhoIsEventArgs>(OnWhoIsReceived);
                Client.NicknameChanged -= new EventHandler<NickChangeEventArgs>(OnNicknameChanged);
                Client.UserIsAway -= new EventHandler<AwayEventArgs>(OnUserIsAway);
                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.UserIsAway"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnUserIsAway(object sender, AwayEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.Nickname == Nickname)
            {
                IsAway = true;
                AwayMessage = e.AwayMessage;
            }
        }

        /// <summary>
        /// Clears the AwayMessage property if the user is no longer away.
        /// </summary>
        private void OnAwayChanged()
        {
            if (!_away)
            {
                AwayMessage = string.Empty;
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.NicknameChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnNicknameChanged(object sender, NickChangeEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.Nickname == Nickname)
            {
                Nickname = e.NewNickname;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of a property that has changed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if propertyName is null or the empty string.</exception>
        protected void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) { throw new ArgumentNullException(nameof(propertyName)); }

            if (CrossThreadInvoke == null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                CrossThreadInvoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            }
            
        }

        /// <summary>
        /// Event handler for the <see cref="Irc.IRfc2812.WhoIsReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> connection raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnWhoIsReceived(object sender, WhoIsEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.Nickname == Nickname)
            {
                RealName = e.RealName;
                UserName = e.UserName;
                HostName = e.HostName;
                IdleTime = e.IdleTime;
                SignOnTime = e.SignOnTime;
                IsNicknameRegistered = e.IsNicknameRegistered;
                IsSecureConnection = e.IsSecureConnection;
                LastWhoIsUpdate = e.TimeStamp;
                IrcServerName = e.ServerInfo;
                IsIRCOperator = e.IsIRCOperator;
                IsHelpOperator = e.IsHelpOperator;
                if (e.ChannelList != null)
                {
                    Channels = new ReadOnlyCollection<IrcChannelName>(e.ChannelList);
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Irc.User"/> class.
        /// </summary>
        /// <returns>A string representation of the user.</returns>
        public override string ToString()
        {
            return (string.Format("{0} is {1}@{2} * {3}", _nick, _name, _host, _realName));
        }

        /// <summary>
        /// Requests information from the IRC network about the currently connected user.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the object has been disposed of.</exception>
        public void WhoIs()
        {
            if (_disposed) { throw new ObjectDisposedException(this.GetType().ToString()); }

            Client.WhoIs(Nickname);
        }

        /// <summary>
        /// Requests information from the IRC network about the previously connected user.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the object has been disposed of.</exception>
        public void WhoWas()
        {
            if (_disposed) { throw new ObjectDisposedException(this.GetType().ToString()); }

            Client.WhoWas(Nickname);
        }
    }
}
