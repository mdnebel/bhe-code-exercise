using System;

namespace Sieve.Tests

{
    [TestClass]
    public class SieveTests
    {
        [TestMethod]
        [DataRow(0, 2)]
        [DataRow(1, 3)]
        [DataRow(2, 5)]
        [DataRow(19, 71)]
        [DataRow(99, 541)]
        [DataRow(500, 3581)]
        [DataRow(986, 7793)]
        [DataRow(2000, 17393)]
        [DataRow(1000000, 15485867)]
        [DataRow(10000000, 179424691)]
        // [DataRow(100000000, 2038074751)] // Passes, but is very slow
        public void TestNthPrime(long n, long expectedPrime)
        {
            ISieve sieve = new SieveImplementation();
            Assert.AreEqual(expectedPrime, sieve.NthPrime(n));
        }

        [TestMethod]
        public void TestNthPrimeExceptions()
        {
            ISieve sieve = new SieveImplementation();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => sieve.NthPrime(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => sieve.NthPrime(long.MaxValue));
            Assert.ThrowsException<OverflowException>(() => sieve.NthPrime(long.MaxValue - 1));
        }

        [TestMethod]
        [DataRow(0, 37)] // actual prime: 2
        [DataRow(1, 37)] // actual prime: 3
        [DataRow(2, 37)] // actual prime: 5
        [DataRow(11, 37)] // actual prime: 37
        [DataRow(12, 41)] // actual prime: 41
        [DataRow(19, 75)] // actual prime: 71
        [DataRow(99, 572)] // actual prime: 541
        [DataRow(500, 3794)] // actual prime: 3581
        [DataRow(986, 8221)] // actual prime: 7793
        [DataRow(2000, 18229)] // actual prime: 17393
        [DataRow(1000000, 15783429)] // actual prime: 15485867
        [DataRow(10000000, 182084922)] // actual prime: 179424691
        [DataRow(100000000, 2061884872)] // actual prime: 2038074751
        public void TestNthUpperBound(long n, long expectedUpperBound)
        {
            Assert.AreEqual(expectedUpperBound, SieveImplementation.GetUpperBound(n));
        }

        [TestMethod]
        public void TestFlaggingCompositeValues()
        {
            // [1 (ignored), 3 (prime), 5 (prime), 7 (prime), 9 (composite)]
            bool[] expectedFlags = [false, false, false, false, true];
            bool[] compositeFlags = SieveImplementation.FlagOddCompositeValues(9);
            CollectionAssert.AreEqual(expectedFlags, compositeFlags);
            compositeFlags = SieveImplementation.FlagOddCompositeValues(10);
            CollectionAssert.AreEqual(expectedFlags, compositeFlags);
        }
    }
}