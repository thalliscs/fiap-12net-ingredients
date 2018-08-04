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

namespace GeekBurguer.Ingredients.Service
{
    public class LabelImageAddedService : ILabelImageAddedService
    {
        private IQueueClient _queueClient;
        private IConfiguration _configuration;
        private IProductsRepository _productRepository;
        private IMapper _mapper;
        private string SubscriptionName = "LabelImageAdded";

        public LabelImageAddedService(IMapper mapper, IConfiguration configuration,
            IProductsRepository productRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _productRepository = productRepository;

            var serviceBusNamespace = _configuration.GetServiceBusNamespace();
        }

        public void ReceiveAsync()
        {
            var config = _configuration.GetSection("serviceBus").Get<ServiceBusConfiguration>();
            _queueClient = new QueueClient(config.ConnectionString, SubscriptionName, ReceiveMode.PeekLock);

            var handlerOptions = new MessageHandlerOptions(ExceptionHandler)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 3
            };

            //TEST - MergeProductsAndIngredients
            //var produto = new System.Collections.Generic.List<string>();
            //produto.Add("teste01");
            //_productRepository.MergeProductsAndIngredients(new Produto() { ItemName = "beef", Ingredients = produto });

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
            _productRepository.MergeProductsAndIngredients(labelImageAdded);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
