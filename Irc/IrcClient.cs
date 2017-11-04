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
using Irc.Ctcp;
using Irc.Ctcp.Dcc;

namespace Irc
{
    /// <summary>
    /// Represents a client for communicating with an Internet Relay Chat (IRC) network.
    /// </summary>
    public class IrcClient : IDisposable, IIrcxProtocol, IDcc
    {     
        #region Fields...

        /// <summary>
        /// The default port for connecting to an IRC server.
        /// </summary>
        public const int DEFAULTPORT = 6667;

        /// <summary>
        /// The default port for an SSL connection to an IRC server.
        /// </summary>
        public const int DEFAULTSSLPORT = 6697;

        /// <summary>
        /// The maximum allowed message length, sans line terminator.
        /// </summary>
        private const int MAXMESSAGELENGTH = 510;

        private Dictionary<IrcChannelName, Channel> _channels;

        private IConnection _connection;

        private ConnectionStates _connectionState;

        private Dictionary<IrcNickname, Friend> _friends;

        private Dictionary<string, IgnoredUser> _ignores;

        private bool _isAway;

        private bool _isDisposed;

        private bool _isReconnecting;

        private Dictionary<IrcNickname, PrivateMessage> _messages;

        private ClientModeString _modes;

        private string _network;

        private System.Threading.Timer _reconnectTimer;

        private WhoIsEventArgs _whoIsResult;

        private Random rand = new Random();

        private bool _userDisconnect;

        private string _password;

        #endregion

        #region Properties...

        /// <summary>
        /// Gets the address of the server on which the connection was established.
        /// </summary>
        public string ActualAddress { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically accept channel invitations.
        /// </summary>
        public bool AutoAcceptChannelInvitations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically reconnect to server on disconnection.
        /// </summary>
        public bool AutoReconnect { get; set; }

        /// <summary>
        /// Gets or sets a message to set when the client status is set to away.
        /// </summary>
        public string AwayMessage { get; set; }

        /// <summary>
        /// Gets the maximum length of an away message allowed by the server,
        /// </summary>
        public int AwayMessageLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of ban exceptions per channel allowed by a server.
        /// </summary>
        public int BanExceptionListLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of bans per channel allowed by a server.
        /// </summary>
        public int BanListLength { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports messaging IRCOps.
        /// </summary>
        public bool CanMessageOperators { get; private set; }

        /// <summary>
        /// Gets the case mapping used by the server for nickname and channel name comparisons.
        /// </summary>
        public string CaseMapping { get; private set; }

        /// <summary>
        /// Gets the maximum number of channels a client can join on a server.
        /// </summary>
        public int ChannelLimit { get; private set; }

        /// <summary>
        /// Gets the list of supported channel modes that require a parameter when being set.
        /// </summary>
        public string ChannelModesParametersWhenSet { get; private set; }

        /// <summary>
        /// Gets a list of supported channel modes that always require a parameter.
        /// </summary>
        public string ChannelModesAlwaysParameters { get; private set; }

        /// <summary>
        /// Gets a list of supported channel modes that never require a parameter.
        /// </summary>
        public string ChannelModesNoParameters { get; private set; }

        /// <summary>
        /// Gets the maximum channel name length allowed by a server.
        /// </summary>
        public int ChannelNameLength { get; private set; }

        /// <summary>
        /// Gets a list of channels the client is currently connected to.
        /// </summary>
        public ReadOnlyDictionary<IrcChannelName, Channel> Channels { get; private set; }

        /// <summary>
        /// Gets the address of the server for which a connection was requested.
        /// </summary>
        public string ConnectedAddress { get { return (Connection.RemoteAddress); } }

        /// <summary>
        /// Gets the underlying IRC connection.
        /// </summary>
        public IConnection Connection
        {
            get { return (_connection); }
        }

        /// <summary>
        /// Gets the current state of the IRC connection.
        /// </summary>
        public ConnectionStates ConnectionState
        {
            get { return (_connectionState); }
            private set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    OnConnectionStateChanged();
                }
            }
        }

        /// <summary>
        /// Gets a  collection of users for which the client is receiving connection and disconnection 
        /// notifications from the network.
        /// </summary>
        public ReadOnlyDictionary<IrcNickname, Friend> Friends { get; private set; }

        /// <summary>
        /// Gets a collection of users or address ranges that the client is ignoring.
        /// </summary>
        public ReadOnlyDictionary<string, IgnoredUser> Ignores { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Direct-Client-Connection (DCC) requests should be ignored.
        /// </summary>
        public bool IgnoreDccChatRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Direct-Client-Connection (DCC) requests should be ignored.
        /// </summary>
        public bool IgnoreDccSendRequests { get; set; }

        /// <summary>
        /// Gets the maximum number of invite exceptions per channel allowed by a server.
        /// </summary>
        public int InviteExceptionListLength { get; private set; }

        /// <summary>
        /// Gets a value indicating if the client status is set to away.
        /// </summary>
        public bool IsAway
        {
            get { return (_isAway); }
            private set
            {
                if (_isAway != value)
                {
                    _isAway = value;
                    IsAwayChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the client is an IRC operator.
        /// </summary>
        public bool IsIrcOperator { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client is currently reconnect.
        /// </summary>
        public bool IsReconnecting
        {
            get { return (_isReconnecting); }
            private set
            {
                if (_isReconnecting != value)
                {
                    _isReconnecting = value;
                    if (_isReconnecting)
                    {
                        _reconnectTimer.Change(10000, 10000);
                    }
                    else
                    {
                        _reconnectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }

                    ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if the client is using an SSL connection to a server.
        /// </summary>
        public bool IsUsingSSL { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL certificates should be validated.
        /// </summary>
        public bool IsValidatingSSLCertificates { get; set; }

        /// <summary>
        /// Gets the maximum length of a message sent with a KICK command allowed by a server.
        /// </summary>
        public int KickMessageLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of changes a server supports with a single channel MODE command.
        /// </summary>
        public int MaximumChannelModeChanges { get; private set; }

        /// <summary>
        /// Gets the maximum number of friends a client can add to a friends list.
        /// </summary>
        public int MaximumFriendsListLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of ignores a client can set on the server.
        /// </summary>
        public int MaximumIgnoreListLength { get; private set; }

        /// <summary>
        /// Gets the maximum number of targets a server suppots with a single PRIVMSG command.
        /// </summary>
        public int MaximumMessageTargets { get; private set; }

        /// <summary>
        /// Gets the maximum length of a nickname allowed by a server.
        /// </summary>
        public int MaximumNicknameLength { get; private set; }

        /// <summary>
        /// Gets the number of channel modes that can be set with a single MODE command.
        /// </summary>
        public int ModesPerCommand { get; private set; }

        /// <summary>
        /// Gets the current modes set by the client.
        /// </summary>
        public ClientModeString ModeString
        {
            get { return (_modes); }
            private set
            {
                if (_modes != value)
                {
                    _modes = value;
                    ClientModesChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the name of the network the connected server belongs to.
        /// </summary>
        public string NetworkName
        {
            get
            {
                return (_network);
            }
            private set
            {
                _network = value;
                NetworkIdentified?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the client's nickname.
        /// </summary>
        public IrcNickname Nickname { get; private set; }

        /// <summary>
        /// Gets or sets a message to print when leaving a channel.
        /// </summary>
        public string PartMessage { get; set; }

        /// <summary>
        /// Gets or sets the port on which to open an IRC connection.
        /// </summary>
        public int Port
        {
            get { return (_connection.Port); }
        }

        /// <summary>
        /// Gets a list of private messages currently open with the client.
        /// </summary>
        public ReadOnlyDictionary<IrcNickname, PrivateMessage> PrivateMessages { get; private set; }

        /// <summary>
        /// Gets or sets a message to print when quitting a server.
        /// </summary>
        public string QuitMessage { get; set; }

        /// <summary>
        /// Gets the client's real name.
        /// </summary>
        public string RealName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the client should rejoin a channel when kicked.
        /// </summary>
        public bool RejoinOnKick { get; set; }

        /// <summary>
        /// Gets the name and version of the IRC server the client is connected to.
        /// </summary>
        public string ServerVersion { get; private set; }

        /// <summary>
        /// Gets the list of channel modes the server supports, one for each character in the string.
        /// </summary>
        public string SupportedChannelModes { get; private set; }

        /// <summary>
        /// Gets a list channel types the server supports, one for each character in the string.
        /// </summary>
        public string SupportedChannelTypes { get; private set; }

        /// <summary>
        /// Gets the list of client modes the server supports, one for each character in the string.
        /// </summary>
        public string SupportedClientModes { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports ban exceptions.
        /// </summary>
        public bool SupportsBanExceptions { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports server-side ignores via the +g client mode.
        /// </summary>
        public bool SupportsCallerId { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports the <see cref="Irc.Commands.CNOTICE"/> command.
        /// </summary>
        public bool SupportsCNotice { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports the <see cref="Irc.Commands.CPRIVMSG"/> command.
        /// </summary>
        public bool SupportsCPrivMsg { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server can force the client to change its 
        /// nickname.
        /// </summary>
        public bool SupportsForcedNicknameChanges { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports friends lists.
        /// </summary>
        public bool SupportsFriendsList { get { return (SupportsWatch || SupportsMonitor); } }

        /// <summary>
        /// Gets a value indicating if the server supports ignore lists.
        /// </summary>
        public bool SupportsIgnoreList { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports invite exceptions.
        /// </summary>
        public bool SupportsInviteExceptions { get; private set; }

        /// <summary>
        /// Gets a value indicating if the server supports the KNOCK command.
        /// </summary>
        public bool SupportsKnock { get; private set; }

        /// <summary>
        /// Gets a value indicating if the network supports channels local to a single server.
        /// </summary>
        public bool SupportsLocalChannels { get; private set; }

        /// <summary>
        /// Gets a value indicating if the network supports channels without modes.
        /// </summary>
        public bool SupportsModelessChannels { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the MONITOR command is supported.
        /// </summary>
        private bool SupportsMonitor { get; set; }

        /// <summary>
        /// Gets a value indicating if the network supports RFC2812.
        /// </summary>
        public bool SupportsRFC2812 { get; private set; }

        /// <summary>
        /// Gets a value indicating if the network supports safe channels.
        /// </summary>
        public bool SupportsSafeChannels { get; private set; }

        /// <summary>
        /// Gets a value indicating if results from the LIST command are sent in iterations to prevent flooding the client.
        /// </summary>
        public bool SupportsSafeList { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the server supports sending mass-notices to channel users 
        /// with a specified status.
        /// </summary>
        public bool SupportsStatusMessaging { get; private set; }

        /// <summary>
        /// Gets a value indicating if a server supports the USERIP command.
        /// </summary>
        public bool SupportsUserIPCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the WATCH command is supported.
        /// </summary>
        private bool SupportsWatch { get; set; }

        /// <summary>
        /// Gets a value indicating if the server supports the WHOX extension for WHOIS queries.
        /// </summary>
        public bool SupportsWhoIsExtensions { get; private set; }

        /// <summary>
        /// Gets the amount of time the client must wait before sending commands to the server.
        /// </summary>
        public int TimeBetweenCommands { get; private set; }

        /// <summary>
        /// Gets the maximum channel topic length allowed by a server.
        /// </summary>
        public int TopicLength { get; private set; }

        /// <summary>
        /// Gets an ID token for re-establishing the previous connection state on a network when 
        /// the client's connection is unexpectedly lost.
        /// </summary>
        private string UniqueID { get; set; }

        /// <summary>
        /// Gets the client's username.
        /// </summary>
        public IrcUsername Username { get; private set; }

        /// <summary>
        /// Gets or sets the version string of the application the client is built for.
        /// </summary>
        public string VersionString { get; set; }

        /// <summary>
        /// Gets or sets a string describing where to download the client.
        /// </summary>
        public string VersionSourceString { get; set; }

        /// <summary>
        /// Gets the URI used to make the client connection.
        /// </summary>
        public Uri ConnectedUri { get { return (Connection.Uri); } }

        #endregion

        #region Events...

        /// <summary>
        /// Occurs when an item in a channel ban exception list is received.
        /// </summary>
        public event EventHandler<ChannelManagementListEventArgs> BanExceptionListItemReceived;

        /// <summary>
        /// Occurs when an item in a channel ban list is received.
        /// </summary>
        public event EventHandler<ChannelManagementListEventArgs> BanListItemReceived;

        /// <summary>
        /// Occurs when the client is prevented from changing its nickname.
        /// </summary>
        public event EventHandler<ErrorEventArgs> CannotChangeNickname;

        /// <summary>
        /// Occurs when a channel creation timestamp has been received.
        /// </summary>
        public event EventHandler<ChannelCreationEventArgs> ChannelCreationTimeReceived;

        /// <summary>
        /// Occurs when a channel is joined.
        /// </summary>
        public event EventHandler<JoinEventArgs> ChannelJoined;

        /// <summary>
        /// Occurs when a user or the client has left a channel.
        /// </summary>
        public event EventHandler<PartEventArgs> ChannelLeft;

        /// <summary>
        /// Occurs when the list of channels on the server is received.
        /// </summary>
        public event EventHandler<ChannelListEventArgs> ChannelListReceived;

        /// <summary>
        /// Occurs when the mode of a channel the client has joined is changed.
        /// </summary>
        public event EventHandler<ChannelModeEventArgs> ChannelModeChanged;

        /// <summary>
        /// Occurs when the modes currently set on a channel are received.  This 
        /// generally occurs when first joining a channel.
        /// </summary>
        public event EventHandler<ChannelModeEventArgs> ChannelModesReceived;

        /// <summary>
        /// Occurs when the name of a channel owner is received.
        /// </summary>
        public event EventHandler<ChannelCreatorEventArgs> ChannelOwnerReceived;

        /// <summary>
        /// Occurs when the topic of a channel the user has joined is changed.
        /// </summary>
        public event EventHandler<TopicChangeEventArgs> ChannelTopicChanged;

        /// <summary>
        /// Occurs when the topic if a channel is received.
        /// </summary>
        public event EventHandler<TopicEventArgs> ChannelTopicReceived;

        /// <summary>
        /// Occurs when the homepage URL for a channel has been received;
        /// </summary>
        public event EventHandler<ChannelUrlEventArgs> ChannelUrlReceived;

        /// <summary>
        /// Occurs when the list of users in a channel the client has joined is received.
        /// </summary>
        public event EventHandler<NameListEventArgs> ChannelUserListItemReceived;

        /// <summary>
        /// Occurs when the client user modes have changed.
        /// </summary>
        public event EventHandler ClientModesChanged;

        /// <summary>
        /// Occurs when a CNOTICE has been received.
        /// </summary>
        public event EventHandler<NoticeEventArgs> CNoticeReceived;

        /// <summary>
        /// Occurs when the client has connected but not yet registered.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when the client is currently connecting.
        /// </summary>
        public event EventHandler Connecting;

        /// <summary>
        /// Occurs when the connection state of the IRC connection has changed.
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        /// <summary>
        /// Occurs when a CPRIVMSG has been received.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> CPrivateMessageReceived;

        /// <summary>
        /// Occurs when a CTCP message is received from a server.
        /// </summary>
        public event EventHandler<CtcpEventArgs> CtcpReceived;

        /// <summary>
        /// Occurs when a request is received to resume a previous file transfer.
        /// </summary>
        public event EventHandler<DccResumeEventArgs> DccResumeRequest;

        /// <summary>
        /// Occurs when a request is received to transfer a file to the local client.
        /// </summary>
        public event EventHandler<DccSendEventArgs> DccSendRequest;

        /// <summary>
        /// Occurs when a request is received to open a private chat with the local client.
        /// </summary>
        public event EventHandler<DccChatEventArgs> DccChatRequest;

        /// <summary>
        /// Occurs when the end of a channel's ban exception list is received.
        /// </summary>
        public event EventHandler EndOfBanExceptionList;

        /// <summary>
        /// Occurs when the end of a channel's ban list is received.
        /// </summary>
        public event EventHandler EndOfBanList;

        /// <summary>
        /// Occurs when the end of the global channel list is received.
        /// </summary>
        public event EventHandler EndOfChannelList;

        /// <summary>
        /// Occurs when the end of a user name list for a channel has been received.
        /// </summary>
        public event EventHandler<DataEventArgs> EndOfChannelUserList;

        /// <summary>
        /// Occurs when the end of a channel's invitation exception list is received.
        /// </summary>
        public event EventHandler EndOfInvitationExceptionList;

        /// <summary>
        /// Occurs when the list of network links has ended.
        /// </summary>
        public event EventHandler EndOfLinks;

        /// <summary>
        /// Occurs when the end of the servist list is reached.
        /// </summary>
        public event EventHandler EndOfServices;

        /// <summary>
        /// Occurs when the end of a silence list is received.
        /// </summary>
        public event EventHandler EndOfIgnoreList;

        /// <summary>
        /// Occurs when the end of a watch (friends) list is received.
        /// </summary>
        public event EventHandler EndOfFriendList;

        /// <summary>
        /// Occurs when the end of a WHO query is received.
        /// </summary>
        public event EventHandler EndOfWho;

        /// <summary>
        /// Occurs when an error reply from the server is received.
        /// </summary>
        public event EventHandler<ErrorEventArgs> ErrorReceived;

        /// <summary>
        /// Occurs when a user is added to the client's friends list.
        /// </summary>
        public event EventHandler<WatchEventArgs> FriendAdded;

        /// <summary>
        /// Occurs when a list of friends online is received.
        /// </summary>
        public event EventHandler<DataEventArgs> FriendIsOnline;

        /// <summary>
        /// Occurs when a user is removed from the client's friends list.
        /// </summary>
        public event EventHandler<WatchEventArgs> FriendRemoved;

        /// <summary>
        /// Occurs when a user has disconnected from the IRC network.
        /// </summary>
        public event EventHandler<WatchEventArgs> FriendSignedOff;

        /// <summary>
        /// Occurs when a user has connected to the IRC network.
        /// </summary>
        public event EventHandler<WatchEventArgs> FriendSignedOn;

        /// <summary>
        /// Occurs when an entry is added to the client's ignore list.
        /// </summary>
        public event EventHandler<SilenceEventArgs> IgnoreAdded;

        /// <summary>
        /// Occurs when an entry is removed from the client's ignore list.
        /// </summary>
        public event EventHandler<SilenceEventArgs> IgnoreRemoved;

        /// <summary>
        /// Occurs when an item in a channel invitation exception list is received.
        /// </summary>
        public event EventHandler<ChannelManagementListEventArgs> InvitationExceptionListItemReceived;

        /// <summary>
        /// Occurs when a channel mode cannot be set because the channel requires Invitation Only status.
        /// </summary>
        public event EventHandler InvitationOnlyRequired;

        /// <summary>
        /// Occurs when a channel invitation has been received by another user.
        /// </summary>
        public event EventHandler<InviteEventArgs> InvitationReceived;

        /// <summary>
        /// Occurs when a user has been invited to a channel.
        /// </summary>
        public event EventHandler<NoticeEventArgs> Inviting;

        /// <summary>
        /// Occurs when the client has been authenticated as an IRC operator.
        /// </summary>
        public event EventHandler<NoticeEventArgs> IrcOperatorGranted;

        /// <summary>
        /// Occurs when the away status of the client changes.
        /// </summary>
        public event EventHandler IsAwayChanged;

        /// <summary>
        /// Occurs when the client has been kicked from a channel.
        /// </summary>
        public event EventHandler<KickEventArgs> Kicked;

        /// <summary>
        /// Occurs when a reply to the LINKS command is received.
        /// </summary>
        public event EventHandler<LinksEventArgs> LinkReceived;

        /// <summary>
        /// Occurs when the name of the network the client is connected to has been set.
        /// </summary>
        public event EventHandler NetworkIdentified;

        /// <summary>
        /// Occurs when the server has notified the client that it needs a new nickname.
        /// </summary>
        public event EventHandler NewNicknameRequired;

        /// <summary>
        /// Occurs when the nickname specified by the client has already been registered.
        /// </summary>
        public event EventHandler NicknameAlreadyRegistered;

        /// <summary>
        /// Occurs when a user has changed their nickname.
        /// </summary>
        public event EventHandler<NickChangeEventArgs> NicknameChanged;

        /// <summary>
        /// Occurs when the server has detected a nickname collision after recovering
        /// from a network split.
        /// </summary>
        public event EventHandler NicknameCollision;

        /// <summary>
        /// Occurs when the nickname the client has attempted to change to is 
        /// already in use.
        /// </summary>
        public event EventHandler<ErrorEventArgs> NicknameInUse;

        /// <summary>
        /// Occurs when a notice from the server is received.
        /// </summary>
        public event EventHandler<NoticeEventArgs> NoticeReceived;

        /// <summary>
        /// Occurs when an error has been encountered while parsing incoming data.
        /// </summary>
        public event EventHandler<ParsingErrorEventArgs> ParsingError;

        /// <summary>
        /// Occurs when a ping command is received from a server.
        /// </summary>
        public event EventHandler Ping;

        /// <summary>
        /// Occurs when a private message is added to the list of open messages.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageAdded;

        /// <summary>
        /// Occurs when a message from a channel or user is received.
        /// </summary>
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;

        /// <summary>
        /// Occurs when the client has closed a private message session.
        /// </summary>
        public event EventHandler<UserEventArgs> PrivateMessageRemoved;

        /// <summary>
        /// Occurs when the client has disconnected.
        /// </summary>
        public event EventHandler Quit;

        /// <summary>
        /// Occurs when the client is reconnecting after an unexpected error.
        /// </summary>
        public event EventHandler Reconnecting;

        /// <summary>
        /// Occurs when the client has registered with the server.
        /// </summary>
        public event EventHandler Registered;

        /// <summary>
        /// Occurs when a reply to the REHASH command is received.
        /// </summary>
        public event EventHandler Rehashing;

        /// <summary>
        /// Occurs when the server requests that the previous command be resent.
        /// </summary>
        public event EventHandler RetryCommand;

        /// <summary>
        /// Occurs when the local server time is received;
        /// </summary>
        public event EventHandler<NoticeEventArgs> ServerTimeReceived;

        /// <summary>
        /// Occurs when information about a service registered on the network is received
        /// in response to the SERVLIST command.
        /// </summary>
        public event EventHandler<ServiceListEventArgs> ServiceReceived;

        /// <summary>
        /// Occurs when the client sends a notification about its status.
        /// </summary>
        public event EventHandler<NoticeEventArgs> StatusMessage;

        /// <summary>
        /// Occurs when a notice in response to the SUMMON command is received.
        /// </summary>
        public event EventHandler<NoticeEventArgs> Summoning;

        /// <summary>
        /// Occurs when the SupportsFriendsList property has changed;
        /// </summary>
        public event EventHandler SupportsFriendsListChanged;

        /// <summary>
        /// Occurs when the author of a topic is received.
        /// </summary>
        public event EventHandler<TopicAuthorEventArgs> TopicAuthorReceived;

        /// <summary>
        /// Occurs when a reply to the TRACE command is received.
        /// </summary>
        public event EventHandler<NoticeEventArgs> TraceReceived;

        /// <summary>
        /// Occurs when an unknown command has been received from the server.
        /// </summary>
        public event EventHandler<UnhandledCommandEventArgs> UnhandledCommand;

        /// <summary>
        /// Occurs when a reply is received that is not handled by any other event handlers.
        /// </summary>
        public event EventHandler<ReplyEventArgs> UnhandledReply;

        /// <summary>
        /// Occurs when a reply to the USERHOST command is received.
        /// </summary>
        public event EventHandler<UserHostEventArgs> UserHostReplyReceived;

        /// <summary>
        /// Occurs when a channel invitation has been sent to another user
        /// in a channel the client has joined.
        /// </summary>
        public event EventHandler<InviteEventArgs> UserInvited;

        /// <summary>
        /// Occurs when the client receives a notice that a visible user is away.
        /// </summary>
        public event EventHandler<AwayEventArgs> UserIsAway;

        /// <summary>
        /// Occurs when a user has joined a channel the client has joined.
        /// </summary>
        public event EventHandler<JoinEventArgs> UserJoinedChannel;

        /// <summary>
        /// Occurs when a user is kicked from a channel the client has joined.
        /// </summary>
        public event EventHandler<KickEventArgs> UserKicked;

        /// <summary>
        /// Occurs when a user leaves a channel the client has joined.
        /// </summary>
        public event EventHandler<PartEventArgs> UserLeftChannel;

        /// <summary>
        /// Occurs when a user quits the network.
        /// </summary>
        public event EventHandler<QuitEventArgs> UserQuit;

        /// <summary>
        /// Occurs when a response to the USERS command is received.
        /// </summary>
        public event EventHandler<UsersCommandEventArgs> UsersReplyReceived;

        /// <summary>
        /// Occurs when the server's version information has been received.
        /// </summary>
        public event EventHandler<VersionEventArgs> VersionReceived;

        /// <summary>
        /// Occurs when the response to a WHOIS command is received.
        /// </summary>
        public event EventHandler<WhoIsEventArgs> WhoIsReceived;

        /// <summary>
        /// Occurs when the reponse to a WHO command is received.
        /// </summary>
        public event EventHandler<WhoEventArgs> WhoReceived;

        /// <summary>
        /// Occurs when the response to a WHOWAS command is received.
        /// </summary>
        public event EventHandler<WhoIsEventArgs> WhoWasReceived;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.IrcClient"/> class.
        /// </summary>
        /// <param name="connection">The underlying connection for the client.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if connection is null.</exception>
        public IrcClient(IConnection connection)
        {
            if (connection == null) { throw new ArgumentNullException(nameof(connection)); }

            _connection = connection;
            _connection.MessageReceived += new EventHandler<IrcMessageEventArgs>(OnMessageReceived);
            _connection.IsConnectedChanged += new EventHandler(OnConnectionIsConnectedChanged);
            _connection.SocketConnectionError += new EventHandler<SocketErrorEventArgs>(OnSocketError);
            _reconnectTimer = new System.Threading.Timer(new System.Threading.TimerCallback(OnReconnectTimerElapsed), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            _channels = new Dictionary<IrcChannelName, Channel>();
            Channels = new ReadOnlyDictionary<IrcChannelName, Channel>(_channels);

            _messages = new Dictionary<IrcNickname, PrivateMessage>();
            PrivateMessages = new ReadOnlyDictionary<IrcNickname, PrivateMessage>(_messages);

            _friends = new Dictionary<IrcNickname, Friend>();
            Friends = new ReadOnlyDictionary<IrcNickname, Friend>(_friends);

            _ignores = new Dictionary<string, IgnoredUser>();
            Ignores = new ReadOnlyDictionary<string, IgnoredUser>(_ignores);

            ResetServerOptions();
            ResetNetworkData();

        }

        /// <summary>
        /// Adds a user to the friends list.
        /// </summary>
        /// <param name="nickname">The nickname of the user to add.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the WATCH  command is not supported on the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void AddFriend(IrcNickname nickname)
        {
            if (!SupportsFriendsList) { throw new NotSupportedException(Properties.Resources.FriendsListNotSupported); }
            if (_friends.ContainsKey(nickname)) { throw new ArgumentException(Properties.Resources.NameAlreadyExistsError); }

            if (SupportsMonitor)
            {
                AddMonitor(nickname);
            }
            else
            {
                AddWatch(nickname);
            }
        }

        private void AddMonitor(IrcNickname nickname)
        {
            Connection.WriteLine(string.Format("{0} + {1}", Commands.MONITOR, nickname));
            
        }

        private void AddWatch(IrcNickname nickname)
        {
            Connection.WriteLine(string.Format("{0} +{1}", Commands.WATCH, nickname));
        }

        /// <summary>
        /// Adds a list of users to the friends list.
        /// </summary>
        /// <param name="nicknames">A list of nicknames to add.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the WATCH command is not supported on the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void AddFriend(IEnumerable<IrcNickname> nicknames)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (!SupportsFriendsList) { throw new NotSupportedException(Properties.Resources.FriendsListNotSupported); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            if (SupportsMonitor)
            {
                AddMonitor(nicknames.ToArray());
            }
            else
            {
                AddWatch(nicknames.ToArray());
            }
        }

        private void AddWatch(IrcNickname[] newFriends)
        {
            StringBuilder sb = new StringBuilder(Commands.WATCH);
            foreach (IrcNickname f in newFriends)
            {
                if (sb.Length + f.Length + 3 < MAXMESSAGELENGTH)
                {
                    sb.Append(" +");
                    sb.Append(f);

                }
                else
                {
                    Connection.WriteLine(sb.ToString());
                    sb.Clear();
                    sb.Append(Commands.WATCH);
                }
            }

            if (sb.ToString() != Commands.WATCH)
            {
                Connection.WriteLine(sb.ToString());
            }
        }

        private void AddMonitor(IrcNickname[] newFriends)
        {
            StringBuilder sb = new StringBuilder(Commands.MONITOR + " + " + newFriends[0]);
            for(int i = 1; i < newFriends.Length; i++)
            {
                if (sb.Length + newFriends[i].Length + 3 < MAXMESSAGELENGTH)
                {
                    sb.Append(",");
                    sb.Append(newFriends[i]);
                    

                }
                else
                {
                    Connection.WriteLine(sb.ToString());
                    sb.Clear();
                    sb.Append(Commands.MONITOR + " + ");
                }
            }

            if (sb.ToString() != Commands.MONITOR + " + ")
            {
                Connection.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Authenticates the user as an IRC operator on the network.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password used for authentications.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if username or password is null.</exception>
        public void AuthenticateOperator(IrcUsername username, string password)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            if (string.IsNullOrWhiteSpace(password)) { throw new ArgumentNullException(nameof(password)); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.OPER, username, password));
        }

        /// <summary>
        /// Changes the client nickname to the specified name.
        /// </summary>
        /// <param name="newNickname">A new nickname.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if newNickname is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if newNickname exceeds the maximum length on the current network.</exception>
        public void ChangeNickname(IrcNickname newNickname)
        {
            if ((int)ConnectionState < (int)ConnectionStates.Connected) { throw new InvalidOperationException(Properties.Resources.NotConnectedMessage); }
            if (newNickname == null) { throw new ArgumentNullException(nameof(newNickname)); }
            if (newNickname.Length > MaximumNicknameLength) { throw new ArgumentException(Properties.Resources.NicknameTooLongError); }

            _connection.WriteLine(string.Format("{0} {1}", Commands.NICK, newNickname));

            //if name has to be changed before the connection is registered, a NICK event is not
            //received from the server and this would make the registered nickname out of sync
            //with the client
            if (ConnectionState != ConnectionStates.Registered) { Nickname = newNickname; }
        }

        /// <summary>
        /// Changes the client's real name to the specified name.
        /// </summary>
        /// <param name="newRealName">A new real name.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if newRealName is null.</exception>
        public void ChangeRealName(string newRealName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (string.IsNullOrEmpty(newRealName)) { throw new ArgumentNullException(nameof(newRealName)); }

            _connection.WriteLine(string.Format("{0} {1}", Commands.SETNAME, newRealName));
        }

        /// <summary>
        /// Closes the private message session with the user with the specified nickname.
        /// </summary>
        /// <param name="nickname">The nickname of the remote user.</param>
        /// <exception cref="ArgumentException"/>
        public void ClosePrivateMessage(IrcNickname nickname)
        {
            if (!_messages.ContainsKey(nickname)) { throw new ArgumentException(Properties.Resources.NameNotFoundError); }

            PrivateMessage pm = _messages[nickname];
            _messages.Remove(nickname);
            pm.Dispose();
            PrivateMessageRemoved?.Invoke(this, new UserEventArgs(nickname));
        }

        /// <summary>
        /// Closes the specified private message session.
        /// </summary>
        /// <param name="message">The message to close.</param>
        public void ClosePrivateMessage(PrivateMessage message)
        {
            if (!_messages.ContainsKey(message.Name)) { throw new ArgumentException(Properties.Resources.NameNotFoundError); }

            _messages.Remove(message.Name);
            message.Dispose();
            PrivateMessageRemoved?.Invoke(this, new UserEventArgs(message.Name));
        }

        /// <summary>
        /// If the client is a channel operator, sends the specified user on the specified
        /// channel a notice that bypasses the server flood protection limit. 
        /// </summary>
        /// <param name="nickname">The nickname of a user to send a notice to.</param>
        /// <param name="channelName">The channel on which the users and client are connected.</param>
        /// <param name="message">The notice message to send.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if any nickname, channelName, or message is null.</exception>
        public void CNotice(IrcNickname nickname, IrcChannelName channelName, string message)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }
            if (!SupportsCNotice) { throw new NotSupportedException("Command not supported"); }

            Connection.WriteLine(string.Format("{0} {1} {2} :{3}", Commands.CNOTICE, nickname, channelName, message));
        }

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        /// <param name="address">The uri of the server to connect to.</param>
        /// <param name="nickname">The name to use to chat with other users.</param>
        /// <param name="username">The username to register with the server.</param>
        /// <param name="realName">The real name of the end user.  It is not recommended to use a real name.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the connection is already open.</exception>
        /// <exception cref="System.FormatException">Thrown if address is not a valid IP or host address.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is outside the valid range of port numbers.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if address is null.</exception>
        public void Connect(Uri address, IrcNickname nickname, IrcUsername username, string realName)
        {
            if (_isDisposed) { throw new ObjectDisposedException(this.GetType().ToString()); }
            if (ConnectionState != ConnectionStates.Disconnected) { throw new InvalidOperationException(Properties.Resources.ConnectionAlreadyOpenError); }
            if (username == null) { throw new ArgumentNullException(nameof(username)); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            _userDisconnect = false;
            Nickname = nickname;
            Username = username;
            RealName = string.IsNullOrWhiteSpace(realName) ? "anon" : realName;

            ConnectionState = ConnectionStates.Connecting;

            try
            {
                _connection.Connect(address);
            }
            catch (Exception)
            {
                ConnectionState = ConnectionStates.Disconnected;
                throw;
            }
        }

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        /// <param name="address">The uri of the server to connect to.</param>
        /// <param name="nickname">The the name to use to chat with other users.</param>
        /// <param name="username">The username to register with the server.</param>
        /// <param name="realName">The real name of the end user.  It is not recommended to use a real name.</param>
        /// <param name="password">The password to use if username authentication is required.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the connection is already open.</exception>
        /// <exception cref="System.FormatException">Thrown if address is not a valid IP or host address.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is outside the valid range of port numbers.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if address is null.</exception>
        public void Connect(Uri address, IrcNickname nickname, IrcUsername username, string realName, string password)
        {
            _password = password;
            Connect(address, nickname, username, realName);
        }

        /// <summary>
        /// If the client is a channel operator, sends the specified user on the specified
        /// channel a private message that bypasses the server flood protection limit. 
        /// </summary>
        /// <param name="nickname">The nickname of a user to send a private message to.</param>
        /// <param name="channelName">The channel on which the users and client are connected.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if any nickname, channelName, or message is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if nicknames is empty.</exception>
        public void CPrivMsg(IrcNickname nickname, IrcChannelName channelName, string message)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }
            if (!SupportsCPrivMsg) { throw new NotSupportedException("Command not supported"); }

            Connection.WriteLine(string.Format("{0} {1} {2} :{3}", Commands.CPRIVMSG, nickname, channelName, message));
        }

        /// <summary>
        /// Sends an action message to the specified target.
        /// </summary>
        /// <param name="targetName">The recipient of the message.</param>
        /// <param name="message">An action message to send.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target or message is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname as defined by RFC2812.</exception>
        public void CtcpAction(IrcNameBase targetName, string message)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1} {2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.ACTION, message));
        }

        /// <summary>
        /// Requests information about a remote client.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname as defined by RFC2812.</exception>
        public void CtcpClientInfo(IrcNameBase targetName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.CLIENTINFO));
        }

        /// <summary>
        /// Sends supported CTCP commands to a remote user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="message">A message providing supported CTCP commands.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or message is null.</exception>
        public void CtcpClientInfoReply(IrcNickname nickname, string message)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendNotice(nickname, string.Format("{0}{1} :{2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.CLIENTINFO, message));
        }

        /// <summary>
        /// Sends a reply that an unknown command was received.
        /// </summary>
        /// <param name="targetName">The name of the target to send the reply to.</param>
        /// <param name="failedCommand">The attempted command that failed to process.</param>
        /// <param name="errorMessage">The error message explaining the reason for the failure.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a channel name or nickname.</exception>
        public void CtcpErrMsg(IrcNameBase targetName, string failedCommand, string errorMessage)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendNotice(targetName, string.Format("{0}{1} {2} :{3}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.ERRMSG, failedCommand, errorMessage));
        }

        /// <summary>
        /// Pings the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname.</exception>
        public void CtcpPing(IrcNameBase targetName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1} {2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.PING, DateTime.Now.ToUniversalTime()));
        }

        /// <summary>
        /// Sends a ping respose to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="receivedTimestamp">The timestamp sent by the remote user.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void CtcpPingReply(IrcNickname nickname, string receivedTimestamp)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            SendNotice(nickname, string.Format("{0}{1} {2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.PING, receivedTimestamp));
        }

        /// <summary>
        /// Requests information about where to obtain a copy of the client software from the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if targetName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname as defined by RFC2812.</exception>
        public void CtcpSource(IrcNameBase targetName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.SOURCE));
        }

        /// <summary>
        /// Sends information about where to obtain a copy of the client software to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to reply to.</param>
        /// <param name="message">A message about where to obtain the client software.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target is null.</exception>
        public void CtcpSourceReply(IrcNickname nickname, string message)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendNotice(nickname, string.Format("{0}{1} {2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.SOURCE, message));
        }

        /// <summary>
        /// Requests the local time from the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if targetName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname.</exception>
        public void CtcpTime(IrcNameBase targetName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.TIME));
        }

        /// <summary>
        /// Sends the local time to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to send the time to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void CtcpTimeReply(IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            SendNotice(nickname, string.Format("{0}{1} :{2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.TIME, DateTime.Now.ToString()));
        }

        /// <summary>
        /// Requests the version and type of the client software.
        /// </summary>
        /// <param name="targetName">The name of the target to send the request to.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if targetName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname.</exception>
        public void CtcpVersion(IrcNameBase targetName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            SendMessage(targetName, string.Format("{0}{1}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.VERSION));
        }

        /// <summary>
        /// Sends the version and type of the client software to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to send the request to.</param>
        /// <param name="message">The message containg the client version information.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void CtcpVersionReply(IrcNickname nickname, string message)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            SendNotice(nickname, string.Format("{0}{1} {2}{0}", CtcpCommands.CtcpDelimeter, CtcpCommands.VERSION, message));
        }

        /// <summary>
        /// Accepts a request to resume a file transfer using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to accept.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="startPosition">The position in the file, in bytes, from which to start the transfer.</param>
        /// <param name="token">The unique token identifying the file transfer sent by the initiator of the file transfer.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or fileName are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is not between 1 and 65535, or startPosition is less than 0.</exception>
        public void DccAccept(IrcNickname nickname, string fileName, ushort port, long startPosition, string token)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); };
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentNullException(nameof(fileName)); }
            if (port < 1 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port), Properties.Resources.PortOutOfRangeError); }
            if (startPosition < 0) { throw new ArgumentOutOfRangeException(nameof(startPosition), Properties.Resources.NegativeNumberError); }

            SendNotice(nickname, string.Format("{0}DCC ACCEPT {1} {2} {3} {4}{0}", CtcpCommands.CtcpDelimeter, fileName, port, startPosition, token));
        }

        /// <summary>
        /// Sends a request to resume a file transfer using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to resume.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="startPosition">The position in the file, in bytes, from which to start the transfer.</param>
        /// <param name="token">The unique token identifying the file transfer sent by the initiator of the file transfer.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or fileName are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is not between 1 and 65535, or startPosition is less than 0.</exception>
        public void DccResume(IrcNickname nickname, string fileName, ushort port, long startPosition, string token)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); };
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentNullException(nameof(fileName)); }
            if (port < 1 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port), Properties.Resources.PortOutOfRangeError); }
            if (startPosition < 0) { throw new ArgumentOutOfRangeException(nameof(startPosition), Properties.Resources.NegativeNumberError); }

            SendMessage(nickname, string.Format("{0}DCC RESUME {1} {2} {3} {4}{0}", CtcpCommands.CtcpDelimeter, fileName, port, startPosition, token));
        }

        /// <summary>
        /// Sends a request to a user to transer a file using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="fileName">The name of the file to send.</param>
        /// <param name="address">The local address for the remote client to connect to, in network byte order.</param>
        /// <param name="port">The port number on which to conduct the transfer, in network byte order.  
        /// If zero is specified, this will initiate the reverse DCC protocol for clients that implement it.</param>
        /// <param name="token">A unique token identifying the file transfer.</param>
        /// <param name="fileSize">The size of the file to send, in bytes.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or fileName are null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is not between 1 and 65535, or fileSize is less than 1.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        public void DccSend(IrcNickname nickname, string fileName, long address, ushort port, long fileSize, string token)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); };
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentNullException(nameof(fileName)); }
            if (port < 1 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port), Properties.Resources.PortOutOfRangeError); }
            if (fileSize < 1) { throw new ArgumentOutOfRangeException(nameof(fileSize), Properties.Resources.LessThanOneError); }

            fileName = fileName.Replace(' ', '_');
            SendMessage(nickname, string.Format("{0}DCC SEND {1} {2} {3} {4} {5}{0}", CtcpCommands.CtcpDelimeter, fileName, address, port, fileSize, token));
        }

        /// <summary>
        /// Sends a request to a user to open a private chat session using the DCC protocol.
        /// </summary>
        /// <param name="nickname">The nickname of the recipient.</param>
        /// <param name="address">The local address for the remote client to connect to, in network byte order.</param>
        /// <param name="port">The local port number for the remote client to connect on, in network byte order.</param>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Registered.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if port is not between 1 and 65535.</exception>
        public void DccChat(IrcNickname nickname, long address, ushort port)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); };
            if (port < 1 || port > 65535) { throw new ArgumentOutOfRangeException(nameof(port)); }

            SendMessage(nickname, string.Format("{0}DCC CHAT chat {1} {2}{0}", CtcpCommands.CtcpDelimeter, address, port));
        }

        /// <summary>
        /// Disconnects the client from a server.
        /// </summary>
        public void Disconnect()
        {
            Disconnect(QuitMessage);
        }

        /// <summary>
        /// Disconnects the client from a server with the specified message.
        /// </summary>
        /// <param name="message">A farewell message.</param>
        /// <see cref="System.IO.IOException"/>
        public void Disconnect(string message)
        {
            _userDisconnect = true;

            try
            {
                if (IsReconnecting)
                {
                    IsReconnecting = false;
                    StatusMessage?.Invoke(this, new NoticeEventArgs() { Message = Properties.Resources.ReconnectCancelled });
                }

                if (ConnectionState > ConnectionStates.Connecting)
                {
                    _connection.WriteLine(string.Format("{0} {1}", Commands.QUIT, message));
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Releases resources used by the object.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                ResetNetworkData();
                ResetServerOptions();

                this.BanExceptionListItemReceived = null;
                this.BanListItemReceived = null;
                this.CannotChangeNickname = null;
                this.ChannelCreationTimeReceived = null;
                this.ChannelJoined = null;
                this.ChannelLeft = null;
                this.ChannelListReceived = null;
                this.ChannelModeChanged = null;
                this.ChannelModesReceived = null;
                this.ChannelOwnerReceived = null;
                this.ChannelTopicChanged = null;
                this.ChannelTopicReceived = null;
                this.ChannelUrlReceived = null;
                this.ChannelUserListItemReceived = null;
                this.ClientModesChanged = null;
                this.CNoticeReceived = null;
                this.Connected = null;
                this.Connecting = null;
                this.ConnectionStateChanged = null;
                this.CPrivateMessageReceived = null;
                this.CtcpReceived = null;
                this.DccChatRequest = null;
                this.DccResumeRequest = null;
                this.DccSendRequest = null;
                this.EndOfBanExceptionList = null;
                this.EndOfBanList = null;
                this.EndOfChannelList = null;
                this.EndOfChannelUserList = null;
                this.EndOfInvitationExceptionList = null;
                this.EndOfLinks = null;
                this.EndOfServices = null;
                this.EndOfIgnoreList = null;
                this.EndOfFriendList = null;
                this.EndOfWho = null;
                this.ErrorReceived = null;
                this.FriendAdded = null;
                this.FriendIsOnline = null;
                this.FriendRemoved = null;
                this.FriendSignedOff = null;
                this.FriendSignedOn = null;
                this.IgnoreAdded = null;
                this.IgnoreRemoved = null;
                this.InvitationExceptionListItemReceived = null;
                this.InvitationOnlyRequired = null;
                this.InvitationReceived = null;
                this.Inviting = null;
                this.IrcOperatorGranted = null;
                this.IsAwayChanged = null;
                this.Kicked = null;
                this.LinkReceived = null;
                this.NetworkIdentified = null;
                this.NewNicknameRequired = null;
                this.NicknameAlreadyRegistered = null;
                this.NicknameChanged = null;
                this.NicknameCollision = null;
                this.NicknameInUse = null;
                this.NoticeReceived = null;
                this.ParsingError = null;
                this.Ping = null;
                this.PrivateMessageAdded = null;
                this.PrivateMessageReceived = null;
                this.PrivateMessageRemoved = null;
                this.Quit = null;
                this.Reconnecting = null;
                this.Registered = null;
                this.Rehashing = null;
                this.UnhandledReply = null;
                this.RetryCommand = null;
                this.ServerTimeReceived = null;
                this.ServiceReceived = null;
                this.StatusMessage = null;
                this.Summoning = null;
                this.SupportsFriendsListChanged = null;
                this.TopicAuthorReceived = null;
                this.TraceReceived = null;
                this.UnhandledCommand = null;
                this.UserHostReplyReceived = null;
                this.UserInvited = null;
                this.UserIsAway = null;
                this.UserJoinedChannel = null;
                this.UserKicked = null;
                this.UserLeftChannel = null;
                this.UserQuit = null;
                this.UsersReplyReceived = null;
                this.VersionReceived = null;
                this.WhoIsReceived = null;
                this.WhoReceived = null;
                this.WhoWasReceived = null;

                _reconnectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _reconnectTimer.Dispose();
                _isReconnecting = false;

                _connection.Dispose();
                _isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Requests that the server send names in the RPL_NAMES reply in the
        /// nickname!username@hostname format.  This request should only be sent if
        /// UHNAMES is listed in a RPL_ISUPPORT reply.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        private void EnableLongNicknames()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine("PROTOCTL UHNAMES");
        }

        /// <summary>
        /// Requests that nicknames are prefixed with channel mode status in 
        /// <see cref="Irc.Numerics.RPL_NAMEREPLY"/> replies.  This request should only be sent if 
        /// NAMESX is listed in a <see cref="Irc.Numerics.RPL_ISUPPORT"/>
        ///  reply.
        /// </summary>
        private void EnableNicknameExtensions()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine("PROTOCTL NAMESX");
        }

        /// <summary>
        /// Adds a user to a server-side ignore list.
        /// </summary>
        /// <param name="mask">The name or hostmask of the user to ignore.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null.</exception>
        public void Ignore(string mask)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (!SupportsIgnoreList) { throw new NotSupportedException(Properties.Resources.IgnoreListNotSupported); }
            if (string.IsNullOrWhiteSpace(mask)) { throw new ArgumentNullException(nameof(mask)); }

            _connection.WriteLine(string.Format("{0} +{1}", Commands.SILENCE, mask));
        }

        /// <summary>
        /// Adds a list of users to a server-side ignore list.
        /// </summary>
        /// <param name="ignoreList">The list of users to ignore.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if ignoreList is null.</exception>
        public void Ignore(IEnumerable<string> ignoreList)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (!SupportsIgnoreList) { throw new NotSupportedException(Properties.Resources.IgnoreListNotSupported); }
            if (ignoreList == null) { throw new ArgumentNullException(nameof(ignoreList)); }

            StringBuilder sb = new StringBuilder(Commands.SILENCE);
            foreach (string iu in ignoreList)
            {
                if (!string.IsNullOrWhiteSpace(iu))
                {
                    if (sb.Length + iu.Length < MAXMESSAGELENGTH)
                    {
                        sb.Append(" +");
                        sb.Append(iu);
                    }
                    else
                    {
                        Connection.WriteLine(sb.ToString());
                        sb.Clear();
                        sb.Append(Commands.SILENCE);
                    }
                }
            }

            if (sb.ToString() != Commands.SILENCE)
            {
                Connection.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Sends an invitation to a channel to the specified user.
        /// </summary>
        /// <param name="channelName">The channel the user is being invited to.</param>
        /// <param name="nickname">The nickname of the user.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or nickname is null.</exception>
        public void Invite(IrcChannelName channelName, IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.INVITE, nickname, channelName));
        }

        /// <summary>
        /// Requests the online status of the specified list of users.
        /// </summary>
        /// <param name="nickname">A user for which to check the online status.</param>
        /// <param name="nicknames">An optional list of additional users for which to check the online status.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if nicknames is empty.</exception>
        public void IsOn(IrcNickname nickname, params IrcNickname[] nicknames)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            StringBuilder sb = new StringBuilder(Commands.ISON);
            sb.Append(" ");
            sb.Append(nickname);

            if (nicknames != null)
            {
                for (int i = 0; i < nicknames.Length; i++)
                {
                    if (nicknames[i] != null)
                    {
                        if (sb.Length + nicknames[i].Length + 1 > MAXMESSAGELENGTH)
                        {
                            Connection.WriteLine(sb.ToString());
                            sb.Clear();
                            sb.Append(Commands.ISON);
                        }
                        sb.Append(" ");
                        sb.Append(nicknames[i]);
                    }
                }
            }

            Connection.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Joins a channel with the specified name.  If the channel does not exist, it will be created.
        /// </summary>
        /// <param name="channelName">The name of the channel to join.</param>
        /// <exception cref="Irc.TooManyChannelsException">Thrown if an attempt is made to join a channel when the client has reached the limit allowed by the server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the channel name length exceeds the limit allowed by the server.</exception>
        public void JoinChannel(IrcChannelName channelName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (channelName.Length > ChannelNameLength) { throw new ArgumentException(string.Format(Properties.Resources.ChannelNameTooLong, ChannelNameLength)); }
            if (Channels.Count >= ChannelLimit) { throw new TooManyChannelsException(string.Format(Properties.Resources.TooManyChannels, NetworkName, Environment.NewLine)) { MaximumChannelsAllowed = ChannelLimit }; }
            if (Channels.ContainsKey(channelName) && Channels[channelName].IsActive) { throw new InvalidOperationException(string.Format(Properties.Resources.AlreadyJoinedChannel, channelName)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.JOIN, channelName));
        }

        /// <summary>
        /// Joins a channel with the specified name.  If the channel does not exist, it will be created.
        /// </summary>
        /// <param name="channelName">The name of the channel to join.</param>
        /// <param name="password">The password required to join the channel.</param>
        /// <exception cref="Irc.TooManyChannelsException">Thrown if an attempt is made to join a channel when the client has reached the limit allowed by the server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the channel name length exceeds the limit allowed by the server.</exception>
        public void JoinChannel(IrcChannelName channelName, IrcPassword password)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (channelName.Length > ChannelNameLength) { throw new ArgumentException(string.Format(Properties.Resources.ChannelNameTooLong, ChannelNameLength)); }
            if (Channels.Count >= ChannelLimit) { throw new TooManyChannelsException(string.Format(Properties.Resources.TooManyChannels, NetworkName, Environment.NewLine)) { MaximumChannelsAllowed = ChannelLimit }; }
            if (Channels.ContainsKey(channelName) && Channels[channelName].IsActive) { throw new InvalidOperationException(string.Format(Properties.Resources.AlreadyJoinedChannel, channelName)); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.JOIN, channelName, password));
        }

        /// <summary>
        /// Joins a channel with the specified name.  If the channel does not exist, it will be created.
        /// </summary>
        /// <param name="channelNames">A list of channels to join.</param>
        /// <exception cref="Irc.TooManyChannelsException">Thrown if an attempt is made to join a channel when the client has reached the limit allowed by the server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the channel name length exceeds the limit allowed by the server.</exception>
        public void JoinChannel(IEnumerable<IrcChannelName> channelNames)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelNames == null) { throw new ArgumentNullException(nameof(channelNames)); }
            if (channelNames.Count() == 0) { throw new ArgumentException("Channel names list cannot be empty.", "channelNames"); }
            if (Channels.Count + channelNames.Count() >= ChannelLimit) { throw new TooManyChannelsException(string.Format(Properties.Resources.TooManyChannels, NetworkName, Environment.NewLine)) { MaximumChannelsAllowed = ChannelLimit }; }

            StringBuilder sb = new StringBuilder(Commands.JOIN + " ");
            foreach (IrcChannelName name in channelNames)
            {
                sb.Append(name);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            Connection.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Joins a channel with the specified name.  If the channel does not exist, it will be created.
        /// </summary>
        /// <param name="channelNames">A list of channels to join.</param>
        /// <param name="passwords">A list of passwords for each channel.</param>
        /// <exception cref="Irc.TooManyChannelsException">Thrown if an attempt is made to join a channel when the client has reached the limit allowed by the server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the channel name length exceeds the limit allowed by the server.</exception>
        public void JoinChannel(IEnumerable<IrcChannelName> channelNames, IEnumerable<IrcPassword> passwords)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelNames == null) { throw new ArgumentNullException(nameof(channelNames)); }
            if (channelNames.Count() == 0) { throw new ArgumentException("Channel names list cannot be empty.", "channelNames"); }
            if (Channels.Count + channelNames.Count() >= ChannelLimit) { throw new TooManyChannelsException(string.Format(Properties.Resources.TooManyChannels, NetworkName, Environment.NewLine)) { MaximumChannelsAllowed = ChannelLimit }; }
            if (passwords == null) { throw new ArgumentNullException(nameof(passwords)); }

            IrcChannelName[] newChannels = channelNames.Except((from c in _channels.Keys
                                                                select c)).ToArray();

            StringBuilder sb = new StringBuilder(Commands.JOIN + " ");
            foreach (IrcChannelName name in newChannels)
            {
                sb.Append(name);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(" ");

            foreach (IrcPassword pass in passwords)
            {
                sb.Append(pass);
                sb.Append(",");
            }

            if (sb[sb.Length - 1] == ',')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            Connection.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Kicks a user from the specified channel.
        /// </summary>
        /// <param name="channelName">The channel to remove the user from.</param>
        /// <param name="nickname">The name of the user to remove.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or nickname is null.</exception>
        public void Kick(IrcChannelName channelName, IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.KICK, channelName, nickname));
        }

        /// <summary>
        /// Kicks a user from the specified channel.
        /// </summary>
        /// <param name="channelName">The channel to remove the user from.</param>
        /// <param name="nickname">The name of the user to remove.</param>
        /// <param name="reason">A message explaining why the user was kicked.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or nickname is null.</exception>
        public void Kick(IrcChannelName channelName, IrcNickname nickname, string reason)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            if (string.IsNullOrEmpty(reason))
            {
                Kick(channelName, nickname);
            }
            else
            {
                Connection.WriteLine(string.Format("{0} {1} {2} :{3}", Commands.KICK, channelName, nickname, reason));
            }
        }

        /// <summary>
        /// Sends a notice to the specified channel requesting an invitation.
        /// </summary>
        /// <param name="channelName">The name of the channel to knock.</param>
        /// <exception cref="System.NotSupportedException">Thrown if the server does not support the KNOCK command.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the ConnectionState is not Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void KnockChannel(IrcChannelName channelName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (!SupportsKnock) { throw new NotSupportedException(Properties.Resources.KnockNotSupported); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.KNOCK, channelName));
        }

        /// <summary>
        /// Sends a notice to the specified channel requesting an invitation with the specified message.
        /// </summary>
        /// <param name="channelName">The name of the channel to knock.</param>
        /// <param name="message">A message to send when requesting an invitation.</param>
        /// <exception cref="System.NotSupportedException">Thrown if the server does not support the KNOCK command.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the ConnectionState is not Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void KnockChannel(IrcChannelName channelName, string message)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (!SupportsKnock) { throw new NotSupportedException(Properties.Resources.KnockNotSupported); }

            if (string.IsNullOrEmpty(message))
            {
                KnockChannel(channelName);
            }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.KNOCK, channelName, message));
        }

        /// <summary>
        /// Leaves all channels that have been joined.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void LeaveAllChannels()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} 0", Commands.JOIN));
        }

        /// <summary>
        /// Exits a chat channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to exit.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the length of the channel name exceeds the server limit.</exception>
        public void LeaveChannel(IrcChannelName channelName)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (channelName.Length > ChannelNameLength) { throw new ArgumentOutOfRangeException("channelName", string.Format(Properties.Resources.ChannelNameTooLong, ChannelNameLength)); }

            if (ConnectionState == ConnectionStates.Registered  && _channels.ContainsKey(channelName) && _channels[channelName].IsActive)
            {
                if (string.IsNullOrWhiteSpace(PartMessage))
                {
                    _connection.WriteLine(string.Format("{0} {1}", Commands.PART, channelName));
                }
                else
                {
                    _connection.WriteLine(string.Format("{0} {1} :{2}", Commands.PART, channelName, PartMessage));
                }
            }
            else
            {
                if (_channels.ContainsKey(channelName))
                {
                    Channel leftChannel = _channels[channelName];
                    _channels.Remove(channelName);
                    leftChannel.Dispose();
                    PartEventArgs e = new PartEventArgs(leftChannel.Name, Nickname) { UserName = Username, RealName = RealName };
                    ChannelLeft?.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// Exits a chat channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to leave.</param>
        /// <param name="farewellMessage">A message to send when leaving the channel.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName or nickname is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the length of the channel name exceeds the server limit.</exception>
        public void LeaveChannel(IrcChannelName channelName, string farewellMessage)
        {
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (channelName.Length > ChannelNameLength) { throw new ArgumentOutOfRangeException("channelName", string.Format(Properties.Resources.ChannelNameTooLong, ChannelNameLength)); }

            if (ConnectionState == ConnectionStates.Registered)
            {
                if (string.IsNullOrWhiteSpace(farewellMessage))
                {
                    LeaveChannel(channelName);
                }
                else
                {
                    _connection.WriteLine(string.Format("{0} {1} :{2}", Commands.PART, channelName, PartMessage));
                }
            }
            else
            {
                if (_channels.ContainsKey(channelName))
                {
                    Channel leftChannel = _channels[channelName];
                    _channels.Remove(channelName);
                    leftChannel.Dispose();
                    PartEventArgs e = new PartEventArgs(leftChannel.Name, Nickname) { UserName = Username, RealName = RealName };
                    ChannelLeft?.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// Handles <see cref="Commands.INVITE"/> events sent to the client.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelInvitationReceived(InviteEventArgs e)
        {
            InvitationReceived?.Invoke(this, e);

            if (AutoAcceptChannelInvitations)
            {
                try
                {
                    JoinChannel(e.ChannelName);
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Event handler for the <see cref="Irc.IRfc2812.ChannelJoined"/> event.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelJoined(JoinEventArgs e)
        {
            if (!_channels.ContainsKey(e.ChannelName))
            {

                Channel newChannel = new Channel(e.ChannelName, this);
                newChannel.NetworkName = NetworkName;
                _channels.Add(e.ChannelName, newChannel);
            }
            ChannelJoined?.Invoke(this, e);
            //some networks automatically send channel modes on join, some don't.
            //those networks are stupid assholes
            this.RequestChannelModes(e.ChannelName);
        }

        /// <summary>
        /// Event handler for the <see cref="Irc.IRfc2812.ChannelLeft"/> event.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnChannelLeft(PartEventArgs e)
        {
            if (_channels.ContainsKey(e.ChannelName))
            {
                Channel leftChannel = _channels[e.ChannelName];
                _channels.Remove(e.ChannelName);
                leftChannel.Dispose();
                ChannelLeft?.Invoke(this, e);
            }
        }

        private void OnConnectionIsConnectedChanged(object sender, EventArgs e)
        {
            if (Connection.IsConnected)
            {
                ConnectionState = ConnectionStates.Connected;
                if (string.IsNullOrEmpty(_password))
                {
                    RegisterUser();
                }
                else
                {
                    RegisterUser(_password);
                }
                _password = null;
            }
            else
            {
                ConnectionState = ConnectionStates.Disconnected;
            }
        }

        /// <summary>
        /// Handles connection state changes.
        /// </summary>
        private void OnConnectionStateChanged()
        {
            switch (ConnectionState)
            {
                case ConnectionStates.Connecting:
                    IsReconnecting = false;
                    Connecting?.Invoke(this, EventArgs.Empty);
                    break;
                case ConnectionStates.Connected:
                    Connected?.Invoke(this, EventArgs.Empty);
                    break;
                case ConnectionStates.Registered:
                    Registered?.Invoke(this, EventArgs.Empty);
                    break;
                case ConnectionStates.Disconnected:
                    ResetServerOptions();
                    Quit?.Invoke(this, EventArgs.Empty);
                    break;
            }

            ConnectionStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCtcpReceived(CtcpEventArgs e)
        {
            try
            {
                CtcpReceived?.Invoke(this, e);
                switch (e.CtcpCommand.ToUpperInvariant())
                {
                    case CtcpCommands.ACTION:
                        break;
                    case CtcpCommands.CLIENTINFO:
                        CtcpClientInfoReply(e.Nickname, string.Format("The following commands are supported: {0} {1} {2} {3} {4} {5}",
                                                                      CtcpCommands.ACTION,
                                                                      CtcpCommands.CLIENTINFO,
                                                                      CtcpCommands.PING,
                                                                      CtcpCommands.SOURCE,
                                                                      CtcpCommands.TIME,
                                                                      CtcpCommands.VERSION));
                        break;
                    case CtcpCommands.ERRMSG:
                        break;
                    case CtcpCommands.PING:
                        CtcpPingReply(e.Nickname, e.Message);
                        break;
                    case CtcpCommands.SOURCE:
                        CtcpSourceReply(e.Nickname, VersionSourceString ?? "Downloadable client unavailable at this time.");
                        break;
                    case CtcpCommands.TIME:
                        CtcpTimeReply(e.Nickname);
                        break;
                    case CtcpCommands.VERSION:
                        CtcpVersionReply(e.Nickname, VersionString ?? "Client did not set version string. Using IRC Library available at github.com/zmilliron/irc ");
                        break;
                    case "DCC":
                        OnDccRequestReceived(e);
                        break;
                    default:
                        CtcpErrMsg(e.Nickname, e.CtcpCommand, "whut");
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorReceived?.Invoke(this, new ErrorEventArgs()
                {
                    Target = e.Nickname,
                    DefaultMessage = string.Format("CTCP reply failed: {0}", ex.Message)
                });
            }

        }

        private void OnDccRequestReceived(CtcpEventArgs e)
        {
            string[] dccArgs = e.Message.Split(new char[] { ' ' });
            switch (dccArgs[0].ToUpperInvariant())
            {
                case DccCommands.CHAT:
                    if (!IgnoreDccChatRequests)
                    {
                        byte[] sendAddress = BitConverter.GetBytes(long.Parse(dccArgs[2])).Reverse().ToArray();
                        byte[] newAddress = new byte[4];
                        Array.Copy(sendAddress, 4, newAddress, 0, 4);
                        DccChatRequest?.Invoke(this, new DccChatEventArgs(e.Nickname, newAddress, int.Parse(dccArgs[3])) { NetworkName = NetworkName });
                    }
                    break;
                case DccCommands.SEND:
                    if (!IgnoreDccSendRequests)
                    {
                        byte[] sendAddress = BitConverter.GetBytes(ulong.Parse(dccArgs[2])).Reverse().ToArray();
                        byte[] newAddress = new byte[4];
                        Array.Copy(sendAddress, 4, newAddress, 0, 4);
                        DccSendRequest?.Invoke(this, new DccSendEventArgs(e.Nickname, 
                                                                        dccArgs[1], 
                                                                        newAddress, 
                                                                        int.Parse(dccArgs[3]), 
                                                                        long.Parse(dccArgs[4])) { NetworkName = NetworkName });
                    }
                    break;
                case DccCommands.RESUME:
                    if (!IgnoreDccSendRequests)
                    {
                    }
                    break;
                case DccCommands.ACCEPT:
                    if (!IgnoreDccSendRequests)
                    {
                    }
                    break;
            }
        }

        /// <summary>
        /// Event handler for the <see cref="Irc.IRfc2812.Kicked"/> event.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnKicked(KickEventArgs e)
        {
            if (_channels.ContainsKey(e.ChannelName))
            {
                Kicked?.Invoke(this, e);

                if (RejoinOnKick)
                {
                    JoinChannel(e.ChannelName);
                }
            }
        }

        private void OnMessageReceived(object sender, IrcMessageEventArgs e)
        {
            //RawInputText?.Invoke(this, new NoticeEventArgs() { Message = line });

            try
            {
                IrcMessage message = e.Message;

                int commandNumber = -1;
                if (int.TryParse(message.Command, out commandNumber))
                {
                    switch ((Numerics)commandNumber)
                    {
                        case Numerics.RPL_WELCOME:
                            ConnectionState = ConnectionStates.Registered;
                            break;
                        case Numerics.RPL_MYINFO:
                            ActualAddress = message.Parameters[1];
                            ServerVersion = message.Parameters[2];
                            SupportedClientModes = message.Parameters[3];
                            SupportedChannelModes = message.Parameters[4];
                            break;
                        case Numerics.RPL_ISUPPORT:
                            OnSupportedOptionsReceived(Parser.ParseSupportedOptions(message));
                            break;
                        case Numerics.RPL_USERHOST:
                            UserHostReplyReceived?.Invoke(this, Parser.ParseUserHost(message));
                            break;
                        case Numerics.RPL_YOURCOOKIE:
                        case Numerics.RPL_YOURID:
                            UniqueID = Parser.ParseUniqueID(message);
                            break;
                        case Numerics.RPL_TRYAGAIN:
                            RetryCommand?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_ISON:
                            IrcNickname[] nicksOnline = Parser.ParseIsOn(message);
                            foreach (IrcNickname nick in nicksOnline)
                            {
                                FriendIsOnline?.Invoke(this, new DataEventArgs() { Data = nick.ToString() });
                            }
                            break;
                        case Numerics.RPL_UMODEIS:
                            ClientModeEventArgs clientModeArgs = Parser.ParseUserModes(message);
                            ModeString = clientModeArgs.ModeString;
                            break;
                        case Numerics.RPL_AWAY:
                            UserIsAway?.Invoke(this, Parser.ParseAwayMessage(message));
                            break;
                        case Numerics.RPL_UNAWAY:
                            IsAway = false;
                            break;
                        case Numerics.RPL_NOWAWAY:
                            IsAway = true;
                            break;
                        case Numerics.RPL_WHOISUSER:
                        case Numerics.RPL_WHOWASUSER:
                            UserEventArgs uea = Parser.ParseWhoIsUser(message);
                            _whoIsResult = new WhoIsEventArgs(uea.Nickname);
                            _whoIsResult.HostName = uea.HostName;
                            _whoIsResult.UserName = uea.UserName;
                            _whoIsResult.RealName = uea.RealName;
                            break;
                        case Numerics.RPL_WHOISSERVER:
                            _whoIsResult.ServerInfo = Parser.ParseWhoIsServer(message);
                            break;
                        case Numerics.RPL_WHOISOPERATOR:
                            _whoIsResult.IsIRCOperator = true;
                            break;
                        case Numerics.RPL_WHOISIDLE:
                            DateTime signOnTime = new DateTime();
                            _whoIsResult.IdleTime = Parser.ParseWhoIsIdle(message, out signOnTime);
                            _whoIsResult.SignOnTime = signOnTime;
                            break;
                        case Numerics.RPL_WHOISSECURE:
                            _whoIsResult.IsSecureConnection = true;
                            break;
                        case Numerics.RPL_WHOISLOGGEDIN:
                        case Numerics.RPL_WHOISHOST:
                            //_whoIsResult.HostName = Parser.ParseWhoIsHost(message);
                            break;
                        case Numerics.RPL_WHOISREGNICK:
                            _whoIsResult.IsNicknameRegistered = true;
                            break;
                        case Numerics.RPL_WHOISHELPOP:
                            if (!message.Trailing.Contains("is using modes"))
                            {
                                _whoIsResult.IsHelpOperator = true;
                            }
                            break;
                        case Numerics.RPL_ENDOFWHOIS:
                            WhoIsReceived?.Invoke(this, _whoIsResult);
                            break;
                        case Numerics.RPL_WHOISCHANNELS:
                            _whoIsResult.ChannelList = Parser.ParseWhoIsChannelList(message);
                            break;
                        case Numerics.RPL_WHOISACTUALLY:
                            break;
                        case Numerics.RPL_ENDOFWHOWAS:
                            WhoWasReceived?.Invoke(this, _whoIsResult);
                            break;
                        case Numerics.RPL_LISTSTART:
                            break;
                        case Numerics.RPL_LIST:
                            ChannelListReceived?.Invoke(this, Parser.ParseChannelListItem(message));
                            break;
                        case Numerics.RPL_LISTEND:
                            EndOfChannelList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_CHANNEL_URL:
                            ChannelUrlReceived?.Invoke(this, Parser.ParseChannelUrl(message));
                            break;
                        case Numerics.RPL_UNIQOPIS:
                            ChannelOwnerReceived?.Invoke(this, Parser.ParseChannelOwner(message));
                            break;
                        case Numerics.RPL_CREATIONTIME:
                            ChannelCreationTimeReceived?.Invoke(this, Parser.ParseChannelCreationTime(message));
                            break;
                        case Numerics.RPL_CHANNELMODEIS:
                            ChannelModesReceived?.Invoke(this, Parser.ParseChannelMode(message, string.Concat(ChannelModesAlwaysParameters, ChannelModesParametersWhenSet)));
                            break;
                        case Numerics.RPL_NOTOPIC:
                            break;
                        case Numerics.RPL_TOPIC:
                            ChannelTopicReceived?.Invoke(this, Parser.ParseTopic(message));
                            break;
                        case Numerics.RPL_TOPICWHOTIME:
                            TopicAuthorReceived?.Invoke(this, Parser.ParseTopicAuthor(message));
                            break;
                        case Numerics.RPL_INVITING:
                            Inviting?.Invoke(this, Parser.ParseInviting(message));
                            break;
                        case Numerics.RPL_INVITELIST:
                            InvitationExceptionListItemReceived?.Invoke(this, Parser.ParseChannelManagementList(message));
                            break;
                        case Numerics.RPL_ENDOFINVITELIST:
                            EndOfInvitationExceptionList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_EXCEPTLIST:
                            BanExceptionListItemReceived?.Invoke(this, Parser.ParseChannelManagementList(message));
                            break;
                        case Numerics.RPL_ENDOFEXCEPTLIST:
                            EndOfBanExceptionList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_VERSION:
                            VersionReceived?.Invoke(this, Parser.ParseVersion(message));
                            break;
                        case Numerics.RPL_WHOREPLY:
                            WhoReceived?.Invoke(this, Parser.ParseWho(message));
                            break;
                        case Numerics.RPL_ENDOFWHO:
                            EndOfWho?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_NAMEREPLY:
                            ChannelUserListItemReceived?.Invoke(this, Parser.ParseNameListItem(message));
                            break;
                        case Numerics.RPL_ENDOFNAMES:
                            EndOfChannelUserList?.Invoke(this, Parser.ParseEndOfNamesList(message));
                            break;
                        case Numerics.RPL_BANLIST:
                            BanListItemReceived?.Invoke(this, Parser.ParseChannelManagementList(message));
                            break;
                        case Numerics.RPL_ENDOFBANLIST:
                            EndOfBanList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_WATCHING:
                        case Numerics.RPL_ALTWATCHING:
                            OnWatchStarted(Parser.ParseWatch(message));
                            break;
                        case Numerics.RPL_STOPWATCHING:
                            OnWatchStopped(Parser.ParseWatch(message));
                            break;
                        case Numerics.RPL_USERONLINE:
                            FriendSignedOn?.Invoke(this, Parser.ParseWatch(message));
                            break;
                        case Numerics.RPL_USEROFFLINE:
                            FriendSignedOff?.Invoke(this, Parser.ParseWatch(message));
                            break;
                        case Numerics.RPL_LINKS:
                            LinkReceived?.Invoke(this, Parser.ParseLinks(message));
                            break;
                        case Numerics.RPL_ENDOFLINKS:
                            EndOfLinks?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_SUMMONING:
                            Summoning?.Invoke(this, Parser.ParseNotice(message));
                            break;
                        case Numerics.RPL_YOUREOPER:
                            IsIrcOperator = true;
                            IrcOperatorGranted?.Invoke(this, Parser.ParseNotice(message));
                            break;
                        case Numerics.RPL_TIME:
                            ServerTimeReceived?.Invoke(this, Parser.ParseNotice(message));
                            break;
                        case Numerics.RPL_REHASHING:
                            Rehashing?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_SERVLIST:
                            ServiceReceived?.Invoke(this, Parser.ParseService(message));
                            break;
                        case Numerics.RPL_USERS:
                            UsersReplyReceived?.Invoke(this, Parser.ParseUsers(message));
                            break;
                        case Numerics.RPL_ENDOFWATCHLIST:
                            EndOfFriendList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_ENDOFSILELIST:
                            EndOfIgnoreList?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_SERVLISTEND:
                            EndOfServices?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.RPL_MONONLINE:
                            OnMonitorOnline(Parser.ParseMonitor(message, true));
                            break;
                        case Numerics.RPL_MONOOFFLINE:
                            OnMonitorOffline(Parser.ParseMonitor(message, false));
                            break;
                        case Numerics.RPL_MONLIST:
                            break;
                        case Numerics.ERR_UMODEUNKNOWNFLAG:
                            RequestClientModes();
                            NoticeReceived?.Invoke(this, new NoticeEventArgs() { Sender = message.Prefix, Message = message.Trailing });
                            break;
                        case Numerics.ERR_MODEIREQUIRED:
                            InvitationOnlyRequired?.Invoke(this, EventArgs.Empty);
                            break;
                        case Numerics.ERR_NICKCHANGENOTALLOWED:
                            CannotChangeNickname?.Invoke(this, Parser.ParseError(message));
                            break;
                        case Numerics.ERR_NICKNAMEINUSE:
                            OnNicknameInUse(message);
                            break;
                        case Numerics.ERR_NICKCOLLISION:
                            NicknameCollision?.Invoke(this, EventArgs.Empty);
                            break;
                        default:
                            UnhandledReply?.Invoke(this, new ReplyEventArgs() { ReplyCode = commandNumber, 
                                                                             Parameters = e.Message.Parameters, 
                                                                             Message = e.Message.Trailing });
                            //ErrorReceived?.Invoke(this, Parser.ParseError(message));
                            break;
                    }
                }
                else
                {
                    switch (message.Command)
                    {
                        case Commands.CNOTICE:
                            CNoticeReceived?.Invoke(this, Parser.ParseNotice(message));
                            break;
                        case Commands.CPRIVMSG:
                            CPrivateMessageReceived?.Invoke(this, Parser.ParsePrivateMessage(message));
                            break;
                        case Commands.ERROR:
                            ErrorReceived?.Invoke(this, Parser.ParseError(message));
                            break;
                        case Commands.INVITE:
                            InviteEventArgs iea = Parser.ParseInvite(message);

                            if (iea.RecipientName == this.Nickname)
                            {
                                OnChannelInvitationReceived(iea);
                            }
                            else
                            {
                                UserInvited?.Invoke(this, iea);
                            }
                            break;
                        case Commands.JOIN:
                            JoinEventArgs jea = Parser.ParseJoin(message);
                            if (jea.Nickname == this.Nickname)
                            {
                                OnChannelJoined(jea);
                            }
                            else
                            {
                                UserJoinedChannel?.Invoke(this, jea);
                            }
                            break;
                        case Commands.KICK:
                            KickEventArgs cea = Parser.ParseKick(message);
                            if (cea.UserKicked == this.Nickname)
                            {
                                OnKicked(cea);
                            }
                            else
                            {
                                UserKicked?.Invoke(this, cea);
                            }
                            break;
                        case Commands.MODE:
                            if (IrcNickname.IsValid(message.Parameters[0]))
                            {
                                ClientModeString newModes = Parser.ParseUserModes(message).ModeString;
                                ClientModeString temp = ModeString;
                                foreach (Mode mode in newModes)
                                {
                                    if (mode.IsAdded)
                                    {
                                        temp += mode;
                                    }
                                    else
                                    {
                                        temp = temp.Remove(ModeString.IndexOf(mode), 1);
                                    }
                                }

                                ModeString = temp;
                            }
                            else
                            {
                                ChannelModeChanged?.Invoke(this, Parser.ParseChannelModeChange(message, ChannelModesAlwaysParameters, ChannelModesParametersWhenSet));
                            }
                            break;
                        case Commands.NICK:
                            NickChangeEventArgs ncea = Parser.ParseNick(message);
                            if (ncea.Nickname == this.Nickname)
                            {
                                Nickname = ncea.NewNickname;
                            }
                            NicknameChanged?.Invoke(this, ncea);
                            break;
                        case Commands.NOTICE:
                            NoticeReceived?.Invoke(this, Parser.ParseNotice(message));
                            break;
                        case Commands.PART:
                            PartEventArgs pea = Parser.ParsePart(message);

                            if (pea.Nickname == this.Nickname)
                            {
                                OnChannelLeft(pea);
                            }
                            else
                            {
                                UserLeftChannel?.Invoke(this, pea);
                            }
                            break;
                        case Commands.PING:
                            Pong(message.Trailing);
                            Ping?.Invoke(this, EventArgs.Empty);
                            break;
                        case Commands.PRIVMSG:
                            if (message.Trailing.Contains(CtcpCommands.CtcpDelimeter))
                            {
                                OnCtcpReceived(Parser.ParseCtcpMessage(message));
                            }
                            else
                            {
                                OnPrivateMessageReceived(Parser.ParsePrivateMessage(message));
                            }
                            break;
                        case Commands.QUIT:
                            UserQuit?.Invoke(this, Parser.ParseQuit(message));
                            break;
                        case Commands.SILENCE:
                            OnSilenceReceived(Parser.ParseSilence(message));
                            break;
                        case Commands.TOPIC:
                            ChannelTopicChanged?.Invoke(this, Parser.ParseTopicChange(message));
                            break;
                        default:
                            UnhandledCommand?.Invoke(this, new UnhandledCommandEventArgs() { Command = message.Command, Message = message.ToString() });
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ParsingError?.Invoke(this, new ParsingErrorEventArgs() { RawText = e.Message.Command, Error = ex });
            }
        }

        private void OnNicknameInUse(IrcMessage message)
        {
            if (ConnectionState != ConnectionStates.Registered)
            {
                ChangeNickname(new IrcNickname(string.Concat(Nickname, rand.Next(9999))));
                NicknameAlreadyRegistered?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                NicknameInUse?.Invoke(this, Parser.ParseError(message));
            }
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IRfc2812.PrivateMessageReceived"/> event.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnPrivateMessageReceived(PrivateMessageEventArgs e)
        {
            if (e.MessageTarget == Nickname && !_messages.ContainsKey(e.Nickname))
            {

                User newUser = new User(e.Nickname, this)
                {
                    UserName = e.UserName,
                    RealName = e.RealName,
                    HostName = e.HostName
                };

                PrivateMessage newMessage = new PrivateMessage(newUser, this, e);
                newMessage.NetworkName = NetworkName;
                _messages.Add(e.Nickname, newMessage);
                PrivateMessageAdded?.Invoke(this, e);
            }
            else
            {
                PrivateMessageReceived?.Invoke(this, e);
            }
        }

        private void OnReconnectTimerElapsed(object sender)
        {
            _reconnectTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            Connect(Connection.Uri, Nickname, Username, RealName);
        }

        /// <summary>
        /// Event handling method for the <see cref="Irc.IConnection.SocketConnectionError"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Irc.IConnection"/> object raising the event.</param>
        /// <param name="e">The event data for the event.</param>
        private void OnSocketError(object sender, SocketErrorEventArgs e)
        {
            StatusMessage?.Invoke(this, new NoticeEventArgs() { Message = string.Concat(Properties.Resources.ConnectionLost, " (", e.Message, ")", Environment.NewLine) });

            if (!_isDisposed && e.IsConnecting)
            {
                ConnectionState = ConnectionStates.Disconnected;
            }
            else if (!_isDisposed && AutoReconnect && !_userDisconnect)
            {
                StatusMessage?.Invoke(this, new NoticeEventArgs() { Message = Properties.Resources.ReconnectingInTen });
                IsReconnecting = true;
            }
        }

        /// <summary>
        /// Handles <see cref="Irc.Commands.SILENCE"/> events.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnSilenceReceived(SilenceEventArgs e)
        {
            foreach (string mask in e.Masks.Keys)
            {
                if (e.Masks[mask])
                {
                    IgnoredUser ignore = new IgnoredUser(mask, NetworkName, this);
                    _ignores.Add(mask, ignore);
                    IgnoreAdded?.Invoke(this, e);
                }
                else
                {
                    _ignores.Remove(mask);
                    IgnoreRemoved?.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// Handles <see cref="Irc.Numerics.RPL_ISUPPORT"/> events.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnSupportedOptionsReceived(SupportedOptionsEventArgs e)
        {
            if (e.Options.ContainsKey(SupportedOptions.NETWORK))
            {
                if (_network != null && !_network.ToUpperInvariant().Contains(e.Options[SupportedOptions.NETWORK].ToUpperInvariant()))
                {
                    ResetNetworkData();
                }
                NetworkName = e.Options[SupportedOptions.NETWORK];
                RejoinChannels();
            }
            foreach (string option in e.Options.Keys)
            {
                try
                {
                    switch (option)
                    {
                        case SupportedOptions.AWAYLEN:
                            AwayMessageLength = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.CALLERID:
                            SupportsCallerId = true;
                            break;
                        case SupportedOptions.CASEMAPPING:
                            CaseMapping = e.Options[option];
                            break;
                        case SupportedOptions.CHANLIMIT:
                            ChannelLimit = int.Parse(e.Options[option].Substring(2));
                            break;
                        case SupportedOptions.CHANMODES:
                            string[] modeGroups = e.Options[option].Split(',');
                            ChannelModesAlwaysParameters = string.Format("{0}{1}{2}", modeGroups[0], modeGroups[1], ChannelModesAlwaysParameters);
                            ChannelModesParametersWhenSet = modeGroups[2];
                            ChannelModesNoParameters = modeGroups[3];
                            break;
                        case SupportedOptions.CHANNELLEN:
                            ChannelNameLength = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.CHANTYPES:
                            SupportedChannelTypes = e.Options[option];
                            break;
                        case SupportedOptions.CMDS:
                            string[] commands = e.Options[option].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string cmd in commands)
                            {
                                switch (cmd)
                                {
                                    case SupportedOptions.KNOCK:
                                        SupportsKnock = true;
                                        break;
                                    case SupportedOptions.USERIP:
                                        SupportsUserIPCommand = true;
                                        break;
                                }
                            }
                            break;
                        case SupportedOptions.CNOTICE:
                            SupportsCNotice = true;
                            break;
                        case SupportedOptions.CPRIVMSG:
                            SupportsCPrivMsg = true;
                            break;
                        case SupportedOptions.ELIST:
                            break;
                        case SupportedOptions.EXCEPTS:
                            SupportsBanExceptions = true;
                            break;
                        case SupportedOptions.FNC:
                            SupportsForcedNicknameChanges = true;
                            break;
                        case SupportedOptions.IDCHAN:
                            break;
                        case SupportedOptions.INVEX:
                            SupportsInviteExceptions = true;
                            break;
                        case SupportedOptions.KICKLEN:
                            KickMessageLength = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.KNOCK:
                            SupportsKnock = true;
                            break;
                        case SupportedOptions.MAXLIST:
                            string[] lens = e.Options[option].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in lens)
                            {
                                switch (s[0])
                                {
                                    case 'b':
                                        BanListLength = int.Parse(s.Substring(2));
                                        break;
                                    case 'I':
                                        InviteExceptionListLength = int.Parse(s.Substring(2));
                                        break;
                                    case 'e':
                                        BanExceptionListLength = int.Parse(s.Substring(2));
                                        break;
                                }
                            }
                            break;
                        case SupportedOptions.MAXTARGETS:
                            MaximumMessageTargets = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.MODES:
                            ModesPerCommand = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.MONITOR:
                            SupportsMonitor = true;
                            SupportsFriendsListChanged?.Invoke(this, EventArgs.Empty);
                            MaximumFriendsListLength = int.Parse(e.Options[option]);
                            if (Friends.Count > 0)
                            {
                                AddFriend(Friends.Keys);
                            }
                            break;
                        case SupportedOptions.NAMESX:
                            EnableNicknameExtensions();
                            break;
                        case SupportedOptions.NICKLEN:
                            MaximumNicknameLength = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.PENALTY:
                            TimeBetweenCommands = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.PREFIX:
                            string optionString = e.Options[option];
                            string[] prefixes = optionString.Split(new string[] { ")" }, StringSplitOptions.RemoveEmptyEntries);
                            prefixes[0] = prefixes[0].Substring(1);
                            ChannelModesAlwaysParameters += prefixes[0];
                            for (int i = 0; i < prefixes[0].Length; i++)
                            {
                                //switch (prefixes[0][i].ToString())
                                //{
                                //    case "o":
                                //        ChannelUser.OPERATORPREFIX = prefixes[1][i].ToString();
                                //        break;
                                //    case "v":
                                //        ChannelUser.VOICEPREFIX = prefixes[1][i].ToString();
                                //        break;
                                //    case "q":
                                //        ChannelUser.OWNERPREFIX = prefixes[1][i].ToString();
                                //        break;
                                //    case "a":
                                //        ChannelUser.PROTECTEDPREFIX = prefixes[1][i].ToString();
                                //        break;
                                //    case "h":
                                //        ChannelUser.HALFOPERATORPREFIX = prefixes[1][i].ToString();
                                //        break;
                                //}
                            }
                            break;
                        case SupportedOptions.RFC2812:
                            SupportsRFC2812 = true;
                            break;
                        case SupportedOptions.SAFELIST:
                            SupportsSafeList = true;
                            break;
                        case SupportedOptions.SILENCE:
                            SupportsIgnoreList = true;
                            MaximumIgnoreListLength = int.Parse(e.Options[option]);

                            if (Ignores.Count > 0)
                            {
                                Ignore(_ignores.Keys);
                            }
                            break;
                        case SupportedOptions.STATUSMSG:
                            SupportsStatusMessaging = true;
                            break;
                        case SupportedOptions.STD:
                            break;
                        case SupportedOptions.TOPICLEN:
                            TopicLength = int.Parse(e.Options[option]);
                            break;
                        case SupportedOptions.UHNAMES:
                            EnableLongNicknames();
                            break;
                        case SupportedOptions.USERIP:
                            SupportsUserIPCommand = true;
                            RequestUserIP(Nickname);
                            break;
                        case SupportedOptions.VCHANS:
                            break;
                        case SupportedOptions.WALLCHOPS:
                            break;
                        case SupportedOptions.WALLVOICES:
                            break;
                        case SupportedOptions.WATCH:
                            SupportsWatch = true;
                            SupportsFriendsListChanged?.Invoke(this, EventArgs.Empty);
                            MaximumFriendsListLength = int.Parse(e.Options[option]);

                            if (Friends.Count > 0)
                            {
                                AddFriend(Friends.Keys);
                            }
                            break;
                        case SupportedOptions.WHOX:
                            SupportsWhoIsExtensions = true;
                            break;
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        /// <summary>
        /// Handles <see cref="Numerics.RPL_ALTWATCHING"/> events.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnWatchStarted(WatchEventArgs e)
        {
            if (!_friends.ContainsKey(e.Nickname))
            {
                Friend newFriend = new Friend(e.Nickname, this);

                newFriend.NetworkName = NetworkName;
                newFriend.HostName = e.HostName;
                newFriend.RealName = e.RealName;
                newFriend.UserName = e.UserName;
                newFriend.IsOnline = e.IsOnline;
                _friends.Add(e.Nickname, newFriend);

                FriendAdded?.Invoke(this, e);
            }
        }

        private void OnMonitorOffline(WatchEventArgs[] watchEventArgs)
        {
            foreach (WatchEventArgs e in watchEventArgs)
            {
                if (!_friends.ContainsKey(e.Nickname))
                {
                    Friend newFriend = new Friend(e.Nickname, this);

                    newFriend.NetworkName = NetworkName;
                    newFriend.HostName = e.HostName;
                    newFriend.RealName = e.RealName;
                    newFriend.UserName = e.UserName;
                    newFriend.IsOnline = e.IsOnline;
                    _friends.Add(e.Nickname, newFriend);

                    FriendAdded?.Invoke(this, e);
                }
                else
                {
                    if (_friends[e.Nickname].IsOnline)
                    {
                        _friends[e.Nickname].IsOnline = false;
                        FriendSignedOff?.Invoke(this, e);
                    }
                }
            }
        }

        private void OnMonitorOnline(WatchEventArgs[] watchEventArgs)
        {
            foreach (WatchEventArgs e in watchEventArgs)
            {
                if (!_friends.ContainsKey(e.Nickname))
                {
                    Friend newFriend = new Friend(e.Nickname, this);

                    newFriend.NetworkName = NetworkName;
                    newFriend.HostName = e.HostName;
                    newFriend.RealName = e.RealName;
                    newFriend.UserName = e.UserName;
                    newFriend.IsOnline = e.IsOnline;
                    _friends.Add(e.Nickname, newFriend);

                    FriendAdded?.Invoke(this, e);
                }
                else
                {
                    if (!_friends[e.Nickname].IsOnline)
                    {
                        _friends[e.Nickname].IsOnline = true;
                        FriendSignedOn?.Invoke(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Handles <see cref="Irc.Numerics.RPL_STOPWATCHING"/> events.
        /// </summary>
        /// <param name="e">The event data for the event.</param>
        private void OnWatchStopped(WatchEventArgs e)
        {
            if (_friends.ContainsKey(e.Nickname))
            {
                Friend friendToRemove = _friends[e.Nickname];
                _friends.Remove(e.Nickname);
                friendToRemove.Dispose();

                FriendRemoved?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Sends a private message to the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user to send a private message to.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if connection state is not Registered or if nickname is the client's nickname.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void OpenPrivateMessage(IrcNickname nickname, string message)
        {
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }
            if (nickname == Nickname) { throw new InvalidOperationException(Properties.Resources.SelfMessageError); }

            if (!_messages.ContainsKey(nickname))
            {
                User newUser = new User(nickname, this);
                PrivateMessage newMessage = new PrivateMessage(newUser, this);
                _messages.Add(nickname, newMessage);
                PrivateMessageEventArgs e = new PrivateMessageEventArgs(nickname, Nickname) { IsLocalEcho = true, Message = message };
                PrivateMessageAdded?.Invoke(this, e);
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                _messages[nickname].SendMessage(message);
            }
        }

        /// <summary>
        /// Sends a response to the IRC server as part of the active connection heartbeat protocol.
        /// This method should only be called in response to a PING command received by the server.
        /// </summary>
        /// <param name="replyString">The string received from the server with the PING command to be echoed back with the PONG command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        private void Pong(string replyString)
        {
            Connection.WriteLine(string.Format("{0} {1}", Commands.PONG, replyString));
        }

        /// <summary>
        /// Registers a service on the network.
        /// </summary>
        /// <param name="nickname">The name of the service.</param>
        /// <param name="distribution">A mask specifying the visibility of the service.  Only servers with a 
        /// matching name will have knowledge of the service.</param>
        /// <param name="type">Reserved for future use.</param>
        /// <param name="description">A description of the service.</param>
        /// <exception cref="System.NotImplementedException">This method has no implementation.</exception>
        [Obsolete()]
        public void RegisterService(string nickname, string distribution, int type, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers the client nickname and username with the IRC server.
        /// </summary>
        /// <exception cref="Irc.AlreadyRegisteredException">Thrown if the connection is already registered on the IRC server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        private void RegisterUser()
        {
            if (ConnectionState == ConnectionStates.Registered) { throw new AlreadyRegisteredException(); }

            ChangeNickname(Nickname);
            Connection.WriteLine(string.Format("{0} {1} 8 * : {2}", Commands.USER, Username, RealName));
        }

        /// <summary>
        /// Registers the client nickname and username with the IRC server.
        /// </summary>
        /// <exception cref="Irc.AlreadyRegisteredException">Thrown if the connection is already registered on the IRC server.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        private void RegisterUser(string password)
        {
            if (ConnectionState == ConnectionStates.Registered) { throw new AlreadyRegisteredException(); }
            if (!string.IsNullOrWhiteSpace(password)) { throw new ArgumentNullException(nameof(password)); }

            Connection.WriteLine(string.Concat(Commands.PASS, " ", password));
            RegisterUser();
        }

        /// <summary>
        /// Rejoins all channels in the client's channel list and current profile.
        /// </summary>
        private void RejoinChannels()
        {
            try
            {
                if (Channels.Count > 0)
                {
                    var currentNames = from chan in Channels.Values
                                       select chan.Name;

                    JoinChannel(currentNames.ToArray());
                }
            }
            catch (TooManyChannelsException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// Removes AWAY status from the client.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RemoveAway()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            if (IsAway)
            {
                Connection.WriteLine(Commands.AWAY);
            }
        }

        /// <summary>
        /// Removes a user from the friends list.
        /// </summary>
        /// <param name="nickname">The nickname of the user to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the WATCH command is not supported on the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if nickname does not exist in the client's friend list.</exception>
        public void RemoveFriend(IrcNickname nickname)
        {
            if (!_friends.ContainsKey(nickname)) { throw new ArgumentException(Properties.Resources.NameNotFoundError); }

            if (ConnectionState == ConnectionStates.Registered)
            {
                if (SupportsMonitor)
                {
                    RemoveMonitor(nickname);
                }
                else
                {
                    RemoveWatch(nickname);
                }
            }
            else
            {
                Friend friendToRemove = _friends[nickname];
                _friends.Remove(nickname);
                friendToRemove.Dispose();
                FriendRemoved?.Invoke(this, new WatchEventArgs(friendToRemove.Nickname));
            }
        }

        private void RemoveWatch(IrcNickname nickname)
        {
            Connection.WriteLine(string.Format("{0} -{1}", Commands.WATCH, nickname));
        }

        private void RemoveMonitor(IrcNickname nickname)
        {
            Connection.WriteLine(string.Format("{0} - {1}", Commands.MONITOR, nickname));
            _friends[nickname].Dispose();
            _friends.Remove(nickname);
            FriendRemoved?.Invoke(this, new WatchEventArgs(nickname));
        }

        /// <summary>
        /// Removes a user from the friends list.
        /// </summary>
        /// <param name="nicknames">The nickname of the user to remove.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the WATCH command is not supported on the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if a name does not exist in the client's friend list.</exception>
        public void RemoveFriend(IEnumerable<IrcNickname> nicknames)
        {
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }

            var friends = (from f in _friends.Values
                           join n in nicknames on f.Nickname equals n
                           select f.Nickname);
            if (friends.Count() == 0) { throw new ArgumentException(Properties.Resources.NameNotFoundError); }

            if (ConnectionState == ConnectionStates.Registered)
            {
                if (SupportsMonitor)
                {
                    RemoveMonitor(friends);
                }
                else
                {
                    RemoveWatch(friends);
                }
            }
            else
            {
                foreach (IrcNickname f in friends)
                {
                    _friends[f].Dispose();
                    _friends.Remove(f);
                    FriendRemoved?.Invoke(this, new WatchEventArgs(Nickname));
                }
            }
        }

        private void RemoveWatch(IEnumerable<IrcNickname> friends)
        {
            StringBuilder sb = new StringBuilder(Commands.WATCH);
            foreach (IrcNickname f in friends)
            {
                if (sb.Length + Nickname.Length + 3 < MAXMESSAGELENGTH)
                {
                    sb.Append(" -");
                    sb.Append(f);
                }
                else
                {
                    Connection.WriteLine(sb.ToString());
                    sb.Clear();
                    sb.Append(Commands.WATCH);
                }
            }

            if (sb.ToString() != Commands.WATCH)
            {
                Connection.WriteLine(sb.ToString());
            }
        }

        private void RemoveMonitor(IEnumerable<IrcNickname> friends)
        {
            StringBuilder sb = new StringBuilder(Commands.MONITOR + " - ");
            foreach (IrcNickname f in friends)
            {
                if (sb.Length + Nickname.Length + 3 < MAXMESSAGELENGTH)
                {
                    sb.Append(f);
                    sb.Append(",");
                }
                else
                {
                    Connection.WriteLine(sb.ToString());
                    sb.Clear();
                    sb.Append(Commands.MONITOR + " - ");
                }
            }

            if (sb.ToString() != Commands.MONITOR + " - ")
            {
                Connection.WriteLine(sb.ToString());
            }

            foreach (IrcNickname f in friends)
            {
                _friends[f].Dispose();
                _friends.Remove(f);
                FriendRemoved?.Invoke(this, new WatchEventArgs(f));
            }
        }

        /// <summary>
        /// Requests information about the administrator of the server currently connected to.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestAdminInfo()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.ADMIN);
        }

        /// <summary>
        /// Requests information about the administrator of the server from the specified target.
        /// </summary>
        /// <param name="target">A server name, a server mask, or the nickname of a user
        /// for the server they are currently connected to.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestAdminInfo(string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.ADMIN, target));
        }

        /// <summary>
        /// Gets the current parameters for the specified channel mode.
        /// </summary>
        /// <param name="channelName">A channel name.</param>
        /// <param name="mode">Then channel mode to request.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        /// <exception cref="Irc.UnsupportedModeException">Thrown if mode is not a supported channel mode.</exception>
        public void RequestChannelModeParameters(IrcChannelName channelName, char mode)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (!SupportedChannelModes.Contains(mode)) { throw new NotSupportedException(Properties.Resources.UnsupportedChannelModeExceptionMessage); }

            if (!SupportedChannelModes.Contains(mode)) { throw new UnsupportedModeException() { Mode = mode }; }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.MODE, channelName, mode));
        }

        /// <summary>
        /// Gets the current mode of the specified channel.
        /// </summary>
        /// <param name="channelName">A channel name.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void RequestChannelModes(IrcChannelName channelName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.MODE, channelName));
        }

        /// <summary>
        /// Requests the current topic of the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void RequestChannelTopic(IrcChannelName channelName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.TOPIC, channelName));
        }

        /// <summary>
        /// Gets the current mode of the client.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestClientModes()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.MODE, Nickname));
        }

        /// <summary>
        /// Requests a list of all visible channels on the server.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestGlobalChannelList()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.LIST);
        }

        /// <summary>
        /// Requests information on help topics.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        public void RequestHelp()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.HELP);
        }

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestLUsers()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.LUSERS);
        }

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        /// <param name="mask">A mask to filter the servers included in the reply.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestLUsers(string mask)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.LUSERS, mask));
        }

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        /// <param name="mask">A mask to filter the servers included in the reply.</param>
        /// <param name="target">The name of the server on the network to process the command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestLUsers(string mask, string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.LUSERS, mask, target));
        }

        /// <summary>
        /// Requests the server message of the day.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestMOTD()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.MOTD);
        }

        /// <summary>
        /// Lists all users and channels visible to the client.
        /// </summary>
        public void RequestNames()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.NAMES);
        }

        /// <summary>
        /// Gets the list of users in the specified channel.
        /// </summary>
        /// <param name="channelName">The name of a channel to list users for.</param>
        /// <param name="channelNames">An optional list of channels to list users for.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void RequestNames(IrcChannelName channelName, params IrcChannelName[] channelNames)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.NAMES, channelName, channelNames));
        }

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        public void RequestNetworkLayout()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(Commands.LINKS);
        }

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        /// <param name="mask">A filter to apply to the list of server names returned.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestNetworkLayout(string mask)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1}", Commands.LINKS, mask));
        }

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        /// <param name="mask">A filter to apply to the list of server names returned.</param>
        /// <param name="remoteServer">The name of a server on the network to process the command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestNetworkLayout(string mask, string remoteServer)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1} {2}", Commands.LINKS, mask, remoteServer));
        }

        /// <summary>
        /// Requests the rules list from the network.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestNetworkRules()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(Commands.RULES);
        }

        /// <summary>
        /// Requests information describing a server.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestServerInfo()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.INFO);
        }

        /// <summary>
        /// Requests information describing a server.
        /// </summary>
        /// <param name="target">A server name, a server mask, or the nickname of a user
        /// for the server they are connected to.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestServerInfo(string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.INFO, target));
        }

        /// <summary>
        /// Requests the local server time.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestServerTime()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.TIME);
        }

        /// <summary>
        /// Requests information describing the server software.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestServerVersion()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.VERSION);
        }

        /// <summary>
        /// Requests information describing the server software of the specified server.
        /// </summary>
        /// <param name="target">A server name.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestServerVersion(string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            if (string.IsNullOrWhiteSpace(target))
            {
                RequestServerVersion();
            }
            else
            {
                Connection.WriteLine(string.Format("{0} {1}", Commands.VERSION, target));
            }
        }

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        [Obsolete()]
        public void RequestServicesList()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(Commands.SERVLIST);
        }

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        /// <param name="mask">A name filter to apply to the list of services returned.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        [Obsolete()]
        public void RequestServicesList(string mask)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1}", Commands.SERVLIST, mask));
        }

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        /// <param name="mask">A name filter to apply to the list of services returned.</param>
        /// <param name="type">The type of service to return.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        [Obsolete()]
        public void RequestServicesList(string mask, string type)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1} {2}", Commands.SERVLIST, mask, type));
        }

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestStats()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(Commands.STATS);
        }

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        /// <param name="query">A command specifying the information to return.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestStats(string query)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1}", Commands.STATS, query));
        }

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        /// <param name="query">A command specifying the information to return.</param>
        /// <param name="target">The name of the server on the network to process the command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestStats(string query, string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            _connection.WriteLine(string.Format("{0} {1} {2}", Commands.STATS, query, target));
        }

        /// <summary>
        /// Requests information about the specified nicknames.
        /// </summary>
        /// <param name="nicknames">A list of nicknames to return information about.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nicknames is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if nicknames contains no values.</exception>
        public void RequestUserHost(params IrcNickname[] nicknames)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nicknames == null) { throw new ArgumentNullException(nameof(nicknames)); }
            if (nicknames.Length == 0) { throw new ArgumentOutOfRangeException("nicknames", "Nicknames must contain at least 1 value."); }

            string line = Commands.USERHOST;

            /**
             * According to the IRC protocol, USERHOST only accepts up to 5 nicknames.
             */
            for (int i = 0; i < nicknames.Length && i < 5; i++)
            {
                if (nicknames[i] != null)
                {
                    line = string.Concat(line, " ", nicknames[i]);
                }
            }

            Connection.WriteLine(line);
        }

        /// <summary>
        /// Requests the IP address of the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of a user.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not Registered.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the USERIP command is not supported on the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void RequestUserIP(IrcNickname nickname)
        {
            if (!SupportsUserIPCommand) { throw new NotSupportedException(Properties.Resources.UserIPNotSupported); }
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("USERIP {0}", nickname));
        }

        /// <summary>
        /// Returns a list of users logged into the server.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestUsers()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.USERS);
        }

        /// <summary>
        /// Returns a list of users logged into the server.
        /// </summary>
        /// <param name="target">The name of the server that will process the command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void RequestUsers(string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.USERS, target));
        }

        /// <summary>
        /// Resets network-scope client data.
        /// </summary>
        private void ResetNetworkData()
        {
            foreach (Channel c in _channels.Values)
            {
                c.Dispose();
                ChannelLeft?.Invoke(this, new PartEventArgs(c.Name, Nickname));
            }
            _channels.Clear();

            foreach (PrivateMessage pm in _messages.Values)
            {
                pm.Dispose();
                PrivateMessageRemoved?.Invoke(this, new UserEventArgs(pm.Name));
            }
            _messages.Clear();

            foreach (Friend f in _friends.Values.ToArray())
            {
                RemoveFriend(f.Nickname);
            }
            //_friends.Clear();


            foreach (IgnoredUser iu in _ignores.Values.ToArray())
            {
                Unignore(iu.Mask);
            }

            NetworkName = null;
        }

        /// <summary>
        /// Resets the server options to default values defined in RFC2812.
        /// </summary>
        private void ResetServerOptions()
        {
            ChannelLimit = 10;
            MaximumNicknameLength = 28;
            BanExceptionListLength = 30;
            InviteExceptionListLength = 30;
            MaximumChannelModeChanges = 4;
            TopicLength = 80;
            KickMessageLength = 80;
            ChannelNameLength = 50;
            AwayMessageLength = 80;
            MaximumMessageTargets = 4;
            SupportsKnock = false;
            CanMessageOperators = false;
            SupportedChannelModes = string.Empty;
            SupportedChannelTypes = string.Empty;
            SupportsBanExceptions = false;
            SupportsMonitor = false;
            SupportsWatch = false;
            SupportsIgnoreList = false;
            SupportsInviteExceptions = false;
            SupportsLocalChannels = false;
            SupportsModelessChannels = false;
            SupportsSafeChannels = false;
            SupportsUserIPCommand = false;
            UniqueID = null;
            IsAway = false;
            ModeString = null;
        }

        /// <summary>
        /// Sends a message to a user or channel.
        /// If the message exceeds the maximum allowed length of 512 characters, the message will be broken into multiple messages.
        /// </summary>
        /// <param name="target">A nickname of a user or channel name.</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if target or message is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if target is not a valid channel name or nickname as defined by RFC2812.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if target is the client's nickname.</exception>
        public void SendMessage(IrcNameBase target, string message)
        {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }
            if (!(target is IrcChannelName) && !(target is IrcNickname)) { throw new ArgumentException(Properties.Resources.TargetNameFormatError); }
            if (target == Nickname) { throw new InvalidOperationException(Properties.Resources.SelfMessageError); }

            /**
             * Since messages can only be a maximum of 512 characters, if the command + target name + message body
             * is greater than 512, it must be broken into multiple messages.
             */
            do
            {
                Connection.WriteLine(string.Format("{0} {1} :{2}", Commands.PRIVMSG, target, message));

                message = message.Substring(message.Length > MAXMESSAGELENGTH ? MAXMESSAGELENGTH : message.Length);
            }
            while (message.Length > 0);
        }

        /// <summary>
        /// Sends a notice message to the specified target.  This method should never be called in reponse to a NOTICE 
        /// received from the server.
        /// </summary>
        /// <param name="targetName">The name of the target to contact.</param>
        /// <param name="message">A message to send.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if targetName or message is null.</exception>
        /// <exception cref="System.InvalidOperationException">Throw if ConnectionState is not Connected or Registered.</exception>
        /// <exception cref="System.IO.IOException"/>
        public void SendNotice(IrcNameBase targetName, string message)
        {
            if (ConnectionState != ConnectionStates.Connected && ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotConnectedMessage); }
            if (targetName == null) { throw new ArgumentNullException(nameof(targetName)); }

            Connection.WriteLine(string.Format("{0} {1} :{2}", Commands.NOTICE, targetName, message));
        }

        /// <summary>
        /// Sends a message to a service.
        /// </summary>
        /// <param name="serviceName">The name of a service..</param>
        /// <param name="message">The message to send.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if serviceName or message is null or the empty string.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if serviceName is the client's nickname.</exception>
        public void SendServiceQuery(string serviceName, string message)
        {
            if (string.IsNullOrEmpty(serviceName)) { throw new ArgumentNullException(nameof(serviceName)); }
            if (string.IsNullOrEmpty(message)) { throw new ArgumentNullException(nameof(message)); }
            if (serviceName == Nickname) { throw new InvalidOperationException(Properties.Resources.SelfMessageError); }

            Connection.WriteLine(string.Format("{0} {1} :{2}", Commands.SQUERY, serviceName, message));
        }

        /// <summary>
        /// Sets the client status as away.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void SetAway()
        {
            SetAway(AwayMessage ?? Properties.Resources.DefaultAwayMessage);
        }

        /// <summary>
        /// Sets the client status as away with the specified away message.
        /// </summary>
        /// <param name="awayMessage">An away message.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void SetAway(string awayMessage)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (string.IsNullOrEmpty(awayMessage)) { throw new ArgumentNullException(nameof(awayMessage)); }

            if (!IsAway)
            {
                if (awayMessage.Length > AwayMessageLength) { awayMessage = awayMessage.Substring(0, AwayMessageLength); }
                Connection.WriteLine(string.Format("{0} :{1}", Commands.AWAY, awayMessage));
            }
        }

        /// <summary>
        /// Changes the channel mode options of the specified user or channel.
        /// </summary>
        /// <param name="channelName">The name of channel to change the modes of.</param>
        /// <param name="modeString">The string of mode changes.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void SetChannelModes(IrcChannelName channelName, ChannelModeString modeString)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }
            if (modeString == null) { throw new ArgumentNullException(nameof(modeString)); }

            foreach (Mode m in modeString)
            {
                if (!SupportedChannelModes.Contains(m.ModeChar)) { throw new UnsupportedModeException() { Mode = m }; }
            }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.MODE, channelName, modeString));
        }

        /// <summary>
        /// Sets or changes the topic of a channel.
        /// </summary>
        /// <param name="channelName">The name of the channel.</param>
        /// <param name="newTopic">The topic to set.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if channelName is null.</exception>
        public void SetChannelTopic(IrcChannelName channelName, string newTopic)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (channelName == null) { throw new ArgumentNullException(nameof(channelName)); }

            Connection.WriteLine(string.Format("{0} {1} :{2}", Commands.TOPIC, channelName, newTopic == null ? string.Empty : newTopic));
        }

        /// <summary>
        /// Sets the specified modes for the client.
        /// </summary>
        /// <param name="modeString">A mode string containing the modes to set.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if modeString is null.</exception>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="Irc.UnsupportedModeException">Thrown if modeString contains an unsupported client mode.</exception>
        public void SetClientModes(ClientModeString modeString)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (modeString == null) { throw new ArgumentNullException(nameof(modeString)); }

            foreach (Mode m in modeString)
            {
                if (!ClientModes.IsMode(m)) { throw new UnsupportedModeException(Properties.Resources.UnsupportedModeExceptionMessage) { Mode = m }; }
            }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.MODE, Nickname, modeString));
        }

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void Summon(IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.SUMMON, nickname));
        }

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        /// <param name="targetServer">The name of the server that will process the command.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void Summon(IrcNickname nickname, string targetServer)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1} {2}", Commands.SUMMON, nickname, targetServer));
        }

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        /// <param name="targetServer">The name of the server that will process the command.</param>
        /// <param name="channelName">A channel name to for the summoned person to join.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void Summon(IrcNickname nickname, string targetServer, IrcChannelName channelName)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1} {2} {3}", Commands.SUMMON, nickname, targetServer, channelName));
        }

        /// <summary>
        /// Requests the servers the local server has a direct connection to.
        /// </summary>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void TraceRoute()
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(Commands.TRACE);
        }

        /// <summary>
        /// Requests a trace route to the specified server.
        /// </summary>
        /// <param name="target">A server name or the server the user with the specified
        /// nickname is connected to.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        public void TraceRoute(string target)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.TRACE, target));
        }

        /// <summary>
        /// Removes a user from the server-side ignore list.
        /// </summary>
        /// <param name="mask">The name or hostmask of the user to remove.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if mask is null.</exception>
        public void Unignore(string mask)
        {
            if (string.IsNullOrWhiteSpace(mask)) { throw new ArgumentNullException(nameof(mask)); }

            if (ConnectionState == ConnectionStates.Registered)
            {
                _connection.WriteLine(string.Format("{0} -{1}", Commands.SILENCE, mask));
            }
            else
            {
                _ignores.Remove(mask);
                SilenceEventArgs e = new SilenceEventArgs();
                e.Masks.Add(mask, false);
                IgnoreRemoved?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Adds a list of users to a server-side ignore list.
        /// </summary>
        /// <param name="ignoreList">The list of users to ignore.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if ignoreList is null.</exception>
        public void Unignore(IEnumerable<string> ignoreList)
        {
            if (ignoreList == null) { throw new ArgumentNullException(nameof(ignoreList)); }
            
            if (ConnectionState == ConnectionStates.Registered)
            {
                StringBuilder sb = new StringBuilder(Commands.SILENCE);

                foreach (string iu in ignoreList)
                {
                    if (!string.IsNullOrWhiteSpace(iu))
                    {
                        if (sb.Length + iu.Length < MAXMESSAGELENGTH)
                        {
                            sb.Append(" -");
                            sb.Append(iu);
                        }
                        else
                        {
                            Connection.WriteLine(sb.ToString());
                            sb.Clear();
                            sb.Append(Commands.SILENCE);
                        }
                    }
                }

                if (sb.ToString() != Commands.SILENCE)
                {
                    Connection.WriteLine(sb.ToString());
                }
            }
            else
            {
                foreach (string iu in ignoreList)
                {
                    _ignores.Remove(iu);
                    SilenceEventArgs e = new SilenceEventArgs();
                    e.Masks.Add(iu, false);
                    IgnoreRemoved?.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// Requests a list of users whose name matches the specified string.
        /// </summary>
        /// <param name="searchString">The string to match the usernames.   A star (*) can be used as a wildcard to match one or more characters.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if matchString is null.</exception>
        public void Who(string searchString)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (string.IsNullOrWhiteSpace(searchString)) { throw new ArgumentNullException(nameof(searchString)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.WHO, searchString));
        }

        /// <summary>
        /// Requests information about a user currently connected to the server, including but not limited to hostname and idle time.
        /// </summary>
        /// <param name="nickname">The nickname of a user.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void WhoIs(IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.WHOIS, nickname));
        }

        /// <summary>
        /// Requests information about a user who was previously connected to the server.  The period of time this information is available may vary between servers.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        /// <exception cref="System.IO.IOException">Thrown if there is a problem communicating with the server.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if ConnectionState is not currently Registered.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname is null.</exception>
        public void WhoWas(IrcNickname nickname)
        {
            if (ConnectionState != ConnectionStates.Registered) { throw new InvalidOperationException(Properties.Resources.NotRegisteredMessage); }
            if (nickname == null) { throw new ArgumentNullException(nameof(nickname)); }

            Connection.WriteLine(string.Format("{0} {1}", Commands.WHOWAS, nickname));
        }
    }
}
