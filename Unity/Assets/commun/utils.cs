using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spelunca
{
    public static class Utils
    {
        /// <summary>
        ///  Add to Collection together
        /// </summary>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }
    }
}
