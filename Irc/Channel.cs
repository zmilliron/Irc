/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using Irc.Ctcp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Irc
{
    /// <summary>
    /// Represents a chat channel on an Internet Relay Chat (IRC) network.
    /// </summary>
    public sealed class Channel : PrivateMessageBase
    {
        private List<string> _banExceptions;
        private List<string> _bans;
        private ChannelUser _clientUser;
        private List<string> _invitationExceptions;
        private object _lock = new object();
        private Dictionary<IrcNickname, ChannelUser> _users;
        private bool _isActive;

        #region Properties...

        /// <summary>
        /// Gets the list of ban exceptions for the channel.
        /// </summary>
        public ReadOnlyCollection<string> BanExceptionList { get; private set; }

        /// <summary>
        /// Gets the maximum number of items allowed in the ban exception list.
        /// </summary>
        public int BanExceptionListMaximum { get { return (Client.BanExceptionListLength); } }

        /// <summary>
        /// Gets the list of bans for the channel.
        /// </summary>
        public ReadOnlyCollection<string> BanList { get; private set; }

        /// <summary>
        /// Gets the maximum number of items allowed in the ban list.
        /// </summary>
        public int BanListMaximum { get { return (Client.BanListLength); } }

        /// <summary>
        /// Gets a value indicating whether the chanenl topic can be changed by the 
        /// client user.
        /// </summary>
        public bool CannotChangeTopic
        {
            get
            {
                return (_clientUser == null ? true : Client.ConnectionState != ConnectionStates.Registered ||
                        (IsTopicLocked && !_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator));
            }
        }

        /// <summary>
        /// Gets the client's user instance in the channel.
        /// </summary>
        public ChannelUser ClientUser { get { return (_clientUser); } }

        /// <summary>
        /// Gets the channel mode flood protection string.
        /// </summary>
        public string FloodProtectionString { get { return (Modes != null && Modes.Contains(ChannelModes.FloodProtected) ? Modes[Modes.IndexOf(ChannelModes.FloodProtected)].Parameter : null); } }

        /// <summary>
        /// Gets a value indicating if the channel has a password requirement to join.
        /// </summary>
        public bool HasPassword { get { return (Modes != null && Modes.Contains(ChannelModes.HasPassword)); } }

        /// <summary>
        /// Gets a value indicating if the channel has a maximum number of allowed users set.
        /// </summary>
        public bool HasUserLimit { get { return (Modes != null && Modes.Contains(ChannelModes.HasLimit)); } }

        /// <summary>
        /// Gets the address of the channel homepage.
        /// </summary>
        public string HomePage { get; private set; }

        /// <summary>
        /// Gets the list of masks that are exceptions to the invitation-only rule.
        /// </summary>
        public ReadOnlyCollection<string> InvitationExceptionList { get; private set; }

        /// <summary>
        /// Gets the maximum number of items allowed in the invitation exception list.
        /// </summary>
        public int InvitationExceptionListMaximum { get { return (Client.InviteExceptionListLength); } }

        /// <summary>
        /// Gets a value whether the client is currently active in the channel.  The client may be inactive if it has 
        /// disconnected from the network or been kicked.
        /// </summary>
        public bool IsActive
        {
            get { return (_isActive); }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if auditorium mode is set.
        /// </summary>
        public bool IsAuditoriumMode { get { return (Modes != null && Modes.Contains(ChannelModes.AuditoriumMode)); } }

        /// <summary>
        /// Gets a value indicating if nickname changes are blocked in the channel.
        /// </summary>
        public bool IsBlockingNickChanges { get { return (Modes != null && Modes.Contains(ChannelModes.BlockNicknameChange)); } }

        /// <summary>
        /// Gets a value indicating if the client user has half-operator status.
        /// </summary>
        public bool IsClientHalfOperator { get { return (_clientUser == null ? false : _clientUser.IsChannelOperator || _clientUser.IsHalfOperator); } }

        /// <summary>
        /// Gets a value indicating if the client user has operator status.
        /// </summary>
        public bool IsClientOperator { get { return (_clientUser == null ? false : _clientUser.IsChannelOperator); } }

        /// <summary>
        /// Gets a value indicating if the profanity filter is enabled.
        /// </summary>
        public bool IsFilteringProfanity { get { return (Modes != null && Modes.Contains(ChannelModes.ProfanityFilter)); } }

        /// <summary>
        /// Gets a value indicating if flood protection is enabled.
        /// </summary>
        public bool IsFloodProtected { get { return (Modes != null && Modes.Contains(ChannelModes.FloodProtected)); } }

        /// <summary>
        /// Gets a value indicating if client-to-client protocol messages are being ignored.
        /// </summary>
        public bool IsIgnoringCtcp { get { return (Modes != null && Modes.Contains(ChannelModes.IgnoreCtcp)); } }

        /// <summary>
        /// Gets a value indicating if the channel is ignoring messages from users
        /// outside the channel.
        /// </summary>
        public bool IsIgnoringExternalMessages { get { return (Modes != null && Modes.Contains(ChannelModes.IgnoreExternalMessages)); } }

        /// <summary>
        /// Gets a value indicating if the KNOCK command is being ignored.
        /// </summary>
        public bool IsIgnoringKnocks { get { return (Modes != null && Modes.Contains(ChannelModes.KnockDisabled)); } }

        /// <summary>
        /// Gets a value indicating if messages with mIRC colors are being ignored.
        /// </summary>
        public bool IsIgnoringMessagesWithMircColors { get { return (Modes != null && Modes.Contains(ChannelModes.IgnoreMessagesWithMircColors)); } }

        /// <summary>
        /// Gets a value indicating if notices are being ignored.
        /// </summary>
        public bool IsIgnoringNotices { get { return (Modes != null && Modes.Contains(ChannelModes.IgnoreNotices)); } }

        /// <summary>
        /// Gets a message indicating if the INVITE command is enabled.
        /// </summary>
        public bool IsInviteDisabled { get { return (Modes != null && Modes.Contains(ChannelModes.InviteDisabled)); } }

        /// <summary>
        /// Gets a value indicating if users can join by invitation-only.
        /// </summary>
        public bool IsInviteOnly { get { return (Modes != null && Modes.Contains(ChannelModes.InvitationOnly)); } }

        /// <summary>
        /// Gets a value indicating if the KICK command is disabled.
        /// </summary>
        public bool IsKickDisabled { get { return (Modes != null && Modes.Contains(ChannelModes.KickDisabled)); } }

        /// <summary>
        /// Gets a value indicating if the channel is moderated.
        /// </summary>
        public bool IsModerated { get { return (Modes != null && Modes.Contains(ChannelModes.Moderated)); } }

        /// <summary>
        /// Gets a value indicating if the channel is only allowing users with 
        /// registered nicknames to join.
        /// </summary>
        public bool IsOnlyAllowingRegisteredUsers { get { return (Modes != null && Modes.Contains(ChannelModes.RegisteredUsersOnly)); } }

        /// <summary>
        /// Gets a value indicating if the channel is only allowing users on an 
        /// SSL connection to join.
        /// </summary>
        public bool IsOnlyAllowingSslUsers { get { return (Modes != null && Modes.Contains(ChannelModes.SSLUsersOnly)); } }

        /// <summary>
        /// Gets a value indicating if the channel is private.
        /// </summary>
        public bool IsPrivate { get { return (Modes != null && Modes.Contains(ChannelModes.Private)); } }

        /// <summary>
        /// Gets a value indicating if the channel is registered with the network.
        /// </summary>
        public bool IsRegistered { get { return (Modes != null && Modes.Contains(ChannelModes.Registered)); } }

        /// <summary>
        /// Gets a value indicating if the channel is secret.
        /// </summary>
        public bool IsSecret { get { return (Modes != null && Modes.Contains(ChannelModes.Secret)); } }

        /// <summary>
        /// Gets a value indicating whether the channel is removing mIRC color 
        /// codes from messages before they are broadcast.
        /// </summary>
        public bool IsStrippingMircColors { get { return (Modes != null && Modes.Contains(ChannelModes.StripMircColors)); } }

        /// <summary>
        /// Gets a value indicating whether channel joins are throttled.
        /// </summary>
        public bool IsThrottlingJoins { get { return (Modes != null && Modes.Contains(ChannelModes.ThrottleJoins)); } }

        /// <summary>
        /// Gets a value indicating if the topic can only be changed by channel operators.
        /// </summary>
        public bool IsTopicLocked { get { return (Modes != null && Modes.Contains(ChannelModes.LockTopic)); } }

        /// <summary>
        /// Gets the current modes set for the channel.
        /// </summary>
        public ChannelModeString Modes { get; private set; }

        /// <summary>
        /// Gets the name of the channel.
        /// </summary>
        public IrcChannelName Name
        {
            get
            {
                return (base.TargetName as IrcChannelName);
            }
            private set { base.TargetName = value; }
        }

        /// <summary>
        /// Gets a value indicating whether only users with registered nicknames 
        /// may speak.
        /// </summary>
        public bool OnlyRegisteredUsersMaySpeak { get { return (Modes != null && Modes.Contains(ChannelModes.OnlyRegisteredUsersMaySpeak)); } }

        /// <summary>
        /// Gets the owner or creator of the channel.
        /// </summary>
        public IrcNickname Owner { get; private set; }

        /// <summary>
        /// Gets the channel password.
        /// </summary>
        public string Password { get { return (Modes != null && Modes.Contains(ChannelModes.HasPassword) ? Modes[Modes.IndexOf(ChannelModes.HasPassword)].Parameter : null); } }

        /// <summary>
        /// Gets the modes supported by this channel.
        /// </summary>
        public string SupportedModes { get { return (Client.SupportedChannelModes); } }

        /// <summary>
        /// Gets a value indicating if the channel supports ban exceptions.
        /// </summary>
        public bool SupportsBanExceptions { get { return (Client.SupportsBanExceptions); } }

        /// <summary>
        /// Gets a value indicating whether the channel supports invitation exceptions.
        /// </summary>
        public bool SupportsInvitationExceptions { get { return (Client.SupportsInviteExceptions); } }

        /// <summary>
        /// Gets the period time over which the join flood protection can 
        /// be triggered.
        /// </summary>
        public int ThrottledJoinDuration { get; private set; }

        /// <summary>
        /// Gets the number of joins allowed by an individual user over 
        /// ThrottledJoinDuration.
        /// </summary>
        public int ThrottledJoinLimit { get; private set; }

        /// <summary>
        /// Gets the mode string used to set the join throttle limit.
        /// </summary>
        public string ThrottledJoinString { get { return (string.Format("{0}:{1}", ThrottledJoinLimit, ThrottledJoinDuration)); } }

        /// <summary>
        /// Gets the channel topic.
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// Gets the nickname of the user that last set the channel topic.
        /// </summary>
        public string TopicAuthor { get; private set; }

        /// <summary>
        /// Gets the maximum number of users allowed in the channel.
        /// </summary>
        public int UserLimit { get { return (Modes != null && Modes.Contains(ChannelModes.HasLimit) ? Convert.ToInt32(Modes[Modes.IndexOf(ChannelModes.HasLimit)].Parameter) : -1); } }

        /// <summary>
        /// Gets a list of users currently in the channel.
        /// </summary>
        public ReadOnlyDictionary<IrcNickname, ChannelUser> Users { get; private set; }

        #endregion

        #region Events...

        /// <summary>
        /// Occurs when the client is no longer in the channel because of disconnection or kick.
        /// </summary>
        public event EventHandler IsActiveChanged;

        /// <summary>
        /// Occurs when the channel modes have channged.
        /// </summary>
        public event EventHandler<ChannelModeEventArgs> ModesChanged;

        /// <summary>
        /// Occurs when a channel notice is received.
        /// </summary>
        public event EventHandler<NoticeEventArgs> Notice;

        /// <summary>
        /// Occurs when the channel topic has changed.
        /// </summary>
        public event EventHandler<TopicChangeEventArgs> TopicChanged;

        /// <summary>
        /// Occurs when the channel topic has been received after joining a channel.
        /// </summary>
        public event EventHandler<TopicEventArgs> TopicReceived;

        /// <summary>
        /// Occurs when a user has joined the channel.
        /// </summary>
        public event EventHandler<JoinEventArgs> UserJoined;

        /// <summary>
        /// Occurs when a user is kicked from the channel.
        /// </summary>
        public event EventHandler<KickEventArgs> UserKicked;

        /// <summary>
        /// Occurs when a user has left the channel.
        /// </summary>
        public event EventHandler<PartEventArgs> UserLeft;

        /// <summary>
        /// Occurs when the complete user list has been received.
        /// </summary>
        public event EventHandler UserListReceived;

        /// <summary>
        /// Occurs when a user in the channel has changed their nickname.
        /// </summary>
        public event EventHandler<NickChangeEventArgs> UserNicknameChanged;

        /// <summary>
        /// Occurs when a user has quit the network.
        /// </summary>
        public event EventHandler<QuitEventArgs> UserQuit;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Channel"/> class.
        /// </summary>
        /// <param name="channelName">The name of the channel.</param>
        /// <param name="client">The client connection to the server the channel belongs to.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or connection are null.</exception>
        public Channel(IrcChannelName channelName, IIrcxProtocol client)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (client == null) { throw new ArgumentNullException(nameof(client)); }

            Name = channelName;
            Client = client;

            _users = new Dictionary<IrcNickname, ChannelUser>();
            Users = new ReadOnlyDictionary<IrcNickname, ChannelUser>(_users);

            _bans = new List<string>();
            BanList = new ReadOnlyCollection<string>(_bans);

            _invitationExceptions = new List<string>();
            InvitationExceptionList = new ReadOnlyCollection<string>(_invitationExceptions);

            _banExceptions = new List<string>();
            BanExceptionList = new ReadOnlyCollection<string>(_banExceptions);

            Client.ConnectionStateChanged += new EventHandler(OnConnectionStateChanged);
            Client.ChannelModeChanged += new EventHandler<ChannelModeEventArgs>(OnModeChanged);
            Client.ChannelTopicChanged += new EventHandler<TopicChangeEventArgs>(OnTopicChanged);
            Client.ChannelTopicReceived += new EventHandler<TopicEventArgs>(OnChannelTopicReceived);
            Client.UserJoinedChannel += new EventHandler<JoinEventArgs>(OnUserJoined);
            Client.UserKicked += new EventHandler<KickEventArgs>(OnUserKicked);
            Client.UserLeftChannel += new EventHandler<PartEventArgs>(OnUserLeft);
            Client.ChannelUserListItemReceived += new EventHandler<NameListEventArgs>(OnNameListUpdated);
            Client.BanListItemReceived += new EventHandler<ChannelManagementListEventArgs>(OnBanListItemReceived);
            Client.InvitationExceptionListItemReceived += new EventHandler<ChannelManagementListEventArgs>(OnInvitationExceptionListItemReceived);
            Client.BanExceptionListItemReceived += new EventHandler<ChannelManagementListEventArgs>(OnBanExceptionListItemReceived);
            Client.UserQuit += new EventHandler<QuitEventArgs>(OnUserQuit);
            Client.ChannelOwnerReceived += new EventHandler<ChannelCreatorEventArgs>(OnChannelOwnerReceived);
            Client.NicknameChanged += new EventHandler<NickChangeEventArgs>(OnUserNicknameChanged);
            Client.NoticeReceived += new EventHandler<NoticeEventArgs>(OnNoticeReceived);
            Client.TopicAuthorReceived += new EventHandler<TopicAuthorEventArgs>(OnTopicAuthorReceived);
            Client.ChannelModesReceived += new EventHandler<ChannelModeEventArgs>(OnChannelModesReceived);
            Client.EndOfChannelUserList += new EventHandler<DataEventArgs>(OnEndOfChannelUserList);
            Client.ChannelUrlReceived += new EventHandler<ChannelUrlEventArgs>(OnChannelUrlReceived);
            Client.Kicked += new EventHandler<KickEventArgs>(OnKicked);
            Client.ChannelJoined += new EventHandler<JoinEventArgs>(OnJoined);

            IsActive = true;
        }

        /// <summary>
        /// Adds a ban to the channel ban list.
        /// </summary>
        /// <param name="mask">The username or host mask to ban.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to ban.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum ban list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null or empty.</exception>
        public void AddBan(string mask)
        {
            if (!SupportedModes.Contains(ChannelModes.Ban)) { throw new NotSupportedException("Bans not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }
            if (Client.BanListLength > 0 && _bans.Count == Client.BanListLength) { throw new InvalidOperationException("Ban list limit reached."); }
            

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.Ban, true, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Adds a list of bans to the channel ban list.
        /// </summary>
        /// <param name="masks">A list of usernames or hostmasks to ban.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to ban.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum ban list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void AddBan(string[] masks)
        {
            if (!SupportedModes.Contains(ChannelModes.Ban)) { throw new NotSupportedException("Bans not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }
            if (Client.BanListLength > 0 && _bans.Count + masks.Length > Client.BanListLength) { throw new InvalidOperationException("Ban list limit reached."); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.Ban, true, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Adds a ban exception to the channel ban list.
        /// </summary>
        /// <param name="mask">The username or host mask to create a ban exception for.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to create ban exceptions.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum ban exception list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null or empty.</exception>
        public void AddBanException(string mask)
        {
            if (!Client.SupportsBanExceptions) { throw new NotSupportedException("Ban exceptions not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }
            if (_banExceptions.Count == Client.BanExceptionListLength) { throw new InvalidOperationException("Ban exception list limit reached."); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.BanException, true, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Adds a list of ban exceptions to the channel ban exception list.
        /// </summary>
        /// <param name="masks">A list of usernames or hostmasks to create ban exceptions for.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to create ban exceptions.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum ban exception list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void AddBanException(string[] masks)
        {
            if (!Client.SupportsBanExceptions) { throw new NotSupportedException("Ban exceptions not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }
            if (_banExceptions.Count + masks.Length > Client.BanExceptionListLength) { throw new InvalidOperationException("Ban exception list limit reached."); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.BanException, true, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Promotes a user to half-operator status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void AddHalfOperator(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.HalfOperator, true, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Promotes a list of users to half-operator status.
        /// </summary>
        /// <param name="nicknames">The list of users to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        public void AddHalfOperator(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.HalfOperator, true, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Adds an invitation exception to the channel invitation exception list.
        /// </summary>
        /// <param name="mask">The username or hostmake to create an invitation exception for.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to create invitation exceptions.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum invitation exception list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null or empty.</exception>
        public void AddInvitationException(string mask)
        {
            if (!Client.SupportsInviteExceptions) { throw new NotSupportedException("Invitation exceptions not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }
            if (_invitationExceptions.Count == Client.InviteExceptionListLength) { throw new InvalidOperationException("Invitation exception limit reached."); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.InvitationException, true, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Adds a list of invitation exceptions to the channel invitation exception list.
        /// </summary>
        /// <param name="masks">A list of usernames or hostmasks to create invitation exceptions for.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to create invitation exceptions.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the maximum invitation exception list size has been exceeded.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void AddInvitationException(string[] masks)
        {
            if (!Client.SupportsInviteExceptions) { throw new NotSupportedException("Invitation exceptions not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }
            if (_invitationExceptions.Count + masks.Length > Client.InviteExceptionListLength) { throw new InvalidOperationException("Invitation exception limit reached."); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.InvitationException, true, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Promotes a user to operator status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void AddOperator(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.ChannelOperator, true, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Promotes a list of users to operator status.
        /// </summary>
        /// <param name="nicknames">The list of users to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        public void AddOperator(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.ChannelOperator, true, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Changes the channel topic to the specified topic.
        /// </summary>
        /// <param name="newTopic">The topic to set.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if client does not have permission to change the topic or the connection is closed.</exception>
        public void ChangeTopic(string newTopic)
        {
            if (IsTopicLocked && !_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }

            Client.SetChannelTopic(Name, newTopic);
        }

        /// <summary>
        /// Clears the channel state.
        /// </summary>
        private void Clear()
        {
            Topic = string.Empty;
            Modes = null;

            foreach (ChannelUser u in _users.Values)
            {
                u.Dispose();
            }
            _users.Clear();
            _bans.Clear();
            _banExceptions.Clear();
            _invitationExceptions.Clear();

            IsActive = false;
        }

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                Client.ConnectionStateChanged -= new EventHandler(OnConnectionStateChanged);
                Client.ChannelModeChanged -= new EventHandler<ChannelModeEventArgs>(OnModeChanged);
                Client.ChannelTopicChanged -= new EventHandler<TopicChangeEventArgs>(OnTopicChanged);
                Client.ChannelTopicReceived -= new EventHandler<TopicEventArgs>(OnChannelTopicReceived);
                Client.UserJoinedChannel -= new EventHandler<JoinEventArgs>(OnUserJoined);
                Client.UserKicked -= new EventHandler<KickEventArgs>(OnUserKicked);
                Client.UserLeftChannel -= new EventHandler<PartEventArgs>(OnUserLeft);
                Client.ChannelUserListItemReceived -= new EventHandler<NameListEventArgs>(OnNameListUpdated);
                Client.BanListItemReceived -= new EventHandler<ChannelManagementListEventArgs>(OnBanListItemReceived);
                Client.InvitationExceptionListItemReceived -= new EventHandler<ChannelManagementListEventArgs>(OnInvitationExceptionListItemReceived);
                Client.BanExceptionListItemReceived -= new EventHandler<ChannelManagementListEventArgs>(OnBanExceptionListItemReceived);
                Client.UserQuit -= new EventHandler<QuitEventArgs>(OnUserQuit);
                Client.ChannelOwnerReceived -= new EventHandler<ChannelCreatorEventArgs>(OnChannelOwnerReceived);
                Client.NicknameChanged -= new EventHandler<NickChangeEventArgs>(OnUserNicknameChanged);
                Client.NoticeReceived -= new EventHandler<NoticeEventArgs>(OnNoticeReceived);
                Client.TopicAuthorReceived -= new EventHandler<TopicAuthorEventArgs>(OnTopicAuthorReceived);
                Client.ChannelModesReceived -= new EventHandler<ChannelModeEventArgs>(OnChannelModesReceived);
                Client.EndOfChannelUserList -= new EventHandler<DataEventArgs>(OnEndOfChannelUserList);
                Client.ChannelUrlReceived -= new EventHandler<ChannelUrlEventArgs>(OnChannelUrlReceived);
                Client.Kicked -= new EventHandler<KickEventArgs>(OnKicked);
                Client.ChannelJoined -= new EventHandler<JoinEventArgs>(OnJoined);

                ModesChanged = null;
                Notice = null;
                TopicChanged = null;
                TopicReceived = null;
                UserJoined = null;
                UserKicked = null;
                UserLeft = null;
                UserNicknameChanged = null;
                UserQuit = null;
                UserListReceived = null;
                IsActiveChanged = null;

                Clear();
            }
            base.Dispose();
        }

        /// <summary>
        /// Emotes in the channel.
        /// </summary>
        /// <param name="message">The emote message to send.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client is not currently connected to a server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if message is null or empty.</exception>
        /// <exception cref="System.ObjectDisposedException">Thrown if the object has been disposed of.</exception>
        public override void Emote(string message)
        {
            if (IsModerated && _clientUser.ChannelStatus == UserChannelModes.None) { throw new InvalidOperationException(Properties.Resources.NotVoicedError); }
            if (OnlyRegisteredUsersMaySpeak && !ClientUser.IsNicknameRegistered) { throw new InvalidOperationException(Properties.Resources.OnlyRegisteredSpeakError); }

            base.Emote(message);
        }

        /// <summary>
        /// Promotes a user to voiced status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void GiveVoice(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.Voice, true, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Promotes a list of users to voiced status.
        /// </summary>
        /// <param name="nicknames">The list of users to promote.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to promote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        /// <exception cref="System.FormatException">Thrown if the nickname is not in the correct format.</exception>
        public void GiveVoice(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.Voice, true, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Invites a user to join the channel.
        /// </summary>
        /// <param name="nickname">The nickname of the user to invite.</param>
        /// <exception cref="System.FormatException">Thrown if the nickname is not in the proper format.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void InviteUser(IrcNickname nickname)
        {
            if (IsInviteDisabled) { throw new InvalidOperationException(Properties.Resources.InviteCommandDisabled); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Client.Invite(Name, nickname);
        }

        /// <summary>
        /// Kicks a user out of the channel.
        /// </summary>
        /// <param name="nickname">The nickname of the user to kick.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to kick a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void KickUser(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (IsKickDisabled) { throw new InvalidOperationException(Properties.Resources.KickCommandDisabled); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            Client.Kick(Name, nickname);
        }

        /// <summary>
        /// Kicks a user out of the channnel for the specified reason.
        /// </summary>
        /// <param name="nickname">The nickname of the user to kick.</param>
        /// <param name="reason">An explanation of why the user was kicked.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to kick a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void KickUser(IrcNickname nickname, string reason)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (IsKickDisabled) { throw new InvalidOperationException(Properties.Resources.KickCommandDisabled); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            if (string.IsNullOrEmpty(reason))
            {
                KickUser(nickname);
            }
            else
            {
                Client.Kick(Name, nickname, reason);
            }
        }

        /// <summary>
        /// Leaves the channel.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if no connection is currently open.</exception>
        public void Leave()
        {
            Client.LeaveChannel(Name);
        }

        /// <summary>
        /// Leaves the channel.
        /// </summary>
        /// <param name="farewellMessage">A message to send upon leaving the channel.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if no connection is currently open.</exception>
        public void Leave(string farewellMessage)
        {
            Client.LeaveChannel(Name, farewellMessage);
        }

        /// <summary>
        /// Event event handling method for the <see cref="Irc.IRfc2812.BanExceptionListItemReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data.</param>
        private void OnBanExceptionListItemReceived(object sender, ChannelManagementListEventArgs e)
        {
            if (e.ChannelName == this.Name)
            {
                _banExceptions.Add(e.Mask);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.BanListItemReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data.</param>
        private void OnBanListItemReceived(object sender, ChannelManagementListEventArgs e)
        {
            if (e.ChannelName == this.Name)
            {
                _bans.Add(e.Mask);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelModeChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelModesReceived(object sender, ChannelModeEventArgs e)
        {
            /**
             * Perform null check on Modes property to avoid a problem with receiving channel modes 
             * twice.  Some servers send the channel modes when joining a channel and some don't.  To 
             * work around this, the client sends a request for the current channel modes regardless 
             * of whether they are automatically sent or not because the client has no way of knowing which 
             * server sends them automatically.  It is assumed if the channel mode string is non-null, 
             * then the server already sent the modes and so they will not be processed.  If this null 
             * check were ignored, the modes would be processed twice and the ModesChanged event would 
             * be raised twice, which could create problems.
             */
            if (e.ChannelName == Name && Modes == null)
            {
                OnModeChanged(sender, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelOwnerReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelOwnerReceived(object sender, ChannelCreatorEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                Owner = e.Nickname;
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelTopicReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelTopicReceived(object sender, TopicEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                Topic = e.Topic;
                TopicReceived?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IIrcxProtocol.ChannelUrlReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelUrlReceived(object sender, ChannelUrlEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                HomePage = e.Url;
            }
        }

        /// <summary>
        /// Event handling method  for the <see cref="Irc.IRfc2812.ConnectionStateChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data.</param>
        private void OnConnectionStateChanged(object sender, EventArgs e)
        {
            if (Client.ConnectionState == ConnectionStates.Disconnected)
            {
                Clear();
            }
        }

        /// <summary>
        /// Event handler for <see cref="Irc.Ctcp.ICtcp.CtcpReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.Ctcp.ICtcp"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected override void OnCtcpReceived(object sender, CtcpEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.MessageTarget is IrcChannelName && Name == e.MessageTarget && e.CtcpCommand == CtcpCommands.ACTION)
            {
                OnNewEmote(e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.EndOfChannelUserList"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnEndOfChannelUserList(object sender, DataEventArgs e)
        {
            if (Name == e.Data)
            {
                UserListReceived?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.InvitationExceptionListItemReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnInvitationExceptionListItemReceived(object sender, ChannelManagementListEventArgs e)
        {
            if (e.ChannelName == this.Name)
            {
                _invitationExceptions.Add(e.Mask);
            }
        }

        private void OnJoined(object sender, JoinEventArgs e)
        {
            if (Name == e.ChannelName && IsActive == false)
            {
                IsActive = true;
            }
        }

        private void OnKicked(object sender, KickEventArgs e)
        {
            if (this.Name == e.ChannelName)
            {
                Clear();
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelModeChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnModeChanged(object sender, ChannelModeEventArgs e)
        {
            if (e.ChannelName == Name && e.ModeString != null)
            {
                foreach (Mode mode in e.ModeString)
                {
                    if (!ChannelModes.UserModes.Contains(mode.ModeChar))
                    {
                        if (mode.IsAdded)
                        {
                            Modes += mode;
                        }
                        else
                        {
                            Modes = Modes.Remove(Modes.IndexOf(mode), 1);
                        }
                    }
                    ProcessMode(mode);
                }

                ModesChanged?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelUserListItemReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnNameListUpdated(object sender, NameListEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                foreach (NameListItem i in e.Users)
                {
                    ChannelUser u = new ChannelUser(i.Nickname, this, Client, i.Modes);
                    u.HostName = i.HostName;
                    u.UserName = i.Username;
                    _users.Add(u.Nickname, u);

                    if (u.Nickname == Client.Nickname)
                    {
                        _clientUser = u;
                    }
                }
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.NoticeReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnNoticeReceived(object sender, NoticeEventArgs e)
        {
            if (e.Target == Name || e.IsCtcpReply)
            {
                Notice?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handler for <see cref="Irc.IRfc2812.PrivateMessageReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        protected override void OnPrivateMessageReceived(object sender, PrivateMessageEventArgs e)
        {
            if (e == null) { throw new ArgumentNullException(nameof(e)); }

            if (e.MessageTarget is IrcChannelName && Name == e.MessageTarget)
            {
                OnNewMessage(e);
            }
        }

        /// <summary>
        /// Event handler for the <see cref="Irc.IIrcxProtocol.TopicAuthorReceived"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IIrcxProtocol"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnTopicAuthorReceived(object sender, TopicAuthorEventArgs e)
        {
            if (e.ChannelName == this.Name)
            {
                if (e.Author != null && e.TimeSet != DateTime.MinValue)
                {
                    TopicAuthor = string.Format(Properties.Resources.TopicInfo, e.Author, e.TimeSet);
                }
                else if (e.Author != null)
                {
                    TopicAuthor = string.Format(Properties.Resources.TopicAuthorOnly, e.Author);
                }
                else if (e.TimeSet != DateTime.MinValue)
                {
                    TopicAuthor = string.Format(Properties.Resources.TopicTimeOnly, e.TimeSet);
                }
                else
                {
                    TopicAuthor = Properties.Resources.TopicInfoUnavailable;
                }
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.ChannelTopicChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnTopicChanged(object sender, TopicChangeEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                Topic = e.Topic;
                TopicAuthor = string.Format(Properties.Resources.TopicInfo, e.Nickname, e.TimeStamp);
                TopicChanged?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.UserJoinedChannel"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnUserJoined(object sender, JoinEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                ChannelUser newUser = new ChannelUser(e.Nickname, this, Client);
                newUser.UserName = e.UserName;
                newUser.HostName = e.HostName;
                _users.Add(newUser.Nickname, newUser);

                UserJoined?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.UserKicked"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnUserKicked(object sender, KickEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                ChannelUser user = _users[e.UserKicked];
                _users.Remove(e.UserKicked);
                user.Dispose();

                UserKicked?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.UserLeftChannel"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event</param>
        private void OnUserLeft(object sender, PartEventArgs e)
        {
            if (e.ChannelName == Name)
            {
                ChannelUser user = _users[e.Nickname];
                _users.Remove(e.Nickname);
                user.Dispose();

                UserLeft?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.NicknameChanged"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> raising the event.</param>
        /// <param name="e">The event data data for the event.</param>
        private void OnUserNicknameChanged(object sender, NickChangeEventArgs e)
        {
            ChannelUser user = _users[e.Nickname];
            _users.Remove(e.Nickname);
            _users.Add(e.NewNickname, user);

            UserNicknameChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.UserQuit"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IRfc2812"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnUserQuit(object sender, QuitEventArgs e)
        {
            ChannelUser user = _users[e.Nickname];
            _users.Remove(user.Nickname);
            user.Dispose();

            UserQuit?.Invoke(this, e);
        }

        /// <summary>
        /// Processes a channel mode to set the channel state.
        /// </summary>
        /// <param name="mode">The mode to process</param>
        private void ProcessMode(Mode mode)
        {
            switch (mode)
            {
                case ChannelModes.ChannelOperator:
                    ChannelUser op = _users[new IrcNickname(mode.Parameter)];
                    op.IsChannelOperator = mode.IsAdded;
                    break;
                case ChannelModes.HalfOperator:
                    ChannelUser halfOp = _users[new IrcNickname(mode.Parameter)];
                    halfOp.IsHalfOperator = mode.IsAdded;
                    break;
                case ChannelModes.Voice:
                    ChannelUser vUser = _users[new IrcNickname(mode.Parameter)];
                    vUser.IsVoiced = mode.IsAdded;
                    break;
                case ChannelModes.Ban:
                    lock (_lock)
                    {
                        if (mode.IsAdded)
                        {
                            _bans.Add(mode.Parameter);
                        }
                        else
                        {
                            _bans.Remove(mode.Parameter);
                        }
                    }
                    break;
                case ChannelModes.BanException:
                    lock (_lock)
                    {
                        if (mode.IsAdded)
                        {
                            _banExceptions.Add(mode.Parameter);
                        }
                        else
                        {
                            _banExceptions.Remove(mode.Parameter);
                        }
                    }
                    break;
                case ChannelModes.InvitationException:
                    lock (_lock)
                    {
                        if (mode.IsAdded)
                        {
                            _invitationExceptions.Add(mode.Parameter);
                        }
                        else
                        {
                            _invitationExceptions.Remove(mode.Parameter);

                        }
                    }
                    break;
                case ChannelModes.ThrottleJoins:
                    if (mode.IsAdded)
                    {
                        string[] throttleParts = mode.Parameter.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        ThrottledJoinLimit = int.Parse(throttleParts[0]);
                        ThrottledJoinDuration = int.Parse(throttleParts[1]);
                    }
                    else
                    {
                        ThrottledJoinLimit = 0;
                        ThrottledJoinDuration = 0;
                    }
                    break;
                case ChannelModes.ChannelOwner:
                    ChannelUser qUser = _users[new IrcNickname(mode.Parameter)];
                    qUser.IsChannelOwner = mode.IsAdded;
                    break;
            }
        }

        /// <summary>
        /// Refreshes the ban exception list.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if no connection is currently open.</exception>
        public void RefreshBanExceptionList()
        {
            if (!Client.SupportsBanExceptions) { throw new NotSupportedException("Ban exception list not supported."); }
            Client.RequestChannelModeParameters(Name, ChannelModes.BanException);

            lock (_lock)
            {
                _banExceptions.Clear();
            }
        }

        /// <summary>
        /// Refreshes the ban list.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if no connection is currently open.</exception>
        public void RefreshBanList()
        {
            Client.RequestChannelModeParameters(Name, ChannelModes.Ban);

            lock (_lock)
            {
                _bans.Clear();
            }
        }

        /// <summary>
        /// Refreshes the invitation exception list.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if no connection is currently open.</exception>
        public void RefreshInvitationExceptionList()
        {
            if (!Client.SupportsInviteExceptions) { throw new NotSupportedException("Invitation exception list not supported."); }
            Client.RequestChannelModeParameters(Name, ChannelModes.InvitationException);

            lock (_lock)
            {
                _invitationExceptions.Clear();
            }
        }

        /// <summary>
        /// Removes a ban from the channel ban list.
        /// </summary>
        /// <param name="mask">The username or host mask of the ban to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove a ban.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null.</exception>
        public void RemoveBan(string mask)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.Ban, false, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Removes a list of bans from the channel ban list.
        /// </summary>
        /// <param name="masks">The list of bans to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove a ban.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void RemoveBan(string[] masks)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.Ban, false, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Removes a ban exception from the channel ban exception list.
        /// </summary>
        /// <param name="mask">The username or host mask of the ban to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove a ban.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null.</exception>
        public void RemoveBanException(string mask)
        {
            if (!Client.SupportsBanExceptions) { throw new NotSupportedException("Ban exception list not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.BanException, false, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Removes a list of ban exceptions from the channel ban exception list.
        /// </summary>
        /// <param name="masks">The list of ban exceptions to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove a ban exception.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void RemoveBanException(string[] masks)
        {
            if (!Client.SupportsBanExceptions) { throw new NotSupportedException("Ban exception list not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.BanException, false, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Demotes a user from half-operator status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void RemoveHalfOperator(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.HalfOperator, false, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Demotes a list of users from half-operator status.
        /// </summary>
        /// <param name="nicknames">The list of users to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        public void RemoveHalfOperator(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.HalfOperator, false, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Removes an invitation exception from the channel invitation exception list.
        /// </summary>
        /// <param name="mask">The username or host mask of the invitation exception to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove an invitation exception.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null.</exception>
        public void RemoveInvitationException(string mask)
        {
            if (!Client.SupportsInviteExceptions) { throw new NotSupportedException("Invitation exception list not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (string.IsNullOrEmpty(mask)) { throw new ArgumentNullException(nameof(mask)); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.InvitationException, false, mask));
            SetModes(modeString);
        }

        /// <summary>
        /// Removes a list of invitation exceptions from the channel invitation exception list.
        /// </summary>
        /// <param name="masks">The list of invitation exceptions to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to remove an invitation exception.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if masks is null.</exception>
        public void RemoveInvitationException(string[] masks)
        {
            if (!Client.SupportsInviteExceptions) { throw new NotSupportedException("Invitation exception list not supported."); }
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (masks == null) { throw new ArgumentNullException(nameof(masks)); }

            List<Mode> modes = new List<Mode>();
            foreach (string m in masks)
            {
                modes.Add(new Mode(ChannelModes.InvitationException, false, m));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Demotes a user from operator status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void RemoveOperator(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.ChannelOperator, false, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Demotes a list of users from operator status.
        /// </summary>
        /// <param name="nicknames">The list of users to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        public void RemoveOperator(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator) { throw new InvalidOperationException(Properties.Resources.OpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.ChannelOperator, false, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Demotes a user from voiced status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void RemoveVoice(IrcNickname nickname)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (!_users.ContainsKey(nickname)) { throw new UserNotFoundException(nickname); }

            ChannelModeString modeString = new ChannelModeString(new Mode(ChannelModes.Voice, false, nickname));
            SetModes(modeString);
        }

        /// <summary>
        /// Demotes a list of users from voiced status.
        /// </summary>
        /// <param name="nicknames">The list of users to demote.</param>
        /// <exception cref="Irc.UserNotFoundException">Thrown if a user with the specified nickname is not found in the channel.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have permission to demote a user.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null or any nickname is null.</exception>
        public void RemoveVoice(IrcNickname[] nicknames)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            List<Mode> modes = new List<Mode>();
            for (int i = 0; i < nicknames.Length; i++)
            {
                if (!_users.ContainsKey(nicknames[i])) { throw new UserNotFoundException(nicknames[i]); }
                modes.Add(new Mode(ChannelModes.Voice, false, nicknames[i]));
            }

            SetModes(new ChannelModeString(modes));
        }

        /// <summary>
        /// Sends a message to the channel.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the channel is moderated and the user does not have permission to speak.</exception>
        /// <exception cref="Irc.UnregisteredNicknameException">Thrown if the channel is only allowing registered users to speak and the client 
        /// does not have a registered nickname.</exception>
        public override void SendMessage(string message)
        {
            if (IsModerated && _clientUser.ChannelStatus == UserChannelModes.None) { throw new InvalidOperationException(Properties.Resources.NotVoicedError); }
            if (OnlyRegisteredUsersMaySpeak && !ClientUser.IsNicknameRegistered) { throw new UnregisteredNicknameException(); }

            base.SendMessage(message);
        }

        /// <summary>
        /// Sets the specified channel modes.
        /// </summary>
        /// <param name="modeString">The modes to set.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the client does not have 
        /// half-operator or operator status in the channel.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if modes is null.</exception>
        /// <exception cref="Irc.UnsupportedModeException">Thrown if modeString contains an unsuppoted channel mode.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        public void SetModes(ChannelModeString modeString)
        {
            if (!_clientUser.IsChannelOperator && !_clientUser.IsHalfOperator) { throw new InvalidOperationException(Properties.Resources.OpOrHalfOpStatusRequired); }
            if (modeString == null) { throw new ArgumentNullException(nameof(modeString)); }

            foreach (Mode m in modeString)
            {
                if (!SupportedModes.Contains(m.ModeChar)) { throw new UnsupportedModeException() { Mode = m }; }
            }

            Client.SetChannelModes(Name, modeString);
        }
    }
}
