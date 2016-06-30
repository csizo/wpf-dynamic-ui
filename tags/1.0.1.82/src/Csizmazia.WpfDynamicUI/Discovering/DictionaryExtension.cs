using System;
using System.Collections.Generic;

namespace Csizmazia.Discovering
{
    public static class DictionaryExtension
    {
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