﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal static class Tasks
    {
        [Pure]
        public static async Task<bool> ForAll<A, B>(A a, IEnumerable<Func<A, B>> fs, Func<B, bool> pred)
        {
            var tasks = fs.Map(f => Task.Run(() => f(a))).ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                if (!pred(completed.Result))
                {
                    return false;
                }
                tasks.Remove(completed);
            }
            return true;
        }

        [Pure]
        public static async Task<bool> ForAll<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred)
        {
            var tasks = fs.ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                if (!pred(completed.Result))
                {
                    return false;
                }
                tasks.Remove(completed);
            }
            return true;
        }

        [Pure]
        public static async Task<bool> Exists<A, B>(A a, IEnumerable<Func<A, B>> fs, Func<B, bool> pred)
        {
            var tasks = fs.Map(f => Task.Run(() => f(a))).ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                var res = pred(completed.Result);
                if (res)
                {
                    return res;
                }
                tasks.Remove(completed);
            }

            return false;
        }

        [Pure]
        public static async Task<bool> Exists<A>(IEnumerable<Task<A>> fs, Func<A, bool> pred)
        {
            var tasks = fs.ToList();

            while (tasks.Count > 0)
            {
                var completed = await Task.WhenAny(tasks);
                var res = pred(completed.Result);
                if (res)
                {
                    return res;
                }
                tasks.Remove(completed);
            }
            return false;
        }
    }

}
