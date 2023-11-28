using MarthasLibrary.Core.Events;

namespace MarthasLibrary.Core.Interfaces;

public interface INotificationService
{
  Task SendNotificationAsync(IEvent eventMessage);
}
