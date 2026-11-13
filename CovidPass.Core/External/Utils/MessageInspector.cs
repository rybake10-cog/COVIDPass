// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace CovidPass.Core.External.Utils
{
    public class MessageInspector : IClientMessageInspector, IEndpointBehavior
    {
        private readonly Action<Message> _requestHandler;
        private readonly Action<Message> _responseHandler;

        public MessageInspector(Action<Message> requestHandler, Action<Message> responseHandler)
        {
            _requestHandler = requestHandler;
            _responseHandler = responseHandler;
        }

        public void AfterReceiveReply(ref Message response, object correlationState)
        {
            _responseHandler?.Invoke(response);
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            _requestHandler?.Invoke(request);
            return null;
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
