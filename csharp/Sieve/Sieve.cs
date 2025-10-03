using System;

namespace Sieve;

public interface ISieve
{
    long NthPrime(long n);
}

public class SieveImplementation : ISieve
{
    private long[] _primes = Array.Empty<long>();

    public long NthPrime(long n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Must be 0 or greater");
        }

        if (n >= _primes.Length)
        {
            // TODO: Look into an efficient way of expanding the array on demand instead of recreating it
            _primes = FindPrimes(n + 1);
        }

        return _primes[n];
    }

    private static long[] FindPrimes(long primeCount)
    {
        if (primeCount == 1)
        {
            // Special case if we only ever need the first prime number
            return [2];
        }

        // TODO: This gets way too large for large values of N; figure out a better way of determining the upper bound
        long upperBound = primeCount * primeCount;

        // Initital naive implementation: an array large enough for every possible prime number.
        // Each index represents a possible prime number.
        // (I considered subtracting 2 from the array size to save space, but saving 2 bools does not seem worth the potential for confusion)
        // TODO: optimize space and time with only odds
        // TODO: investigate optimizing space and time with wheel factorization
        // TODO: investigate optimizing space with segmentation
        var compositeFlags = FlagCompositeValues(upperBound);

        var primes = new long[primeCount];
        int primeIndex = 0;

        for (int i = 2; i < upperBound && primeIndex < primes.Length; i++)
        {
            if (!compositeFlags[i])
            {
                primes[primeIndex] = i;
                primeIndex++;
            }
        }

        return primes;
    }

    internal static bool[] FlagCompositeValues(long maxValue)
    {
        var compositeFlags = new bool[maxValue + 1];
        var sqrtMaxValue = (long)Math.Sqrt(maxValue);

        for (int i = 2; i <= sqrtMaxValue; i++)
        {
            if (compositeFlags[i])
            {
                continue;
            }

            for (int j = i * i; j <= maxValue; j += i)
            {
                compositeFlags[j] = true;
            }
        }

        return compositeFlags;
    }
}