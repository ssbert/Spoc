﻿using System;
using System.Collections.Generic;

namespace SPOC.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
                return;
            foreach (T obj in items)
                action(obj);
        }
    }
}