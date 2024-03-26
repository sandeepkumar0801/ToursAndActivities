using System;
using System.Text.RegularExpressions;

namespace Util
{
    public static class Sanitizer
    {
        private static Regex _tags = new Regex("<[^>]*(>|$)",
           RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static Regex _whitelist = new Regex(@"
        ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
        ^<(b|h)r\s?/?>$",
        RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static Regex _separatorwhitelist = new Regex(@"
        ^<(b|h)r\s?/?>$",
        RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static Regex _whitelist_a = new Regex(@"
        ^<a\s
        href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
        (\stitle=""[^""<>]+"")?\s?>$|
        ^</a>$",
        RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static Regex _whitelist_img = new Regex(@"
        ^<img\s
        src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
        (\swidth=""\d{1,3}"")?
        (\sheight=""\d{1,3}"")?
        (\salt=""[^""<>]*"")?
        (\stitle=""[^""<>]*"")?
        \s?/?>$",
        RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        ///
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string Sanitize(string html)
        {
            if (String.IsNullOrEmpty(html)) return html;

            string tagname;
            Match tag;

            // match every HTML tag in the input
            var tags = _tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(_separatorwhitelist.IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                    System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
                else if (html.StartsWith(tagname))
                    html = html.Remove(tag.Index, tag.Length);
            }
            return html.Replace("<br>", string.Empty).Replace("<br >", string.Empty).Replace("<br />", string.Empty).Replace("<br/>", string.Empty);
        }

        /// <summary>
        /// To senitize the html For Google
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string SanitizeHtmlForGoogle(this string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;
            string acceptableTags = "h1|h2|h3|h4|h5|h6|ul|ol|li|p|div|br|strong|em";
            string stringPattern = @"</?(?(?=" + acceptableTags + @")notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            return Regex.Replace(html, stringPattern, string.Empty);
        }

    }
}