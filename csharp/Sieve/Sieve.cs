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
    private long[] _primes = Array.Empty<long>();

    /// <param name="maxN">The largest value of n expected to be needed; used to pre-create the sieve as an optimization.</param>
    public SieveImplementation(long? maxN = null)
    {
        if (maxN is null)
        {
            // Wait until first use to populate primes
            return;
        }

        ValidateN(maxN.GetValueOrDefault());

        _primes = FindPrimes(maxN.GetValueOrDefault() + 1);
    }

    public long NthPrime(long n)
    {
        ValidateN(n);

        if (n >= _primes.Length)
        {
            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            long primeCount = Math.Max(n + 1, _primes.Length * 2);
            _primes = FindPrimes(primeCount);
        }

        return _primes[n];
    }

    private static long[] FindPrimes(long primeCount)
    {
        var primes = new long[primeCount];

        // Special case for the first prime (2) since we only use odds to populate the sieve
        primes[0] = 2;

        if (primeCount == 1)
        {
            return primes;
        }

        long upperBound = GetUpperBound(primeCount);

        // Potential improvement: further optimize space and time using wheel factorization
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
            for (long j = value * value / 2; j < compositeFlags.Length; j += i * 2 + 1)
            {
                compositeFlags[j] = true;
            }
        }

        return compositeFlags;
    }

    private static void ValidateN(long n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Must be 0 or greater");
        }

        // Potential improvement: calculate the n of the highest upper bound that can fit in a long to use here instead
        if (n == long.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(n), n, "Cannot be max value");
        }
    }
}