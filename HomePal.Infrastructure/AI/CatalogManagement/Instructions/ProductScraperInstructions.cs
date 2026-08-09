namespace HomePal.Infrastructure.AI.CatalogManagement.Instructions;

public static class ProductScraperInstructions
{
    public const string SystemInstructions = "You are an AI assistant that extracts food product offers from promotional images.";

    public static string BuildPrompt(string categoriesFormatted, string unitsFormatted, string culture)
    {
        return $$"""
            Analyze the provided image of a supermarket promotional flyer, ad, or post.

            Target Language/Culture: {{culture}}

            Available Product Categories in Database:
            {{categoriesFormatted}}

            Available Measuring Units in Database:
            {{unitsFormatted}}

            SCOPE — INCLUDE ONLY FOOD AND COOKING INGREDIENTS:
            Include items a person buys to cook or prepare food/beverages at home (meat, poultry, seafood, dairy, produce, grains, bakery, canned goods, spices, oils, condiments).
            Skip non-food items (cleaning supplies, electronics, cookware, clothing, cosmetics).

            POST CAPTION & OCR CONTEXT:
            If a post caption or image OCR text is provided in the user prompt below, actively use it as supplementary context to extract or refine missing product names, prices, quantities, or descriptions that may be truncated or blurry in the image.

            FOR EACH QUALIFYING PRODUCT, EXTRACT:
            - Name: Product name in English.
            - NameAr: Product name in Arabic.
            - Description: Short product description in English (if shown).
            - DescriptionAr: Short product description in Arabic (if shown).
            - Quantity: Numeric size/weight/volume shown (e.g. 1.0, 500.0, 2.5); default 1.0 if not shown.
            - UnitName: Closest matching Measuring Unit Name from the database list above; null if unmapped.
            - CategoryName: Closest matching Product Category Name from the database list above; null if unmapped.
            - OriginalPrice: Price before discount (numeric only); null if not shown.
            - DiscountedPrice: The offer / current price (numeric only); null if not shown.
            - ValidFrom: Offer start date in yyyy-MM-dd format (Egypt Local Time). Note: Egyptian flyers often use DD/MM/YYYY (e.g., 01/08/2026 = 2026-08-01); null if not specified.
            - ValidTo: Offer expiration / end date in yyyy-MM-dd format (Egypt Local Time). Note: Egyptian flyers often use DD/MM/YYYY (e.g., 15/08/2026 = 2026-08-15); null if not specified.
            - BoundingBox: Location of this product's price/label area within the image as FRACTIONS strictly between 0.0 and 1.0:
              - X: left edge fraction (0.0 to 1.0)
              - Y: top edge fraction (0.0 to 1.0)
              - Width: box width fraction (0.0 to 1.0)
              - Height: box height fraction (0.0 to 1.0)

            OUTPUT FORMAT:
            Output ONLY a valid JSON object matching this schema exactly, with no additional commentary:
            {
              "products": [
                {
                  "name": "Product Name EN",
                  "nameAr": "اسم المنتج بالعربية",
                  "description": "Description EN",
                  "descriptionAr": "الوصف بالعربية",
                  "quantity": 1.0,
                  "unitName": "Unit Name",
                  "categoryName": "Category Name",
                  "originalPrice": 100.0,
                  "discountedPrice": 75.0,
                  "validFrom": "2026-08-01",
                  "validTo": "2026-08-15",
                  "boundingBox": {
                    "x": 0.1,
                    "y": 0.2,
                    "width": 0.3,
                    "height": 0.4
                  }
                }
              ]
            }
            """;
    }
}
