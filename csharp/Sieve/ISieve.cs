namespace Sieve;

public interface ISieve
{
    /// <summary>
    /// Returns the nth prime starting with 0, where n=0 returns 2, n=1 returns 3, etc.
    /// </summary>
    long NthPrime(long n);
}
