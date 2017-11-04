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
    /// Defines values for commands and parameters supported by a server beyond
    /// the offical IRC RFCs.
    /// </summary>
    public static class SupportedOptions
    {
        /// <summary>
        /// Defines the maximum number of characters allowed in an away message.
        /// </summary>
        public const string AWAYLEN = "AWAYLEN";

        /// <summary>
        /// Specifies whether the server supports the +g (server-side ignore) mode.
        /// </summary>
        public const string CALLERID = "CALLERID";

        /// <summary>
        /// Defines the case mapping used by the server.
        /// </summary>
        public const string CASEMAPPING = "CASEMAPPING";

        /// <summary>
        /// Definess the maximum number of channels a client may join.
        /// </summary>
        public const string CHANLIMIT = "CHANLIMIT";

        /// <summary>
        /// Defines the list of channel modes allowed on a network.
        /// </summary>
        public const string CHANMODES = "CHANMODES";

        /// <summary>
        /// Defines the maximum number of characters allowed in a channel name.
        /// </summary>
        public const string CHANNELLEN = "CHANNELLEN";

        /// <summary>
        /// Defines the channel types allowed on a network.
        /// </summary>
        public const string CHANTYPES = "CHANTYPES";

        /// <summary>
        /// Defines the list of operator commands available to a client.
        /// </summary>
        public const string CMDS = "CMDS";

        /// <summary>
        /// Specifies whether a server supports the CNOTICE command.
        /// </summary>
        public const string CNOTICE = "CNOTICE";

        /// <summary>
        /// Specifies whether a server supports the CPRIVMSG command.
        /// </summary>
        public const string CPRIVMSG = "CPRIVMSG";

        /// <summary>
        /// Specifies whether a server supports the DCCALLOW command.
        /// </summary>
        public const string DCCALLOW = "DCCALLOW";

        /// <summary>
        /// Specifies whether the server supports extensions to the LIST command and defines
        /// the list of acceptable extensions.
        /// </summary>
        public const string ELIST = "ELIST";

        /// <summary>
        /// Specifies whether a network supports ban exceptions.
        /// </summary>
        public const string EXCEPTS = "EXCEPTS";

        /// <summary>
        /// Specifies whether a server may issue forced nickname changes.
        /// </summary>
        public const string FNC = "FNC";

        /// <summary>
        /// Defines the maximum number of characters allowed in a channel ID.
        /// </summary>
        public const string IDCHAN = "IDCHAN";

        /// <summary>
        /// Specifies whether a network supports invitation exceptions.
        /// </summary>
        public const string INVEX = "INVEX";

        /// <summary>
        /// Defines the maximum number of characters allowed in a KICK message.
        /// </summary>
        public const string KICKLEN = "KICKLEN";

        /// <summary>
        /// Specifies whether a server supports the KNOCK command.
        /// </summary>
        public const string KNOCK = "KNOCK";

        /// <summary>
        /// Specifies whether a server supports the MAP command.
        /// </summary>
        public const string MAP = "MAP";

        /// <summary>
        /// Defines the maximum number of entries in a ban, ban exception, and invitation list.
        /// </summary>
        public const string MAXLIST = "MAXLIST";

        /// <summary>
        /// </summary>
        public const string MAXNICKLEN = "MAXNICKLEN";

        /// <summary>
        /// Defines the maximum number of targets allowed for a single command.
        /// </summary>
        public const string MAXTARGETS = "MAXTARGETS";

        /// <summary>
        /// Defines the maximum number of channel modes with parameter allowed per MODE command.
        /// </summary>
        public const string MODES = "MODES";

        /// <summary>
        /// Specifies whether the server supports the MONITOR command.
        /// </summary>
        public const string MONITOR = "MONITOR";

        /// <summary>
        /// Specifies whether the server supports channel status in a NAMES list.
        /// </summary>
        public const string NAMESX = "NAMESX";

        /// <summary>
        /// Specifies a network name.
        /// </summary>
        public const string NETWORK = "NETWORK";

        /// <summary>
        /// Defines the maximum number of characters allowed in a nickname.
        /// </summary>
        public const string NICKLEN = "NICKLEN";

        /// <summary>
        /// Defines the delay required between specified commands.
        /// </summary>
        public const string PENALTY = "PENALTY";

        /// <summary>
        /// Defines the characters prefixed to a nickname that specify channel status.
        /// </summary>
        public const string PREFIX = "PREFIX";

        /// <summary>
        /// Specifies whether a server implements RFC2812.
        /// </summary>
        public const string RFC2812 = "RFC2812";

        /// <summary>
        /// Specifies whether the server sends a LIST reply in several iterations.
        /// </summary>
        public const string SAFELIST = "SAFELIST";

        /// <summary>
        /// Specifies whether a server supports the SILENCE command.
        /// </summary>
        public const string SILENCE = "SILENCE";

        /// <summary>
        /// Specifies whether a user can message all channel members with a given 
        /// channel status and defines the channel statuses a user is allowed to message.
        /// </summary>
        public const string STATUSMSG = "STATUSMSG";

        /// <summary>
        /// Defines the standard which the IRC server implementation is using.
        /// </summary>
        public const string STD = "STD";

        /// <summary>
        /// Defines the maximum number of characters allowed in a channel topic.
        /// </summary>
        public const string TOPICLEN = "TOPICLEN";

        /// <summary>
        /// Specifies whether the server supports the extended nicknames in a NAMES list.
        /// </summary>
        public const string UHNAMES = "UHNAMES";

        /// <summary>
        /// Specifies whether a server supports the USERIP command.
        /// </summary>
        public const string USERIP = "USERIP";

        /// <summary>
        /// Specifies whether a server supports virtual channels.
        /// </summary>
        public const string VCHANS = "VCHANS";

        /// <summary>
        /// Specifies whether the server supports messaging channel operators.
        /// </summary>
        public const string WALLCHOPS = "WALLCHOPS";

        /// <summary>
        /// Specifies whether channel notices go to all voiced users.
        /// </summary>
        public const string WALLVOICES = "WALLVOICES";

        /// <summary>
        /// Specifies whether a server supports the WATCH command.
        /// </summary>
        public const string WATCH = "WATCH";

        /// <summary>
        /// Specifies whether a server supports the WHOX command.
        /// </summary>
        public const string WHOX = "WHOX";
    }
}
