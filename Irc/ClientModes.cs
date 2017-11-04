using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Defines values that represent a list of valid client modes on an 
    /// Internet Relay Chat (IRC) network.
    /// </summary>
    public static class ClientModes
    {
        /// <summary>
        /// The user is a service administrator.
        /// </summary>
        public const string ServiceAdministrator = "a";

        /// <summary>
        /// The user is the administrator of the local server.
        /// </summary>
        public const string ServerAdministrator = "A";

        /// <summary>
        /// The user is an administrator on the network.
        /// </summary>
        public const string NetworkAdministrator = "N";

        /// <summary>
        /// The user is a co-administrator of the local server.
        /// </summary>
        public const string CoAdministrator = "C";

        /// <summary>
        /// The user is invisible.
        /// </summary>
        public const string Invisible = "i";

        /// <summary>
        /// The user can receive WALLOPS messages.
        /// </summary>
        public const string ReceiveWallops = "w";

        /// <summary>
        /// The user has a registered nickname on the network.
        /// </summary>
        public const string Registered = "r";

        /// <summary>
        /// The user is an operator on the entire network.
        /// </summary>
        public const string GlobalOperator = "o";

        /// <summary>
        /// The user is an operator on the local server.
        /// </summary>
        public const string LocalOperator = "O";

        /// <summary>
        /// The user is accepting NOTICE messages from the server.
        /// </summary>
        public const string ReceiveServerNotices = "s";

        /// <summary>
        /// The user's IP is hidden from other users.
        /// </summary>
        public const string HiddenIP = "x";

        /// <summary>
        /// The user is blocking all notices, private messages, and channel messages.
        /// </summary>
        public const string Deaf = "d";

        /// <summary>
        /// The user is ignoring all private messages except those allowed with an 
        /// ACCEPT command.
        /// </summary>
        public const string CallerID = "g";

        /// <summary>
        /// The user is a help operator on the local server.
        /// </summary>
        public const string HelpOperator = "h";

        /// <summary>
        /// The user has disabled receiving channels in a WHOIS command.
        /// </summary>
        public const string IgnoreChannelsInWho = "p";

        /// <summary>
        /// Only users defined in a ULine file can kick the client user.
        /// </summary>
        public const string OnlyULinesKick = "q";

        /// <summary>
        /// The user is using a modified host.
        /// </summary>
        public const string UsingVHOST = "t";

        /// <summary>
        /// The user can receive notices about potentially infected files sent in a 
        /// DCC request.
        /// </summary>
        public const string ReceiveInfectedDCCNotices = "v";

        /// <summary>
        /// The user is connected using a Secure Sockets Layer (SSL) connection.
        /// </summary>
        public const string SSLConnection = "z";

        /// <summary>
        /// The user is registered as a bot.
        /// </summary>
        public const string IsBot = "B";

        /// <summary>
        /// Enables or disables the profanity filter.
        /// </summary>
        public const string ProfanityFilter = "G";

        /// <summary>
        /// The user is hiding IRC operator status from non-operators.
        /// </summary>
        public const string HideIRCOPStatus = "H";

        /// <summary>
        /// The user is ignoring messages from all users that do not have a nickname 
        /// registered on the network.
        /// </summary>
        public const string OnlyReceiveFromRegisteredUsers = "R";

        /// <summary>
        /// The user is blocking all Client-to-Client Protocol messages.
        /// </summary>
        public const string BlockCTCP = "T";

        /// <summary>
        /// The user is connected using WebTV.
        /// </summary>
        public const string WebTVUser = "V";

        /// <summary>
        /// The user receives a message when another use has performed a WHOIS command 
        /// on them.
        /// </summary>
        public const string SpyOnWHOIS = "W";

        /// <summary>
        /// Determines if the specified character is a valid user mode.
        /// </summary>
        /// <param name="mode">A mode to check.</param>
        /// <returns>True if the character is a valid user mode, false otherwise.</returns>
        public static bool IsMode(char mode)
        {
            return (IsMode(mode.ToString()));
        }

        /// <summary>
        /// Determines if the characters in the specified string are valid user modes.
        /// </summary>
        /// <param name="mode">A string to check.</param>
        /// <returns>True if all characters are a valid user mode, false otherwise.</returns>
        public static bool IsMode(string mode)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(mode, "[aANCiwroOsxdghpqtvzBGHRTVW]"));
        }
    }
}
