using System;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// Measures how much an operation allocates on the managed heap.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The library promises that its hot paths - conversion, comparison, arithmetic, parsing and
    /// formatting into a caller buffer - allocate nothing at all. Nothing but a measurement
    /// enforces that: a rewrite which reintroduces an intermediate string or boxes an enumeration
    /// value still passes every functional test.
    /// </para>
    /// <para>
    /// Take care when consuming the result of the operation under test. Passing a measurement to
    /// anything which takes an `object`, `GC.KeepAlive` included, boxes the structure and is
    /// counted as an allocation of the test itself. Accumulate into a field of a value type.
    /// </para>
    /// </remarks>
    internal static class AllocationProbe
    {
        /// <summary>
        /// The number of measured calls.
        /// </summary>
        public const int Iterations = 1000;

        /// <summary>
        /// The number of calls made before the measurement starts.
        /// </summary>
        /// <remarks>
        /// The one-time allocations - the static constructors which build the parse list and
        /// compile the conversion delegates, and the culture caches - must all be done by then.
        /// </remarks>
        private const int WarmUp = 100;

        /// <summary>
        /// Returns the bytes allocated on the current thread by the specified number of calls.
        /// </summary>
        /// <remarks>
        /// The delegate is created by the caller, outside of the measured region. Invoking a
        /// delegate does not allocate, so what is measured is the body alone.
        /// </remarks>
        public static long AllocatedBy(Action action, int iterations = Iterations)
        {
            for (int i = 0; i < WarmUp; i++)
                action();

            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < iterations; i++)
                action();
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }
    }
}
