using MediatR;

namespace UL.Application.Abstractions.Command;
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
where TCommand : ICommand<TResponse>
{
}
