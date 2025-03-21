using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceAPI.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;


        [Required, ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;


        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
