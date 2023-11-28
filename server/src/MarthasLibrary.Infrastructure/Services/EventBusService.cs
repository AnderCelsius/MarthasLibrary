using MarthasLibrary.Core.Interfaces;

namespace MarthasLibrary.Infrastructure.Services;

public class EventBusService : IEventBus
{
  // Configuration and connection details would be injected via the constructor

  public Task PublishAsync<T>(T @event) where T : class
  {
    throw new NotImplementedException();
  }
}