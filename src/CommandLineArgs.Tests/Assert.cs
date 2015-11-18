using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineArgs.Tests
{
    public static class Assert
    {
        private class TestException : Exception { public TestException(string message) : base(message) { } }

        public static void True(bool condition)
        {
            if (!condition)
            {
                throw new TestException("Assert.True");
            }
        }

        public static void Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
                throw new TestException($"Expected exception of type {typeof(T).FullName}. Nothing thrown.");
            }
            catch (T)
            {
            }
            catch (Exception e)
            {
                throw new TestException($"Expected exception of type {typeof(T).FullName}. Got exception fo type {e.GetType().FullName}");
            }
        }

        public static void Equal(object a, object b)
        {
            if (!a.Equals(b))
            {
                throw new TestException("Objects not equal.");
            }
        }
    }
}
