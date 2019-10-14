using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwinCAT.JsonExtension.Tests
{
    public class CalculatorTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 10 };
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new DateTime(2019, 11, 26, 14, 26, 18) };
            yield return new object[] { new DateTime(2019, 10, 31, 22, 10, 12) };
            yield return new object[] { DateTime.Now.Date };
            yield return new object[] { DateTime.Now.Date - TimeSpan.FromDays(10) };
            yield return new object[] { TimeSpan.FromMilliseconds(638787) };
            yield return new object[] { TimeSpan.FromMilliseconds(538787) };
            yield return new object[] { TimeSpan.FromMilliseconds(832092) };
            yield return new object[] { TimeSpan.FromMilliseconds(732092) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}