namespace EcommerceAPI.Models.DTOs.OrderDetail
{
    public class StockErrorDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int AvailableStock { get; set; }
        public int RequestedQuantity { get; set; }
        public string Message => $"Insufficient stock for '{ProductName}' (ID: {ProductId}). Available: {AvailableStock}, Requested: {RequestedQuantity}.";
    }
}
