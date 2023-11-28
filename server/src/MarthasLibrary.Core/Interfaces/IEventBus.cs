namespace MarthasLibrary.Core.Interfaces;

public interface IEventBus
{
  Task PublishAsync<T>(T @event) where T : class;
}
