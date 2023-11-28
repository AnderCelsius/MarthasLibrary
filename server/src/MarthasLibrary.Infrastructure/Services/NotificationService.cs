using MarthasLibrary.Core.Events;
using MarthasLibrary.Core.Interfaces;

namespace MarthasLibrary.Infrastructure.Services;

public class NotificationService : INotificationService
{
  public Task SendNotificationAsync(IEvent eventMessage)
  {
    throw new NotImplementedException();
  }
}