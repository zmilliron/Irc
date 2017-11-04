/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Text.RegularExpressions;

namespace Irc
{
    /// <summary>
    /// Represents a valid Internet Relay Chat (IRC) password.
    /// </summary>
    public sealed class IrcPassword
    {
        private const string REGEXSTRING = @"[^\x00\r\n\t\s]";
        private static readonly Regex _passwordRegex = new Regex(REGEXSTRING);
        private const int MAXLENGTH = 23;

        //yeah, technically this should probably be SecureString, but I'm lazy and we're not securing Ft. Knox
        //there was also a technical reason I can no longer remember why this would be difficul to manage
        private readonly string _pass;

        /// <summary>
        /// Gets the number of characters in the current Irc.IrcPassword object.
        /// </summary>
        public int Length
        {
            get { return (_pass.Length); }
        }

        /// <summary>
        /// Initializes a new instance of the Irc.IrcPassword class.
        /// </summary>
        /// <param name="password">The password the structure will represent.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if password is null or empty.</exception>
        /// <exception cref="System.FormatException">Thrown if password is not a valid password as defined by RFC2812.</exception>
        public IrcPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException(nameof(password)); }
            if (!_passwordRegex.IsMatch(password)) { throw new FormatException(Properties.Resources.PasswordFormatError); }
            //if(password.Length > MAXLENGTH){ throw new ArgumentOutOfRangeException(Properties.Resources.PasswordLengthError); }

            _pass = password;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object.
        /// </summary>
        /// <param name="obj">The System.Object to compare.</param>
        /// <returns>True if the System.Object is equal to the current instance, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool retVal = false;
            if (obj != null && obj is IrcPassword && (IrcPassword)obj == this)
            {
                retVal = true;
            }

            return (retVal);
        }

        /// <summary>
        /// Returns the hash code for this System.Object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (_pass.GetHashCode());
        }

        /// <summary>
        /// Determines whether the specified string is a valid password.
        /// </summary>
        /// <param name="password">The string to validate.</param>
        /// <returns>True if the string is a valid password, false otherwise.</returns>
        public static bool IsValid(string password)
        {
            return (_passwordRegex.IsMatch(password));
        }

        /// <summary>
        /// Determines value equality between two Irc.Password objects.
        /// </summary>
        /// <param name="password1">The first Irc.Password to compare.</param>
        /// <param name="password2">The second Irc.Password to compare.</param>
        /// <returns>True if Passwords are equal independent of casing, false otherwise.</returns>
        public static bool operator ==(IrcPassword password1, IrcPassword password2)
        {
            bool retVal = false;
            if ((System.Object)password1 != null && (System.Object)password2 != null)
            {
                if (Object.ReferenceEquals(password1, password2) ||
                    password1._pass == password2._pass)
                {
                    retVal = true;
                }
            }
            else if ((System.Object)password1 == null && (System.Object)password1 == null)
            {
                retVal = true;
            }
            return (retVal);
        }

        /// <summary>
        /// Determines value inequality between two Irc.Password objects.
        /// </summary>
        /// <param name="password1">The first Irc.Password to compare.</param>
        /// <param name="password2">The second Irc.Password to compare.</param>
        /// <returns>True if the Passwords are not equal independent of casing, false otherwise.</returns>
        public static bool operator !=(IrcPassword password1, IrcPassword password2)
        {
            return (!(password1 == password2));
        }

        /// <summary>
        /// Returns the string representation of this Irc.Password instance.
        /// </summary>
        /// <returns>A string representation of this Irc.Password instance.</returns>
        public override string ToString()
        {
            return (_pass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static implicit operator string(IrcPassword i)
        {
            return (i == null ? null : i.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static explicit operator IrcPassword(string s)
        {
            return (s == null ? null : new IrcPassword(s));
        }
    }
}
