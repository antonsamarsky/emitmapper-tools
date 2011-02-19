using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace EmitMapperTests
{
    class CustomAssert
    {
        public static void AreEqual(ICollection expected, ICollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            IEnumerator enumExpected = expected.GetEnumerator();
            IEnumerator enumActual = actual.GetEnumerator();
            while (enumExpected.MoveNext() && enumActual.MoveNext())
            {
                Assert.AreEqual(enumExpected.Current, enumActual.Current);
            }
        }

		public static void AreEqualEnum<T>(IEnumerable<T> expected, IEnumerable<T> actual)
		{
			Assert.AreEqual(expected.Count(), actual.Count());
			IEnumerator enumExpected = expected.GetEnumerator();
			IEnumerator enumActual = actual.GetEnumerator();
			while (enumExpected.MoveNext() && enumActual.MoveNext())
			{
				Assert.AreEqual(enumExpected.Current, enumActual.Current);
			}
		}
   }
}