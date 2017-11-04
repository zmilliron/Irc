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
    /// Provides data for an event raised when an error has occurred
    /// while parsing data received from an IRC server.
    /// </summary>
    public class ParsingErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the unformatted line of text received that caused the parsing error.
        /// </summary>
        public string RawText { get; set; }

        /// <summary>
        /// Gets or sets the exception caught when parsing a line of text.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ParsingErrorEventArgs"/> class.
        /// </summary>
        public ParsingErrorEventArgs() { }
    }
}
