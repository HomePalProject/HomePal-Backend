using System.ComponentModel.DataAnnotations;
using HomePal.Shared.Results;

namespace HomePal.Application.Features.PantryManagement.DTOs;

public class BulkAddPantryItemsRequest
{
    [Required(ErrorMessage = ErrorMessages.Validation.Required)]
    public List<CreatePantryItemRequest> Items { get; set; } = new();
}
