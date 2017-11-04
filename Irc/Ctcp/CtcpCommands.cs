/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
namespace Irc.Ctcp
{
    /// <summary>
    /// Defines values for sending Client-to-Client Protocol (CTCP) commands.
    /// </summary>
    public class CtcpCommands
    {
        /// <summary>
        /// The delimeter character (character code 0x01) that encapsulates a CTCP message.
        /// 
        ///  <example>
        ///  Example: \x01PING\x01
        /// </example>
        /// </summary>
        public const string CtcpDelimeter = "\x01";

        /// <summary>
        /// The emote or action command.
        /// </summary>
        public const string ACTION = "ACTION";

        /// <summary>
        /// The error message command.
        /// </summary>
        public const string ERRMSG = "ERRMSG";

        /// <summary>
        /// The TIME command.
        /// </summary>
        public const string TIME = "TIME";

        /// <summary>
        /// The command to request a client version.
        /// </summary>
        public const string VERSION = "VERSION";

        /// <summary>
        /// The FINGER command.
        /// </summary>
        public const string FINGER = "FINGER";

        /// <summary>
        /// The command to request remote user info.
        /// </summary>
        public const string USERINFO = "USERINFO";

        /// <summary>
        /// The command to request remote client info.
        /// </summary>
        public const string CLIENTINFO = "CLIENTINFO";

        /// <summary>
        /// The PING command.
        /// </summary>
        public const string PING = "PING";

        /// <summary>
        /// The command to request remote client download source.
        /// </summary>
        public const string SOURCE = "SOURCE";
    }
}
