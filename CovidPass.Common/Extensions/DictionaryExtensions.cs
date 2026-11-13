// © 2020 Partners HealthCare Technology & Innovation Department
using System.Collections.Generic;

namespace CovidPass.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key, default(TValue));
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue value = defaultValue;
            dictionary?.TryGetValue(key, out value);
            return value;
        }
    }
}
