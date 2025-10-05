using System;
using System.Collections.Generic;

namespace Sieve;

public class OddsOnlySieve : ISieve
{
    private long[] _primes = Array.Empty<long>();

    /// <param name="maxN">The largest value of n expected to be needed; used to pre-create the sieve as an optimization.</param>
    public OddsOnlySieve(long maxN = -1)
    {
        if (maxN < 0)
        {
            // Wait until first use to populate primes
            return;
        }

        SieveUtils.ValidateN(maxN);

        _primes = FindPrimes(maxN + 1);
    }

    public long NthPrime(long n)
    {
        SieveUtils.ValidateN(n);

        if (n >= _primes.Length)
        {
            // Use double the currently known number of primes to mitigate worst case of NthPrime being called on increasing consecutive values of n
            long primeCount = Math.Max(n + 1, _primes.Length * 2);
            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            _primes = FindPrimes(primeCount);
        }

        return _primes[n];
    }

    private long[] FindPrimes(long primeCount)
    {
        var primes = new long[primeCount];

        // Special case for the first prime (2) since we only use odds to populate the sieve
        primes[0] = 2;

        if (primeCount == 1)
        {
            return primes;
        }

        long upperBound = SieveUtils.GetUpperBound(primeCount);
        var compositeFlags = FlagOddCompositeValues(upperBound);
        long primeIndex = 1;
        // Skip i = 0, representing the integer 1, which is not prime
        for (long i = 1; i < compositeFlags.Length && primeIndex < primeCount; i++)
        {
            if (!compositeFlags[i])
            {
                primes[primeIndex] = 2 * i + 1;
                primeIndex++;
            }
        }

        if (primeIndex != primeCount)
        {
            throw new Exception($"Only {primeIndex + 1} prime values were found but {primeCount} were required. {nameof(upperBound)}: {upperBound}");
        }

        return primes;
    }

    /// <summary>
    /// Returns an array where each element represents an odd integer starting at 1,
    /// where false indicates prime and true indicates composite.
    /// </summary>
    internal static bool[] FlagOddCompositeValues(long maxValue)
    {
        var compositeFlags = new bool[(maxValue + 1) / 2];
        var sqrtMaxValue = (long)Math.Sqrt(maxValue);
        long indexOfSqrtMaxValue = sqrtMaxValue / 2;

        for (long i = 1; i <= indexOfSqrtMaxValue; i++)
        {
            if (compositeFlags[i])
            {
                // If the value at this index is already marked as composite,
                // anything it could identify as composite would already be marked as such.
                continue;
            }

            // Start at the index of the value represented by the square of this prime,
            // since everything prior to that will already be marked properly.
            long value = i * 2 + 1;
            for (long j = value * value / 2; j < compositeFlags.Length; j += value)
            {
                compositeFlags[j] = true;
            }
        }

        return compositeFlags;
    }
}