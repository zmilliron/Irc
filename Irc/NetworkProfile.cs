/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Irc
{
    /// <summary>
    /// Represents a set of user-defined settings to automatically apply when 
    /// connecting to a specific Internet Relay Chat (IRC) network.
    /// </summary>
    public sealed class NetworkProfile : INotifyPropertyChanged
    {
        private const int DEFAULTPORT = 6667;
        private const int DEFAULTSSLPORT = 6697;

        #region Fields...

        private IrcNickname _nick;
        private bool _acceptSSLCertificates;
        private object _pass;
        private bool _useSSL;
        private object _nickservPass;
        private string _network;
        private bool _hasPass;
        private bool _hasNickServPass;
        private int _port = DEFAULTPORT;
        private string _host;
        private Uri _uri;
        private List<IrcChannelName> _channels;
        private List<IrcNickname> _friends;
        private List<string> _ignores;

        #endregion

        #region Properties...

        /// <summary>
        /// Gets or sets the address of the network server.
        /// </summary>
        public string Address
        {
            get
            {
                return (_host);
            }
            set
            {
                if (value != _host)
                {
                    if (value != null)
                    {
                        value.Trim();
                    }

                    _host = string.IsNullOrEmpty(value) ? null : value;
                    OnPropertyChanged(nameof(Address));

                    if (_host != null)
                    {
                        _uri = new System.Uri(string.Format("{0}://{1}:{2}", UseSSL ? IrcUriSchemes.Ssl : IrcUriSchemes.Default, value, _port));
                    }
                    else
                    {
                        _uri = null;
                    }
                    OnPropertyChanged(nameof(Uri));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the client should automatically 
        /// accept SSL certificates from a server on the network.
        /// </summary>
        public bool AutoAcceptSslCertificates
        {
            get { return (_acceptSSLCertificates); }
            set
            {
                if (_acceptSSLCertificates != value)
                {
                    _acceptSSLCertificates = value;
                    OnPropertyChanged(nameof(AutoAcceptSslCertificates));
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of channels to automatically join upon connecting to 
        /// an IRC network.
        /// </summary>
        public ReadOnlyCollection<IrcChannelName> AutoJoinChannels { get; set; }

        /// <summary>
        /// Gets or sets a list of friends to monitor after connecting to an IRC network.
        /// </summary>
        public ReadOnlyCollection<IrcNickname> Friends { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile contains a password for 
        /// authenticating with the network.
        /// </summary>
        public bool HasPassword
        {
            get { return (_hasPass); }
            set
            {
                if (_hasPass != value)
                {
                    _hasPass = value;
                    OnPropertyChanged(nameof(HasPassword));
                    if (!_hasPass)
                    {
                        Password = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the profile contains as password for 
        /// identifying with NickServ.
        /// </summary>
        public bool HasNickServPassword
        {
            get { return (_hasNickServPass); }
            set
            {
                if (_hasNickServPass != value)
                {
                    _hasNickServPass = value;
                    OnPropertyChanged(nameof(HasNickServPassword));
                    if (!_hasNickServPass)
                    {
                        NickservPassword = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of users to ignore after connecting to an IRC network.
        /// </summary>
        public ReadOnlyCollection<string> IgnoredUsers { get; set; }

        /// <summary>
        /// Gets or sets the name of the network the profile applies to.
        /// </summary>
        public string NetworkName
        {
            get { return (_network); }
            set
            {
                if (value != _network)
                {
                    _network = value;
                    OnPropertyChanged(nameof(NetworkName));
                }
            }
        }

        /// <summary>
        /// Gets or sets a password to use when connecting to an IRC network.
        /// </summary>
        public object Password
        {
            get
            {
                return (_pass);
            }
            set
            {
                if (value != _pass)
                {
                    _pass = value;
                    OnPropertyChanged(nameof(Password));

                    HasPassword = _pass != null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the nickname to use on this network.
        /// </summary>
        public IrcNickname Nickname
        {
            get { return (_nick); }
            set
            {
                if (_nick != value)
                {
                    _nick = value;
                    OnPropertyChanged(nameof(Nickname));
                }
            }
        }

        /// <summary>
        /// Gets or sets a password to use when connecting to an IRC network.
        /// </summary>
        public object NickservPassword
        {
            get
            {
                return (_nickservPass);
            }
            set
            {
                if (value != _pass)
                {
                    _nickservPass = value;
                    OnPropertyChanged(nameof(NickservPassword));

                    HasNickServPassword = _nickservPass != null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the port to use when connecting to the remote server.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int Port
        {
            get
            {
                return (_port);
            }
            set
            {
                if (value < 1 || value > 65535) { throw new ArgumentOutOfRangeException(Properties.Resources.PortOutOfRangeError); }

                if (value != _port)
                {
                    _port = value;
                    OnPropertyChanged(nameof(Port));
                    if (_host != null)
                    {
                        _uri = new System.Uri(string.Format("{0}://{1}:{2}", UseSSL ? IrcUriSchemes.Ssl : IrcUriSchemes.Default, _host, _port));
                        OnPropertyChanged(nameof(Uri));
                    }
                    
                }
            }
        }

        /// <summary>
        /// Gets the server URI of the network.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return (_uri);
            }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged(nameof(Uri));

                    if (_uri != null)
                    {
                        _host = _uri.Host;
                        _port = _uri.Port;
                        OnPropertyChanged(nameof(Address));
                        OnPropertyChanged(nameof(Port));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if an SSL connection should be used with
        /// this profile.
        /// </summary>
        public bool UseSSL
        {
            get { return (_useSSL); }
            set
            {
                if (_useSSL != value)
                {
                    _useSSL = value;
                    OnPropertyChanged(nameof(UseSSL));

                    if (_useSSL)
                    {
                        AutoAcceptSslCertificates = false;

                        if (Port == DEFAULTPORT)
                        {
                            Port = DEFAULTSSLPORT;
                        }
                    }
                    else if (Port == DEFAULTSSLPORT)
                    {
                        Port = DEFAULTPORT;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.NetworkProfile"/> class.
        /// </summary>
        public NetworkProfile()
        {
            _channels = new List<IrcChannelName>();
            AutoJoinChannels = new ReadOnlyCollection<IrcChannelName>(_channels);

            _friends = new List<IrcNickname>();
            Friends = new ReadOnlyCollection<IrcNickname>(_friends);

            _ignores = new List<string>();
            IgnoredUsers = new ReadOnlyCollection<string>(_ignores);
        }

        /// <summary>
        /// Adds a channel to automatically join after connecting to a network.
        /// </summary>
        /// <param name="channelName">The name of the channel to join.</param>
        public void AddChannel(IrcChannelName channelName)
        {
            if (!_channels.Contains(channelName))
            {
                _channels.Add(channelName);
            }
        }

        /// <summary>
        /// Adds a friend to automatically register in the friends list after connecting to a network.
        /// </summary>
        /// <param name="nickname">The nickname of the friend to add.</param>
        public void AddFriend(IrcNickname nickname)
        {
            if (!_friends.Contains(nickname))
            {
                _friends.Add(nickname);
            }
        }

        /// <summary>
        /// Adds a name or hostmask to register in the ignore list after connecting to a network.
        /// </summary>
        /// <param name="ignoreMask"></param>
        public void AddIgnore(string ignoreMask)
        {
            if (!_ignores.Contains(ignoreMask))
            {
                _ignores.Add(ignoreMask);
            }
        }

        /// <summary>
        /// Returns a value indicating if this network profile contains a valid name and address.
        /// </summary>
        /// <returns>True if the profile has a name and address set, false otherwise.</returns>
        public bool IsValid()
        {
            bool retVal = !string.IsNullOrWhiteSpace(NetworkName) &&
                          !string.IsNullOrWhiteSpace(Address) &&
                          NetworkName != Properties.Resources.NewServer;
            return (retVal);
        }

        /// <summary>
        /// Removes a channel to automatically join from this profile.
        /// </summary>
        /// <param name="channelName">The name of the channel to remove.</param>
        public void RemoveChannel(IrcChannelName channelName)
        {
            _channels.Remove(channelName);
        }

        /// <summary>
        /// Removes a friend to automatically register in the friends list.
        /// </summary>
        /// <param name="nickname">The name of the friend to remove.  Sad face.</param>
        public void RemoveFriend(IrcNickname nickname)
        {
            _friends.Remove(nickname);
        }

        /// <summary>
        /// Removes an ignore to automatically register in the ignore list.
        /// </summary>
        /// <param name="ignoreMask">The name or hostmask to remove.</param>
        public void RemoveIgnore(string ignoreMask)
        {
            _ignores.Remove(ignoreMask);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns a string representation of the <see cref="NetworkProfile"/> class.
        /// </summary>
        /// <returns>A string representation of the profile.</returns>
        public override string ToString()
        {
            return (this.Uri.ToString());
        }
    } 
}
