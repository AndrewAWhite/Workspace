using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Workshop.Collections
{
    public static class LinqExtensions
    {
        //           Misc

        /// <summary>
        /// Performs an action count number of times.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action"></param>
        public static void Do(this int count, Action action)
        {
            for (var i = 0; i < count; i++)
            {
                action();
            }
        }

        //              Generic

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        //               IEnumerable

        /// <summary>
        /// Clumps an enumerable into chunks of a given length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="blockSize">Length of chunks.</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> source, int blockSize)
        {
            var section = new List<T>(blockSize);
            foreach (var item in source)
            {
                section.Add(item);
                if (section.Count != blockSize) continue;
                yield return section;
                section = new List<T>(blockSize);
            }
            if (section.Count > 0)
            {
                yield return section;
            }
        }

        public static IEnumerable<IEnumerable<T>> SplitConsecutive<T>(this IEnumerable<T> source)
        {
            if (!source.Any()) yield break;
            var section = new List<T>();
            T currentVal = source.First();
            foreach (T item in source.Skip(1))
            {
                if (!item.Equals(currentVal) && section.Any())
                {
                    yield return section;
                    section = new List<T>();
                    currentVal = item;
                }
                section.Add(item);
            }
            if (section.Any())
            yield return section;
        }

        public static IEnumerable<Tuple<T,HashSet<T>>> Perturbations<T>(this IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                var set = new HashSet<T>();
                foreach (T otherItem in source.Where(x => !x.Equals(item)))
                {
                    set.Add(otherItem);
                }
                yield return new Tuple<T, HashSet<T>>(item, set);
            }
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T toPrepend)
        {
            yield return toPrepend;
            foreach (T item in source)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, IEnumerable<T> toPrepend)
        {
            foreach (T item in toPrepend)
            {
                yield return item;
            }
            foreach (T item in source)
            {
                yield return item;
            }
        }

        public static void ToFile<T>(this IEnumerable<T> enumerable, string outPath)
        {
            using (FileStream stream = File.OpenWrite(outPath))
            {
                var writer = new StreamWriter(stream);
                foreach (T line in enumerable)
                {
                    writer.WriteLine(line);
                }
                writer.Flush();
            }
        }

        //                   HashSet

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            var set = new HashSet<T>();
            foreach (T item in source)
            {
                set.Add(item);
            }
            return set;
        }

        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> toAdd)
        {
            foreach (T item in toAdd)
            {
                set.Add(item);
            }
        }


        //                  String

        //public static string Restrict(this string s, string startString = "", string endstring = "")
        //{
        //    var startSlength = startString.Length;
        //    if (!s.Contains(startString)) return "";
        //    if (!s.Contains(endstring)) return "";
        //    var startIndex = (startString == "") ? 0 : s.IndexOf(startString, StringComparison.InvariantCulture) + startSlength;
        //    var endIndex = (endstring == "") ? s.Length : s.IndexOf(endstring, startIndex, StringComparison.InvariantCulture);
        //    return string.Join("", s.ToArray().SubArray(startIndex, endIndex - startIndex));
        //}

        public static string Print<TKey, TValue>(this Dictionary<TKey, TValue> line)
        {
            return string.Join("\t", line.Select(x => $"{x.Key}: {x.Value}"));
        }

        //                 Range

        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            var outArr = new T[length];
            Array.Copy(array, startIndex, outArr, 0, length);
            return outArr;
        }

        public static T[] SubArray<T>(this T[] array, int startIndex)
        {
            var length = array.Length - startIndex - 1;
            var outArr = new T[length];
            Array.Copy(array, startIndex, outArr, 0, length);
            return outArr;
        }

        public static T[] SubArray<T>(this IEnumerable<T> enumerable, int startIndex, int length)
        {
            var asArr = enumerable.ToArray();
            if (asArr.Length < length + startIndex)
            {
                throw new ArgumentOutOfRangeException("Supplied enumerable of insufficient length.");
            }
            return asArr.SubArray(startIndex, length);
        }

        public static T[] SubArray<T>(this IEnumerable<T> enumerable, int startIndex)
        {
            var asArr = enumerable.ToArray();
            return asArr.SubArray(startIndex);
        }

        /// <summary>
        /// Returns the last <code>count</code> elements of an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="count">number of elements to return</param>
        /// <returns></returns>
        public static T[] Last<T>(this T[] array, int count)
        {
            var length = array.Length;
            return array.SubArray(length - count - 1, length - 1);
        }

        public static T[] First<T>(this T[] array, int count)
        {
            var length = array.Length;
            return length > count - 2 ? array : array.SubArray(0, count);
        }

        public static T[] Until<T>(this T[] array, int exclude)
        {
            var length = array.Length;
            return exclude < length ? array.SubArray(0, length - exclude) : array;
        }


        //                 Numerical

        public static bool Between(this int n, int n1, int n2)
        {
            return (n >= n1 && n <= n2);
        }

        public static bool Between(this long n, long n1, long n2)
        {
            return (n >= n1 && n <= n2);
        }

        public static bool Between(this double n, double n1, double n2)
        {
            return (n >= n1 && n <= n2);
        }

        public static bool Between(this decimal n, decimal n1, decimal n2)
        {
            return (n >= n1 && n <= n2);
        }


        //               BitArray


        public static int Count(this BitArray b, Func<bool, bool> func)
        {
            var count = 0;
            for (var i = 0; i < b.Length; i++)
            {
                if (!func(b[i])) continue;
                count++;
            }
            return count;
        }
    }
}
