using EcommerceAPI.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models.DTOs.OrderDetail
{
    public class OrderDetailAddDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
