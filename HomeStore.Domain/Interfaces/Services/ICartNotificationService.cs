namespace HomeStore.Domain.Interfaces.Services;

public interface ICartNotificationService
{
    Task SendProductRemovedFromCartAsync(List<int> userIds, int productId);
}
