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
    /// Provides data for a Direct Client Connection (DCC) file transfer error.
    /// </summary>
    public class DccErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or the DCC error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception that was thrown during a DCC file transfer.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccErrorEventArgs"/> class.
        /// </summary>
        public DccErrorEventArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorMessage"></param>
        public DccErrorEventArgs(string errorMessage)
        {
            Message = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccErrorEventArgs"/> class.
        /// </summary>
        /// <param name="dccError">An exception that was thrown during a DCC file transfer.</param>
        public DccErrorEventArgs(Exception dccError)
        {
            this.Exception = dccError;
            if (dccError != null)
            {
                Message = dccError.Message;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.Ctcp.Dcc.DccErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="dccError"></param>
        public DccErrorEventArgs(string errorMessage, Exception dccError)
        {
            Message = errorMessage;
            Exception = dccError;
        }
    }
}
