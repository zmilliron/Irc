/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irc.Ctcp.Dcc
{
    /// <summary>
    /// Represents the error that occurs when a file that is received over a 
    /// Direct Client-To-Client (DCC) connection is larger than the file size 
    /// specified by the remote host.
    /// </summary>
    public class FileTooLargeException : Exception
    {
        /// <summary>
        /// Gets the number of bytes that were expected to be transferred.
        /// </summary>
        public long ExpectedFileSize { get; private set; }

        /// <summary>
        /// Gets the number of bytes that were actually transferred before the connection was closed.
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        public FileTooLargeException() : base(Properties.Resources.FileTooLargeException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FileTooLargeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null if none is specified.</param>
        public FileTooLargeException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        /// <param name="expectedFileSize">The expected size, in bytes, of a file that was transferred.</param>
        /// <param name="actualFileSize">The actual size, in bytes, of a file that was transferred.</param>
        public FileTooLargeException(long expectedFileSize, long actualFileSize)  : this()
        {
            ExpectedFileSize = expectedFileSize;
            FileSize = actualFileSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        /// <param name="expectedFileSize">The expected size, in bytes, of a file that was transferred.</param>
        /// <param name="actualFileSize">The actual size, in bytes, of a file that was transferred.</param>
        /// <param name="message">The message that describes the error.</param>
        public FileTooLargeException(long expectedFileSize, long actualFileSize, string message) : this (message)
        {
            ExpectedFileSize = expectedFileSize;
            FileSize = actualFileSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.FileTooLargeException"/> class.
        /// </summary>
        /// <param name="expectedFileSize">The expected size, in bytes, of a file that was transferred.</param>
        /// <param name="actualFileSize">The actual size, in bytes, of a file that was transferred.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null if none is specified.</param>
        public FileTooLargeException(long expectedFileSize, long actualFileSize, string message, Exception innerException) : this(message, innerException)
        {
            ExpectedFileSize = expectedFileSize;
            FileSize = actualFileSize;
        }
    }
}
