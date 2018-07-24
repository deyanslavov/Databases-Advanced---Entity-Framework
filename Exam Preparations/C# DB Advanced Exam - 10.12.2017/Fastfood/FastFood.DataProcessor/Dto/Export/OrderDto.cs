namespace FastFood.DataProcessor.Dto.Export
{
    using System.Collections.Generic;

    public class OrderDto
    {
        public string Customer { get; set; }
        
        public ICollection<ItemDto> Items { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
