namespace MultiLoader.ConsoleFacade.ProgressReporter
{
    internal static class Extensions
    {
        /// <summary>
        /// Align to the left, padd with spaces, and truncate any oveflow characters.
        /// </summary>
        internal static string FixLeft(this string src, int len)
        {
            var slen = src.Length;
            if (slen > len) return src.Substring(0, len);
            return string.Format("{0}{1}", src, new string(' ', len-slen));
        }

        internal static int Max(this int src, int max)
        {
            return src > max ? max : src;
        }
    }
}