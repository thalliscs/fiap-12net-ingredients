using AutoMapper;
using GeekBurger.LabelLocader.Contract.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurguer.Ingredients.Service
{
    public class LabelImageAddedService : ILabelImageAddedService
    {
        private IQueueClient _queueClient;
        private IConfiguration _configuration;
        private IMapper _mapper;
        private string SubscriptionName = "LabelImageAdded";

        public LabelImageAddedService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;

            //var serviceBusNamespace = _configuration.GetServiceBusNamespace();
            //var queue = serviceBusNamespace.Topics.GetByName(SubscriptionName);
            //if (!queue.Subscriptions.List().Any(subscription => subscription.Name.Equals(
            //    SubscriptionName, StringComparison.InvariantCultureIgnoreCase)))
            //    queue.Subscriptions.Define(SubscriptionName).Create();        }

        public void ReceiveAsync()
        {
            var config = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            _queueClient = new QueueClient(config.ConnectionString, SubscriptionName, ReceiveMode.PeekLock);

            var handlerOptions = new MessageHandlerOptions(ExceptionHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 3
            };

            _queueClient.RegisterMessageHandler(MessageHandler, handlerOptions);
        }

        private Task ExceptionHandler(ExceptionReceivedEventArgs arg)
        {
            var context = arg.ExceptionReceivedContext;
            return Task.CompletedTask;
        }

        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            var labelImageAddedString = Encoding.UTF8.GetString(message.Body);
            var labelImageAdded = JsonConvert.DeserializeObject<Produto>(labelImageAddedString);

            //TODO: MergeProductsAndIngredients

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
