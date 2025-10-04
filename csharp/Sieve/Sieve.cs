using System;

namespace Sieve;

public interface ISieve
{
    /// <summary>
    /// Returns the nth prime starting with 0, where n=0 returns 2, n=1 returns 3, etc.
    /// </summary>
    long NthPrime(long n);
}

public class SieveImplementation : ISieve
{
    public SieveImplementation(long? maxN = null)
    {
        if (maxN is null)
        {
            // Wait until first use to populate primes
            return;
        }

        if (maxN < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxN), maxN, "Must be 0 or greater");
        }

        _primes = FindPrimes(maxN.GetValueOrDefault() + 1);
    }

    private long[] _primes = Array.Empty<long>();

    public long NthPrime(long n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Must be 0 or greater");
        }

        if (n >= _primes.Length)
        {
            if (n == long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(n), n, "Cannot be max value");
            }

            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            long primeCount = Math.Max(n + 1, _primes.Length * 2);
            _primes = FindPrimes(primeCount);
        }

        return _primes[n];
    }

    private static long[] FindPrimes(long primeCount)
    {
        long upperBound = GetUpperBound(primeCount);

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

    internal static long GetUpperBound(long n)
    {
        if (n < 12)
        {
            return 37; // the 12th prime; the nth prime must be <= this value
            // Could return from an array of the first 12 primes, but 37 is a pretty trivially small number anyway
        }

        // p(n) < n (ln n + ln ln n - 1 + 1.8 ln ln n / ln n)
        // Equation sourced from: https://t5k.org/howmany.html
        // Only works for n >= 12
        checked
        {
            // Increment n as this equation expects n to start at 1
            ++n;
            double logN = Math.Log(n);
            double logLogN = Math.Log(logN);
            var upperBound = (long)(n * (logN + logLogN - 1 + 1.8 * logLogN / logN));
            return upperBound;
        }
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