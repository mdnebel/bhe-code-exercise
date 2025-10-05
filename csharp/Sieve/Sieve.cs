using System;

namespace Sieve;

public abstract class Sieve : ISieve
{
    private long[] _primes = Array.Empty<long>();

    /// <param name="maxN">The largest value of n expected to be needed; used to pre-create the sieve as an optimization.</param>
    public Sieve(long? maxN = null)
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
            // Use double the currently known number of primes to mitigate worst case of NthPrime being called on increasing consecutive values of n
            long primeCount = Math.Max(n + 1, _primes.Length * 2);
            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            _primes = FindPrimes(primeCount);
        }

        return _primes[n];
    }

    protected abstract long[] FindPrimes(long primeCount);

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