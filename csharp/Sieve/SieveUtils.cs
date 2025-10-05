using System;

namespace Sieve;

public static class SieveUtils
{
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

    internal static void ValidateN(long n)
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