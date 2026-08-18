namespace HomePal.Application.Features.Catalog.DTOs;

public class FacebookPostDto
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public string? PostUrl { get; set; }
    public List<FacebookMediaDto>? Media { get; set; }
}

public class FacebookMediaDto
{
    public string? ImgUrl { get; set; }
    public string? OcrText { get; set; }
}
