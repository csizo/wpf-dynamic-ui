using System;
using System.Collections.Generic;

// ReSharper disable CheckNamespace
namespace Csizmazia.Discovering
// ReSharper restore CheckNamespace
{
    public static class DictionaryExtension
    {

        /// <summary>
        /// Try get value from dictionary
        /// <para>If key is not present adds initValue callback using the specified key</para>
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="initValue">init value callback</param>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                       Func<TValue> initValue)
        {
            if (initValue == null) throw new ArgumentNullException("initValue");

            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                lock (dictionary)
                {
                    if (!dictionary.TryGetValue(key, out value))
                    {
                        value = initValue();
                        dictionary.Add(key, value);
                    }
                }
            }
            return value;
        }
    }
}