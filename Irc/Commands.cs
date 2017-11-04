/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/

namespace Irc
{
    /// <summary>
    /// Defines command strings used to send commands to an Internet Relay Chat (IRC) server.
    /// </summary>
    /// <remarks>See https://www.rfc-editor.org/rfc/rfc2812.txt for formally defined commands.  Some 
    /// commands are widely supported by servers but are not part of a formal specification.</remarks>
    public static class Commands
    {
        /// <summary>
        /// The command string used to get administrator information.
        /// </summary>
        public const string ADMIN = "ADMIN";

        /// <summary>
        /// The command string used to set the client user status as away.
        /// </summary>
        public const string AWAY = "AWAY";

        /// <summary>
        /// The command used for channel operators to send channel notices while bypassing
        /// the limit on the maximum number of recipients.
        /// </summary>
        public const string CNOTICE = "CNOTICE";

        /// <summary>
        /// The command used for channel operators to send channel messages while bypassing
        /// the limit on the maximum number of recipients.
        /// </summary>
        public const string CPRIVMSG = "CPRIVMSG";

        /// <summary>
        /// The command string sent by a server when an error has occurred.
        /// </summary>
        public const string ERROR = "ERROR";

        /// <summary>
        /// The help command.
        /// </summary>
        public const string HELP = "HELP";

        /// <summary>
        /// The command string used to get server information.
        /// </summary>
        public const string INFO = "INFO";

        /// <summary>
        /// The command string used to invite a user to a channel.
        /// </summary>
        /// <example>:TestUser!TestName@isp.com INVITE TestUser :#SomeChannel</example>
        public const string INVITE = "INVITE";

        /// <summary>
        /// The command used to check the online status of a user.
        /// </summary>
        public const string ISON = "ISON";

        /// <summary>
        /// The command string used to join a channel.
        /// </summary>
        /// <example>:Nick!User@hide-FF3294B6.isp.com JOIN :#SomeChannel</example>
        public const string JOIN = "JOIN";

        /// <summary>
        /// The command string used to remove a user from a channel.
        /// </summary>
        /// <example>:TestUser!TestName@isp.com KICK #SomeChannel TestUser :afk</example>
        public const string KICK = "KICK";

        /// <summary>
        /// The command used by IRC operators to forcibly kick a user off of the network.
        /// </summary>
        public const string KILL = "KILL";

        /// <summary>
        /// The command string used for sending a knock command to a channel.
        /// </summary>
        public const string KNOCK = "KNOCK";

        /// <summary>
        /// The command used to request a list of servers known to the current server.
        /// </summary>
        public const string LINKS = "LINKS";

        /// <summary>
        /// The command string used to get a list of channel names and topics.
        /// </summary>
        public const string LIST = "LIST";

        /// <summary>
        /// The command used to request information about users on the network.
        /// </summary>
        public const string LUSERS = "LUSERS";

        /// <summary>
        /// The command used to request a map of the network.
        /// </summary>
        public const string MAP = "MAP";

        /// <summary>
        /// The command string used to specify user or channel mode, and
        /// receiving notification that the user or channel mode has changed.
        /// </summary>
        /// <example>
        /// 	<para>:TestUser MODE TestUser :+T</para>
        /// 	<para>:TestUser!TestUser@isp.com MODE #SomeChannel +sm </para>
        /// 	<para>:TestUser!TestName@isp.com MODE #SomeChannel +o TestUser</para>
        /// 	<para>:TestUser!TestName@isp.com MODE #SomeChannel +v TestUser</para>
        /// 	<para>:TestUser!TestName@isp.com MODE #SomeChannel -v TestUser</para>
        /// </example>
        public const string MODE = "MODE";

        /// <summary>
        /// A command string used for adding or removing a user to a server-side friends list.
        /// </summary>
        public const string MONITOR = "MONITOR";

        /// <summary>
        /// The command string used to return the server message of the day
        /// </summary>
        public const string MOTD = "MOTD";

        /// <summary>
        /// The command string used to return a list of names in the current channel.
        /// </summary>
        public const string NAMES = "NAMES";

        /// <summary>
        /// The command string used for registering or changing a nickname,
        /// and receiving notification that a user has changed their name.
        /// </summary>
        /// <example>:TestUser!TestUser@isp.com NICK :TestUser2</example>
        public const string NICK = "NICK";

        /// <summary>
        /// The command string used to send notices to other users.
        /// </summary>
        /// <example>:NickServ!NickServ@services.foonetic.net NOTICE TestUser :Welcome to Foonetic, TestUser! 
        /// Here on Foonetic, we provide services to enable the registration of nicknames and channels! 
        /// For details, type /msg Nickserv help and /msg ChanServ help.
        /// </example>
        public const string NOTICE = "NOTICE";

        /// <summary>
        /// The command string used to authenticate as an IRC operator.
        /// </summary>
        public const string OPER = "OPER";

        /// <summary>
        /// The command string used to leave a channel.
        /// </summary>
        /// <example>:TestUser!TestUser@isp.com PART #mooptest</example>
        public const string PART = "PART";

        /// <summary>
        /// The command string used for specifying a connection password.
        /// </summary>
        public const string PASS = "PASS";

        /// <summary>
        /// The command string sent by a server to poll for an active client connection.
        /// </summary>
        /// <example>
        /// 	<para>PING :naos.foonetic.net</para>
        /// 	<para>PING :1DB8F47F</para>
        /// </example>
        public const string PING = "PING";

        /// <summary>
        /// The command string returned to a server in response to a PING command.
        /// </summary>
        public const string PONG = "PONG";

        /// <summary>
        /// The command string used to send a private message to another user.
        /// </summary>
        /// <example>:TestUser!TestName@isp.com PRIVMSG #SomeChannel :hello welcome to the land of sausage</example>
        public const string PRIVMSG = "PRIVMSG";

        /// <summary>
        /// The command string used to close the client connection.
        /// </summary>
        /// <example>:Nick!User@hide-FF3294B6.isp.com QUIT #SomeChannel :later tater</example>
        public const string QUIT = "QUIT";

        /// <summary>
        /// The command used for requesting a list of server rules.
        /// </summary>
        public const string RULES = "RULES";

        /// <summary>
        /// The command used to list services currently connected to
        /// the network and visible to the user issuing the command.
        /// </summary>
        public const string SERVLIST = "SERVLIST";

        /// <summary>
        /// The command string used to change the client's real name.
        /// </summary>
        public const string SETNAME = "SETNAME";

        /// <summary>
        /// The command string used for enabling or disabling server-side ignores.
        /// </summary>
        /// <example>:TestUser!TestUser@isp.com SILENCE +*!*@*.im</example>
        public const string SILENCE = "SILENCE";

        /// <summary>
        /// The command used to message a network service.
        /// </summary>
        public const string SQUERY = "SQUERY";

        /// <summary>
        /// The command used to request server statistics.
        /// </summary>
        public const string STATS = "STATS";

        /// <summary>
        /// The command used to summon a user onto IRC.
        /// </summary>
        public const string SUMMON = "SUMMON";

        /// <summary>
        /// The command string used to get the current server time.
        /// </summary>
        public const string TIME = "TIME";

        /// <summary>
        /// The command string used to view or change a channel topic.
        /// </summary>
        public const string TOPIC = "TOPIC";

        /// <summary>
        /// The command used to perform a traceroute on the network.
        /// </summary>
        public const string TRACE = "TRACE";

        /// <summary>
        /// The command string used to specify username, hostname, and real name
        /// when establishing a connection.
        /// </summary>
        public const string USER = "USER";

        /// <summary>
        /// The USERHOST command.
        /// </summary>
        public const string USERHOST = "USERHOST";

        /// <summary>
        /// The command used to request a list of users logged in to a server.
        /// </summary>
        public const string USERS = "USERS";

        /// <summary>
        /// The command string used to query the application version.
        /// </summary>
        public const string VERSION = "VERSION";

        /// <summary>
        /// A command string used for adding or removing a user to a server-side friends list.
        /// </summary>
        public const string WATCH = "WATCH";

        /// <summary>
        /// The command string used to request information about a user or channel on the network.
        /// </summary>
        public const string WHO = "WHO";

        /// <summary>
        /// The command string used to get information about a particular user currently
        /// connected to the network.
        /// </summary>
        public const string WHOIS = "WHOIS";

        /// <summary>
        /// The command string used to get information about a particular user no longer
        /// connected to the network.
        /// </summary>
        public const string WHOWAS = "WHOWAS";

        /// <summary>
        /// The extended WHO command.
        /// </summary>
        public const string WHOX = "WHOX";

    }
}
