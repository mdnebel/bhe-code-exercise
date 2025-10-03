using System;

namespace Sieve.Tests

{
    [TestClass]
    public class UnitTest1
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
        //[DataRow(100000000, 2038074751)] not required, just a fun challenge
        public void TestNthPrime(long n, long expectedPrime)
        {
            ISieve sieve = new SieveImplementation();
            Assert.AreEqual(expectedPrime, sieve.NthPrime(n));
        }

        [TestMethod]
        public void TestNthPrimeOutOfRange()
        {
            ISieve sieve = new SieveImplementation();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => sieve.NthPrime(-1));
        }

        [TestMethod]
        public void TestFlaggingCompositeValues()
        {
            bool[] compositeFlags = SieveImplementation.FlagCompositeValues(10);
            // Should return an array of values 0-10 with the expected flags for false indicating prime
            // (except for the first two elements, which are ignored)
            CollectionAssert.AreEqual(new bool[] { false, false, false, false, true, false, true, false, true, true, true }, compositeFlags);
        }
    }
}