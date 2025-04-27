using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models.DTOs.Product;

namespace EcommerceAPI.Models.DTOs.OrderDetail
{
    public class OrderDetailDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = null!;

        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
    }
}
