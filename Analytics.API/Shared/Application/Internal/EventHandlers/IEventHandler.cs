using Cortex.Mediator.Notifications;
using OsitoPolar.Analytics.Service.Shared.Domain.Model.Events;

namespace OsitoPolar.Analytics.Service.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    
}