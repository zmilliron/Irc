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
    /// Defines the numeric reply and error codes sent from a server in reponse to client commands.
    /// </summary>
    public enum Numerics
    {
        /// <summary>
        /// A welcome message indicating the client has successfully registered with the network.
        /// </summary>
        /// <example>
        /// :naos.foonetic.net 001 TestUser :Welcome to the Foonetic IRC Network TestUser!TestName@192-168-1-1.isp.com
        /// </example>
        RPL_WELCOME = 1,

        /// <summary>
        /// The server name, and the name and version number of the IRC server software.
        /// </summary>
        /// <example>:naos.foonetic.net 002 TestUser :Your host is naos.foonetic.net, running version Unreal3.2.7</example>
        RPL_YOURHOST = 2,

        /// <summary>
        /// The creation date of the server.
        /// </summary>
        /// <example>:naos.foonetic.net 003 TestUser :This server was created Fri Jun 20 2008 at 23:46:38 UTC</example>
        RPL_CREATED = 3,

        /// <summary>
        /// The server name, name and version number of the IRC server software, 
        /// the supported user modes, and the supported channel modes.
        /// </summary>
        /// <example>
        /// :naos.foonetic.net 004 TestUser naos.foonetic.net Unreal3.2.7 iowghraAsORTVSxNCWqBzvdHtGp lvhopsmntikrRcaqOALQbSeIKVfMCuzNTGj
        /// </example>
        RPL_MYINFO = 4,

        /// <summary>
        /// A list of commands supported beyond the default IRC protocol, 
        /// command parameter specifications, and protocol modifications.
        /// </summary>
        /// <remarks>
        /// This numeric has transformed IRC into an anti-protocol.  :(
        /// </remarks>
        /// <example>
        /// 
        /// <para>:naos.foonetic.net 005 TestUser CMDS=KNOCK,MAP,DCCALLOW,USERIP NAMESX SAFELIST HCN MAXCHANNELS=40 CHANLIMIT=#:40 MAXLIST=b:600,e:600,I:600 
        /// NICKLEN=30 CHANNELLEN=32 TOPICLEN=307 KICKLEN=307 AWAYLEN=307 MAXTARGETS=20 :are supported by this server
        /// </para>
        /// <para>:naos.foonetic.net 005 TestUser WALLCHOPS WATCH=128 SILENCE=15 MODES=12 CHANTYPES=# PREFIX=(qaohv)~&amp;@%+ NETWORK=Foonetic 
        /// CHANMODES=beI,kfL,lj,psmntirRcOAQKVCuzNSMTG CASEMAPPING=ascii EXTBAN=~,cqnr ELIST=MNUCT STATUSMSG=~&amp;@%+ EXCEPTS :are supported by this server
        /// </para>
        /// <para>:naos.foonetic.net 005 TestUser INVEX :are supported by this server</para>
        /// </example>
        RPL_ISUPPORT = 5,

        /// <summary>
        /// Reply to MAP command.
        /// 
        /// :daemonic.foonetic.net 006 TestBot :%s-*s(%ld)  %s
        /// </summary>
        /// <remarks>
        /// I have no idea how to parse the connectivity, other than the top is the "root" in this 
        /// particular context. The number in parenthesis I'm pretty sure is ping time, 
        /// and the number in brackets is obviously clients connected to that server.
        /// </remarks>
        /// <example>
        /// <code>
        /// P:Oslo-R.NO.EU.Undernet.org (0s) [2050 clients]
        /// |-Y:Oslo1.NO.EU.Undernet.org (5s) [875 clients]
        /// `-F:Gothenburg.Se.Eu.Undernet.org (0s) [4 clients]
        /// |-Q:Amsterdam-R.NL.EU.Undernet.org (57962s) [1 clients]
        /// |-7:Graz.AT.EU.Undernet.org (1s) [2535 clients]
        /// |-D:Caen.FR.EU.Undernet.org (3s) [921 clients]
        /// `-O:Flanders.BE.EU.Undernet.org (0s) [1710 clients]
        /// `-AN:Brussels.Be.Eu.Undernet.org (4s) [40 clients]
        /// </code>
        /// </example>
        RPL_MAP = 6,

        /// <summary>
        /// End of MAP command reply.
        /// 
        /// :daemonic.foonetic.net 007 TestBot :End of /MAP
        /// </summary>
        RPL_MAPEND = 7,

        /// <summary>
        /// Server notice mask.
        /// 
        /// :daemonic.foonetic.net 009 TestBot :Server notice mask ([mask])
        /// </summary>
        RPL_SNOMASK = 8,

        /// <summary>
        /// Redirects a client to a new server.
        /// 
        /// <para>:daemonic.foonetic.net 010 TestBot [servername] [port] :Please us this Server/Port instead</para>
        /// </summary>
        RPL_REDIR = 10,

        /// <summary>
        /// A cookie to use when recovering network status if disconnected.
        /// </summary>
        ///
        RPL_YOURCOOKIE = 14,

        /// <summary>
        /// An additional listing in the RPL_MAP reply.
        /// </summary>
        RPL_MAPMORE = 16,

        /// <summary>
        /// The client is still connecting and must wait before sending commands.
        /// </summary>
        RPL_CONNECTING = 20,

        /// <summary>
        /// 
        /// </summary>
        /// <example>:irc.cccp-project.net 042 TestUser 88CAAHWA2 :your unique ID</example>
        RPL_YOURID = 42,

        /// <summary>
        /// Client's nickname was forced to change due to a collision.
        /// </summary>
        RPL_SAVENICK = 43,

        /// <summary>
        /// 
        /// </summary>
        ///  /// <remarks>
        /// More information needed.
        /// </remarks>
        RPL_ATTEMPTINGJUNC = 50,

        /// <summary>
        /// Attempting to reroute the client connection to another sever.
        /// </summary>
        RPL_ATTEMPTINGREROUTE = 51,

        /// <summary>
        /// A list of commands supported beyond the default IRC protocol, 
        /// command parameter specifications, and protocol modifications.
        /// </summary>
        RPL_REMOTEISUPPORT = 105,

        /// <summary>
        /// The header of a response to a TRACE command containing network
        /// link information.
        /// </summary>
        RPL_TRACELINK = 200,

        /// <summary>
        /// Connections which have not been fully established.
        /// </summary>
        RPL_TRACECONNECTING = 201,

        /// <summary>
        /// Connections completing a server handshake protocol.
        /// </summary>
        RPL_TRACEHANDSHAKE = 202,

        /// <summary>
        /// Unknown connections.
        /// </summary>
        RPL_TRACEUNKNOWN = 203,

        /// <summary>
        /// Information describing IRC operators in the network link.
        /// </summary>
        RPL_TRACEOPERATOR = 204,

        /// <summary>
        /// Information describing a user in the network link.
        /// </summary>
        RPL_TRACEUSER = 205,

        /// <summary>
        /// Information describing a server in the network link.
        /// </summary>
        RPL_TRACESERVER = 206,

        /// <summary>
        /// Information describing a service in the network link.
        /// </summary>
        RPL_TRACESERVICE = 207,

        /// <summary>
        /// Reply to TRACE command.
        /// </summary>
        RPL_TRACENEWTYPE = 208,

        /// <summary>
        /// Information describing user connection class.
        /// </summary>
        RPL_TRACECLASS = 209,

        /// <summary>
        /// Information in response to the STATS command.
        /// </summary>
        RPL_STATS = 210,

        /// <summary>
        /// Reports statistics on a connection.
        /// </summary>
        RPL_STATSLINKINFO = 211,

        /// <summary>
        /// Reports statistics on command usage.
        /// 
        /// [command] [count] [byte count] [remote count]
        /// </summary>
        RPL_STATSCOMMANDS = 212,

        /// <summary>
        /// A server the current server may connect to or receive connections from.
        /// </summary>
        RPL_STATSCLINE = 213,

        /// <summary>
        /// Obsolete NLine numeric for Unreal IRC servers.
        /// </summary>
        RPL_STATSOLDNLINE = 214,

        /// <summary>
        /// An accepted host clients may connect from.
        /// </summary>
        RPL_STATSILINE = 215,

        /// <summary>
        /// A banned user/hostname combination.  Kill line.
        /// </summary>
        RPL_STATSKLINE = 216,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSQLINE = 217,

        /// <summary>
        /// A class line from the server's configuration file.
        /// </summary>
        RPL_STATSYLINE = 218,

        /// <summary>
        /// End of STATS report.
        /// 
        /// [letter] :End of STATS report.
        /// </summary>
        RPL_ENDOFSTATS = 219,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSBLINE = 220,

        /// <summary>
        ///The current client user modes set.
        /// </summary>
        RPL_UMODEIS = 221,

        /// <summary>
        /// 
        /// </summary>
        RPL_SQLINE_NICK = 222,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSGLINE = 223,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSTLINE = 224,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSELINE = 225,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSNLINE = 226,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSVLINE = 227,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSBANVER = 228,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSSPAMF = 229,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSEXCEPTTKL = 230,


        /// <summary>
        /// Information describing a service.
        /// </summary>
        [Obsolete()]
        RPL_SERVICEINFO = 231,

        /// <summary>
        /// A line from a server's rules.
        /// </summary>
        /// <example>
        /// <code>
        /// <para>:naos.foonetic.net 232 TestUser :- /----------------------------------------------------\</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |    ____        _                                   |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   |  _ \ _   _| | ___  ___                         |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   | |_) | | | | |/ _ \/ __|                        |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   |  _ &lt;| |_| | |  __/\__ \                        |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   |_| \_\\__,_|_|\___||___/  Foonetic IRC Network  |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- +----------------------------------------------------+</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * No flooding. Flooding of any kind is prohibited. |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   No abusive behavior.  No spamming or             |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   advertising.  Use of exploits is prohibited.     |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   No ban or kline evasion.                         |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * No WaReZ, no illegal activities.  It goes        |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   without saying that all illegal activity is      |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   strictly prohibited.                             |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * Users shall have no more than FIVE clients       |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   connected at a single time.  Session limits      |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   will result in kills if you exceed 5.  Contact   |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   an oper if you need an exception.  Abusive       |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   cloning is prohibited no matter how many         |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   clients are connected.                           |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * Well behaved bots are allowed.  Please do not    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   bring bots into channels without ensuring that   |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   the channel's policy permits doing so.           |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   _Absolutely_ no "botnets!". Please mark bots     |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   with user mode +B.                               |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * Please do not interfere with private channels.   |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * If you have any questions about these rules,     |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |   please ask in #help.                             |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- | * Be nice.  Enjoy!                                 |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- |                                                    |</para>
        /// <para>:naos.foonetic.net 232 TestUser :- \----------------------------------------------------/</para>
        /// </code>
        /// </example>
        RPL_RULES = 232,
        
        /// <summary>
        /// An identified service on the network.
        /// </summary>
        [Obsolete()]
        RPL_SERVICE = 233,


        /// <summary>
        /// A service currently running on the server. 
        /// </summary>
        [Obsolete()]
        RPL_SERVLIST = 234,

        /// <summary>
        /// Indicates the end of the services list.
        /// </summary>
        [Obsolete()]
        RPL_SERVLISTEND = 235,

        /// <summary>
        /// A more descriptive reply to the STATS command.
        /// </summary>
        RPL_STATSVERBOSE = 236,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSENGINE = 237,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSFLINE = 238,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSIAUTH = 239,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSVLINE2 = 240,

        /// <summary>
        /// Information about a server's connections.
        /// </summary>
        RPL_STATSLLINE = 241,

        /// <summary>
        /// Reports the server uptime.
        /// 
        /// :Server Up %d days %d:%02d:%02d
        /// </summary>
        RPL_STATSUPTIME = 242,

        /// <summary>
        /// Reports IRC operators online.
        /// 
        /// O [hostmask] * [name]
        /// </summary>
        RPL_STATSOLINE = 243,

        /// <summary>
        /// A server forced to be treated as a leaf or allowed to act as a hub.
        /// </summary>
        RPL_STATSHLINE = 244,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSSLINE = 245,

        /// <summary>
        /// 
        /// </summary>
        RPL_STASXLINEDALNET = 246,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSXLINE = 247,

        /// <summary>
        /// The current server uptime.
        /// </summary>
        RPL_STATSULINE = 248,

        /// <summary>
        /// 
        /// </summary>
        RPL_STATSDEBUG = 249,

        /// <summary>
        /// 
        /// </summary>
        /// <example>:card.freenode.net 250 TestUser :Highest connection count: 5533 (5530 clients) (916101 connections received)</example>
        RPL_STATSCONN = 250,

        /// <summary>
        /// The total number of users currently connected on the network, the number of users
        /// with the invisible mode set, and the number of servers on the network.
        /// </summary>
        /// <example>:naos.foonetic.net 251 TestUser :There are 271 users and 925 invisible on 6 servers</example>
        RPL_LUSERCLIENT = 251,

        /// <summary>
        /// The number of IRC operators currently online.
        /// </summary>
        /// <example>:naos.foonetic.net 252 TestUser 14 :operator(s) online</example>
        RPL_LUSEROP = 252,

        /// <summary>
        /// The number of users with unknown connections.
        /// </summary>
        /// <example>:irc.cccp-project.net 253 TestUser 51 :unknown connection(s)</example>
        RPL_LUSERUNKNOWN = 253,

        /// <summary>
        /// The total number of channels created on the network.
        /// </summary>
        /// <example>:naos.foonetic.net 254 TestUser 419 :channels formed</example>
        RPL_LUSERCHANNELS = 254,

        /// <summary>
        /// The number of clients and servers connected to the server.
        /// </summary>
        /// <example>:naos.foonetic.net 255 TestUser :I have 270 clients and 3 servers</example>
        RPL_LUSERME = 255,

        /// <summary>
        /// Server administrator information header.
        /// </summary>
        /// <example>:moorcock.freenode.net 256 TestUser :Administrative info about moorcock.freenode.net</example>
        RPL_ADMINME = 256,

        /// <summary>
        /// Server administrator address line 1.
        /// </summary>
        /// <example>:moorcock.freenode.net 257 TestUser :You're using freenode! For assistance, please '/stats p' and message someone on the list</example>
        RPL_ADMINLOC1 = 257,

        /// <summary>
        /// Server administrator address line 2.
        /// </summary>
        /// <example>:moorcock.freenode.net 258 TestUser :For further assistance, please see http://freenode.net/faq.shtml, or email</example>
        RPL_ADMINLOC2 = 258,

        /// <summary>
        /// Server administrator e-mail address.
        /// </summary>
        /// <example>:moorcock.freenode.net 259 TestUser :support@freenode.net</example>
        RPL_ADMINEMAIL = 259,

        /// <summary>
        /// File location of the trace log.
        /// </summary>
        RPL_TRACELOG = 261,

        /// <summary>
        /// Footer of a response to the TRACE command.
        /// </summary>
        RPL_TRACEEND = 262,

        /// <summary>
        /// Request for the previous command to be retried because it was
        /// not processed by the server.
        /// </summary>
        RPL_TRYAGAIN = 263,

        /// <summary>
        /// The total number of users connected to the local server.
        /// </summary>
        /// <example>:naos.foonetic.net 265 TestUser :Current Local Users: 270  Max: 650</example>
        RPL_LUSERS = 265,

        /// <summary>
        /// The total number of users connected on the network.
        /// </summary>
        /// <example>:naos.foonetic.net 266 TestUser :Current Global Users: 1196  Max: 1910</example>
        RPL_GUSERS = 266,

        /// <summary>
        /// Header for a response to a NETSTAT command.
        /// </summary>
        RPL_STARTNETSTAT = 267,

        /// <summary>
        /// Information about network statistics.
        /// </summary>
        RPL_NETSTAT = 268,

        /// <summary>
        /// The end of a NETSTAT command.
        /// </summary>
        RPL_ENDNETSTAT = 269,

        /// <summary>
        /// A list of commands the client is allowed to use.
        /// </summary>
        RPL_PRIVS = 270,

        /// <summary>
        /// Silence list entry.
        /// </summary>
        RPL_SILELIST = 271,

        /// <summary>
        /// End of silence list.
        /// </summary>
        RPL_ENDOFSILELIST = 272,

        /// <summary>
        /// An entry in the client's notify list.
        /// </summary>
        RPL_NOTIFY = 273,

        /// <summary>
        /// The end of the client's notify list.
        /// </summary>
        RPL_ENDNOTIFY = 274,

        /// <summary>
        /// Unreal ircd is STATSDLINE
        /// 
        /// Bahamut is RPL_USINGSSL
        /// </summary>
        RPL_STATSDLINE = 275,

        /// <summary>
        /// An entry in the client's DCC accept list.
        /// </summary>
        RPL_ACCEPTLIST = 281,

        /// <summary>
        /// The end of the client's DCC accept. list.
        /// </summary>
        RPL_ENDOFACCEPT = 282,

        /// <summary>
        /// Help request forward to help operators.
        /// </summary>
        RPL_HELPFWD = 294,

        /// <summary>
        /// Help request ignored.
        /// </summary>
        RPL_HELPIGN = 295,

        /// <summary>
        /// No reply.
        /// </summary>
        /// <remarks>
        /// This is a reply sent by a server when there is no reply for a given command or user action.
        /// Had there been a reply, a proper numeric or response would have been sent, however because there 
        /// was no reply this reply was sent instead.  Monitor for this numeric when you need to be absolutely 
        /// sure you do not receive any reply from the server.
        /// </remarks>
        RPL_NONE = 300,

        /// <summary>
        /// The specified user is away.
        /// </summary>
        /// <example>:naos.foonetic.net 301 TestUser Username :away</example>
        RPL_AWAY = 301,

        /// <summary>
        /// USERHOST command reply.
        /// </summary>
        /// <example>:lindbohm.freenode.net 302 TestUser :TestUser=+TestUser@192.168.1.1</example>
        RPL_USERHOST = 302,

        /// <summary>
        /// The online status of a specified list of users.
        /// </summary>
        /// <example>:niven.freenode.net 303 TestUser :TestUser </example>
        RPL_ISON = 303,

        /// <summary>
        /// A plain-text human-readable reply.
        /// </summary>
        RPL_TEXT = 304,

        /// <summary>
        /// The client status has been set to away.
        /// </summary>
        /// <example>:naos.foonetic.net 305 TestUser :You are no longer marked as being away</example>
        RPL_UNAWAY = 305,

        /// <summary>
        /// The client status is no longer set to away.
        /// </summary>
        /// <example>:naos.foonetic.net 306 TestUser :You have been marked as being away</example>
        RPL_NOWAWAY = 306,

        /// <summary>
        /// WHOIS reply: User is a registered nickname.
        /// </summary>
        RPL_WHOISREGNICK = 307,

        /// <summary>
        /// Start of RULES command.
        /// </summary>
        /// <example>:naos.foonetic.net 308 TestUser :- naos.foonetic.net Server </example>
        RPL_RULESSTART = 308,

        /// <summary>
        /// End of RULES command.
        /// </summary>
        /// <example>:naos.foonetic.net 309 TestUser :End of RULES command.</example>
        RPL_ENDOFRULES = 309,

        /// <summary>
        /// Indicates that a user specified in a WHOIS command is a help operator.
        /// </summary>
        /// <example>:irc.rizon.no 310 TestUser TestUser :is using modes +ix authflags: [none]</example>
        RPL_WHOISHELPOP = 310,

        /// <summary>
        /// The nickname, user name, hostmask, and realname of the user specified
        /// in a WHOIS command.
        /// </summary>
        /// <example>:naos.foonetic.net 311 TestUser TestUser TestUser hide-FF3294B6.isp.com * : anon</example>
        RPL_WHOISUSER = 311,

        /// <summary>
        /// The name of the server a user specified in a WHOIS command is connected to.
        /// </summary>
        /// <example>:naos.foonetic.net 312 TestUser TestUser naos.foonetic.net :FooNetic Server</example>
        RPL_WHOISSERVER = 312,

        /// <summary>
        /// Indicates a user specified in a WHOIS command is an IRC operator.
        /// </summary>
        RPL_WHOISOPERATOR = 313,

        /// <summary>
        /// The nickname, username, hostmask, and real name of a user specified in
        /// a WHOWAS command.
        /// </summary>
        RPL_WHOWASUSER = 314,

        /// <summary>
        /// Indicates the end of a WHO command.
        /// </summary>
        RPL_ENDOFWHO = 315,

        /// <summary>
        /// A list of channels on which a user is a channel operator.
        /// </summary>
        RPL_WHOISCHANOP = 316,

        /// <summary>
        /// Indicates the idle time, in UNIX time, of a user specified in a WHOIS command.
        /// </summary>
        /// <example>:naos.foonetic.net 317 TestUser TestUser 871 1332379136 :seconds idle, signon time</example>
        RPL_WHOISIDLE = 317,

        /// <summary>
        /// Indicates the end of a WHOIS command.
        /// </summary>
        /// <example>:naos.foonetic.net 318 TestUser TestUser :End of /WHOIS list.</example>
        RPL_ENDOFWHOIS = 318,

        /// <summary>
        /// A list of channels the user specified in a WHOIS command is currently on.
        /// </summary>
        /// <example>:naos.foonetic.net 319 TestUser TestUser :@#test </example>
        RPL_WHOISCHANNELS = 319,

        /// <summary>
        /// 
        /// </summary>
        RPL_WHOISSPECIAL = 320,

        /// <summary>
        /// Start of LIST command.
        /// </summary>
        [Obsolete()]
        RPL_LISTSTART = 321,

        /// <summary>
        /// A single entry in the global channel list, containing a channel name, current
        /// number of users, and the current channel topic.
        /// </summary>
        /// <example>
        /// <para>:naos.foonetic.net 322 TestUser #minephleg 4 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #don't_click 3 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #shadowcities 1 :Shadow Cities in its waning days :</para>
        /// <para>:naos.foonetic.net 322 TestUser #bismuth 2 :</para>
        /// <para>:naos.foonetic.net 322 TestUser #crumpets 2 :</para>
        /// </example>
        RPL_LIST = 322,

        /// <summary>
        /// Indicates the end of the global channel list.
        /// </summary>
        /// <example>:naos.foonetic.net 323 TestUser :End of /LIST</example>
        RPL_LISTEND = 323,

        /// <summary>
        /// The current modes set for the specified channel.
        /// </summary>
        /// <example>
        /// :naos.foonetic.net 324 TestUser #test + 
        /// :naos.foonetic.net 324 TestUser #test +nt 
        /// </example>
        RPL_CHANNELMODEIS = 324,

        /// <summary>
        /// The nickname of the creator for the specified channel.
        /// </summary>
        RPL_UNIQOPIS = 325,

        /// <summary>
        /// Provides a website URL for a channel.
        /// </summary>
        RPL_CHANNEL_URL = 328,

        /// <summary>
        /// The creation time, in UNIX time, of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 329 TestUser #test 1332379793</example>
        RPL_CREATIONTIME = 329,

        /// <summary>
        /// The username used to register with a network.
        /// </summary>
        RPL_WHOISLOGGEDIN = 330,

        /// <summary>
        /// Indicates no topic is set for the specified channel.
        /// </summary>
        RPL_NOTOPIC = 331,

        /// <summary>
        /// The current topic of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 332 TestUser #test :test</example>
        RPL_TOPIC = 332,

        /// <summary>
        /// Indicates the user who set a channel topic and the time it was set.
        /// </summary>
        /// <example>:naos.foonetic.net 333 LocalNick #SomeChannel OtherNick 1321288272</example>
        RPL_TOPICWHOTIME = 333,

        /// <summary>
        /// 
        /// </summary>
        RPL_LISTSYNTAX = 334,

        /// <summary>
        /// Indicates the user is a bot in a WHOIS reply.
        /// </summary>
        RPL_WHOISBOT = 335,

        /// <summary>
        /// The specified user is actually another name.
        /// </summary>
        /// <example>:irc.rizon.no 338 TestUser TestUser :is actually TestUser@192-168-1-1.isp.com [192.168.1.1]</example>
        RPL_WHOISACTUALLY = 338,

        /// <summary>
        /// Reply to a USERIP command.
        /// </summary>
        RPL_USERIP = 340,

        /// <summary>
        /// Indicates the specified user has been invited to the specified channel.
        /// </summary>
        /// <example>:anchor.foonetic.net 341 LocalNick OtherNick #SomeChannel</example>
        RPL_INVITING = 341,

        /// <summary>
        /// Indicates the specified user is being summoned.
        /// </summary>
        RPL_SUMMONING = 342,

        /// <summary>
        /// A single entry in the invitation exception list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 346 TestUser #test *!*@*.im Username 1332382077</example>
        RPL_INVITELIST = 346,

        /// <summary>
        /// Indicates the end of the invitation exception list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 347 TestUser #test :End of Channel Invite List</example>
        RPL_ENDOFINVITELIST = 347,

        /// <summary>
        /// A single entry in the ban exception list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 348 TestUser #test *!*@*.jp Username 1332382069</example>
        RPL_EXCEPTLIST = 348,

        /// <summary>
        /// Indicates the end of the ban exception list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 349 TestUser #test :End of Channel Exception List</example>
        RPL_ENDOFEXCEPTLIST = 349,

        /// <summary>
        /// The name and version number of the IRC server software.
        /// </summary>
        /// <example>:anchor.foonetic.net 351 TestUser Unreal3.2.8.1. anchor.foonetic.net :FhinXeOoZE [*=2309]</example>
        RPL_VERSION = 351,

        /// <summary>
        /// A reply to the WHO command.
        /// </summary>
        /// <example>
        /// <code>
        /// :vervet.foonetic.net 352 TestUser * TestUser hide-FF3294B6.isp.com vervet.foonetic.net TestUser H :0  anon
        /// :naos.foonetic.net 352 TestUser #test TestUser hide-FF3294B6.isp.com naos.foonetic.net TestUser H :0  anon
        /// </code>
        /// </example>
        RPL_WHOREPLY = 352,

        /// <summary>
        /// A single entry in the current user list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 353 TestUser = #test :@TestUser </example>
        RPL_NAMEREPLY = 353,

        /// <summary>
        /// Need description.
        /// </summary>
        RPL_RWHOREPLY = 354,

        /// <summary>
        /// A reply that the KILL command has completed.
        /// </summary>
        RPL_KILLDONE = 361,

        /// <summary>
        /// Reserved.
        /// </summary>
        RPL_CLOSING = 362,

        /// <summary>
        /// Reserved.
        /// </summary>
        RPL_CLOSEEND = 363,

        /// <summary>
        /// Information about a network link.
        /// 
        /// [mask] [server] :[hopcount] [server info]
        /// </summary>
        /// <example>
        /// <para>:naos.foonetic.net 364 TestUser daemonic.foonetic.net naos.foonetic.net :1 Foonetic.Net IRC Services</para>
        /// <para>:naos.foonetic.net 364 TestUser staticfree.foonetic.net vervet.foonetic.net :2 Staticfree IRC Services</para>
        /// <para>:naos.foonetic.net 364 TestUser vervet.foonetic.net naos.foonetic.net :1 Foonetic IRC at isomerica.net (vervet)</para>
        /// <para>:naos.foonetic.net 364 TestUser anchor.foonetic.net naos.foonetic.net :1 FooNetic Server</para>
        /// <para>:naos.foonetic.net 364 TestUser naos.foonetic.net naos.foonetic.net :0 FooNetic Server</para>
        /// </example>
        RPL_LINKS = 364,

        /// <summary>
        /// Indicates the end of the LINKS list.
        /// 
        /// [mask] :End of LINKS list
        /// </summary>
        RPL_ENDOFLINKS = 365,

        /// <summary>
        /// Indicates the end of the current user list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 366 TestUser #test :End of /NAMES list.</example>
        RPL_ENDOFNAMES = 366,

        /// <summary>
        /// A single entry in the ban list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 367 TestUser #test *!*@*.im Username 1332382058</example>
        RPL_BANLIST = 367,

        /// <summary>
        /// Indicates the end of the ban list of the specified channel.
        /// </summary>
        /// <example>:naos.foonetic.net 368 TestUser #test :End of Channel Ban List</example>
        RPL_ENDOFBANLIST = 368,

        /// <summary>
        /// Indicates the end of a WHOWAS command.
        /// </summary>
        RPL_ENDOFWHOWAS = 369,

        /// <summary>
        /// Information describing the server.
        /// </summary>
        RPL_INFO = 371,

        /// <summary>
        /// A single line in the server message of the day.
        /// </summary>
        /// <example>
        /// <code>
        /// :naos.foonetic.net 372 TestUser :- 11/7/2008 0:32
        /// :naos.foonetic.net 372 TestUser :-     __                        _   _     
        /// :naos.foonetic.net 372 TestUser :-    / _| ___   ___  _ __   ___| |_(_) ___ 
        /// :naos.foonetic.net 372 TestUser :-   | |_ / _ \ / _ \| '_ \ / _ \ __| |/ __|
        /// :naos.foonetic.net 372 TestUser :-   |  _| (_) | (_) | | | |  __/ |_| | (__ 
        /// :naos.foonetic.net 372 TestUser :-   |_|  \___/ \___/|_| |_|\___|\__|_|\___|
        /// :naos.foonetic.net 372 TestUser :-                            
        /// :naos.foonetic.net 372 TestUser :- Welcome to Foonetic IRC @ naos.foonetic.net
        /// :naos.foonetic.net 372 TestUser :-                Dallas, Texas
        /// :naos.foonetic.net 372 TestUser :-     
        /// :naos.foonetic.net 372 TestUser :-       Server Administrator:   dyfrgi
        /// :naos.foonetic.net 372 TestUser :- 
        /// :naos.foonetic.net 372 TestUser :-           http://foonetic.net/ 
        /// :naos.foonetic.net 372 TestUser :- 
        /// :naos.foonetic.net 372 TestUser :-       To get help or find IRCops:
        /// :naos.foonetic.net 372 TestUser :-                #help
        /// :naos.foonetic.net 372 TestUser :- 
        /// :naos.foonetic.net 372 TestUser :-       Random server: irc.foonetic.net
        /// :naos.foonetic.net 372 TestUser :-     
        /// :naos.foonetic.net 372 TestUser :-       To see the rules, type /quote rules
        /// :naos.foonetic.net 372 TestUser :-     
        /// :naos.foonetic.net 372 TestUser :-       Port: 80, 6667, 6668, 6669, 7000
        /// :naos.foonetic.net 372 TestUser :-       SSL Ports: 443, 6697, 7001
        /// </code>
        /// </example>
        RPL_MOTD = 372,

        /// <summary>
        /// Start of a reply to the INFO command.
        /// </summary>
        RPL_INFOSTART = 373,

        /// <summary>
        /// Indicates the end of the information describing a server.
        /// </summary>
        RPL_ENDOFINFO = 374,

        /// <summary>
        /// Indicates the start of the server message of the day.
        /// </summary>
        /// <example>:naos.foonetic.net 375 TestUser :- naos.foonetic.net Message of the Day - </example>
        RPL_MOTDSTART = 375,

        /// <summary>
        /// Indicates the end of the server message of the day.
        /// </summary>
        /// <example>:naos.foonetic.net 376 TestUser :End of /MOTD command.</example>
        RPL_ENDOFMOTD = 376,

        /// <summary>
        /// Indicates where a user is connecting from in a WHOIS reply.
        /// </summary>
        /// <example>:naos.foonetic.net 378 TestUser TestUser :is connecting from *@192-168-1-1.isp.com 192.168.1.1</example>
        RPL_WHOISHOST = 378,

        /// <summary>
        /// Indicates what modes a user has set in a WHOIS reply.
        /// </summary>
        RPL_WHOISMODES = 379,

        /// <summary>
        /// The client has successfully registered as an IRC operator.
        /// </summary>
        RPL_YOUREOPER = 381,

        /// <summary>
        /// The REHASH command has been processed.
        /// 
        /// [config file] :Rehashing
        /// </summary>
        RPL_REHASHING = 382,

        /// <summary>
        /// The service has successfully registered on the network.
        /// </summary>
        [Obsolete()]
        RPL_YOURESERVICE = 383,

        /// <summary>
        /// The local server's port number.
        /// </summary>
        RPL_MYPORTIS = 384,

        /// <summary>
        /// The client is not operator anymore.
        /// </summary>
        RPL_NOTOPERANYMORE = 385,

        /// <summary>
        /// A single entry in the channel owner list.
        /// </summary>
        RPL_QLIST = 386,

        /// <summary>
        /// End of the channel owner list.
        /// </summary>
        RPL_ENDOFQLIST = 387,

        /// <summary>
        /// A single entry in the protected user list.
        /// </summary>
        RPL_ALIST = 388,

        /// <summary>
        /// End of the protected user list.
        /// </summary>
        RPL_ENDOFALIST = 389,

        /// <summary>
        /// The local time of the server the client is connected to.
        /// </summary>
        RPL_TIME = 391,

        /// <summary>
        /// Header for USERS report.
        /// 
        /// :UserID   Terminal  Host
        /// </summary>
        RPL_USERSSTART = 392,

        /// <summary>
        /// USERS report.
        /// 
        /// :[username] [ttyline] [hostname]
        /// </summary>
        RPL_USERS = 393,

        /// <summary>
        /// End of the USERS report.
        /// 
        /// :End of users
        /// </summary>
        RPL_ENDOFUSERS = 394,

        /// <summary>
        /// :Nobody logged in
        /// </summary>
        RPL_NOUSERS = 395,

        /// <summary>
        /// The specified nickname does not exist.
        /// </summary>
        /// <example>:naos.foonetic.net 401 TestUser Username :No such nick/channel</example>
        ERR_NOSUCHNICK = 401,

        /// <summary>
        /// The specified server name does not exist.
        /// </summary>
        ERR_NOSUCHSERVER = 402,

        /// <summary>
        /// The specified channel does not exist.
        /// </summary>
        ERR_NOSUCHCHANNEL = 403,

        /// <summary>
        /// A message cannot be sent to the specified channel.  This error is returned if
        /// the client sends a message to a channel it is not currently in and
        /// the channel is blocking messages from external users, or the
        /// channel is moderated and the user does not have permission to speak.
        /// </summary>
        ERR_CANNOTSENDTOCHAN = 404,

        /// <summary>
        /// The client has already joined the maximum amount of channels allowed by the server.
        /// </summary>
        ERR_TOOMANYCHANNELS = 405,

        /// <summary>
        /// Returned after a WHOWAS command to indicate there is no information about
        /// the specified nickname.
        /// </summary>
        ERR_WASNOSUCHNICK = 406,

        /// <summary>
        /// Too many or duplicate targets specified in a PRIVMSG or NOTICE command when
        /// using the nickname@hostmask format for targets.
        /// </summary>
        ERR_TOOMANYTARGETS = 407,

        /// <summary>
        /// The specified service does not exist.
        /// </summary>
        [Obsolete()]
        ERR_NOSUCHSERVICE = 408,

        /// <summary>
        /// PONG reply is missing the reply token.
        /// </summary>
        ERR_NOORIGIN = 409,

        /// <summary>
        /// Malformed CAP command.
        /// </summary>
        ERR_INVALIDCAPCMD = 410,

        /// <summary>
        /// No target was given in the specified command.
        /// </summary>
        ERR_NORECIPIENT = 411,

        /// <summary>
        /// No text was given in the specified command.
        /// </summary>
        ERR_NOTEXTTOSEND = 412,

        /// <summary>
        /// No top-level domain was specified when using the PRIVMSG command
        /// with a server specified.
        /// </summary>
        ERR_NOTOPLEVEL = 413,

        /// <summary>
        /// A wildcard character was encountered in a top-level domain name
        /// when using the PRIVMSG command with a server specified.
        /// </summary>
        ERR_WILDTOPLEVEL = 414,

        /// <summary>
        /// The hostmask was in an invalid format.
        /// </summary>
        ERR_BADMASK = 415,

        /// <summary>
        /// Too many matches were found in the previous search and must be filtered.
        /// </summary>
        ERR_TOOMANYMATCHES = 416,

        /// <summary>
        /// The server does not support the specified command.
        /// </summary>
        ERR_UNKNOWNCOMMAND = 421,

        /// <summary>
        /// The server does not have a message of the day.
        /// </summary>
        ERR_NOMOTD = 422,

        /// <summary>
        /// The server does not have administrator information.
        /// </summary>
        ERR_NOADMININFO = 423,

        /// <summary>
        /// A failed file operation occurred while processing the specified command.
        /// </summary>
        ERR_FILEERROR = 424,

        /// <summary>
        /// No IRC operator message of the day.
        /// </summary>
        ERR_NOOPERMOTD = 425,

        /// <summary>
        /// AWAY command flood detected.
        /// </summary>
        ERR_TOOMANYAWAY = 429,

        /// <summary>
        /// No nickname was given for a command that expected a nickname.
        /// </summary>
        ERR_NONICKNAMEGIVEN = 431,

        /// <summary>
        /// The specified nickname was not in the correct format.
        /// </summary>
        ERR_ERRONEUSNICKNAME = 432,

        /// <summary>
        /// The specified nickname is already in use.
        /// </summary>
        /// <example>:naos.foonetic.net 433 TestUser Username :Nickname is already in use.</example>
        ERR_NICKNAMEINUSE = 433,

        /// <summary>
        /// No RULES file on the server.
        /// </summary>
        ERR_NORULES = 434,

        /// <summary>
        /// Unknown.
        /// </summary>
        [Obsolete()]
        ERR_SERVICECONFUSED = 435,

        /// <summary>
        /// The nickname in use by the client is in use on another server on
        /// the same network.  This error can occur after a network split.
        /// </summary>
        ERR_NICKCOLLISION = 436,

        /// <summary>
        /// The specified nickname or channel is currently unavailable.  This error
        /// can occur after a nickname or channel name is requested after a network split.
        /// </summary>
        ERR_UNAVAILRESOURCE = 437,

        /// <summary>
        /// The client has attempted to change its nickname too quickly.
        /// </summary>
        ERR_NCHANGETOOFAST = 438,

        /// <summary>
        /// The client has attempted to send messages to different targets too quickly.
        /// </summary>
        /// <example>:irc.cccp-project.net 439 * :Please wait while we process your connection.</example>
        ERR_TARGETTOOFAST = 439,

        /// <summary>
        /// Network services are unavailable.
        /// </summary>
        ERR_SERVICESDOWN = 440,

        /// <summary>
        /// The specified user is not currently in the specified channel.
        /// </summary>
        ERR_USERNOTINCHANNEL = 441,

        /// <summary>
        /// The client has issued a channel command but is not currently 
        /// in the specified channel.
        /// </summary>
        ERR_NOTONCHANNEL = 442,

        /// <summary>
        /// The channel invitation failed because the specified user is already
        /// in the channel.
        /// </summary>
        /// <example>:naos.foonetic.net 443 TestUser TestUser #test :is already on channel</example>
        ERR_USERONCHANNEL = 443,

        /// <summary>
        /// The SUMMON command failed because the specified user is not currently
        /// logged in.
        /// </summary>
        ERR_NOLOGIN = 444,

        /// <summary>
        /// The SUMMON command is not supported by the server.
        /// </summary>
        ERR_SUMMONDISABLED = 445,

        /// <summary>
        /// The USERS command is not supported by the server.
        /// </summary>
        ERR_USERSDISABLED = 446,

        /// <summary>
        /// The client is unable to change its nickname because it is currently in
        /// a channel that has blocked nickname changes.
        /// </summary>
        /// <example>:daemonic.foonetic.net 447 TestUser :Can not change nickname while on #SomeChannel (+N)</example>
        ERR_NICKCHANGENOTALLOWED = 447,

        /// <summary>
        /// The client is not currently registered with the server.
        /// </summary>
        ERR_NOTREGISTERED = 451,

        /// <summary>
        /// The username specified in an IDENTD request contained invalid characters and
        /// has been changed.
        /// </summary>
        ERR_HOSTILENAME = 455,

        /// <summary>
        /// The client's DCC accept list is full.
        /// </summary>
        ERR_ACCEPTFULL = 456,

        /// <summary>
        /// The specified user already exists on the client's DCC accept list.
        /// </summary>
        ERR_ACCEPTEXIST = 457,

        /// <summary>
        /// The specified user does not exist on the client's DCC accept list.
        /// </summary>
        ERR_ACCEPTNOT = 458,

        /// <summary>
        /// The client cannot join the channel because it is +H.
        /// </summary>
        ERR_NOHIDING = 459,

        /// <summary>
        /// Halfops cannot set the specified mode.
        /// </summary>
        ERR_NOTFORHALFOPS = 460,

        /// <summary>
        /// The specified command was missing parameters.
        /// </summary>
        /// <example>:lindbohm.freenode.net 461 TestUser USERHOST :Not enough parameters</example>
        ERR_NEEDMOREPARAMS = 461,

        /// <summary>
        /// The client has already registered with the network.
        /// </summary>
        ERR_ALREADYREGISTRED = 462,

        /// <summary>
        /// The client does not have permission to register with the server.
        /// </summary>
        ERR_NOPERMFORHOST = 463,

        /// <summary>
        /// Invalid password specified when registering a connection.
        /// </summary>
        ERR_PASSWDMISMATCH = 464,

        /// <summary>
        /// The client has been banned from the server.
        /// </summary>
        ERR_YOUREBANNEDCREEP = 465,

        /// <summary>
        /// The client will soon be banned from the server.
        /// </summary>
        ERR_YOUWILLBEBANNED = 466,

        /// <summary>
        /// The channel password has already been set.
        /// </summary>
        ERR_KEYSET = 467,

        /// <summary>
        /// The specified channel mode can only be set by a server.
        /// </summary>
        ERR_ONLYSERVERSCANCHANGE = 468,

        /// <summary>
        /// Channel link has already been set.
        /// </summary>
        ERR_LINKSET = 469,

        /// <summary>
        /// The specified channel is full so the client is being redirected to
        /// a linked channel.
        /// </summary>
        ERR_LINKCHANNEL = 470,

        /// <summary>
        /// Unable to join the specified channel because it is full.
        /// </summary>
        ERR_CHANNELISFULL = 471,

        /// <summary>
        /// The specified channel mode is not supported.
        /// </summary>
        ERR_UNKNOWNMODE = 472,

        /// <summary>
        /// Unable to join the specified channel because it is invitation-only.
        /// </summary>
        ERR_INVITEONLYCHAN = 473,

        /// <summary>
        /// Unable to join the specified channel because the client is banned.
        /// </summary>
        ERR_BANNEDFROMCHAN = 474,

        /// <summary>
        /// Unable to join the specified channel because the specified password is incorrect.
        /// </summary>
        ERR_BADCHANNELKEY = 475,

        /// <summary>
        /// The channel name specified is not in the correct format.
        /// </summary>
        ERR_BADCHANMASK = 476,

        /// <summary>
        /// The specified channel does not support modes, or
        /// unable to join the specified channel because the client's nickname
        /// has not been registered with the server.
        /// </summary>
        ERR_NOCHANMODES = 477,

        /// <summary>
        /// Unable to add the specified ban to the channel because the ban list is full.
        /// </summary>
        ERR_BANLISTFULL = 478,

        /// <summary>
        /// Invalid channel link set.
        /// </summary>
        ERR_LINKFAIL = 479,

        /// <summary>
        /// Unable to send the KNOCK command to the specified channel because the 
        /// channel is blocking it.
        /// </summary>
        ERR_CANNOTKNOCK = 480,

        /// <summary>
        /// Command failed because IRC operator status required.
        /// </summary>
        ERR_NOPRIVILEGES = 481,

        /// <summary>
        /// Command failed because channel operator status is required.
        /// </summary>
        ERR_CHANOPRIVSNEEDED = 482,

        /// <summary>
        /// KILL command not supported.
        /// </summary>
        ERR_CANTKILLSERVER = 483,

        /// <summary>
        /// The client connection has been set to a restricted mode by the server.
        /// </summary>
        ERR_RESTRICTED = 484,

        /// <summary>
        /// Channel creator status required to set the specified mode on the specified channel.
        /// </summary>
        ERR_UNIQOPPRIVSNEEDED = 485,

        /// <summary>
        /// Registered nickname required to send a private message to the specified user.
        /// </summary>
        ERR_NONONREG = 486,

        /// <summary>
        /// The specified command is for servers only.
        /// </summary>
        ERR_NOTFORUSERS = 487,

        /// <summary>
        /// 
        /// </summary>
        ERR_HTMDISABLED = 488,

        /// <summary>
        /// Unable to join channel because an SSL connection is required.
        /// </summary>
        ERR_SECUREONLYCHANNEL = 489,

        /// <summary>
        /// Profanity filter set.
        /// </summary>
        ERR_NOSWEAR = 490,

        /// <summary>
        /// The client is not recognized as a valid IRC operator.
        /// </summary>
        ERR_NOOPERHOST = 491,

        /// <summary>
        /// The specified user does not accept CTCPs.
        /// </summary>
        ERR_NOCTCP = 492,

        /// <summary>
        /// Unable to message the specified user because the client does 
        /// not share a channel with them.
        /// </summary>
        ERR_NOSHAREDCHAN = 493,

        /// <summary>
        /// Unable to message the specified user because of the specified mode.
        /// </summary>
        ERR_OWNMODE = 494,

        /// <summary>
        /// Channel owner status required.
        /// </summary>
        ERR_CHANOWNPRIVNEEDED = 499,

        /// <summary>
        /// Too many JOIN commands, flood protection enabled.  Please wait.
        /// </summary>
        ERR_TOOMANYJOINS = 500,

        /// <summary>
        /// The specified user mode is not supported.
        /// </summary>
        ERR_UMODEUNKNOWNFLAG = 501,

        /// <summary>
        /// Unable to change the mode of another user.
        /// </summary>
        ERR_USERSDONTMATCH = 502,

        /// <summary>
        /// Undeliverable message.
        /// </summary>
        ERR_UNDELIVERABLEMSG = 503,

        /// <summary>
        /// The specified user is not on the server.
        /// </summary>
        ERR_USERNOTONSERV = 504,

        /// <summary>
        /// Unable to add another entry to the ignore list because the client's
        /// ignore list is full.
        /// </summary>
        ERR_SILELISTFULL = 511,

        /// <summary>
        /// Unable to add another friend to the friends list because the client's
        /// friends list is full.
        /// </summary>
        ERR_TOOMANYWATCH = 512,

        /// <summary>
        /// Malformed PONG response.
        /// </summary>
        ERR_NEEDPONG = 513,

        /// <summary>
        /// Unable to add another user to the DCCALLOW list because it is full.
        /// </summary>
        ERR_TOOMANYDCC = 514,

        /// <summary>
        /// Unknown.
        /// </summary>
        ERR_DISABLED = 517,

        /// <summary>
        /// Channel invitations are disabled.
        /// </summary>
        ERR_INVITEDISABLED = 518,

        /// <summary>
        /// Unable to join the channel because it is administrator only.
        /// </summary>
        ERR_ADMINONLYCHANNEL = 519,

        /// <summary>
        /// Unable to join the channel because it is for IRCops only.
        /// </summary>
        ERR_OPERONLYCHANNEL = 520,

        /// <summary>
        /// Malformed LIST command.
        /// </summary>
        ERR_LISTSYNTAX = 521,

        /// <summary>
        /// Malformed WHO command.
        /// </summary>
        ERR_WHOSYNTAX = 522,

        /// <summary>
        /// WHO reply limit exceeded.  Please narrow search reasults.
        /// </summary>
        ERR_WHOLIMEXCEED = 523,

        /// <summary>
        /// Unable to join secret or private channel as an operator.  The
        /// client should send an invitation to itself first.
        /// </summary>
        ERR_OPERSPVERIFY = 524,

        /// <summary>
        /// A watched user has returned to away.
        /// </summary>
        RPL_REAWAY = 597,

        /// <summary>
        /// A watched user is now away.
        /// </summary>
        RPL_GONEAWAY = 598,

        /// <summary>
        /// A watched user is no longer away.
        /// </summary>
        RPL_NOTAWAY = 599,

        /// <summary>
        /// Indicates the specified user has connected to the network.
        /// </summary>
        /// <example>:naos.foonetic.net 600 TestUser Username Realname hide-FF3294B6.isp.com 1332382299 :logged online</example>
        RPL_USERONLINE = 600,

        /// <summary>
        /// Indicates the specified user has quit the network.
        /// </summary>
        /// <example>:naos.foonetic.net 601 TestUser Username Realname hide-FF3294B6.isp.com 1332382274 :logged offline</example>
        RPL_USEROFFLINE = 601,

        /// <summary>
        /// Indicates the specified nickname has been removed from the client's friends list.
        /// </summary>
        /// <example>:naos.foonetic.net 602 TestUser TestUser TestUser hide-FF3294B6.isp.com 1332379136 :stopped watching</example>
        RPL_STOPWATCHING = 602,

        /// <summary>
        /// The number of users on the client's watch list and the number of watch lists
        /// the client is on.
        /// </summary>
        RPL_WATCHSTAT = 603,

        /// <summary>
        /// Indicates the specified nickname has been added to the client's friends list.
        /// </summary>
        /// <example>:naos.foonetic.net 604 TestUser TestUser TestUser hide-FF3294B6.isp.com 1332379136 :is online</example>
        RPL_WATCHING = 604,

        /// <summary>
        /// Indicates the specified nickname has been added to the client's friends list. 
        /// Some servers use this numeric instead of RPL_WATCHING.
        /// </summary>
        RPL_ALTWATCHING = 605,

        /// <summary>
        /// A single entry in the WATCH list.
        /// </summary>
        RPL_WATCHLIST = 606,

        /// <summary>
        /// End of the WATCH list.
        /// </summary>
        RPL_ENDOFWATCHLIST = 607,

        /// <summary>
        /// The client's WATCH list has been cleared.
        /// </summary>
        RPL_CLEARWATCH = 608,

        /// <summary>
        /// A user added to the client's WATCH list is currently away.
        /// </summary>
        RPL_NOWISAWAY = 609,

        /// <summary>
        /// An alternative to RPL_MAPMORE.
        /// </summary>
        RPL_MAPMORE2 = 610,

        /// <summary>
        /// User has been added to the client's DCCALLOW list.
        /// </summary>
        RPL_DCCSTATUS = 617,

        /// <summary>
        /// A single entry in the client's DCCALLOW list.
        /// </summary>
        RPL_DCCLIST = 618,

        /// <summary>
        /// End of the DCCALLOW list.
        /// </summary>
        RPL_ENDOFDCCLIST = 619,

        /// <summary>
        /// Information about an entry in the client's DCCALLOW list.
        /// </summary>
        RPL_DCCINFO = 620,

        /// <summary>
        /// Command has been flagged as potential spam.
        /// </summary>
        RPL_SPAMCMDFWD = 659,

        /// <summary>
        /// STARTTLS successful.
        /// </summary>
        RPL_STARTTLS = 670,

        /// <summary>
        /// Indicates if the user on a SSL connection.
        /// </summary>
        /// <example>:vervet.foonetic.net 671 TestUser TestUser :is using a Secure Connection</example>
        RPL_WHOISSECURE = 671,

        /// <summary>
        /// Error with the STARTTLS command.
        /// </summary>
        ERR_STARTTLS = 691,

        /// <summary>
        /// A module loaded by the server.
        /// </summary>
        RPL_MODLIST = 702,

        /// <summary>
        /// The end of the list of modules loaded by the server.
        /// </summary>
        RPL_ENDOFMODLIST = 703,

        /// <summary>
        /// The header of a server's HELP file.
        /// </summary>
        RPL_HELPSTART = 704,

        /// <summary>
        /// A line in the server's help file.
        /// </summary>
        RPL_HELPTXT = 705,

        /// <summary>
        /// Marks the end of a server's help file.
        /// </summary>
        RPL_ENDOFHELP = 706,

        /// <summary>
        /// The previous message was dropped because the client has messaged too many users too quickly.
        /// </summary>
        ERR_TARGCHANGE = 707,

        /// <summary>
        /// 
        /// </summary>
        RPL_ETRACEFULL = 708,

        /// <summary>
        /// 
        /// </summary>
        RPL_ETRACE = 709,

        /// <summary>
        /// 
        /// </summary>
        RPL_KNOCK = 710,

        /// <summary>
        /// Indicates that the KNOCK command was sent.
        /// </summary>
        RPL_KNOCKDLVR = 711,

        /// <summary>
        /// The client has sent too many KNOCK commands and has been temporarily throttled.
        /// </summary>
        ERR_TOOMANYKNOCK = 712,

        /// <summary>
        /// Unable to join the specified channel.
        /// </summary>
        ERR_CHANOPEN = 713,

        /// <summary>
        /// Cannot use KNOCK command on a channel the client is already on.
        /// </summary>
        ERR_KNOCKONCHAN = 714,

        /// <summary>
        /// The KNOCK command has been disabled on the specified channel.
        /// </summary>
        ERR_KNOCKDISABLED = 715,

        /// <summary>
        /// The message could not be sent because the specified user is ignoring private messages messages.
        /// </summary>
        ERR_TARGUMODEG = 716,


        /// <summary>
        /// Indicates that a user has been notify that the client has messaged them.
        /// </summary>
        RPL_TARGNOTIFY = 717,

        /// <summary>
        /// 
        /// </summary>
        RPL_UMODEMSG = 718,

        /// <summary>
        /// The header of the IRC operator message of the day.
        /// </summary>
        RPL_OMOTDSTART = 720,

        /// <summary>
        /// A line in the IRC operator message of the day.
        /// </summary>
        RPL_OMOTD = 721,

        /// <summary>
        /// Marks the end of the IRC operator message of the day.
        /// </summary>
        RPL_ENDOFOMOTD = 722,

        /// <summary>
        /// The client does not have permission to perform the specified command.
        /// </summary>
        ERR_NOPRIVS = 723,

        /// <summary>
        /// Debug reply
        /// </summary>
        RPL_TESTMASK = 724,

        /// <summary>
        /// Debug reply
        /// </summary>
        RPL_TESTLINE = 725,

        /// <summary>
        /// Debug reply
        /// </summary>
        RPL_NOTESTLINE = 726,

        /// <summary>
        /// Debug reply
        /// </summary>
        RPL_TESTMASKGECOS = 727,

        /// <summary>
        /// The specified monitored user is online.
        /// </summary>
        RPL_MONONLINE = 730,

        /// <summary>
        /// The specified monitored user is offline.
        /// </summary>
        RPL_MONOOFFLINE = 731,

        /// <summary>
        /// An entry in the client's MONITOR list.
        /// </summary>
        RPL_MONLIST = 732,

        /// <summary>
        /// Marks the end of the client's MONITOR list.
        /// </summary>
        RPL_ENDOFMONLIST = 733,

        /// <summary>
        /// The client's MONITOR list is full.
        /// </summary>
        ERR_MONLISTFULL = 734,

        /// <summary>
        /// 
        /// </summary>
        RPL_RSACHALLENGE2 = 740,

        /// <summary>
        /// 
        /// </summary>
        RPL_ENDOFRSACHALLENGE2 = 741,

        /// <summary>
        /// Unable to perform command.
        /// </summary>
        ERR_CANNOTDOCOMMAND = 972,

        /// <summary>
        /// Unable to block the KNOCK command because the specified channel has not
        /// been set to invitation-only.
        /// </summary>
        ERR_MODEIREQUIRED = 974,

        /// <summary>
        /// Internal server error.  Client should never see this.
        /// </summary>
        ERR_NUMERICERR = 999

    }
}
