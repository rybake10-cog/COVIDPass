// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Configuration;
using CovidPass.Core.Models.Config;

namespace CovidPass.Core.External.Utils
{
    public abstract class BaseWcfService
    {
        protected readonly ExternalServicesConfigModel Config;
        private readonly MessageHeader _header;

        protected BaseWcfService(IConfiguration config, MessageHeader header = null)
        {
            Config = config.GetSection("ExternalServices").Get<ExternalServicesConfigModel>();
            _header = header ?? new SecurityHeader(Config.Login, Config.Password);
        }

        protected (Binding binding, EndpointAddress endpoint) CreateClientParameters(string url)
        {
            var endpoint = new EndpointAddress(url);
            var binding = new BasicHttpsBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                SendTimeout = TimeSpan.FromMinutes(3)
            };
            return (binding, endpoint);
        }

        protected void AddRequestHandler<TChannel>(ClientBase<TChannel> client, Action<Message> requestHandler) where TChannel : class
        {
            var messageInspector = new MessageInspector(requestHandler, null);
            client.ChannelFactory.Endpoint.EndpointBehaviors.Add(messageInspector);
        }

        protected void AddResponseHandler<TChannel>(ClientBase<TChannel> client, Action<Message> responseHandler) where TChannel : class
        {
            var messageInspector = new MessageInspector(null, responseHandler);
            client.ChannelFactory.Endpoint.EndpointBehaviors.Add(messageInspector);
        }

        protected void AddAuthHandler<TChannel>(ClientBase<TChannel> client) where TChannel : class
        {
            AddRequestHandler(client, message => message.Headers.Add(_header));
        }
    }
}
