/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System.Text.RegularExpressions;
using System;
using System.Text;

namespace Irc
{
    /// <summary>
    /// Defines the basic component parts of a message received from an Internet Relay Chat (IRC) server.
    /// </summary>
    public struct IrcMessage
    {
        /// <summary>
        /// A regular expression for removing illegal characters from a line of text.
        /// </summary>
        private const string ILLEGALCHARACTERSREGEXSTRING = @"[\r\n\x00]+";

        private const int MAXPARAMSPERLINE = 14;

        /// <summary>
        /// 
        /// </summary>
        private static readonly Regex IllegalCharsRegex = new Regex(ILLEGALCHARACTERSREGEXSTRING);

        private string _stringRepresentation;

        /// <summary>
        /// An optional message prefix containing the server name or nickname of the 
        /// message source, or null if no source is specified.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// The text or numeric command.  This field is required.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// The optional command parameters, up to 14.
        /// </summary>
        public string[] Parameters { get; private set; }

        /// <summary>
        /// The tail-end of a message, usually containing an informational message but 
        /// may contain additional processable data.
        /// </summary>
        public string Trailing { get; private set; }

        /// <summary>
        /// Parses a line of text from an IRC server into its message parts as defined by RFC2812.
        /// </summary>
        /// <param name="input">A line of text to parse.</param>
        /// <returns>An IRC message.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if input is null.</exception>
        /// <exception cref="System.FormatException">Thrown if there is a problem parsing the input string.  
        /// The input string can be found in the Data dictionary with the "input" key.</exception>
        /// <example>[ ":" prefix ] command [ params*14 ] [ [ ":" ] message ] crlf</example>
        public static IrcMessage Parse(string input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }

            input = RemoveIllegalChars(input);

            try
            {
                string[] parts = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                IrcMessage message = new IrcMessage();
                int tokenIndex = 0;

                if (parts[tokenIndex][0] == ':')
                {
                    message.Prefix = parts[tokenIndex++].Substring(1);
                }

                message.Command = parts[tokenIndex++];

                /**
                 * If the message has a trailing component, the trailing end starts with a colon.  
                 * However, we can't simply perform a substring operation ending at the next colon 
                 * because some messages may contain IP6 addresses which would fuck it up.  
                 * (Willing to bet the original RFC expected a substring indexed against a colon would be possible)
                 * 
                 * So instead, we have to go word by word until we find one that starts with a colon.
                 */
                int paramCount = 0;
                while (tokenIndex < parts.Length && parts[tokenIndex][0] != ':' && paramCount < MAXPARAMSPERLINE)
                {
                    paramCount++;
                    tokenIndex++;
                }

                message.Parameters = new string[paramCount];
                Array.Copy(parts, tokenIndex - paramCount, message.Parameters, 0, paramCount);

                while (tokenIndex < parts.Length)
                {
                    message.Trailing = string.Concat(message.Trailing, parts[tokenIndex++], " ");
                }

                if (!string.IsNullOrEmpty(message.Trailing))
                {
                    message.Trailing = message.Trailing.Substring(1).TrimEnd();
                }

                message._stringRepresentation = input;

                return (message);
            }
            catch (IndexOutOfRangeException)
            {
                FormatException ex = new FormatException(Properties.Resources.IrcMessageFormatError);
                ex.Data["input"] = input;
                throw ex;
            }
        }

        /// <summary>
        /// Removes characters that are forbidden to send in an IRC message, as defined by RFC2812.
        /// These characters are the carriage return, new line, and null characters.
        /// </summary>
        /// <param name="line">A line of text to parse.</param>
        /// <returns>A string with carriage returns, new lines, and nulls removed.</returns>
        /// <remarks>This method is automatically called internally by every parsing method, so it should
        /// only be used in special circumstances when removing illegal characters is necessary outside
        /// of the Parser class.</remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if line is null.</exception>
        public static string RemoveIllegalChars(string line)
        {
            return (IllegalCharsRegex.Replace(line, string.Empty));
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Irc.IrcMessage"/>.
        /// </summary>
        /// <returns>A string representation of this message.</returns>
        public override string ToString()
        {
            return (_stringRepresentation);
        }

        /// <summary>
        /// Parses a line of text from an IRC server into its message parts as defined by RFC2812.  
        /// A return value indicates whether the conversion was successful.
        /// </summary>
        /// <param name="input">A line of text to parse.</param>
        /// <param name="result">If this method succeeds, result contains the parsed message from the input string. 
        /// Otherwise, it contains an empty message.  </param>
        /// <returns>True if the conversion succeeded, false otherwise.</returns>
        /// <example>[ ":" prefix ] command [ params ] [ ":" message ] crlf</example>
        public static bool TryParse(string input, out IrcMessage result)
        {
            try
            {
                result = Parse(input);
                return (true);
            }
            catch (FormatException) { }
            catch (ArgumentNullException) { }

            result = new IrcMessage();
            return (false);
        }
    }
}
