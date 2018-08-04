using AutoMapper;
using GeekBurger.LabelLoader.Contract.Models;
using GeekBurguer.Ingredients.Repository;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace GeekBurguer.Ingredients.Service
{
    public class LabelImageAddedService : ILabelImageAddedService
    {
        private IQueueClient _queueClient;
        private IConfiguration _configuration;
        private IProductsRepository _productRepository;
        private IMapper _mapper;
        private string TopicName = "LabelImageAdded";
        private string SubscriptionName = "IngredientsSubscription";

        public LabelImageAddedService(IMapper mapper, IProductsRepository productRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceBusNamespace = _configuration.GetServiceBusNamespace();

            if (!serviceBusNamespace.Topics.List().Any(anyTopic => anyTopic.Name.Equals(TopicName,
                StringComparison.InvariantCultureIgnoreCase)))
            {
                serviceBusNamespace.Topics.Define(TopicName).Create();
            }

            var topic = serviceBusNamespace.Topics.GetByName(TopicName);            if (!topic.Subscriptions.List().Any(subscription => subscription.Name.Equals(SubscriptionName,
                    StringComparison.InvariantCultureIgnoreCase)))
                topic.Subscriptions.Define(SubscriptionName).Create();        }

        public async void ReceiveMessages()
        {
            var config = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            var subscriptionClient = new SubscriptionClient(config.ConnectionString, TopicName, SubscriptionName);

            //TEST - MergeProductsAndIngredients
            //var produto = new System.Collections.Generic.List<string>();
            //produto.Add("teste01");
            //_productRepository.MergeProductsAndIngredients(new Produto() { ItemName = "beef", Ingredients = produto });
            //produto.Add("teste02");
            //_productRepository.MergeProductsAndIngredients(new Produto() { ItemName = "beef", Ingredients = produto });

            subscriptionClient.RegisterMessageHandler(MessageHandler,
                new MessageHandlerOptions(ExceptionHandle) { AutoComplete = true });
        }
        private static Task ExceptionHandle(ExceptionReceivedEventArgs arg)
        {
            var context = arg.ExceptionReceivedContext;
            return Task.CompletedTask;
        }

        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            var labelImageAddedString = Encoding.UTF8.GetString(message.Body);
            var labelImageAdded = JsonConvert.DeserializeObject<Produto>(labelImageAddedString);

            _productRepository.MergeProductsAndIngredients(labelImageAdded);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
