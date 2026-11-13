// © 2020 Partners HealthCare Technology & Innovation Department
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CovidPass.Common.Models;

namespace CovidPass.Core.Infrastructure.Models
{
    public class Query<T> : IRequest<T>
    {
    }

    public class Command : IRequest
    {
    }

    public class Command<T> : IRequest<T>
    {
    }

    public abstract class QueryHandler<TQuery, TResponse> : BaseHandler, IRequestHandler<TQuery, TResponse> where TQuery : Query<TResponse>
    {
        public Task<TResponse> Handle(TQuery model, CancellationToken cancellationToken)
        {
            return Handle(model);
        }

        public abstract Task<TResponse> Handle(TQuery model);
    }

    public abstract class CommandHandler<TCommand, TResponse> : BaseHandler, IRequestHandler<TCommand, TResponse> where TCommand : Command<TResponse>
    {
        public Task<TResponse> Handle(TCommand model, CancellationToken cancellationToken)
        {
            return Handle(model);
        }

        public abstract Task<TResponse> Handle(TCommand model);
    }

    public abstract class CommandHandler<TCommand> : BaseHandler, IRequestHandler<TCommand> where TCommand : Command
    {
        public async Task<Unit> Handle(TCommand model, CancellationToken cancellationToken)
        {
            await Handle(model);
            return Unit.Value;
        }

        public abstract Task Handle(TCommand model);
    }

    public class BaseHandler
    {
        protected void BadRequest(string message)
        {
            throw new BadRequestException(message);
        }
        protected void NotFound(string message = "Can't find entity")
        {
            throw new NotFoundException(message);
        }
    }
}
