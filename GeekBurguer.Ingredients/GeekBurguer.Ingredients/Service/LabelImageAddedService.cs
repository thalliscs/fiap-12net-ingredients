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
using System.Collections.Generic;
using GeekBurguer.Ingredients.Model;

namespace GeekBurguer.Ingredients.Service
{
    public class LabelImageAddedService : ILabelImageAddedService
    {
        private ISubscriptionClient _subscriptionClient;
        private IConfiguration _configuration;
        private IMapper _mapper;
        private ILogService _logService;

        private ServiceBusConfiguration _serviceBusConfiguration;
        private string TopicName = "LabelImageAdded";
        private string SubscriptionName = "IngredientsSubscription";

        public LabelImageAddedService(IMapper mapper, ILogService logService)
        {
            _mapper = mapper;
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _serviceBusConfiguration = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            _logService = logService;
            EnsureSubscriptionCreated();        }

        private void EnsureSubscriptionCreated()
        {
            var serviceBusNamespace = _configuration.GetServiceBusNamespace();

            if (!serviceBusNamespace.Topics.List().Any(anyTopic => anyTopic.Name.Equals(TopicName,
                StringComparison.InvariantCultureIgnoreCase)))
            {
                serviceBusNamespace.Topics.Define(TopicName).Create();
            }

            var topic = serviceBusNamespace.Topics.GetByName(TopicName);            if (!topic.Subscriptions.List().Any(subscription => subscription.Name.Equals(SubscriptionName,
                    StringComparison.InvariantCultureIgnoreCase)))
                topic.Subscriptions.Define(SubscriptionName).Create();
        }

        public async Task ReceiveMessages()
        {
            try
            {
                _subscriptionClient = new SubscriptionClient(_serviceBusConfiguration.ConnectionString, TopicName, SubscriptionName);

                _subscriptionClient.RegisterMessageHandler(MessageHandler, 
                    new MessageHandlerOptions(ExceptionHandle) { AutoComplete = true, MaxConcurrentCalls = 3 });
            }
            catch
            {
            }
        }
        private static Task ExceptionHandle(ExceptionReceivedEventArgs arg)
        {
            var context = arg.ExceptionReceivedContext;
            return Task.CompletedTask;
        }

        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            if (_subscriptionClient.IsClosedOrClosing)
                return;

            _logService.SendMessagesAsync("LabelImageAddedService consumindo topico");

            var labelImageAddedString = Encoding.UTF8.GetString(message.Body);
            var responseItens = JsonConvert.DeserializeObject<List<Produto>>(labelImageAddedString);

            foreach (var responseItem in responseItens)
            {
                using (var context = new IngredientsContext())
                {
                    var productRepository = new ProductsRepository(context);
                    var itemsWithIngredient = productRepository.ListProductByIngredientName(responseItem.ItemName);

                    if (itemsWithIngredient.Any())
                    {
                        itemsWithIngredient.ForEach(x =>
                        {
                            x.Ingredients.Clear();
                            x.Ingredients.AddRange(responseItem.Ingredients.Select(ingredient => new Ingredient
                            {
                                ItemId = x.ItemId,
                                Name = ingredient.ToUpper()
                            }));

                            productRepository.Save();
                        });
                    }
                }
            }

            _logService.SendMessagesAsync("LabelImageAddedService consumido com sucesso");

            await Task.CompletedTask;
        }
    }
}
