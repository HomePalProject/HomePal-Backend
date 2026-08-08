namespace HomePal.Infrastructure.AI.PantryManagement.Instructions;

public static class PantryAgentInstructions
{
    public const string SystemInstructions = "You are an expert Pantry & Grocery Receipt Scanner Agent for HomePal.";

    public static string BuildPrompt(string unitsFormatted, string categoriesFormatted, DateTime currentDate, string culture)
    {
        return $$"""
            Analyze the provided image, which may be either a photo of physical food items/pantry/fridge/shelves OR a grocery store receipt/invoice/bill.

            Target Language/Culture: {{culture}}

            Available Measuring Units in Database:
            {{unitsFormatted}}

            Available Product Categories in Database:
            {{categoriesFormatted}}

            Instructions:
            1. Determine whether the image is a physical food photo or a store receipt:
               - If it is a photo of food items/pantry: identify each visible food item.
               - If it is a grocery store receipt/invoice: read and extract each purchased item line with its quantity listed on the receipt.
            2. Extract or estimate the quantity for each item (default to 1.0 if not explicitly specified).
            3. Match each item to the closest Measuring Unit ID from the provided units list.
            4. Match each item to the closest Product Category ID from the provided categories list.
            5. Estimate a realistic expiration date starting from today ({{currentDate:yyyy-MM-dd}}).
            6. Provide item names and descriptions in the specified target language/culture: '{{culture}}'.
            7. Output ONLY a valid JSON object matching this schema exactly, with no additional formatting or commentary:
            {
              "items": [
                {
                  "name": "Item name in {{culture}}",
                  "quantity": 1.0,
                  "measuringUnitId": "00000000-0000-0000-0000-000000000000",
                  "measuringUnitName": "Unit Name in {{culture}}",
                  "categoryId": "00000000-0000-0000-0000-000000000000",
                  "categoryName": "Category Name in {{culture}}",
                  "suggestedExpireDate": "2026-08-20T00:00:00Z"
                }
              ]
            }
            """;
    }
}
