﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = null) where TValue : class
        {
            if (key == null) return defaultValue;
            return dict.TryGetValue(key, out var t) ? t : defaultValue;
        }
    }
}
