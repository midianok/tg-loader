using System;
using System.Collections.Generic;

namespace MultiLoader.Core.Tool
{
    public static class FunctionalExtensions
    {
        public static TResult Tee<TResult>(this TResult obj, Action<TResult> action)
        {
            action(obj);
            return obj;
        }

        public static void Each<TInput>(this IEnumerable<TInput> objs, Action<TInput> action)
        {
            foreach (var obj in objs)
                action(obj);
        }

        public static TResult Map<TInput, TResult>(this TInput obj, Func<TInput, TResult> func) =>
            func(obj);
    }
}
