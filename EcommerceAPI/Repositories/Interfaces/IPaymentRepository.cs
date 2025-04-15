using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> AddPayment(PaymentEntity payment);
        Task<bool> DeletePayment(int id);
        Task<IEnumerable<PaymentEntity>> GetAllPayments();
        Task<PaymentEntity?> GetPaymentById(int id);
        Task<PaymentEntity?> GetPaymentByIdWithOrder(int id);
        Task<IEnumerable<PaymentEntity>> GetPaymentsByMethod(PaymentMethod method);
        Task<IEnumerable<PaymentEntity>> GetPaymentsByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentEntity>> GetPaymentsByStatus(PaymentStatus status);
        Task<IEnumerable<PaymentEntity>> GetPaymentsByUserId(int userId);
        Task<decimal> GetTotalPaymentsByDateRange(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPaymentsByUserId(int userId);
        Task<bool> UpdatePayment(PaymentEntity payment);
        Task<bool> UpdatePaymentStatus(int id, PaymentStatus newStatus);
    }
}