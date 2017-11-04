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
using System.Linq;

namespace Irc
{
    /// <summary>
    /// Parses data strings received from an Internet Relay Chat (IRC) server.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses information returned from the ADMIN command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the admin event.</returns>
        /// <remarks>In modern server implementations the ADMIN command usually returns a generic string 
        /// reponse with all replies, so this method can be used to process all replies including 
        /// RPL_ADMINME, RPL_ADMINLOC1, RPL_ADMINLOC2, and RPL_ADMINEMAIL.</remarks>
        /// <example>:moorcock.freenode.net 256 TestUser :Administrative info about moorcock.freenode.net</example>
        /// <seealso cref="Irc.Commands.ADMIN"/>
        /// <seealso cref="Irc.Numerics.RPL_ADMINME"/>
        /// <seealso cref="Irc.Numerics.RPL_ADMINLOC1"/>
        /// <seealso cref="Irc.Numerics.RPL_ADMINLOC2"/>
        /// <seealso cref="Irc.Numerics.RPL_ADMINEMAIL"/>
        /// <exception cref="FormatException"/>
        public static NoticeEventArgs ParseAdmin(IrcMessage message)
        {
            return (ParseNotice(message));
        }

        /// <summary>
        /// Parses information about the away status of a user.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The away message of a user.</returns>
        /// <example>:naos.foonetic.net 301 LocalNick OtherNick :I am away.</example>
        /// <seealso cref="Irc.Numerics.RPL_AWAY"/>
        /// <exception cref="FormatException"/>
        public static AwayEventArgs ParseAwayMessage(IrcMessage message)
        {
            try
            {
                IrcNickname nick = new IrcNickname(message.Parameters[1]);
                AwayEventArgs retVal = new AwayEventArgs(nick);
                retVal.AwayMessage = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the creation time of a channel.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The time a channel was created in local client time.</returns>
        /// <example>:naos.foonetic.net 329 TestUser #SomeChannel 1332379793</example>
        /// <seealso cref="Irc.Numerics.RPL_CREATIONTIME"/>
        /// <exception cref="FormatException"/>
        public static ChannelCreationEventArgs ParseChannelCreationTime(IrcMessage message)
        {
            try
            {
                IrcChannelName channel =  new IrcChannelName(message.Parameters[1]);
                long creationSeconds = 0;
                bool timeSuccess = long.TryParse(message.Parameters[2], out creationSeconds);
                DateTime creationDate = timeSuccess ? ParseUnixTime(creationSeconds) : DateTime.MinValue;

                ChannelCreationEventArgs retVal = new ChannelCreationEventArgs(channel, creationDate);

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses channel information from a LIST command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Information about a channel in the global channel list.</returns>
        /// <example>
        /// <para>:naos.foonetic.net 322 TestUser #minephleg 4 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #don't_click 3 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #shadowcities 1 :Shadow Cities in its waning days</para>
        /// <para>:naos.foonetic.net 322 TestUser #bismuth 2 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #crumpets 2 :</para>
        /// </example>
        /// <seealso cref="Irc.Commands.LIST"/>
        /// <seealso cref="Irc.Numerics.RPL_LIST"/>
        /// <seealso cref="Irc.Numerics.RPL_LISTSTART"/>
        /// <seealso cref="Irc.Numerics.RPL_LISTEND"/>
        /// <exception cref="FormatException"/>
        public static ChannelListEventArgs ParseChannelListItem(IrcMessage message)
        {
            try
            {
                IrcChannelName channel = new IrcChannelName(message.Parameters[1]);
                ChannelListEventArgs retVal = new ChannelListEventArgs(channel);
                retVal.NumberOfUsers = int.Parse(message.Parameters[2]);
                retVal.Topic = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a list item from a channel's user management list.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data about a management list item.</returns>
        /// <remarks>Ban, ban exception, and invitation exception lists all share an identical format and
        /// can be parsed using this method.</remarks>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 367 LocalNick #SomeChannel *!*@*.im
        /// :naos.foonetic.net 367 LocalNick #SomeChannel *!*@*.im OtherNick 1332382058
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Numerics.RPL_INVITELIST"/>
        /// <seealso cref="Irc.Numerics.RPL_BANLIST"/>
        /// <seealso cref="Irc.Numerics.RPL_INVITELIST"/>
        /// <seealso cref="Irc.Numerics.RPL_EXCEPTLIST"/>
        /// <exception cref="FormatException"/>
        public static ChannelManagementListEventArgs ParseChannelManagementList(IrcMessage message)
        {
            try
            {
                IrcChannelName channel = new IrcChannelName(message.Parameters[1]);
                ChannelManagementListEventArgs retVal = new ChannelManagementListEventArgs(channel, message.Parameters[2]);

                if (message.Parameters.Length == 4)
                {
                    retVal.SetBy = new IrcNickname(message.Parameters[3]);
                    long creationSeconds = 0;
                    bool timeSuccess = long.TryParse(message.Parameters[4], out creationSeconds);
                    retVal.TimeStamp = timeSuccess ? ParseUnixTime(creationSeconds) : DateTime.MinValue;
                }

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses channel modes for a given channel.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <param name="modesWithParms">A list of channel modes that have parameters.</param>
        /// <returns>Channel mode data.</returns>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 324 TestUser #SomeChannel + 
        /// :naos.foonetic.net 324 TestUser #SomeChannel +nt
        /// :naos.foonetic.net 324 TestUser #SomeChannel +ntl 50
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Numerics.RPL_CHANNELMODEIS"/>
        /// <exception cref="FormatException"/>
        public static ChannelModeEventArgs ParseChannelMode(IrcMessage message, string modesWithParms)
        {
            try
            {
                ChannelModeString modeString = null;
                int modeParamIndex = 3;
                foreach (char c in message.Parameters[2])
                {
                    if (c != '+' && c != '-')
                    {
                        modeString += new Mode(c, true, modesWithParms.Contains(c) ? message.Parameters[modeParamIndex++] : null);
                    }
                }

                ChannelModeEventArgs cmea = new ChannelModeEventArgs(new IrcChannelName(message.Parameters[1]), null, modeString);
                return (cmea);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a channel mode change.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <param name="modesAlwaysParameters">A list of modes that always require a parameter.</param>
        /// <param name="modesAddParameters">A list of modes that only require a parameter when adding.</param>
        /// <returns>Channel mode data.</returns>
        /// <example>
        /// <code>
        /// :Nick!User@hide-FF3294B6.isp.com MODE #SomeChannel +sm
        /// :Nick!User@hide-FF3294B6.isp.com MODE #SomeChannel +o OtherNick
        /// :Nick!User@hide-FF3294B6.isp.com MODE #SomeChannel +v OtherNick
        /// :Nick!User@hide-FF3294B6.isp.com MODE #SomeChannel -v OtherNick
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.MODE"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static ChannelModeEventArgs ParseChannelModeChange(IrcMessage message, string modesAlwaysParameters, string modesAddParameters)
        {
            string[] user = null;

            try
            {
                user = ParseUserData(message.Prefix);
            }
            catch (FormatException) { }

            ChannelModeString modeString = null;
            bool modeAdded = true;
            int modeParameterIndex = 2;
            foreach (char c in message.Parameters[1])
            {
                switch (c)
                {
                    case '+':
                        modeAdded = true;
                        break;
                    case '-':
                        modeAdded = false;
                        break;
                    default:
                        if (modeAdded)
                        {
                            string addParam = null;
                            string allparams = string.Format("{0}{1}", modesAlwaysParameters, modesAddParameters);
                            if (allparams != null)
                            {
                                addParam = allparams.Contains(c) ? message.Parameters[modeParameterIndex++] : null;
                            }
                            modeString += new Mode(c, modeAdded, addParam); 
                        }
                        else
                        {
                            string removeParam = null;
                            if (modesAlwaysParameters != null)
                            {
                                removeParam = modesAlwaysParameters.Contains(c) ? message.Parameters[modeParameterIndex++] : null;
                            }
                            modeString += new Mode(c, modeAdded, removeParam);
                        }
                        
                        break;
                }
            }

            ChannelModeEventArgs cmea = new ChannelModeEventArgs(new IrcChannelName(message.Parameters[0]),
                                                                 user != null ? new IrcNickname(user[0]) : null,
                                                                 modeString);
            cmea.UserName = user != null ? new IrcUsername(user[1]) : null;
            cmea.HostName = user != null ? user[2] : null;

            return (cmea);

        }

        /// <summary>
        /// Parses the channel owner of a channel.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The owner of a channel and the channel owned.</returns>
        /// <example>:naos.foonetic.net 325 #SomeChannel TestUser</example>
        /// <seealso cref="Irc.Numerics.RPL_UNIQOPIS"/>
        /// <exception cref="FormatException"/>
        public static ChannelCreatorEventArgs ParseChannelOwner(IrcMessage message)
        {
            try
            {
                return (new ChannelCreatorEventArgs(new IrcChannelName(message.Parameters[0]), new IrcNickname(message.Parameters[1])));
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the URL for a channel homepage.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The URL of a channel.</returns>
        /// <example>:services. 328 Gardocki #freenode :http://freenode.net/ </example>
        /// <seealso cref="Irc.Numerics.RPL_CHANNEL_URL"/>
        /// <exception cref="FormatException"/>
        public static ChannelUrlEventArgs ParseChannelUrl(IrcMessage message)
        {
            try
            {
                return (new ChannelUrlEventArgs(new IrcChannelName(message.Parameters[1]), message.Trailing));
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a message sent via the Client-to-Client Protocol (CTCP).
        /// </summary>
        /// <param name="message">The <see cref="Irc.IrcMessage"/> to parse.</param>
        /// <returns>The CTCP message, the associated action, and the user who sent it.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static CtcpEventArgs ParseCtcpMessage(IrcMessage message)
        {
            try
            {
                string[] user = ParseUserData(message.Prefix);

                IrcNickname sender = new IrcNickname(user[0]);
                IrcNameBase target = Irc.IrcChannelName.IsValid(message.Parameters[0]) ? (IrcNameBase)(new IrcChannelName(message.Parameters[0])) : (IrcNameBase)(new IrcNickname(message.Parameters[0]));
                CtcpEventArgs retVal = new CtcpEventArgs(sender, target);
                retVal.UserName = new IrcUsername(user[1]);
                retVal.HostName = user[2];

                string ctcpMessage = message.Trailing.Replace(CtcpCommands.CtcpDelimeter, string.Empty);

                if (ctcpMessage.Contains(" "))
                {
                    retVal.CtcpCommand = ctcpMessage.Substring(0, ctcpMessage.IndexOf(' '));
                    retVal.Message = ctcpMessage.Substring(ctcpMessage.IndexOf(' ') + 1);
                }
                else
                {
                    retVal.CtcpCommand = ctcpMessage;
                }
                

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the end of a name list from the NAMES command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The name of the channel for which the NAMES command was processed.</returns>
        /// <example>:naos.foonetic.net 366 TestUser #SomeChannel :End of /NAMES list.</example>
        /// <seealso cref="Irc.Commands.NAMES"/>
        /// <seealso cref="Irc.Numerics.RPL_NAMEREPLY"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFNAMES"/>
        /// <exception cref="FormatException"/>
        public static DataEventArgs ParseEndOfNamesList(IrcMessage message)
        {
            try
            {
                DataEventArgs retVal = new DataEventArgs();
                retVal.Data = message.Parameters[1];

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses an error message.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from an error event.</returns>
        /// <example>:daemonic.foonetic.net 447 TestUser :Can not change nickname while on #SomeChannel (+N)</example>
        /// <seealso cref="Irc.Numerics"/>
        /// <exception cref="FormatException"/>
        public static ErrorEventArgs ParseError(IrcMessage message)
        {
            try
            {
                ErrorEventArgs retVal = new ErrorEventArgs();
                //retVal.ErrorCode = int.Parse(message.Command);
                retVal.Target = message.Parameters[0];
                retVal.DefaultMessage = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a channel invitation from an INVITE command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from an invite event.</returns>
        /// <example>:LocalNick!User@hide-FF3294B6.isp.com INVITE OtherNick :#SomeChannel</example>
        /// <seealso cref="Irc.Commands.INVITE"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static InviteEventArgs ParseInvite(IrcMessage message)
        {
            try
            {
                string[] userData = ParseUserData(message.Prefix);

                InviteEventArgs retVal = new InviteEventArgs(new IrcChannelName(message.Trailing),
                                                             new IrcNickname(userData[0]),
                                                             new IrcNickname(message.Parameters[0]));
                retVal.UserName = new IrcUsername(userData[1]);
                retVal.HostName = userData[2];

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a notice that a user is being invited to a channel.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data about a channel invitation.</returns>
        /// <example>:anchor.foonetic.net 341 LocalNick OtherNick :#SomeChannel</example>
        /// <seealso cref="Irc.Numerics.RPL_INVITING"/>
        /// <seealso cref="Irc.Commands.INVITE"/>
        /// <exception cref="FormatException"/>
        public static NoticeEventArgs ParseInviting(IrcMessage message)
        {
            return (ParseNotice(message));
        }

        /// <summary>
        /// Parses information returned from the ISON command. 
        /// </summary>
        ///  <param name="message">An IRC message to parse.</param>
        /// <returns>A list of nicknames of users that are online.</returns>
        /// <remarks>The ISON command only returns nicknames for users that are currently online, not the
        /// status of all users queried by the client.  As a result, the array returned by this method may 
        /// have a lenghth of 0.</remarks>
        /// <example>:niven.freenode.net 303 TestUser :Nickname1 Nickname2 </example>
        /// <seealso cref="Irc.Commands.ISON"/>
        /// <seealso cref="Irc.Numerics.RPL_ISON"/>
        /// <exception cref="FormatException"/>
        public static IrcNickname[] ParseIsOn(IrcMessage message)
        {
            string[] names = message.Trailing.Split(' ');

            var nicknames = from name in names
                            select new IrcNickname(name);

            return (nicknames.ToArray());
        }

        /// <summary>
        /// Parses information from a JOIN command.
        /// </summary>
        ///  <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a JOIN event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com JOIN :#SomeChannel</example>
        /// <seealso cref="Irc.Commands.JOIN"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static JoinEventArgs ParseJoin(IrcMessage message)
        {
            string[] userData = ParseUserData(message.Prefix);

            IrcChannelName channel = new IrcChannelName(message.Trailing ?? message.Parameters[0]);
            JoinEventArgs retVal = new JoinEventArgs(channel, new IrcNickname(userData[0]));
            retVal.UserName = new IrcUsername(userData[1]);
            retVal.HostName = userData[2];

            return (retVal);
        }

        /// <summary>
        /// Parse information from a KICK command.
        /// </summary>
        ///  <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a KICK event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com KICK #SomeChannel KickedNick :afk</example>
        /// <seealso cref="Irc.Commands.KICK"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static KickEventArgs ParseKick(IrcMessage message)
        {
            try
            {
                string[] userData = ParseUserData(message.Prefix);

                KickEventArgs retVal = new KickEventArgs(new IrcChannelName(message.Parameters[0]), new IrcNickname(userData[0]), new IrcNickname(message.Parameters[1]));
                retVal.UserName = new IrcUsername(userData[1]);
                retVal.HostName = userData[2];
                retVal.Message = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from the LINKS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the LINKS event.</returns>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 364 TestUser daemonic.foonetic.net naos.foonetic.net :1 Foonetic.Net IRC Services
        /// :naos.foonetic.net 364 TestUser staticfree.foonetic.net vervet.foonetic.net :2 Staticfree IRC Services
        /// :naos.foonetic.net 364 TestUser vervet.foonetic.net naos.foonetic.net :1 Foonetic IRC at isomerica.net (vervet)
        /// :naos.foonetic.net 364 TestUser anchor.foonetic.net naos.foonetic.net :1 FooNetic Server
        /// :naos.foonetic.net 364 TestUser naos.foonetic.net naos.foonetic.net :0 FooNetic Server
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.LINKS"/>
        /// <seealso cref="Irc.Numerics.RPL_LINKS"/>
        /// <exception cref="FormatException"/>
        public static LinksEventArgs ParseLinks(IrcMessage message)
        {
            try
            {
                LinksEventArgs retVal = new LinksEventArgs();
                retVal.Mask = message.Parameters[1];
                retVal.Server = message.Parameters[2];

                retVal.HopCount = int.Parse(message.Trailing.Substring(0, message.Trailing.IndexOf(' ')));
                retVal.Description = message.Trailing.Substring(message.Trailing.IndexOf(' ') + 1);

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information returned from the LUSERS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the LUsers event.</returns>
        /// <remarks>A response to the LUSERS command is treated as a simple notice reply string.  While it 
        /// may be desirable to isolate the exact number of users in each reply numeric, due to the inconsistent
        /// formatting of where numbers appear in reply strings it is practically impossible to reliably isolate
        /// them.  See examples for details.</remarks>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 251 TestUser :There are 271 users and 925 invisible on 6 servers
        /// :naos.foonetic.net 252 TestUser 14 :operator(s) online
        /// :irc.cccp-project.net 253 TestUser 51 :unknown connection(s)
        /// :naos.foonetic.net 255 TestUser :I have 270 clients and 3 servers
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.LUSERS"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSERS"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSERCLIENT"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSEROP"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSERUNKNOWN"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSERCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_LUSERME"/>
        /// <exception cref="FormatException"/>
        public static NoticeEventArgs ParseLUsers(IrcMessage message)
        {
            try
            {
                int val = 0;
                string noticeMessage = message.Trailing;
                if (int.TryParse(message.Parameters[1], out val))
                {
                    noticeMessage = string.Concat(message.Parameters[1], " ", noticeMessage);
                }

                NoticeEventArgs retVal = new NoticeEventArgs() { Message = noticeMessage, Target = new IrcNickname(message.Parameters[0]) };

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a <see cref="Commands.MONITOR"/> command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <param name="online">True if the client is currently connected to a server, false otherwise.</param>
        /// <returns>Data from a <see cref="Commands.MONITOR"/> event.</returns>
        /// <seealso cref="Irc.Commands.MONITOR"/>
        /// <seealso cref="Irc.Numerics.RPL_MONLIST"/>
        /// <seealso cref="Irc.Numerics.RPL_MONONLINE"/>
        /// <seealso cref="Irc.Numerics.RPL_MONOOFFLINE"/>
        /// <seealso cref="Irc.Numerics.ERR_MONLISTFULL"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static WatchEventArgs[] ParseMonitor(IrcMessage message, bool online)
        {
            try
            {
                string[] targets = message.Trailing.Split(',');
                List<WatchEventArgs> args = new List<WatchEventArgs>();
                foreach (string s in targets)
                {
                    if (s.Contains('!'))
                    {
                        string[] user = ParseUserData(s);
                        args.Add(new WatchEventArgs(new IrcNickname(user[0])) { UserName = new IrcUsername(user[1]), HostName = user[2], IsOnline = online });
                    }
                    else
                    {
                        args.Add(new WatchEventArgs(new IrcNickname(s)));
                    }
                }

                return (args.ToArray());
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a username in a channel list from a NAMES reply.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A user in a channel name list.</returns>
        /// <example>:naos.foonetic.net 353 TestUser = #SomeChannel :@OtherNick</example>
        /// <seealso cref="Irc.Commands.NAMES"/>
        /// <seealso cref="Irc.Numerics.RPL_NAMEREPLY"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static NameListEventArgs ParseNameListItem(IrcMessage message)
        {
            try
            {
                List<NameListItem> items = new List<NameListItem>();
                string[] names = message.Trailing.Split(' ');
                
                for (int i = 0; i < names.Length; i++)
                {
                    items.Add(NameListItem.Parse(names[i]));
                }

                NameListEventArgs retVal = new NameListEventArgs(new IrcChannelName(message.Parameters[2]), items);
                retVal.IsPublicChannel = message.Parameters[1] == "=";
                retVal.IsSecretChannel = !retVal.IsPublicChannel;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a NICK command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a NICK event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com NICK :NewNick</example>
        /// <seealso cref="Irc.Commands.NICK"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static NickChangeEventArgs ParseNick(IrcMessage message)
        {
            string[] user = ParseUserData(message.Prefix);

            IrcNickname oldNick = new IrcNickname(user[0]);
            IrcNickname newNick = new IrcNickname(message.Trailing);
            NickChangeEventArgs retVal = new NickChangeEventArgs(oldNick, newNick);
            retVal.UserName = new IrcUsername(user[1]);
            retVal.HostName = user[2];

            return (retVal);
        }

        /// <summary>
        /// Parses information received in a NOTICE reply.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a notice event.</returns>
        /// <example>:NickServ!NickServ@services.foonetic.net NOTICE Moopbot :Welcome to Foonetic, Moopbot! 
        /// Here on Foonetic, we provide services to enable the registration of nicknames and channels! 
        /// For details, type /msg Nickserv help and /msg ChanServ help.
        /// </example>
        /// <seealso cref="Irc.Commands.NOTICE"/>
        /// <exception cref="FormatException"/>
        public static NoticeEventArgs ParseNotice(IrcMessage message)
        {
            try
            {
                NoticeEventArgs retVal = new NoticeEventArgs();
                retVal.Sender = message.Prefix;

                if (!string.IsNullOrEmpty(message.Trailing) && message.Trailing.Contains(CtcpCommands.CtcpDelimeter))
                {
                    retVal.Message = ParseCtcpReply(message.Trailing);
                    retVal.IsCtcpReply = true;
                    retVal.Sender = retVal.Sender.Substring(0, retVal.Sender.IndexOf("!"));
                }
                else
                {
                    retVal.Message = message.Trailing;
                }
                

                if (message.Parameters.Length > 0)
                {
                    if (IrcChannelName.IsValid(message.Parameters[0]))
                    {
                        retVal.Target = new IrcChannelName(message.Parameters[0]);
                    }
                    else if (IrcNickname.IsValid(message.Parameters[0]))
                    {
                        retVal.Target = new IrcNickname(message.Parameters[0]);
                    }
                }

                return (retVal);
            }
            catch (ArgumentException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a CTCP response string.
        /// </summary>
        /// <param name="ctcpReplyMessage">The CTCP message to parse.</param>
        /// <returns>Data from a CTCP message.</returns>
        public static string ParseCtcpReply(string ctcpReplyMessage)
        {
            string retVal = ctcpReplyMessage.Replace(CtcpCommands.CtcpDelimeter, string.Empty);
            if (retVal.StartsWith("PING"))
            {
                DateTime time = DateTime.MinValue;
                if (DateTime.TryParse(retVal.Substring(retVal.IndexOf(' ') + 1), out time))
                {
                    TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(time);
                    retVal = string.Format("CTCP PING REPLY - {0}ms", (int)diff.TotalMilliseconds);
                }
            }

            return (retVal);
        }

        /// <summary>
        /// Parses information from a PART command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a PART event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com PART #SomeChannel :Farewell forever</example>
        /// <seealso cref="Irc.Commands.PART"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static PartEventArgs ParsePart(IrcMessage message)
        {
            try
            {
                string[] user = ParseUserData(message.Prefix);

                //message.Parameters[0] is supposed to be the channel name, but
                //but Undernet (and possibly others) is fucking retarded and sends it
                //as the trailing message.
                string channelName = message.Parameters.Length > 0 ? message.Parameters[0] : message.Trailing;

                PartEventArgs retVal = new PartEventArgs(new IrcChannelName(channelName), new IrcNickname(user[0]));
                retVal.FarewellMessage = message.Trailing;
                retVal.UserName = new IrcUsername(user[1]);
                retVal.HostName = user[2];

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a PRIVMSG command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a PRIVMSG event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com PRIVMSG #SomeChannel :hello welcome to the land of sausage</example>
        /// <seealso cref="Irc.Commands.PRIVMSG"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static PrivateMessageEventArgs ParsePrivateMessage(IrcMessage message)
        {
            try
            {
                string[] user = ParseUserData(message.Prefix);

                IrcNickname sender = new IrcNickname(user[0]);
                IrcNameBase target = Irc.IrcChannelName.IsValid(message.Parameters[0]) ? (IrcNameBase)(new IrcChannelName(message.Parameters[0])) : (IrcNameBase)(new IrcNickname(message.Parameters[0]));
                PrivateMessageEventArgs retVal = new PrivateMessageEventArgs(sender, target);
                retVal.UserName = new IrcUsername(user[1]);
                retVal.HostName = user[2];

                if (message.Trailing != null)
                {
                    retVal.Message = message.Trailing.Trim();
                }

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a QUIT command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a QUIT event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com QUIT #SomeChannel :later tater</example>
        /// <seealso cref="Irc.Commands.QUIT"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static QuitEventArgs ParseQuit(IrcMessage message)
        {
            string[] user = ParseUserData(message.Prefix);

            QuitEventArgs retVal = new QuitEventArgs(new IrcNickname(user[0]));
            retVal.UserName = new IrcUsername(user[1]);
            retVal.HostName = user[2];
            retVal.FarewellMessage = message.Trailing;

            return (retVal);
        }

        /// <summary>
        /// Parses information about a network service.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Information about a network service.</returns>
        /// <example></example>
        /// <exception cref="FormatException"/>
        public static ServiceListEventArgs ParseService(IrcMessage message)
        {
            ServiceListEventArgs retVal = new ServiceListEventArgs(null);
            return (retVal);
        }

        /// <summary>
        /// Parses information from a SILENCE command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a SILENCE event.</returns>
        /// <example>:Moopbot!Moopbot@hide-FF3294B6.client.mchsi.com SILENCE +*!*@*.im</example>
        /// <seealso cref="Irc.Commands.SILENCE"/>
        /// <exception cref="FormatException"/>
        public static SilenceEventArgs ParseSilence(IrcMessage message)
        {
            SilenceEventArgs retVal = new SilenceEventArgs();
            for (int i = 0; i < message.Parameters.Length; i++)
            {
                retVal.Masks.Add(message.Parameters[i].Substring(1), message.Parameters[i].Contains("+"));
            }

            return (retVal);
        }

        /// <summary>
        /// Parses command options supported by the IRC server.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A collection of options and associated parameters, if applicable.</returns>
        /// <remarks>The manner in which option parameters are formatted is somewhat arbitrary, so no effort is
        /// made to parse them.  Instead, option parameter parsing is left to consumers of the data returned by
        /// this method.</remarks>
        /// <example>
        /// <para>:naos.foonetic.net 005 Moopbot CMDS=KNOCK,MAP,DCCALLOW,USERIP NAMESX SAFELIST HCN MAXCHANNELS=40 CHANLIMIT=#:40 MAXLIST=b:600,e:600,I:600 
        /// NICKLEN=30 CHANNELLEN=32 TOPICLEN=307 KICKLEN=307 AWAYLEN=307 MAXTARGETS=20 :are supported by this server
        /// </para>
        /// <para>:naos.foonetic.net 005 Moopbot WALLCHOPS WATCH=128 SILENCE=15 MODES=12 CHANTYPES=# PREFIX=(qaohv)~&amp;@%+ NETWORK=Foonetic 
        /// CHANMODES=beI,kfL,lj,psmntirRcOAQKVCuzNSMTG CASEMAPPING=ascii EXTBAN=~,cqnr ELIST=MNUCT STATUSMSG=~&amp;@%+ EXCEPTS :are supported by this server
        /// </para>
        /// <para>:naos.foonetic.net 005 Moopbot INVEX :are supported by this server</para>
        /// </example>
        /// <seealso cref="Irc.Numerics.RPL_ISUPPORT"/>
        /// <exception cref="FormatException"/>
        public static SupportedOptionsEventArgs ParseSupportedOptions(IrcMessage message)
        {
            try
            {
                SupportedOptionsEventArgs retVal = new SupportedOptionsEventArgs();
           
                for (int i = 1; i < message.Parameters.Length; i++)
                {
                    //if an option contains an = sign, it contains parameters
                    if (message.Parameters[i].Contains("="))
                    {
                        string[] pair = message.Parameters[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        retVal.Options.Add(pair[0].Trim(), pair[1].Trim());
                    }
                    else
                    {
                        retVal.Options.Add(message.Parameters[i].Trim(), null);
                    }
                }

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the topic of a channel.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A channel topic and the channel for which the topic is set.</returns>
        /// <example>:naos.foonetic.net 332 TestUser #SomeChannel :Today's topic is CRAZY BANANA FACE MAN</example>
        /// <seealso cref="Irc.Commands.TOPIC"/>
        /// <seealso cref="Irc.Numerics.RPL_TOPIC"/>
        /// <exception cref="FormatException"/>
        public static TopicEventArgs ParseTopic(IrcMessage message)
        {
            try
            {
                return (new TopicEventArgs(new IrcChannelName(message.Parameters[1])) { Topic = message.Trailing });
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the author of a channel topic.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The author of a channel topic and the time it was set.</returns>
        /// <example>:naos.foonetic.net 333 LocalNick #SomeChannel OtherNick 1321288272</example>
        /// <seealso cref="Irc.Numerics.RPL_TOPICWHOTIME"/>
        /// <exception cref="FormatException"/>
        public static TopicAuthorEventArgs ParseTopicAuthor(IrcMessage message)
        {
            try
            {
                TopicAuthorEventArgs retVal = new TopicAuthorEventArgs(new IrcChannelName(message.Parameters[1]), new IrcNickname(message.Parameters[2]));

                long seconds = 0;
                bool timeSuccess = long.TryParse(message.Parameters[3], out seconds);
                retVal.TimeSet = timeSuccess ? ParseUnixTime(seconds) : DateTime.MinValue;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a TOPIC command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a TOPIC event.</returns>
        /// <example>:Nick!User@hide-FF3294B6.isp.com TOPIC #SomeChannel :Today's topic is funk</example>
        /// <seealso cref="Irc.Commands.TOPIC"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static TopicChangeEventArgs ParseTopicChange(IrcMessage message)
        {
            try
            {
                string[] user = ParseUserData(message.Prefix);

                TopicChangeEventArgs retVal = new TopicChangeEventArgs(new IrcChannelName(message.Parameters[0]), new IrcNickname(user[0]));
                retVal.UserName = new IrcUsername(user[1]);
                retVal.HostName = user[2];
                retVal.Topic = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a unique ID used to resynchronize user state after a temporary disconnection from a server.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A unique ID.</returns>
        /// <example>:irc.pantsuland.net 042 TestUser 42CAACWND :your unique ID</example>
        /// <seealso cref="Irc.Numerics.RPL_YOURID"/>
        /// <exception cref="FormatException"/>
        public static string ParseUniqueID(IrcMessage message)
        {
            try
            {
                return (message.Parameters[1]);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Returns the current date and time parsed from the number of seconds elapsed since 
        /// January 1st, 1970 12:00:00AM
        /// </summary>
        /// <param name="seconds">The number of seconds elapsed from 1/1/1970 12:00:00AM</param>
        /// <returns>A System.DateTime converted to the local time.</returns>
        public static DateTime ParseUnixTime(long seconds)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            time = time.AddSeconds(seconds);
            return (time.ToLocalTime());
        }

        /// <summary>
        /// Parses mode information from a MODE command.
        /// </summary>
        /// <param name="message">Mode data.</param>
        /// <returns>Data from a MODE event.</returns>
        /// <example>
        /// <code>
        /// :TestUser MODE TestUser :+T
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.MODE"/>
        /// <exception cref="FormatException"/>
        public static ClientModeEventArgs ParseUserModes(IrcMessage message)
        {
            ClientModeString modeString = null;
            bool adding = false;

            string messageString = message.Trailing ?? message.Parameters[1];
            foreach (char c in messageString)
            {
                switch (c.ToString())
                {
                    case "+":
                        adding = true;
                        break;
                    case "-":
                        adding = false;
                        break;
                    default:
                        modeString += new Mode(c, adding);
                        break;
                }
            }

            ClientModeEventArgs mea = new ClientModeEventArgs(new IrcNickname(message.Parameters[0]), modeString);

            return (mea);
        }

        /// <summary>
        /// Parses received from a USERHOST command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the USERHOST event.</returns>
        /// <example>:lindbohm.freenode.net 302 TestUser :TestUser=+TestUser@173.28.202.160</example>
        /// <seealso cref="Irc.Commands.USERHOST"/>
        /// <seealso cref="Irc.Numerics.RPL_USERHOST"/>
        /// <exception cref="FormatException"/>
        public static UserHostEventArgs ParseUserHost(IrcMessage message)
        {
            try
            {
                string[] userData = message.Trailing.Split(' ');
                UserHostEventArgs retVal = new UserHostEventArgs();

                foreach (string s in userData)
                {
                    string[] user = s.Split('=');
                    retVal.Users.Add(user[0], user[1]);
                }

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses user data.
        /// </summary>
        /// <param name="line">A line of text to parse.</param>
        /// <returns>The nickname, username, and hostname of a user sending a message.</returns>
        /// <example>Nick!User@hide-FJ909FE-isp-com</example>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="System.FormatException">Thrown if line is not in the correct format</exception>
        public static string[] ParseUserData(string line)
        {
            if (string.IsNullOrEmpty(line)) { throw new ArgumentNullException(nameof(line)); }

            try
            {
                string[] userData = null;
                int nickStart = 0;
                int userStart = line.IndexOf('!') + 1;
                int hostStart = line.IndexOf('@') + 1;
                int nickLen = userStart - 1;
                int userLen = hostStart - 1 - userStart;

                string nick = line.Substring(nickStart, nickLen);
                string userName = line.Substring(userStart, userLen);
                string host = line.Substring(hostStart);

                userData = new string[] { nick, userName, host };

                return (userData);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new FormatException("User data not in the correct format.");
            }
        }

        /// <summary>
        /// Parses information from the USERS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a USERS event.</returns>
        /// <example>:lindbohm.freenode.net 393 TestUser :[username] [ttyline] [hostname]</example>
        /// <exception cref="System.FormatException">Thrown if line is not in the correct format</exception>
        public static UsersCommandEventArgs ParseUsers(IrcMessage message)
        {
            try
            {
                string[] data = message.Trailing.Split(' ');

                UsersCommandEventArgs retVal = new UsersCommandEventArgs();
                retVal.UserID = data[0];
                retVal.Terminal = data[1];
                retVal.Host = data[2];

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses version information from a VERSION command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the VERSION event.</returns>
        /// <example>:anchor.foonetic.net 351 TestUser Unreal3.2.8.1. anchor.foonetic.net :FhinXeOoZE [*=2309]</example>
        /// <seealso cref="Irc.Commands.VERSION"/>
        /// <seealso cref="Irc.Numerics.RPL_VERSION"/>
        /// <exception cref="FormatException"/>
        public static VersionEventArgs ParseVersion(IrcMessage message)
        {
            try
            {
                VersionEventArgs retVal = new VersionEventArgs();
                retVal.Version = message.Parameters[1];
                retVal.Server = message.Parameters[2];
                retVal.Comments = message.Trailing;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a WATCH command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data from a WATCH event.</returns>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 604 LocalNick OtherNick OtherUsername hide-FF3294B6.isp.com 1332382299 :is online
        /// :naos.foonetic.net 604 LocalNick OtherNick * * 0 :is offline
        /// :naos.foonetic.net 600 LocalNick OtherNick OtherUsername hide-FF3294B6.isp.com 1332382299 :logged online
        /// :naos.foonetic.net 601 LocalNick OtherNick OtherUsername hide-FF3294B6.isp.com 1332382274 :logged offline
        /// :naos.foonetic.net 602 LocalNick OtherNick OtherUsername hide-FF3294B6.isp.com 1332382274 :stopped watching
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.WATCH"/>
        /// <seealso cref="Irc.Numerics.RPL_WATCHING"/>
        /// <seealso cref="Irc.Numerics.RPL_ALTWATCHING"/>
        /// <seealso cref="Irc.Numerics.RPL_STOPWATCHING"/>
        /// <seealso cref="Irc.Numerics.RPL_USERONLINE"/>
        /// <seealso cref="Irc.Numerics.RPL_USEROFFLINE"/>
        /// <exception cref="FormatException"/>
        public static WatchEventArgs ParseWatch(IrcMessage message)
        {
            try
            {
                WatchEventArgs retVal = new WatchEventArgs(new IrcNickname(message.Parameters[1]));
                retVal.UserName = new IrcUsername(message.Parameters[2]);
                retVal.HostName = message.Parameters[3];

                long seconds = 0;
                bool timeSuccess = long.TryParse(message.Parameters[4], out seconds);
                retVal.IsOnline = timeSuccess & (seconds > 0);
                retVal.TimeStamp = timeSuccess ? ParseUnixTime(seconds) : retVal.TimeStamp;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses information from a WHO command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Data for the WHO event.</returns>
        /// <example>
        /// <code>
        /// :vervet.foonetic.net 352 TestUser * OtherUserName hide-FF3294B6.isp.com vervet.foonetic.net OtherNick H :0 anon
        /// :naos.foonetic.net 352 TestUser #SomeChannel OtherUserName hide-FF3294B6.isp.com naos.foonetic.net OtherNick H :0 anon
        /// </code>
        /// </example>
        /// <seealso cref="Irc.Commands.WHO"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOREPLY"/>
        /// <exception cref="FormatException"/>
        public static WhoEventArgs ParseWho(IrcMessage message)
        {
            try
            {
                WhoEventArgs retVal = new WhoEventArgs(new IrcNickname(message.Parameters[5]));
                retVal.ChannelName = message.Parameters[1] == "*" ? null : new IrcChannelName(message.Parameters[1]);
                retVal.HostName = message.Parameters[3];
                retVal.Server = message.Parameters[4];
                retVal.UserName = new IrcUsername(message.Parameters[2]);
                retVal.IsAway = message.Parameters[6] == "G";

                retVal.HopCount = int.Parse(message.Trailing.Substring(0, message.Trailing.IndexOf(' ')));
                retVal.RealName = message.Trailing.Substring(message.Trailing.IndexOf(' ') + 1);

                return(retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses a list of channels from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A list of channels a user is currently on.</returns>
        /// <example>:naos.foonetic.net 319 LocalNick Othernick :@#SomeChannel #SomeChannel2</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static IrcChannelName[] ParseWhoIsChannelList(IrcMessage message)
        {
            var list = from name in message.Trailing.Split(' ')
                       select new IrcChannelName(name.Substring(name.LastIndexOf("#") + 1));

            return (list.ToArray());
        }

        /// <summary>
        /// Parses host information from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Host information about a user.</returns>
        /// <example>:naos.foonetic.net 378 LocalNick OtherNick :is connecting from *@192-168-1-1.isp.com 192.168.1.1</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static string ParseWhoIsHost(IrcMessage message)
        {
            return (message.Trailing);
        }

        /// <summary>
        /// Parses idle time information from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <param name="signOnTime">A DateTime to store the date and time the user signed onto the network.</param>
        /// <returns>The number of seconds idle.</returns>
        /// <example>:naos.foonetic.net 317 LocalNick OtherNick 871 1332379136 :seconds idle, signon time</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static int ParseWhoIsIdle(IrcMessage message, out DateTime signOnTime)
        {
            try
            {
                int retVal = int.Parse(message.Parameters[2]);

                long signOnSeconds = 0;
                bool timeSuccess = long.TryParse(message.Parameters[3], out signOnSeconds);
                signOnTime = timeSuccess ? ParseUnixTime(signOnSeconds) : DateTime.MinValue;

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses server information from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>A server name.</returns>
        /// <example>:naos.foonetic.net 312 LocalNick OtherNick naos.foonetic.net :FooNetic Server</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static string ParseWhoIsServer(IrcMessage message)
        {
            return (message.Parameters[2]);
        }

        /// <summary>
        /// Parses user information from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>Information about a user.</returns>
        /// <example>:naos.foonetic.net 311 LocalNick OtherNick OtherUsername hide-FF3294B6.client.mchsi.com * : anon</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static UserEventArgs ParseWhoIsUser(IrcMessage message)
        {
            try
            {
                UserEventArgs retVal = new UserEventArgs(new IrcNickname(message.Parameters[1]))
                {
                    UserName = new IrcUsername(message.Parameters[2]),
                    HostName = message.Parameters[3],
                    RealName = message.Trailing
                };

                return (retVal);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new FormatException(Properties.Resources.ResponseFormatError, ex);
            }
        }

        /// <summary>
        /// Parses the nickname queried from a WHOIS command.
        /// </summary>
        /// <param name="message">An IRC message to parse.</param>
        /// <returns>The nickname of a user queried in a WHOIS command.</returns>
        /// <example>:naos.foonetic.net 318 LocalUser OtherNick :End of /WHOIS list.</example>
        /// <seealso cref="Irc.Commands.WHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISUSER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISHELPOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSERVER"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISOPERATOR"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANOP"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISIDLE"/>
        /// <seealso cref="Irc.Numerics.RPL_ENDOFWHOIS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISACTUALLY"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISSECURE"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISCHANNELS"/>
        /// <seealso cref="Irc.Numerics.RPL_WHOISLOGGEDIN"/>
        /// <exception cref="FormatException"/>
        public static IrcNickname ParseWhoIsNickname(IrcMessage message)
        {
            return (new IrcNickname(message.Parameters[1]));
        }
    }
}
