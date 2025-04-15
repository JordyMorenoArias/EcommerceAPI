using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models.Entities
{
    public class OrderDetailEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; } = null!;


        [Required, ForeignKey("Product")]
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;


        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
