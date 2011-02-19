using System.Linq;

namespace EmitMapperTests
{
    public static class TestUtils
    {
        public static bool AreEqualArraysUnordered<T>(T[] arr1, T[] arr2)
        {
            if (arr1 == null || arr2 == null)
            {
                return arr1 == null && arr2 == null;
            }
            return arr1.Length == arr2.Length && arr1.All(a1 => arr2.Contains(a1));
        }

        public static bool AreEqualArrays<T>(T[] arr1, T[] arr2)
        {
            if (arr1 == null || arr2 == null)
            {
                return arr1 == null && arr2 == null;
            }
            return arr1.Length == arr2.Length && arr1.Select((a1, i) => arr2[i].Equals(a1)).All(b => b);
        }
    }
}