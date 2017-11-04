/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using Irc.Ctcp;
using System.Collections.Generic;

namespace Irc
{
    /// <summary>
    /// Defines the methods, properties, and events for implementing Internet Relay Chat (IRC) protocol extensions beyond RFC2812.
    /// </summary>
    public interface IIrcxProtocol : IRfc2812, ICtcp
    {
        #region Properties...

        /// <summary>
        /// Gets the maximum length of an away message allowed by the server,
        /// </summary>
        int AwayMessageLength { get; }

        /// <summary>
        /// Gets the maximum number of ban exceptions per channel allowed by a server.
        /// </summary>
        int BanExceptionListLength { get; }

        /// <summary>
        /// Gets the maximum number of bans per channel allowed by a server.
        /// </summary>
        int BanListLength { get; }

        /// <summary>
        /// Gets a value indicating if the server supports messaging IRCOps.
        /// </summary>
        bool CanMessageOperators { get; }

        /// <summary>
        /// Gets the case mapping used by the server for nickname and channel name comparisons.
        /// </summary>
        string CaseMapping { get; }

        /// <summary>
        /// Gets the maximum number of channels a client can join on a server.
        /// </summary>
        int ChannelLimit { get; }

        /// <summary>
        /// Gets the maximum channel name length allowed by a server.
        /// </summary>
        int ChannelNameLength { get; }

        /// <summary>
        /// Gets the maximum number of invite exceptions per channel allowed by a server.
        /// </summary>
        int InviteExceptionListLength { get; }

        /// <summary>
        /// Gets the maximum length of a message sent with a KICK command allowed by a server.
        /// </summary>
        int KickMessageLength { get; }

        /// <summary>
        /// Gets the maximum number of changes a server supports with a single channel MODE command.
        /// </summary>
        int MaximumChannelModeChanges { get; }

        /// <summary>
        /// Gets the maximum number of friends a client can add to a friends list.
        /// </summary>
        int MaximumFriendsListLength { get; }

        /// <summary>
        /// Gets the maximum number of ignores a client can set on the server.
        /// </summary>
        int MaximumIgnoreListLength { get; }

        /// <summary>
        /// Gets the maximum number of targets a server suppots with a single PRIVMSG command.
        /// </summary>
        int MaximumMessageTargets { get; }

        /// <summary>
        /// Gets the maximum length of a nickname allowed by a server.
        /// </summary>
        int MaximumNicknameLength { get; }

        /// <summary>
        /// Gets the number of channel modes that can be set with a single MODE command.
        /// </summary>
        int ModesPerCommand { get; }

        /// <summary>
        /// Gets the name of the network the connected server belongs to.
        /// </summary>
        string NetworkName { get; }

        /// <summary>
        /// Gets a list channel types the server supports, one for each character in the string.
        /// </summary>
        string SupportedChannelTypes { get; }

        /// <summary>
        /// Gets a value indicating if the server supports server-side ignores via the +g client mode.
        /// </summary>
        bool SupportsCallerId { get; }

        /// <summary>
        /// Gets a value indicating if the server supports the <see cref="Irc.Commands.CNOTICE"/> command.
        /// </summary>
        bool SupportsCNotice { get; }

        /// <summary>
        /// Gets a value indicating if the server supports the <see cref="Irc.Commands.CPRIVMSG"/> command.
        /// </summary>
        bool SupportsCPrivMsg { get; }

        /// <summary>
        /// Gets a value indicating if the server can force the client to change its 
        /// nickname.
        /// </summary>
        bool SupportsForcedNicknameChanges { get; }

        /// <summary>
        /// Gets a value indicating if the server supports friends lists.
        /// </summary>
        bool SupportsFriendsList { get; }

        /// <summary>
        /// Gets a value indicating if the server supports ignore lists.
        /// </summary>
        bool SupportsIgnoreList { get; }

        /// <summary>
        /// Gets a value indicating if the server supports the KNOCK command.
        /// </summary>
        bool SupportsKnock { get; }

        /// <summary>
        /// Gets a value indicating if the network supports channels local to a single server.
        /// </summary>
        bool SupportsLocalChannels { get; }

        /// <summary>
        /// Gets a value indicating if the network supports channels without modes.
        /// </summary>
        bool SupportsModelessChannels { get; }

        /// <summary>
        /// Gets a value indicating if the network supports RFC2812.
        /// </summary>
        bool SupportsRFC2812 { get; }

        /// <summary>
        /// Gets a value indicating if the network supports safe channels.
        /// </summary>
        bool SupportsSafeChannels { get; }

        /// <summary>
        /// Gets a value indicating if results from the LIST command are sent in iterations to prevent flooding the client.
        /// </summary>
        bool SupportsSafeList { get; }

        /// <summary>
        /// Gets a value indicating whether the server supports sending mass-notices to channel users 
        /// with a specified status.
        /// </summary>
        bool SupportsStatusMessaging { get; }

        /// <summary>
        /// Gets a value indicating if a server supports the USERIP command.
        /// </summary>
        bool SupportsUserIPCommand { get; }

        /// <summary>
        /// Gets a value indicating if the server supports the WHOX extension for WHOIS queries.
        /// </summary>
        bool SupportsWhoIsExtensions { get; }

        /// <summary>
        /// Gets the amount of time the client must wait before sending commands to the server.
        /// </summary>
        int TimeBetweenCommands { get; }

        /// <summary>
        /// Gets the maximum channel topic length allowed by a server.
        /// </summary>
        int TopicLength { get; }

        #endregion

        #region Events...

        /// <summary>
        /// Occurs when the URL for a channel homepage is received.
        /// </summary>
        event EventHandler<ChannelUrlEventArgs> ChannelUrlReceived;

        /// <summary>
        /// Occurs when a CNOTICE has been received.
        /// </summary>
        event EventHandler<NoticeEventArgs> CNoticeReceived;

        /// <summary>
        /// Occurs when a CPRIVMSG has been received.
        /// </summary>
        event EventHandler<PrivateMessageEventArgs> CPrivateMessageReceived;

        /// <summary>
        /// Occurs when the end of a friends list is received.
        /// </summary>
        event EventHandler EndOfFriendList;

        /// <summary>
        /// Occurs when the end of an ignore list is received.
        /// </summary>
        event EventHandler EndOfIgnoreList;

        /// <summary>
        /// Occurs when a user is added to the client's friends list.
        /// </summary>
        event EventHandler<WatchEventArgs> FriendAdded;

        /// <summary>
        /// Occurs when a user is removed from the client's friends list.
        /// </summary>
        event EventHandler<WatchEventArgs> FriendRemoved;

        /// <summary>
        /// Occurs when a user has disconnected from the IRC network.
        /// </summary>
        event EventHandler<WatchEventArgs> FriendSignedOff;

        /// <summary>
        /// Occurs when a user has connected to the IRC network.
        /// </summary>
        event EventHandler<WatchEventArgs> FriendSignedOn;

        /// <summary>
        /// Occurs when an item is added to the client's server-side ignore list.
        /// </summary>
        event EventHandler<SilenceEventArgs> IgnoreAdded;

        /// <summary>
        /// Occurs when an item is removed from the client's server-side ignore list.
        ///  </summary>
        event EventHandler<SilenceEventArgs> IgnoreRemoved;

        /// <summary>
        /// Occurs when the author of a topic is received.
        /// </summary>
        event EventHandler<TopicAuthorEventArgs> TopicAuthorReceived;

        #endregion

        /// <summary>
        /// Adds a user to a server-side friends list for monitoring connection status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to add.</param>
        /// <see cref="Irc.Commands.WATCH"/>
        void AddFriend(IrcNickname nickname);

        /// <summary>
        /// Adds a list of users to a server-side friendds list for monitoring connection status.
        /// </summary>
        /// <param name="friendList">A list of nicknames of users to add.</param>
        /// <see cref="Irc.Commands.WATCH"/>
        void AddFriend(IEnumerable<IrcNickname> friendList);

        /// <summary>
        /// Changes the client's real name.
        /// </summary>
        /// <param name="newRealName">The new real name to change to.</param>
        void ChangeRealName(string newRealName);

        /// <summary>
        /// If the client is a channel operator, sends the specified user on the specified
        /// channel a notice that bypasses the server flood protection limit. 
        /// </summary>
        /// <param name="nickname">The nickname of a user to send a notice to.</param>
        /// <param name="channelName">The channel on which the users and client are connected.</param>
        /// <param name="message">The notice message to send.</param>
        void CNotice(IrcNickname nickname, IrcChannelName channelName, string message);

        /// <summary>
        /// If the client is a channel operator, sends the specified user on the specified
        /// channel a private message that bypasses the server flood protection limit. 
        /// </summary>
        /// <param name="nickname">The nickname of a user to send a private message to.</param>
        /// <param name="channelName">The channel on which the users and client are connected.</param>
        /// <param name="message">The message to send.</param>
        void CPrivMsg(IrcNickname nickname, IrcChannelName channelName, string message);

        /// <summary>
        /// Adds or removes a user or range of users to a server-side ignore list.
        /// </summary>
        /// <param name="mask">A nickname or hostmask of ignore.</param>
        void Ignore(string mask);

        /// <summary>
        /// Adds or remvoes a list of users to a server-side ignore list.
        /// </summary>
        /// <param name="ignoreList">A list of nicknames or hostmasks to ignore.</param>
        void Ignore(IEnumerable<string> ignoreList);

        /// <summary>
        /// Sends a notice to the specified channel requesting an invitation.
        /// </summary>
        /// <param name="channelName">The name of the channel to knock.</param>
        void KnockChannel(IrcChannelName channelName);

        /// <summary>
        /// Sends a notice to the specified channel requesting an invitation with the specified message.
        /// </summary>
        /// <param name="channelName">The name of the channel to knock.</param>
        /// <param name="message">A message to send when requesting an invitation.</param>
        void KnockChannel(IrcChannelName channelName, string message);

        /// <summary>
        /// Removes a user from a server-side friends list for monitoring connection status.
        /// </summary>
        /// <param name="nickname">The nickname of the user to remove.</param>
        /// <see cref="Irc.Commands.WATCH"/>
        void RemoveFriend(IrcNickname nickname);

        /// <summary>
        /// Removes a list of users from a server-side friends list for monitoring connection status.
        /// </summary>
        /// <param name="nicknames">A list of nicknames of users to remove.</param>
        /// <see cref="Irc.Commands.WATCH"/>
        void RemoveFriend(IEnumerable<IrcNickname> nicknames);

        /// <summary>
        /// Requests information on help topics.
        /// </summary>
        void RequestHelp();

        /// <summary>
        /// Requests the server rules.
        /// </summary>
        void RequestNetworkRules();

        /// <summary>
        /// Request the IP address of the specified user.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        void RequestUserIP(IrcNickname nickname);

        /// <summary>
        /// Removes a user or range of users from a server-side ignore list.
        /// </summary>
        /// <param name="mask">A nickname or hostmask to unignore.</param>
        void Unignore(string mask);

        /// <summary>
        /// Remvoes a list of users from a server-side ignore list.
        /// </summary>
        /// <param name="ignoreList">A list of nicknames or hostmasks to unignore.</param>
        void Unignore(IEnumerable<string> ignoreList);
    }
}
