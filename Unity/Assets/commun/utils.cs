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

        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(sArray[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(sArray[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat)
            );

            return result;
        }
    }
}
