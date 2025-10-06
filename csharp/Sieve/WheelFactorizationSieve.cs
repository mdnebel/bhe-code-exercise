using System;
using System.Collections.Generic;

namespace Sieve;

public class WheelFactorizationSieve : ISieve
{
    private static readonly long[] DefaultBasis = [2, 3, 5];
    // The first prime numbers uses a basis for filtering out a large number of subsequent composites
    private readonly long[] _basis;
    private readonly long _basisLeastCommonMultiple;
    // The coprime values in the first "turn" of the wheel given the basis above
    private long[] _firstTurn = Array.Empty<long>();
    // The differences between values in the first turn
    private long[] _incrementsFirstTurn = Array.Empty<long>();
    private Dictionary<long, long>? _indicesOfFirstTurnValues;
    // Primes found after the basis
    private long[] _primesAfterBasis = Array.Empty<long>();

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
        _basisLeastCommonMultiple = _basis[0];
        for (int i = 1; i < _basis.Length; i++)
        {
            _basisLeastCommonMultiple *= _basis[i];
        }

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

        _firstTurn = CreateFirstTurn();
        _primesAfterBasis = FindPrimesAfterBasis(maxN + 1);
    }

    public long NthPrime(long n)
    {
        SieveUtils.ValidateN(n);

        if (n < _basis.Length)
        {
            return _basis[n];
        }

        if (_firstTurn.Length == 0)
        {
            _firstTurn = CreateFirstTurn();
        }

        long adjustedN = n - _basis.Length;
        if (adjustedN >= _primesAfterBasis.Length)
        {
            // Use double the currently known number of primes to mitigate worst case of NthPrime being called on increasing consecutive values of n
            long primeCount = Math.Max(n + 1, (_basis.Length + _primesAfterBasis.Length) * 2);
            // Potential improvement: find primes only in the new set of numbers instead of recreating the entire array
            _primesAfterBasis = FindPrimesAfterBasis(primeCount);
        }

        return _primesAfterBasis[adjustedN];
    }

    private long[] CreateFirstTurn()
    {
        var compositeFlags = new bool[_basisLeastCommonMultiple + 2];
        for (int i = 0; i < _basis.Length; i++)
        {
            long prime = _basis[i];
            for (long j = prime * 2; j < compositeFlags.Length; j += prime)
            {
                compositeFlags[j] = true;
            }
        }

        var firstTurn = new List<long>();
        for (long i = _basis[^1] + 1; i < compositeFlags.Length; i++)
        {
            if (!compositeFlags[i])
            {
                firstTurn.Add(i);
            }
        }

        _incrementsFirstTurn = new long[firstTurn.Count];
        _indicesOfFirstTurnValues = new Dictionary<long, long>(firstTurn.Count);
        for (int i = 0; i < firstTurn.Count; i++)
        {
            long value = firstTurn[i];
            long nextValue = i + 1 < firstTurn.Count
                ? firstTurn[i + 1]
                : firstTurn[0] + _basisLeastCommonMultiple;
            _incrementsFirstTurn[i] = nextValue - value;

            _indicesOfFirstTurnValues.Add(value, i);
        }

        return firstTurn.ToArray();
    }

    private long[] FindPrimesAfterBasis(long primeCount)
    {
        long upperBound = SieveUtils.GetUpperBound(primeCount);
        // Each index of compositeFlags represents a value from turning the wheel, starting with the first turn.
        // For example, with a basis of [2,3,5] creating a first turn of [7,11,13,17,19,23,29,31],
        // compositeFlags represents [7,11,13,17,19,23,29,31,37,41,43,47,49,53,59,61,...]
        bool[] compositeFlags;

        checked
        {
            // Checked to detect an overflow from multiplying primeCount by the number of primes in the first turn
            compositeFlags = new bool[(upperBound + 1) * _firstTurn.Length / _basisLeastCommonMultiple];
        }

        var sqrtUpperBound = (long)Math.Sqrt(upperBound);
        long indexOfSqrtUpperBound = ApproximateIndexFromValue(sqrtUpperBound);
        for (long i = 0; i <= indexOfSqrtUpperBound; i++)
        {
            if (compositeFlags[i])
            {
                // If the value at this index is already marked as composite,
                // anything it could identify as composite would already be marked as such.
                continue;
            }

            // Start at the index of the value represented by the square of this starting value,
            // since everything prior to that will already be marked properly.
            long value = GetValueFromIndex(i);
            long compositeValue = value * value;
            long incrementIndex = i % _incrementsFirstTurn.Length;
            while (compositeValue <= upperBound)
            {
                long index = GetIndexFromValue(compositeValue);
                compositeFlags[index] = true;

                // Use the fact that there are consistent increments between each prime in the wheel
                // to calculate the next value that exists in the array
                compositeValue += _incrementsFirstTurn[incrementIndex] * value;
                ++incrementIndex;
                if (incrementIndex >= _incrementsFirstTurn.Length)
                {
                    incrementIndex = 0;
                }
            }
        }

        long primesAfterBasisCount = primeCount - _basis.Length;
        var primes = new long[primesAfterBasisCount];
        long primeIndex = 0;

        for (long i = 0; i < compositeFlags.Length && primeIndex < primesAfterBasisCount; i++)
        {
            if (!compositeFlags[i])
            {
                primes[primeIndex] = GetValueFromIndex(i);
                primeIndex++;
            }
        }

        if (primeIndex != primesAfterBasisCount)
        {
            throw new Exception($"Only {primeIndex + 1} prime values were found but {primeCount} were required. {nameof(upperBound)}: {upperBound}");
        }

        return primes;
    }

    private long GetValueFromIndex(long index) =>
        _firstTurn[index % _firstTurn.Length] + _basisLeastCommonMultiple * (index / _firstTurn.Length);

    private long GetIndexFromValue(long value)
    {
        (long multipleOfLcm, long firstTurnValue) = GetAssociatedValueFromFirstTurn(value);
        // I can't come up with an O(1) math-based way of converting from value back to index in this array,
        // nor can I figure out a way to avoid doing that conversion entirely,
        // so using a dictionary for a backwards look-up at least avoids O(n) time of traversing _primesFirstTurn each usage
        long firstTurnIndex = _indicesOfFirstTurnValues![firstTurnValue];
        long index = firstTurnIndex + _firstTurn.Length * multipleOfLcm;
        return index;
    }

    // This is intended to be used when the value may not be represented by an index in the array,
    // such as when finding the index for the square root of the upper bound,
    // so get the index of the closest relevant value instead
    private long ApproximateIndexFromValue(long value)
    {
        (long multipleOfLcm, long firstTurnValue) = GetAssociatedValueFromFirstTurn(value);
        long index = 0;
        for (long i = 0; i < _firstTurn.Length; i++)
        {
            long firstTurnPrime = _firstTurn[i];
            if (firstTurnPrime == firstTurnValue)
            {
                index = i;
                break;
            }

            if (firstTurnPrime > firstTurnValue)
            {
                if (i > 0)
                {
                    index = i - 1;
                }

                break;
            }
        }

        return index + _firstTurn.Length * multipleOfLcm;
    }

    private (long multipleOfLcm, long firstTurnValue) GetAssociatedValueFromFirstTurn(long value)
    {
        // Subtract 2 from the value when calculating the multiple
        // to account for the fact that the last value in the first turn will always be the least common multiple + 1
        long multipleOfLcm = (value - 2) / _basisLeastCommonMultiple;
        long firstTurnValue = value - (multipleOfLcm * _basisLeastCommonMultiple);
        return (multipleOfLcm, firstTurnValue);
    }
}