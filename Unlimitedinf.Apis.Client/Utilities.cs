using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    internal static class U
    {
        /// <summary>
        /// <see cref="Log.Inf(string, bool?, bool?, bool?)"/> with <see cref="Formatting.Indented"/>.
        /// </summary>
        public static void WriteObject(object obj) => Log.Inf(JsonConvert.SerializeObject(obj, Formatting.Indented));

        public static void Exit() => Environment.Exit(0);
    }
}
