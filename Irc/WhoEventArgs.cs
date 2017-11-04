
namespace Irc
{
    /// <summary>
    /// Provides data for an event raised after a WHO reply is received from a server.
    /// </summary>
    /// <seealso cref="Irc.Numerics.RPL_WHOREPLY"/>
    /// <seealso cref="Irc.Commands.WHO"/>
    public class WhoEventArgs : UserEventArgs
    {
        /// <summary>
        /// The name of a channel the user is on.
        /// </summary>
        public IrcChannelName ChannelName { get; set; }

        /// <summary>
        /// The server on which the target is connected.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The number of hops between the client's server and the target's server.
        /// </summary>
        public int HopCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the target is away.
        /// </summary>
        public bool IsAway { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the target is a channel operator.
        /// </summary>
        public bool IsChannelOp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the target is voiced.
        /// </summary>
        public bool IsVoiced { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.WhoEventArgs"/> class.
        /// </summary>
        /// <param name="searchedUser">The nickname of the user searched for with a WHO command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if searchedUser is null.</exception>
        public WhoEventArgs(IrcNickname searchedUser) : base(searchedUser) { }
    }
}
