using System;
using System.Threading.Tasks;
using Logcast.Recruitment.DataAccess.Repositories;
using Logcast.Recruitment.Shared.Models;

namespace Logcast.Recruitment.Domain.Services
{
    public interface ISubscriptionService
    {
        Task<Guid> RegisterSubscriptionAsync(string name, string emailAddress);
        Task<SubscriberModel> GetSubscriberAsync(Guid subscriberId);
        Task DeleteSubscriberAsync(Guid subscriberId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        //no need async await here, it created redundant state machine...
        public Task<Guid> RegisterSubscriptionAsync(string name, string emailAddress)
        {
            return _subscriptionRepository.RegisterSubscriptionAsync(name, emailAddress);
        }

        public Task<SubscriberModel> GetSubscriberAsync(Guid subscriberId)
        {
            return _subscriptionRepository.GetSubscription(subscriberId);
        }

        public Task DeleteSubscriberAsync(Guid subscriberId)
        {
            return _subscriptionRepository.DeleteSubscription(subscriberId);
        }
    }
}