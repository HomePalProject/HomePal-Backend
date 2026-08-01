using System.Globalization;

namespace HomePal.Domain.Common;

public class LocalizedItem
{
    public string Culture { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public LocalizedItem()
    {
    }

    public LocalizedItem(string culture, string value)
    {
        Culture = culture;
        Value = value;
    }
}

public static class LocalizedItemExtensions
{
    public static string Get(this IEnumerable<LocalizedItem>? items, string? culture = null, string fallbackCulture = "en")
    {
        if (items == null)
            return string.Empty;

        var itemList = items.ToList();
        if (itemList.Count == 0)
            return string.Empty;

        var targetCulture = !string.IsNullOrWhiteSpace(culture)
            ? culture
            : CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var match = itemList.FirstOrDefault(x => x.Culture.Equals(targetCulture, StringComparison.OrdinalIgnoreCase));
        if (match != null && !string.IsNullOrWhiteSpace(match.Value))
            return match.Value;

        if (!string.IsNullOrWhiteSpace(culture) && culture.Contains('-'))
        {
            var baseCulture = culture.Split('-')[0];
            var baseMatch = itemList.FirstOrDefault(x => x.Culture.Equals(baseCulture, StringComparison.OrdinalIgnoreCase));
            if (baseMatch != null && !string.IsNullOrWhiteSpace(baseMatch.Value))
                return baseMatch.Value;
        }

        var fallback = itemList.FirstOrDefault(x => x.Culture.Equals(fallbackCulture, StringComparison.OrdinalIgnoreCase));
        if (fallback != null && !string.IsNullOrWhiteSpace(fallback.Value))
            return fallback.Value;

        return itemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Value))?.Value ?? string.Empty;
    }
}

