using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;

namespace HomePal.Infrastructure.AI.MealPlanning.Tools;

public class CalculatorTools
{
    [Description(
        "Perform mathematical calculations when an exact numerical result is required. " +
        "Supports standard math expressions ('250 * 0.2', '500 + 350', '2000 / 4') and percentage formulas ('15% of 800', '20% * 500')."
    )]
    public double Calculate(
        [Description("The mathematical expression to calculate. Examples: '250 * 0.2', '15% of 800', '500 + 350'.")] string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be empty.", nameof(expression));

        try
        {
            // Pre-process percentage expressions: "X% of Y" → "(X/100) * Y"
            string processed = Regex.Replace(
                expression,
                @"(\d+(?:\.\d+)?)\s*%\s*(?:of\s*)?(\d+(?:\.\d+)?)",
                "($1/100)*$2",
                RegexOptions.IgnoreCase);

            // Standalone "X%" → "(X/100)"
            processed = Regex.Replace(
                processed,
                @"(\d+(?:\.\d+)?)\s*%",
                "($1/100)");

            var result = new DataTable().Compute(processed, null);
            return Convert.ToDouble(result);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid mathematical expression: '{expression}'", nameof(expression), ex);
        }
    }
}
