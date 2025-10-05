using System;
using System.Collections.Generic;

namespace Sieve;

public class WheelFactorizationSieve : ISieve
{
    private static readonly long[] DefaultBasis = [2, 3, 5];
    // The first prime numbers uses a basis for filtering out a large number of subsequent composites
    private readonly long[] _basis;
    // The primes in the first "turn" of the wheel given the basis above
    private long[] _primesFirstTurn = Array.Empty<long>();
    // Primes found after the first turn of the wheel
    private long[] _primesAfterFirstTurn = Array.Empty<long>();

    /// <param name="basis">The set of prime numbers used to populate the wheel used for factorization. Defaults to [2, 3, 5]</param>
    /// <param name="maxN">The largest value of n expected to be needed; used to pre-create the sieve as an optimization.</param>
    public WheelFactorizationSieve(long[]? basis = null, long maxN = -1)
    {
        // Potential improvement: also validate that the contents of basis are actually the first X primes in increasing order
        if (basis?.Length == 0)
        {
            throw new ArgumentException("Cannot be empty", nameof(basis));
        }

        _basis = basis ?? DefaultBasis;

        if (maxN < 0)
        {
            // Wait until first use to populate primes
            return;
        }

        SieveUtils.ValidateN(maxN);

        if (maxN < _basis.Length)
        {
            return;
        }

        _primesFirstTurn = FindPrimesInFirstTurn(_basis);

        if (maxN < _basis.Length + _primesFirstTurn.Length)
        {
            return;
        }

        _primesAfterFirstTurn = FindPrimesAfterFirstTurn(maxN + 1);
    }

    public long NthPrime(long n)
    {
        SieveUtils.ValidateN(n);

        if (n < _basis.Length)
        {
            return _basis[n];
        }

        if (_primesFirstTurn.Length == 0)
        {
            _primesFirstTurn = FindPrimesInFirstTurn(_basis);
        }

        long adjustedN = n - _basis.Length;
        if (adjustedN < _primesFirstTurn.Length)
        {
            return _primesFirstTurn[adjustedN];
        }

        adjustedN -= _primesFirstTurn.Length;
        if (adjustedN >= _primesAfterFirstTurn.Length)
        {
            // Use double the currently known number of primes to mitigate worst case of NthPrime being called on increasing consecutive values of n
            long primeCount = Math.Max(n + 1, (_basis.Length + _primesFirstTurn.Length + _primesAfterFirstTurn.Length) * 2);
            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            _primesAfterFirstTurn = FindPrimesAfterFirstTurn(primeCount);
        }

        return _primesAfterFirstTurn[adjustedN];
    }

    private static long[] FindPrimesInFirstTurn(long[] basis)
    {
        long basisLeastCommonMultiple = basis[0];
        for (int i = 1; i < basis.Length; i++)
        {
            basisLeastCommonMultiple *= basis[i];
        }

        var compositeFlags = new bool[basisLeastCommonMultiple + 2];
        for (int i = 0; i < basis.Length; i++)
        {
            long prime = basis[i];
            for (long j = prime * 2; j < compositeFlags.Length; j += prime)
            {
                compositeFlags[j] = true;
            }
        }

        var primesFirstTurn = new List<long>();
        for (long i = basis[^1] + 1; i < compositeFlags.Length; i++)
        {
            if (!compositeFlags[i])
            {
                primesFirstTurn.Add(i);
            }
        }

        return primesFirstTurn.ToArray();
    }

    private long[] FindPrimesAfterFirstTurn(long primeCount)
    {
        return [];
    }
}