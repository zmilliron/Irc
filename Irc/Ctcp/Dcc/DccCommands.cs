/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
namespace Irc.Ctcp.Dcc
{
    /// <summary>
    /// Defines values for sending Direct Client Connection (DCC) commands.
    /// </summary>
    public static class DccCommands
    {
        /// <summary>
        /// The private chat command.
        /// </summary>
        public const string CHAT = "CHAT";

        /// <summary>
        /// The file transfer command.
        /// </summary>
        public const string SEND = "SEND";

        /// <summary>
        /// The command to accept a file transfer resume request.
        /// </summary>
        public const string ACCEPT = "ACCEPT";

        /// <summary>
        /// The resume file transfer command.
        /// </summary>
        public const string RESUME = "RESUME";
    }
}
