
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Irc.UnitTests
{
    [TestClass]
    public class IrcNicknameTests
    {
        /// <summary>
        /// Returns a randomized valid name, no greater than 24 characters in length.  The length
        /// limit is arbitarily chosen.  Valid names are defined by the IRC protocol as having ASCII
        /// values between 65 and 125.
        /// </summary>
        /// <returns>A string containing a valid IRC name.</returns>
        private string GetRandomValidName()
        {
            Random rand = new Random();
            int nameLength = rand.Next(1, 24);
            int firstChar = rand.Next(65, 125);
            string name = ((char)firstChar).ToString();

            for (int i = 1; i < nameLength; i++)
            {
                //insert a number on every third character, if applicable
                if (i % 3 == 0)
                {
                    name += ((char)rand.Next(48, 57)).ToString();
                }
                else
                {
                    name += ((char)rand.Next(65, 125)).ToString();
                }
            }

            return (name);
        }

        /// <summary>
        /// Returns a randomized invalid name, no greater than 25 characters in length.  The length 
        /// limit is arbitrarily chosen.  An invalid name contains characters outside the ASCII range 
        /// of 65 - 127.  returned name is guaranteed to end with ASCI character 181 (µ);
        /// </summary>
        /// <returns></returns>
        private string GetRandomInvalidName()
        {
            Random rand = new Random();
            int nameLength = rand.Next(1, 24);
            string name = string.Empty;

            for (int i = 0; i <= nameLength; i++)
            {
                name += ((char)rand.Next(32, 255)).ToString();
            }

            name += "µ";

            return (name);
        }

        #region Inheritance and Interface Tests...

        [TestMethod]
        public void IsIrcNameBase()
        {
            IrcNickname name = new IrcNickname("TestName");
            Assert.IsInstanceOfType(name, typeof(IrcNameBase));
        }

        [TestMethod]
        public void IsTypedComparable()
        {
            IrcNickname name = new IrcNickname("TestName");
            Assert.IsInstanceOfType(name, typeof(IComparable<IrcNickname>));
        }

        [TestMethod]
        public void IsComparable()
        {
            IrcNickname name = new IrcNickname("TestName");
            Assert.IsInstanceOfType(name, typeof(IComparable));
        }

        [TestMethod]
        public void IsEquatable()
        {
            IrcNickname name = new IrcNickname("TestName");
            Assert.IsInstanceOfType(name, typeof(IEquatable<IrcNameBase>));
        }

        #endregion

        #region Constructor Tests...

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorShouldNotAcceptNull()
        {
            new IrcNickname(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructorShouldNotAcceptEmptyString()
        {
            new IrcNickname(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructorShouldNotAcceptWhiteSpace()
        {
            new IrcNickname("      ");
        }

        [TestMethod]
        public void ConstructorFormatExceptionShouldHaveCorrectMessage()
        {
            try
            {
                new IrcNickname("      ");
            }
            catch(FormatException ex)
            {
                Assert.AreEqual(Irc.Properties.Resources.NicknameFormatError, ex.Message);
            }
        }

        [TestMethod]
        public void FirstCharacterShouldAllowAscii65Through125()
        {
            //accepts ascii 65 through 125
            for (int i = 65; i <= 125; i++)
            {
                new IrcNickname(((char)i).ToString());
            }
        }

        [TestMethod]
        public void ConstructorShouldAcceptValidName()
        {
            new IrcNickname(GetRandomValidName());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConstructorShouldNotAcceptInvalidName()
        {
            new IrcNickname(GetRandomInvalidName());
        }

        #endregion

        #region IsValid Tests...

        [TestMethod]
        public void IsValidShouldReturnTrueIfFirstCharValid()
        {
            //accepts ascii 65 through 125
            for (int i = 65; i <= 125; i++)
            {
                Assert.IsTrue(IrcNickname.IsValid(((char)i).ToString()));
            }
        }

        [TestMethod]
        public void IsValidShouldReturnTrue()
        {
            Assert.IsTrue(IrcNickname.IsValid(GetRandomValidName()));
        }

        [TestMethod]
        public void IsValidShouldReturnFalse()
        {
            Assert.IsFalse(IrcNickname.IsValid(GetRandomInvalidName()));
        }

        #endregion

        #region CompareTo Tests...

        [TestMethod]
        public void CompareToNullShouldBePositive()
        {
            string testName = "TestName";
            IrcNickname nickname = new IrcNickname(testName);

            Assert.IsTrue(nickname.CompareTo(null) > 0);
        }

        [TestMethod]
        public void CompareToSelfShouldBeZero()
        {
            string testName = "TestName";
            IrcNickname nickname = new IrcNickname(testName);

            Assert.AreEqual(0, nickname.CompareTo(nickname));
        }

        [TestMethod]
        public void CompareToLaterNameShouldBeNegative()
        {
            string testName = "TestName";
            string testName2 = "TettName";
            IrcNickname nickname = new IrcNickname(testName);
            IrcNickname nickname2 = new IrcNickname(testName2);

            Assert.IsTrue(nickname.CompareTo(nickname2) < 0);
        }

        [TestMethod]
        public void CompareToEqualNameShouldBeZero()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            IrcNickname nickname2 = new IrcNickname("TestName");

            Assert.AreEqual(0, nickname.CompareTo(nickname2));
        }

        [TestMethod]
        public void CompareToDifferentObjectShouldBePositive()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            object o = new object();

            Assert.IsTrue(nickname.CompareTo(o) > 0);
        }

        #endregion

        #region Equals Tests...

        [TestMethod]
        public void EqualsNullShouldBeFalse()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsFalse(nickname.Equals(null));
        }

        [TestMethod]
        public void EqualsSelfShouldBeTrue()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsTrue(nickname.Equals(nickname));
        }

        [TestMethod]
        public void EqualsEqualNameShouldBeTrue()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            IrcNickname nickname2 = new IrcNickname("TestName");

            Assert.IsTrue(nickname.Equals(nickname2));
            Assert.IsTrue(nickname2.Equals(nickname));
        }

        [TestMethod]
        public void EqualsShouldIgnoreCaseAndBeTrue()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            IrcNickname nickname2 = new IrcNickname("testname");

            Assert.IsTrue(nickname.Equals(nickname2));
            Assert.IsTrue(nickname2.Equals(nickname));
        }

        [TestMethod]
        public void EqualsUnequalNameShouldBeFalse()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            IrcNickname nickname2 = new IrcNickname("HelloWorld");

            Assert.IsFalse(nickname.Equals(nickname2));
            Assert.IsFalse(nickname2.Equals(nickname));
        }

        [TestMethod]
        public void EqualsOtherObjectShouldBeFalse()
        {
            IrcNickname nickname = new IrcNickname("TestName");
            object o = new object();

            Assert.IsFalse(nickname.Equals(o));
        }

        #endregion

        #region Contains Tests...

        [TestMethod]
        public void ContainsShouldBeTrue()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsTrue(nickname.Contains("stN"));
        }

        [TestMethod]
        public void ContainsShouldBeFalse()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsFalse(nickname.Contains("r"));
        }

        #endregion

        #region StartsWith Tests...

        [TestMethod]
        public void StartsWithShouldBeTrue()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsTrue(nickname.StartsWith("TestN"));
        }

        [TestMethod]
        public void StartsWithShouldBeFalse()
        {
            IrcNickname nickname = new IrcNickname("TestName");

            Assert.IsFalse(nickname.StartsWith("Hello"));
        }

        #endregion

        #region Explicit Cast Tests...

        [TestMethod]
        public void ExplicitCastToValidNameShouldSucceed()
        {
            IrcNickname nickname = (IrcNickname)"TestName";
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ExplicitCastToInvalidNameShouldFail()
        {
            IrcNickname nickname = (IrcNickname)"3TestName";
        }

        [TestMethod]
        public void ExplicitCastToInvalidNameShouldHaveCorrectMessage()
        {
            string name = "3TestName";
            try
            {
                IrcNickname nickname = (IrcNickname)name;
            }
            catch(InvalidCastException ex)
            {
                Assert.AreEqual(string.Format(Irc.Properties.Resources.InvalidCastNickname, name), ex.Message);
            }
        }

        #endregion

        #region Equality operator (==) Tests...

        [TestMethod]
        public void EqualityOperatorShouldBeTrue()
        {
            string name = "TestName";
            string name2 = "TestName";
            IrcNickname nickname = new IrcNickname(name);
            IrcNickname nickname2 = new IrcNickname(name2);

            Assert.IsTrue(nickname == nickname2);
            Assert.IsTrue(nickname2 == nickname);
        }

        [TestMethod]
        public void EqualityOperatorShouldIgnoreCaseAndBeTrue()
        {
            string name = "TestName";
            string name2 = "testname";
            IrcNickname nickname = new IrcNickname(name);
            IrcNickname nickname2 = new IrcNickname(name2);

            Assert.IsTrue(nickname == nickname2);
            Assert.IsTrue(nickname2 == nickname);
        }

        [TestMethod]
        public void EqualityToSelfShouldBeTrue()
        {
            string name = "TestName";
            IrcNickname nickname = new IrcNickname(name);

            Assert.IsTrue(nickname == nickname);
        }

        [TestMethod]
        public void EqualityShouldBeFalse()
        {
            string name = "TestName";
            string name2 = "HelloWorld";
            IrcNickname nickname = new IrcNickname(name);
            IrcNickname nickname2 = new IrcNickname(name2);

            Assert.IsFalse(nickname == nickname2);
            Assert.IsFalse(nickname2 == nickname);
        }

        #endregion

        [TestMethod]
        public void ShouldImplicitCastToString()
        {
            string expectedName = "TestName";
            IrcNickname name = new IrcNickname(expectedName);
            string stringName = name;

            Assert.AreEqual(expectedName, name);
            Assert.AreEqual(expectedName, stringName);
        }

        [TestMethod]
        public void ToStringShouldReturnName()
        {
            string testName = "TestName";
            IrcNickname nickname = new IrcNickname(testName);

            Assert.AreEqual(testName, nickname.ToString());
        }

        [TestMethod]
        public void LengthShouldBeEqualToInputStringLength()
        {
            string name = "TestName";
            IrcNickname nickname = new IrcNickname(name);

            Assert.AreEqual(name.Length, nickname.Length);
        }

        [TestMethod]
        public void GetHashCodeShouldReturnUpperInvariantHashCode()
        {
            string name = "TestName";
            IrcNickname nickname = new IrcNickname(name);

            Assert.AreEqual(name.ToUpperInvariant().GetHashCode(), nickname.GetHashCode());
        }
    }
}
