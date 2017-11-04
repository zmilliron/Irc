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
    /// Represents a user on a channel on an Internet Relay Chat (IRC) network.
    /// </summary>
    public sealed class ChannelUser : User, IComparable<ChannelUser>
    {
        #region Fields...

        /// <summary>
        /// The prefix used to desginate channel operator status.
        /// </summary>
        public const string OPERATORPREFIX = "@";

        /// <summary>
        /// The prefix used to designate voice status.
        /// </summary>
        public const string VOICEPREFIX = "+";

        /// <summary>
        /// The prefix used to designate chanel owner status.
        /// </summary>
        public const string OWNERPREFIX = "~";

        /// <summary>
        /// The prefix used to designate channel half-operator status.
        /// </summary>
        public const string HALFOPERATORPREFIX = "%";

        /// <summary>
        /// The prefix used to desginate protected user status.
        /// </summary>
        public const string PROTECTEDPREFIX = "&";

        private bool _ignored;
        private UserChannelModes _channelStatus;
        
        #endregion

        #region Properties...

        /// <summary>
        /// Gets or sets the channel the user belongs to.
        /// </summary>
        public Channel Channel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the user has channel operator status.
        /// </summary>
        public bool IsChannelOperator
        {
            get { return ((_channelStatus & UserChannelModes.Operator) == UserChannelModes.Operator); }
            set
            {
                if (((_channelStatus & UserChannelModes.Operator) == UserChannelModes.Operator) != value)
                {
                    _channelStatus = value ? _channelStatus | UserChannelModes.Operator : _channelStatus ^ UserChannelModes.Operator;
                    OnPropertyChanged(nameof(IsChannelOperator));
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user has voiced status.
        /// </summary>
        public bool IsVoiced
        {
            get { return ((_channelStatus & UserChannelModes.Voice) == UserChannelModes.Voice); }
            set
            {
                if (((_channelStatus & UserChannelModes.Voice) == UserChannelModes.Voice) != value)
                {
                    _channelStatus = value ? _channelStatus | UserChannelModes.Voice : _channelStatus ^ UserChannelModes.Voice;
                    OnPropertyChanged(nameof(IsVoiced));
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user is a channel owner.
        /// </summary>
        public bool IsChannelOwner
        {
            get { return ((_channelStatus & UserChannelModes.Owner) == UserChannelModes.Owner); }
            set
            {
                if (((_channelStatus & UserChannelModes.Owner) == UserChannelModes.Owner) != value)
                {
                    _channelStatus = value ? _channelStatus | UserChannelModes.Owner : _channelStatus ^ UserChannelModes.Owner;
                    OnPropertyChanged(nameof(IsChannelOwner));
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user is currently being ignored.
        /// </summary>
        public bool IsIgnored
        {
            get
            {
                return (_ignored);
            }
            set
            {
                if (_ignored != value)
                {
                    _ignored = value;
                    OnPropertyChanged(nameof(IsIgnored));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user has half-operator status.
        /// </summary>
        public bool IsHalfOperator
        {
            get { return ((_channelStatus & UserChannelModes.HalfOperator) == UserChannelModes.HalfOperator); }
            set
            {
                if (((_channelStatus & UserChannelModes.HalfOperator) == UserChannelModes.HalfOperator) != value)
                {
                    _channelStatus = value ? _channelStatus | UserChannelModes.HalfOperator : _channelStatus ^ UserChannelModes.HalfOperator;
                    OnPropertyChanged(nameof(IsHalfOperator));
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user has protected status.
        /// </summary>
        public bool IsProtected
        {
            get { return ((_channelStatus & UserChannelModes.Protected) == UserChannelModes.Protected); }
            set
            {
                if (((_channelStatus & UserChannelModes.Protected) == UserChannelModes.Protected) != value)
                {
                    _channelStatus = value ? _channelStatus | UserChannelModes.Protected : _channelStatus ^ UserChannelModes.Protected;
                    OnPropertyChanged(nameof(IsProtected));
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        /// <summary>
        /// Gets or sets the combined channel status of the user.
        /// </summary>
        public UserChannelModes ChannelStatus
        {
            get
            {
                return (_channelStatus);
            }
            set
            {
                if (_channelStatus != value)
                {
                    _channelStatus = value;
                    OnPropertyChanged(nameof(ChannelStatus));
                }
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelUser"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        /// <param name="channel">The channel the user belongs to.</param>
        /// <param name="client">The connection to the network on which the user exists.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or connection is null.</exception>
        public ChannelUser(IrcNickname nickname, Channel channel, IIrcxProtocol client)
            : base(nickname, client)
        {
            if (channel == null) { throw new ArgumentNullException(nameof(channel)); }
            Channel = channel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Irc.ChannelUser"/> class.
        /// </summary>
        /// <param name="nickname">The nickname of the user.</param>
        /// <param name="channel">The channel the user belongs to.</param>
        /// <param name="client">The connection to the network on which the user exists.</param>
        /// <param name="modes">The current channel modes set for the user.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if nickname or connection is null.</exception>
        public ChannelUser(IrcNickname nickname, Channel channel, IIrcxProtocol client, string modes)
            : this(nickname, channel, client)
        {
            //set the channel status of the user
            if (!string.IsNullOrEmpty(modes))
            {
                if (modes.Contains(OPERATORPREFIX))
                {
                    _channelStatus |= UserChannelModes.Operator;
                }
                if (modes.Contains(VOICEPREFIX))
                {
                    _channelStatus |= UserChannelModes.Voice;
                }
                if (modes.Contains(HALFOPERATORPREFIX))
                {
                    _channelStatus |= UserChannelModes.HalfOperator;
                }
                if (modes.Contains(OWNERPREFIX))
                {
                    _channelStatus |= UserChannelModes.Owner;
                }
                if (modes.Contains(PROTECTEDPREFIX))
                {
                    _channelStatus |= UserChannelModes.Protected;
                }
            }    
        }

        /// <summary>
        /// Compares the current instance with another <see cref="System.Object"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort 
        /// order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">An object that evalutes to a <see cref="Irc.ChannelUser"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows or occurs 
        /// in the same position in the sort order as the specified <see cref="Irc.ChannelUser"/></returns>
        /// <exception cref="System.ArgumentException">Thrown if obj is not a <see cref="Irc.ChannelUser"/>.</exception>
        public override int CompareTo(object obj)
        {
            if (!(obj is ChannelUser)) { throw new ArgumentException("Object must be of type Irc.ChannelUser."); }


            /**
             * If objects are equal: return 0
             * If this object occurs before the parameter: return less than 0
             * If this object occurs after the parameter: return greater than 0
             * 
             */

            int retVal = 1;
            if (obj != null)
            {
                ChannelUser other = (ChannelUser)obj;

                //compare channel status first
                retVal = this.ChannelStatus.CompareTo(other.ChannelStatus) * -1;

                //if channel status is equal, sort by name
                if(retVal == 0)
                {
                    retVal = Nickname.CompareTo(other.Nickname);
                }
            }

            return (retVal);
        }

        /// <summary>
        /// Compares the current instance with another <see cref="System.Object"/> and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort 
        /// order as the specified <see cref="System.Object"/>.
        /// </summary>
        /// <param name="other">An object that evalutes to a <see cref="Irc.ChannelUser"/>.</param>
        /// <returns>An integer that indicates whether the current instance precedes, follows or occurs 
        /// in the same position in the sort order as the specified <see cref="Irc.ChannelUser"/></returns>
        public int CompareTo(ChannelUser other)
        {
            return (this.CompareTo((object)other));
        }
    }
}
