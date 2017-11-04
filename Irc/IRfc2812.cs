/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;

namespace Irc
{
    /// <summary>
    /// Defines methods, properties, and events for implementing the Internet Relay Chat (IRC) protocol as outlined in RFC2812.
    /// </summary>
    /// <remarks>
    /// See https://www.rfc-editor.org/rfc/rfc2812.txt
    /// </remarks>
    public interface IRfc2812
    {
        #region Properties...

        /// <summary>
        /// Gets the current connection state of the IRC connection.
        /// </summary>
        ConnectionStates ConnectionState { get; }

        /// <summary>
        /// Gets the channel modes supported by the server.
        /// </summary>
        string SupportedChannelModes { get; }

        /// <summary>
        /// Gets the client modes supported by the server.
        /// </summary>
        string SupportedClientModes { get; }

        /// <summary>
        /// Gets a value indicating if the server supports ban exceptions.
        /// </summary>
        bool SupportsBanExceptions { get; }

        /// <summary>
        /// Gets a value indicating if the server supports invite exceptions.
        /// </summary>
        bool SupportsInviteExceptions { get; }

        /// <summary>
        /// Gets the nickname specified to register on an IRC network.
        /// </summary>
        IrcNickname Nickname { get;}

        /// <summary>
        /// Gets the real name specified to register on an IRC network.
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// Gets the user name specified to register on an IRC network.
        /// </summary>
        IrcUsername Username { get; }

        #endregion

        #region Events...

        /// <summary>
        /// Occurs when an item in a channel ban exception list is received.
        /// </summary>
        event EventHandler<ChannelManagementListEventArgs> BanExceptionListItemReceived;

        /// <summary>
        /// Occurs when an item in a channel ban list is received.
        /// </summary>
        event EventHandler<ChannelManagementListEventArgs> BanListItemReceived;

        /// <summary>
        /// Occurs when the client is prevented from changing its nickname.
        /// </summary>
        event EventHandler<ErrorEventArgs> CannotChangeNickname;

        /// <summary>
        /// Occurs when a channel creation timestamp has been received.
        /// </summary>
        event EventHandler<ChannelCreationEventArgs> ChannelCreationTimeReceived;

        /// <summary>
        /// Occurs when a channel is joined.
        /// </summary>
        event EventHandler<JoinEventArgs> ChannelJoined;

        /// <summary>
        /// Occurs when a user or the client has left a channel.
        /// </summary>
        event EventHandler<PartEventArgs> ChannelLeft;

        /// <summary>
        /// Occurs when the list of channels on the server is received.
        /// </summary>
        event EventHandler<ChannelListEventArgs> ChannelListReceived;

        /// <summary>
        /// Occurs when the mode of a channel the client has joined is changed.
        /// </summary>
        event EventHandler<ChannelModeEventArgs> ChannelModeChanged;

        /// <summary>
        /// Occurs when the modes currently set on a channel are received.  This 
        /// generally occurs when first joining a channel.
        /// </summary>
        event EventHandler<ChannelModeEventArgs> ChannelModesReceived;

        /// <summary>
        /// Occurs when the name of a channel owner is received.
        /// </summary>
        event EventHandler<ChannelCreatorEventArgs> ChannelOwnerReceived;

        /// <summary>
        /// Occurs when the topic of a channel the user has joined is changed.
        /// </summary>
        event EventHandler<TopicChangeEventArgs> ChannelTopicChanged;

        /// <summary>
        /// Occurs when the topic if a channel is received.
        /// </summary>
        event EventHandler<TopicEventArgs> ChannelTopicReceived;

        /// <summary>
        /// Occurs when the list of users in a channel the client has joined is received.
        /// </summary>
        event EventHandler<NameListEventArgs> ChannelUserListItemReceived;

        /// <summary>
        /// Occurs when the client mode is changed.
        /// </summary>
        event EventHandler ClientModesChanged;

        /// <summary>
        /// Occurs when the connection state of the IRC connection has changed.
        /// </summary>
        event EventHandler ConnectionStateChanged;

        /// <summary>
        /// Occurs when the end of a channel's ban exception list is received.
        /// </summary>
        event EventHandler EndOfBanExceptionList;

        /// <summary>
        /// Occurs when the end of a channel's ban list is received.
        /// </summary>
        event EventHandler EndOfBanList;

        /// <summary>
        /// Occurs when the end of a user name list for a channel has been received.
        /// </summary>
        event EventHandler<DataEventArgs> EndOfChannelUserList;

        /// <summary>
        /// Occurs when the end of a channel's invitation exception list is received.
        /// </summary>
        event EventHandler EndOfInvitationExceptionList;

        /// <summary>
        /// Occurs when the list of network links has ended.
        /// </summary>
        event EventHandler EndOfLinks;

        /// <summary>
        /// Occurs when the end of the servist list is reached.
        /// </summary>
        event EventHandler EndOfServices;

        /// <summary>
        /// Occurs when the end of a WHO query is received.
        /// </summary>
        event EventHandler EndOfWho;

        /// <summary>
        /// Occurs when an item in a channel invitation exception list is received.
        /// </summary>
        event EventHandler<ChannelManagementListEventArgs> InvitationExceptionListItemReceived;

        /// <summary>
        /// Occurs when a channel mode cannot be set because the channel requires Invitation Only status.
        /// </summary>
        event EventHandler InvitationOnlyRequired;

        /// <summary>
        /// Occurs when a channel invitation has been received by another user.
        /// </summary>
        event EventHandler<InviteEventArgs> InvitationReceived;

        /// <summary>
        /// Occurs when a user has been invited to a channel.
        /// </summary>
        event EventHandler<NoticeEventArgs> Inviting;

        /// <summary>
        /// Occurs when the client has been authenticated as an IRC operator.
        /// </summary>
        event EventHandler<NoticeEventArgs> IrcOperatorGranted;

        /// <summary>
        /// Occurs when the client status is changed to or from away.
        /// </summary>
        event EventHandler IsAwayChanged;

        /// <summary>
        /// Occurs when the client has been kicked from a channel.
        /// </summary>
        event EventHandler<KickEventArgs> Kicked;

        /// <summary>
        /// Occurs when a reply to the LINKS command is received.
        /// </summary>
        event EventHandler<LinksEventArgs> LinkReceived;

        /// <summary>
        /// Occurs when the nickname specified by the client has already been registered.
        /// </summary>
        event EventHandler NicknameAlreadyRegistered;

        /// <summary>
        /// Occurs when a user has changed their nickname.
        /// </summary>
        event EventHandler<NickChangeEventArgs> NicknameChanged;

        /// <summary>
        /// Occurs when the server has detected a nickname collision after recovering
        /// from a network split.
        /// </summary>
        event EventHandler NicknameCollision;

        /// <summary>
        /// Occurs when the nickname the client has attempted to change to is 
        /// already in use.
        /// </summary>
        event EventHandler<ErrorEventArgs> NicknameInUse;

        /// <summary>
        /// Occurs when a notice from the server is received.
        /// </summary>
        event EventHandler<NoticeEventArgs> NoticeReceived;

        /// <summary>
        /// Occurs when a message from a channel or user is received.
        /// </summary>
        event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;

        /// <summary>
        /// Occurs when a reply to the REHASH command is received.
        /// </summary>
        event EventHandler Rehashing;

        /// <summary>
        /// Occurs when the server requests that the previous command be resent.
        /// </summary>
        event EventHandler RetryCommand;

        /// <summary>
        /// Occurs when the local server time is received;
        /// </summary>
        event EventHandler<NoticeEventArgs> ServerTimeReceived;

        /// <summary>
        /// Occurs when information about a service registered on the network is received
        /// in response to the SERVLIST command.
        /// </summary>
        event EventHandler<ServiceListEventArgs> ServiceReceived;

        /// <summary>
        /// Occurs when a notice in response to the SUMMON command is received.
        /// </summary>
        event EventHandler<NoticeEventArgs> Summoning;

        /// <summary>
        /// Occurs when a reply to the TRACE command is received.
        /// </summary>
        event EventHandler<NoticeEventArgs> TraceReceived;

        /// <summary>
        /// Occurs when a reply is received that is not handled by any other event handlers.
        /// </summary>
        event EventHandler<ReplyEventArgs> UnhandledReply;

        /// <summary>
        /// Occurs when a reply to the USERHOST command is received.
        /// </summary>
        event EventHandler<UserHostEventArgs> UserHostReplyReceived;

        /// <summary>
        /// Occurs when a channel invitation has been sent to another user
        /// in a channel the client has joined.
        /// </summary>
        event EventHandler<InviteEventArgs> UserInvited;

        /// <summary>
        /// Occurs when the client receives a notice that a visible user is away.
        /// </summary>
        event EventHandler<AwayEventArgs> UserIsAway;

        /// <summary>
        /// Occurs when a user has joined a channel the client has joined.
        /// </summary>
        event EventHandler<JoinEventArgs> UserJoinedChannel;

        /// <summary>
        /// Occurs when a user is kicked from a channel the client has joined.
        /// </summary>
        event EventHandler<KickEventArgs> UserKicked;

        /// <summary>
        /// Occurs when a user leaves a channel the client has joined.
        /// </summary>
        event EventHandler<PartEventArgs> UserLeftChannel;

        /// <summary>
        /// Occurs when a user quits the network.
        /// </summary>
        event EventHandler<QuitEventArgs> UserQuit;

        /// <summary>
        /// Occurs when a response to the USERS command is received.
        /// </summary>
        event EventHandler<UsersCommandEventArgs> UsersReplyReceived;

        /// <summary>
        /// Occurs when the server's version information has been received.
        /// </summary>
        event EventHandler<VersionEventArgs> VersionReceived;

        /// <summary>
        /// Occurs when the response to a WHOIS command is received.
        /// </summary>
        event EventHandler<WhoIsEventArgs> WhoIsReceived;

        /// <summary>
        /// Occurs when the reponse to a WHO command is received.
        /// </summary>
        event EventHandler<WhoEventArgs> WhoReceived;

        #endregion

        /// <summary>
        /// Authenticates the user as an IRC operator on the network.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password used for authentications.</param>
        void AuthenticateOperator(IrcUsername username, string password);

        /// <summary>
        /// Changes the client's nickname.
        /// </summary>
        /// <param name="newNickname">The nickname to change to.</param>
        void ChangeNickname(IrcNickname newNickname);

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <see cref="Irc.Commands.QUIT"/>
        void Disconnect();

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <param name="farewellMessage">A farewell message to send when disconnecting from the server.</param>
        /// <see cref="Irc.Commands.QUIT"/>
        void Disconnect(string farewellMessage);

        /// <summary>
        /// Invites a user to the specified channel.
        /// </summary>
        /// <param name="channelName">The channel to invite a user to.</param>
        /// <param name="nickname">The nickname of the user to invite.</param>
        void Invite(IrcChannelName channelName, IrcNickname nickname);

        /// <summary>
        /// Requests the online status of the specified list of users.
        /// </summary>
        /// <param name="nickname">The nickname of a user for whom to check online status.</param>
        /// <param name="nicknames">An optional list of additional nicknames for whom to 
        /// check online status.</param>
        void IsOn(IrcNickname nickname, params IrcNickname[] nicknames);

        /// <summary>
        /// Joins the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to join.</param>
        void JoinChannel(IrcChannelName channelName);

        /// <summary>
        /// Joins the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to join.</param>
        /// <param name="password">An optional password, if required.</param>
        void JoinChannel(IrcChannelName channelName, IrcPassword password);

        /// <summary>
        /// Joins the specified list of channels.
        /// </summary>
        /// <param name="channelNames">An array containing a list of channels to join.</param>
        /// <example>
        /// JOIN #foo,#bar fubar           ;Command to join channel #foo using key "fubar" and #bar using no key.
        /// JOIN #foo,#bar fubar,foobar    ;Command to join channel #foo using key "fubar", and channel #bar using key "foobar".
        /// </example>
        void JoinChannel(IEnumerable<IrcChannelName> channelNames);

        /// <summary>
        /// Joins the specified list of channels.
        /// </summary>
        /// <param name="channelNames">An array containing a list of channels to join.</param>
        /// <param name="passwords">Optional passwords if required, null otherwise.</param>
        /// <remarks>
        /// Calling this method with a list of channels does not require including a list of passwords.  
        /// The passwords parameter may be a null value, or an array of lesser size than the list of 
        /// channel names.  Passwords are applied to channels at the same index in the channel name 
        /// array.
        /// </remarks>
        /// <example>
        /// JOIN #foo,#bar fubar           ;Command to join channel #foo using key "fubar" and #bar using no key.
        /// JOIN #foo,#bar fubar,foobar    ;Command to join channel #foo using key "fubar", and channel #bar using key "foobar".
        /// </example>
        void JoinChannel(IEnumerable<IrcChannelName> channelNames, IEnumerable<IrcPassword> passwords);

        /// <summary>
        /// Forcibly removes a user from the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel from which to remove a user.</param>
        /// <param name="nickname">The name of the user to remove.</param>
        void Kick(IrcChannelName channelName, IrcNickname nickname);

        /// <summary>
        /// Forcibly removes a user from the specified channel.
        /// </summary>
        /// <param name="channelName">The name of the channel from which to remove a user.</param>
        /// <param name="nickname">The name of the user to remove.</param>
        /// <param name="reason">An optional message to explain why the user is being removed.</param>
        void Kick(IrcChannelName channelName, IrcNickname nickname, string reason);

        /// <summary>
        /// Leaves a channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to leave.</param>
        void LeaveChannel(IrcChannelName channelName);

        /// <summary>
        /// Leave a channel.
        /// </summary>
        /// <param name="channelName">The name of the channel to leave.</param>
        /// <param name="farewellMessage">A farewell message to send when leaving the channel.</param>
        void LeaveChannel(IrcChannelName channelName, string farewellMessage);

        /// <summary>
        /// Registers a service on the network.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="distribution">A mask specifying the visibility of the service.  Only servers with a 
        /// matching name will have knowledge of the service.</param>
        /// <param name="type">Reserved for future use.</param>
        /// <param name="description">A description of the service.</param>
        [Obsolete()]
        void RegisterService(string serviceName, string distribution, int type, string description);

        /// <summary>
        /// Sets the client status as away, or removes the status if the client is already away.
        /// </summary>
        void RemoveAway();

        /// <summary>
        /// Requests information about the administrator of the server currently connected to.
        /// </summary>
        void RequestAdminInfo();

        /// <summary>
        /// Requests information about the administrator about the server at the specified target.
        /// </summary>
        /// <param name="target">A server name, a server mask, or the nickname of a user
        /// for the server they are currently connected to.</param>
        void RequestAdminInfo(string target);

        /// <summary>
        /// Gets the current parameters for the specified channel mode.
        /// </summary>
        /// <param name="channelName">A channel name.</param>
        /// <param name="mode">The channel mode to request.</param>
        void RequestChannelModeParameters(IrcChannelName channelName, char mode);

        /// <summary>
        /// Gets the current modes of the specified channel.
        /// </summary>
        /// <param name="channelName">A channel name.</param>
        void RequestChannelModes(IrcChannelName channelName);

        /// <summary>
        /// Requests the current topic of the specified channel.
        /// </summary>
        /// <param name="channelName">The name of a channel.</param>
        void RequestChannelTopic(IrcChannelName channelName);

        /// <summary>
        /// Gets the current modes set for the client.
        /// </summary>
        void RequestClientModes();

        /// <summary>
        /// Requests a list of all available public channels on the server.
        /// </summary>
        void RequestGlobalChannelList();

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        void RequestLUsers();

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        /// <param name="mask">A mask to filter the servers included in the reply.</param>
        void RequestLUsers(string mask);

        /// <summary>
        /// Requests statistics about the size of the network.
        /// </summary>
        /// <param name="mask">A mask to filter the servers included in the reply.</param>
        /// <param name="target">The name of the server on the network to process the command.</param>
        void RequestLUsers(string mask, string target);

        /// <summary>
        /// Requests the server Message of the Day.
        /// </summary>
        void RequestMOTD();

        /// <summary>
        /// Lists all users and channels visible to the client.
        /// </summary>
        void RequestNames();

        /// <summary>
        /// Lists all users visible to the client on the specified channels.
        /// </summary>
        /// <param name="channelName">The name of a channel to list users for.</param>
        /// <param name="channelNames">An optional list of channels to list users for.</param>
        void RequestNames(IrcChannelName channelName, params IrcChannelName[] channelNames);

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        void RequestNetworkLayout();

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        /// <param name="mask">A filter to apply to the list of server names returned.</param>
        void RequestNetworkLayout(string mask);

        /// <summary>
        /// Requests a list of servers known by the current server.
        /// </summary>
        /// <param name="mask">A filter to apply to the list of server names returned.</param>
        /// <param name="remoteServer">The name of a server on the network to process the command.</param>
        void RequestNetworkLayout(string mask, string remoteServer);

        /// <summary>
        /// Requests information describing a server.
        /// </summary>
        void RequestServerInfo();

        /// <summary>
        /// Requests information describing a server.
        /// </summary>
        /// <param name="target">A server name, a server mask, or the nickname of a user
        /// for the server they are connected to.</param>
        void RequestServerInfo(string target);

        /// <summary>
        /// Requests the local server time.
        /// </summary>
        void RequestServerTime();

        /// <summary>
        /// Requests information describing the server software.
        /// </summary>
        void RequestServerVersion();

        /// <summary>
        /// Requests information describing the server software of the specified server.
        /// </summary>
        /// <param name="target">A server name.</param>
        void RequestServerVersion(string target);

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        [Obsolete()]
        void RequestServicesList();

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        /// <param name="mask">A name filter to apply to the list of services returned.</param>
        [Obsolete()]
        void RequestServicesList(string mask);

        /// <summary>
        /// Requests a list of services connected to the network.
        /// </summary>
        /// <param name="mask">A name filter to apply to the list of services returned.</param>
        /// <param name="type">The type of service to return.</param>
        [Obsolete()]
        void RequestServicesList(string mask, string type);

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        void RequestStats();

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        /// <param name="query">A command specifying the information to return.</param>
        void RequestStats(string query);

        /// <summary>
        /// Requests statistics about a server.
        /// </summary>
        /// <param name="query">A command specifying the information to return.</param>
        /// <param name="target">The name of the server that will process the command.</param>
        void RequestStats(string query, string target);

        /// <summary>
        /// Requests information about the specified nicknames.
        /// </summary>
        /// <param name="nicknames">A list of nicknames to return information about.</param>
        void RequestUserHost(params IrcNickname[] nicknames);

        /// <summary>
        /// Returns a list of users logged into the server.
        /// </summary>
        void RequestUsers();

        /// <summary>
        /// Returns a list of users logged into the server.
        /// </summary>
        /// <param name="target">The name of the server that will process the command.</param>
        void RequestUsers(string target);

        /// <summary>
        /// Sends a message to the specified user or channel.
        /// </summary>
        /// <param name="target">The nickname of a user or channel to send a message to.</param>
        /// <param name="message">The message to send.</param>
        void SendMessage(IrcNameBase target, string message);

        /// <summary>
        /// Sends a notice message to the specified user.
        /// </summary>
        /// <param name="targetName">The name of the target to contact.</param>
        /// <param name="message">A message to send.</param>
        void SendNotice(IrcNameBase targetName, string message);

        /// <summary>
        /// Sends a message to a service.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="message">A message to send.</param>
        [Obsolete()]
        void SendServiceQuery(string serviceName, string message);

        /// <summary>
        /// Sets the client status as away with the specified away-message, or
        /// removes the status if the client is already away.
        /// </summary>
        /// <param name="message"></param>
        void SetAway(string message);

        /// <summary>
        /// Changes the mode options of the specified channel.
        /// </summary>
        /// <param name="channelName">The name of a channel to change the modes of.</param>
        /// <param name="modeString">The string of mode changes.</param>
        void SetChannelModes(IrcChannelName channelName, ChannelModeString modeString);

        /// <summary>
        /// Sets or changes the topic of the specified channel.
        /// </summary>
        /// <param name="channelName">The name of a channel.</param>
        /// <param name="newTopic">The topic to change to.</param>
        void SetChannelTopic(IrcChannelName channelName, string newTopic);

        /// <summary>
        /// Sets the specified modes for the client.
        /// </summary>
        /// <param name="modeString">A mode string containing the modes to set.</param>
        void SetClientModes(ClientModeString modeString);

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        void Summon(IrcNickname nickname);

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        /// <param name="targetServer">The name of the server that will process the command.</param>
        void Summon(IrcNickname nickname, string targetServer);

        /// <summary>
        /// Attempts to send a message to a user on the server host machine requesting their
        /// presence on IRC.
        /// </summary>
        /// <param name="nickname">The nickname of the user to summon.</param>
        /// <param name="targetServer">The name of the server that will process the command.</param>
        /// <param name="channelName">A channel name to for the summoned person to join.</param>
        void Summon(IrcNickname nickname, string targetServer, IrcChannelName channelName);

        /// <summary>
        /// Requests the servers the local server has a direct connection to.
        /// </summary>
        void TraceRoute();

        /// <summary>
        /// Requests a trace route to the specified server.
        /// </summary>
        /// <param name="target">A server name or the server the user with the specified
        /// nickname is connected to.</param>
        void TraceRoute(string target);

        /// <summary>
        /// Requests information about users or channels currently connected to a server.
        /// </summary>
        /// <param name="matchString">The string used for matching users.</param>
        void Who(string matchString);

        /// <summary>
        /// Requests information about a user currently connected to a server.
        /// </summary>
        /// <param name="nickname">The nickname of a user.</param>
        void WhoIs(IrcNickname nickname);

        /// <summary>
        /// Requests information about a user previously connected to a server.
        /// </summary>
        /// <param name="nickname">The nickname of a user to look up information about.</param>
        void WhoWas(IrcNickname nickname);

    }
}
