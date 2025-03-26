using EcommerceAPI.Constants;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> AddPayment(Payment payment);
        Task<bool> DeletePayment(int id);
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<Payment?> GetPaymentById(int id);
        Task<Payment?> GetPaymentByIdWithOrder(int id);
        Task<IEnumerable<Payment>> GetPaymentsByMethod(PaymentMethod method);
        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);
        Task<IEnumerable<Payment>> GetPaymentsByStatus(PaymentStatus status);
        Task<IEnumerable<Payment>> GetPaymentsByUserId(int userId);
        Task<decimal> GetTotalPaymentsByDateRange(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPaymentsByUserId(int userId);
        Task<bool> UpdatePayment(Payment payment);
        Task<bool> UpdatePaymentStatus(int id, PaymentStatus newStatus);
    }
}