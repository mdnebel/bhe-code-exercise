# Implementer's Notes

I started by researching how the sieve of Eratosthenes works in order to start my implementation.
After gaining a grasp of the core algorithm, I began working on an initial version using an array 
the size of the max prime value to flag composites and assemble an array of only prime numbers
in order to return the Nth one.

The first challenge I encountered was figuring what to use for the max prime value. I really wanted 
the user of this API not have to care what the maximum prime value could possibly be, but using long.MaxValue 
as the max seemed prohibitively expensive, especially when the primes being required are fairly small.
I spent some time researching formulas for estimating the upper bound of the Nth prime value and found 
one that met my needs given the range of numbers I was working with for the unit tests.

Once I had the first version functional using the upper bound formula and an array of bools for each possible value, 
I decided to start investigating improvements, primarily targeting the large memory cost of allocating such an array.
Based on my research, I updated my initial implementation to skip even values to halve the size of the needed array.

Once I had an implementation of the sieve working by only considering odd values, I proceeded to investigate 
further optimizations using wheel factorization. I was less confident in my understanding of this approach, so I decided to split 
off the wheel factorization implementation into its own class so I could preserve the odds-only approach as-is as a known 
working version of the sieve.

I believe my implementation of the sieve using wheel factorization works, but it executes slower than my odds-only version 
despite using a significantly smaller array. I suspect there is some math involved in traversing the array that can speed it up that I'm missing.
Also, my implementation uses a fixed basis on construction to create the entire list of primes, but the next step in optimization 
would likely be using a recursive chain of ever-increasing wheel sieves to determine the Nth prime.

I have left comments starting with "Potential improvement:" around the code for places where I recognized there
were additional safety or performance improvements to be made, but didn't want to spend the time on them in order to 
focus on the core algorithms.

# BHE Software Engineer Coding Exercise

## The Sieve of Eratosthenes

Prime numbers have many modern day applications and a long history in 
mathematics. Utilizing your own resources, research the sieve of Eratosthenes,
an algorithm for generating prime numbers. Based on your research, implement 
an API that allows the caller to retrieve the Nth prime number.
Some stub code and a test suite have been provided as a convenience. However, 
you are encouraged to deviate from Eratosthenes's algorithm, modify the 
existing functions/methods, or anything else that might showcase your ability; 
provided the following requirements are satisfied.

You must author your work in Go, JavaScript/TypeScript, Python, or C# - all 
other language submissions will be rejected. Stub code has been provided, so 
please choose from one of the provided language stubs that is most 
relevant to your skill set and the position you are applying for.

### Requirements

- Click on the "Use this template" button to create a new GitHub repository, in which you may implement your solution
- The library package provides an API for retrieving the Nth prime number using 0-based indexing where the 0th prime number is 2
- Interviewers must be able to execute a suite of tests
  - Go: `go test ./...`
  - C#: `dotnet test Sieve.Tests`
  - Javascript: `npm run test`
  - Python: `python -m unittest test_sieve.py`
- Your solution is committed to your project's `main` branch, no uncommitted changes or untracked files please
- Submit the link to your public repo for review

### Considerations

You may add more tests or restructure existing tests, but you may NOT change or remove
the existing test outcomes; eg- f(0)=2, f(19)=71, f(99)=541, ..., f(10000000)=179424691 

During the technical interview, your submission will be discussed, and you will be evaluated in the following areas:

- Technical ability
- Communication skills
- Work habits and complementary skills
