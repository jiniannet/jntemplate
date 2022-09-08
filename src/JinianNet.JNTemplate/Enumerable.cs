
#if NF20
using System.Collections.Generic;

namespace System.Linq.Expressions
{

}

namespace System.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) return null;
            var list = new List<TResult>();
            foreach (TSource t in source)
                list.Add(selector(t));
            return list.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) return null;
            var list = new List<TSource>();
            foreach (TSource t in source)
                list.Add(t);
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source != null)
            {
                foreach (TSource t in source)
                    return t;
            }
            return default(TSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource FirstOrDefault<TSource>(this List<TSource> source)
        {
            if (source != null)
            {
                return source[0];
            }
            return default(TSource);
        }
    }

}
#endif