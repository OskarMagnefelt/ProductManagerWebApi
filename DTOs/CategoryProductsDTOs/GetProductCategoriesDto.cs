public class GetProductCategoriesDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public List<GetProductInCategoryDto> Products { get; set; }
}