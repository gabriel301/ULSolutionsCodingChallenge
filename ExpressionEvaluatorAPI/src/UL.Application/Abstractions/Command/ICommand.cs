using MediatR;

namespace UL.Application.Abstractions.Command;
public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
{
}
