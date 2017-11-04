/****************************************************************************************
 * Copyright (c) Zachary Milliron
 *
 * This source is subject to the Microsoft Public License.
 * See https://opensource.org/licenses/MS-PL.
 * All other rights worth reserving are reserved.
 ****************************************************************************************/
using System;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguageAttribute("en")]

namespace Irc
{
    /// <summary>
    /// Contains classes and types that provide an interface for the Internet Relay Chat protocol.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    sealed class NamespaceDoc
    {
        //this allows Sandcastle to generate a summary for the namespace.
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class Global
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Contains(this string containingString, char value)
        {
            bool contains = false;
            int i = 0;
            while (!contains && i < containingString.Length)
            {
                contains = containingString[i] == value;
                i++;
            }
            return (contains);
        }
    }

    /// <summary>
    /// Defines values that specify the valid IRC uri schemes.
    /// </summary>
    public static class IrcUriSchemes
    {
        /// <summary>
        /// The default scheme handling unsecure connections.
        /// </summary>
        public const string Default = "irc";

        /// <summary>
        /// The scheme for handling IPv6 IPs.
        /// </summary>
        public const string IPv6 = "irc6";

        /// <summary>
        /// The scheme for handling IRC over Secure Socket Layers.
        /// </summary>
        public const string Ssl = "ircs";

        /// <summary>
        /// Returns a value indicating if the specified URI scheme is valid for the IRC protocol.
        /// </summary>
        /// <param name="uriScheme">The URI scheme to validate.</param>
        /// <returns>True if urischeme is valid, false otherwise.</returns>
        public static bool IsValid(string uriScheme)
        {
            return (uriScheme == Default || uriScheme == IPv6 || uriScheme == Ssl);
        }
    }
}
