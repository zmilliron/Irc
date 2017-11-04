/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Linq;

namespace Irc
{
    /// <summary>
    /// Represents a valid channel mode on an Internet Relay Chat (IRC) network.
    /// </summary>
    public static class ChannelModes
    {
        /// <summary>
        /// A string containing all known channel modes.
        /// </summary>
        private const string AllModes = "aAbcCefFgGhiIjkKlLmMnNoOpPqQrRsStTuvVz";

        /// <summary>
        /// A string containing all modes that act on users or groups of users.
        /// </summary>
        public const string UserModes = "behIov";

        /// <summary>
        /// Only network administrators may join the channel.
        /// </summary>
        public const char AdminOnlyChannel = 'A';

        /// <summary>
        /// All conversations within the channel are anonymous.
        /// </summary>
        public const char Anonymous = 'a';

        /// <summary>
        /// The NAMES command will only return channel operators.
        /// </summary>
        public const char AuditoriumMode = 'u';

        /// <summary>
        /// Sets or removes a channel ban.
        /// </summary>
        public const char Ban = 'b';

        /// <summary>
        /// Sets or removes exceptions to channel bans.
        /// </summary>
        public const char BanException = 'e';

        /// <summary>
        /// Prevents users from changing their nickname while in a channel.
        /// </summary>
        public const char BlockNicknameChange = 'N';

        /// <summary>
        /// Gives a user operator status in a channel.
        /// </summary>
        public const char ChannelOperator = 'o';

        /// <summary>
        /// Marks a user as channel owner.
        /// </summary>
        public const char ChannelOwner = 'q';

        /// <summary>
        /// Sets or removes the flood protection filter on a channel.
        /// </summary>
        public const char FloodProtected = 'f';

        /// <summary>
        /// Gives a user permission to change the channel topic, kick and ban any user from
        /// the channel if they are not an operator, and give voice status to any user.
        /// </summary>
        public const char HalfOperator = 'h';

        /// <summary>
        /// Sets the maximum number of users that may join a channel.
        /// </summary>
        public const char HasLimit = 'l';

        /// <summary>
        /// Users must provide a password when joining a channel.
        /// </summary>
        public const char HasPassword = 'k';

        /// <summary>
        /// Prevents Client-to-Client Protocol (CTCP) messages from being sent
        /// to the channel.
        /// </summary>
        public const char IgnoreCtcp = 'C';

        /// <summary>
        /// Ignores messages sent from users outside a channel.
        /// </summary>
        public const char IgnoreExternalMessages = 'n';

        /// <summary>
        /// Prevents messages from other users from being broadcast to
        /// the channel if they contain mIRC color codes.
        /// </summary>
        public const char IgnoreMessagesWithMircColors = 'c';

        /// <summary>
        /// The channel is ignoring notices sent to it.
        /// </summary>
        public const char IgnoreNotices = 'T';

        /// <summary>
        /// Sets or removes an exception to an invitation-only channel.
        /// </summary>
        public const char InvitationException = 'I';

        /// <summary>
        /// Users may only join the channel if they are invited.
        /// </summary>
        public const char InvitationOnly = 'i';

        /// <summary>
        /// Prevents the INVITE command from being used in a channel.
        /// </summary>
        public const char InviteDisabled = 'V';

        /// <summary>
        /// Only IRC operators may join a channel.
        /// </summary>
        public const char IRCOpOnly = 'O';

        /// <summary>
        /// Prevents the KICK command from being used in a channel.
        /// </summary>
        public const char KickDisabled = 'Q';

        /// <summary>
        /// Prevents the KNOCK command from being sent to a channel.
        /// </summary>
        public const char KnockDisabled = 'K';

        /// <summary>
        /// Combines a channel with an existing channel.  If the existing channel is full,
        /// users will be redirected to the linked one.  Users in both channels will be able
        /// to communicate with one another.
        /// </summary>
        public const char LinkedChannel = 'L';

        /// <summary>
        /// Prevents users without half-operator status or greater from changing the channel topic.
        /// </summary>
        public const char LockTopic = 't';

        /// <summary>
        /// Only users with voice status or greater may speak in a channel.
        /// </summary>
        public const char Moderated = 'm';

        /// <summary>
        /// Only users with a nickname registered on the network may speak in a channel.
        /// </summary>
        public const char OnlyRegisteredUsersMaySpeak = 'M';

        /// <summary>
        /// Marks a channel as private.
        /// </summary>
        public const char Private = 'p';

        /// <summary>
        /// Enables the profanity filter on a channel.
        /// </summary>
        public const char ProfanityFilter = 'G';

        /// <summary>
        /// A flag marking a channel as having been registered with a network.  
        /// Only network servers may set this mode.
        /// </summary>
        public const char Registered = 'r';

        /// <summary>
        /// Only users with a nickname registered on a network may join a channel.
        /// </summary>
        public const char RegisteredUsersOnly = 'R';

        /// <summary>
        /// Removes a channel from the global channel listing, blocks the NAMES command from being used by 
        /// users outside the channel, and prevents the channel name from appearing in a WHOIS response 
        /// unless the user issuing the WHOIS is a member of the channel.
        /// </summary>
        public const char Secret = 's';

        /// <summary>
        /// Only users connected to the network with a Secure Sockets Layer (SSL) connection
        /// may join the channel.
        /// </summary>
        public const char SSLUsersOnly = 'z';

        /// <summary>
        /// Removes mIRC color codes from messages before they are broadcast to a channel.
        /// </summary>
        public const char StripMircColors = 'S';

        /// <summary>
        /// Throttles the number of times any user may join over a specified duration.
        /// </summary>
        public const char ThrottleJoins = 'j';

        /// <summary>
        /// Gives a user permission to speak in a moderated channel.
        /// </summary>
        public const char Voice = 'v';

        /// <summary>
        /// Determines if the specified character is a valid channel mode.
        /// </summary>
        /// <param name="mode">A mode to check</param>
        /// <returns>True if a valid channel mode, false otherwise.</returns>
        public static bool IsMode(char mode)
        {
            return (AllModes.Contains(mode));
        }

        /// <summary>
        /// Determines if the characters in the specified string are valid channel modes.
        /// </summary>
        /// <param name="modes">A string to check.</param>
        /// <returns>True if all characters are valid channel modes, false otherwise.</returns>
        public static bool AreModes(string modes)
        {
            bool retVal = false;
            foreach (char c in modes)
            {
                retVal = IsMode(c);
            }
            return (retVal);
        }
    }
}
