using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class CartItemEntity
    {
        public int Id { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public CartEntity Cart { get; set; } = null!;

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;

        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
