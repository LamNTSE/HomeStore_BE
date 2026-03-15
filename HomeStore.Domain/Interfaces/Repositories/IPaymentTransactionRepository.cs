using System.Threading.Tasks;
using HomeStore.Domain.Entities;

namespace HomeStore.Domain.Interfaces.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction);
    }
}