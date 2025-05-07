using Elastic.Clients.Elasticsearch.Mapping;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models.DTOs.Product
{
    public class ProductElasticDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsActive { get; set; }
    }
}
