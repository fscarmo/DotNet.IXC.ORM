using System.Globalization;
using System.Text;


namespace DotNet.IXC.ORM.Api;


public static class Utils
{
    public static class Text
    {
        public static string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char c in normalizedString)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLower()
                .Trim();
        }
    }
}
