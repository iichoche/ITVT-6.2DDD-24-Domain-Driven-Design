using MediatR;

namespace BCImplementatie.Application.Commands.StartGebruik
{
    public record StartGebruikCommand(int ClientId, Guid ProductItemId) : IRequest<Guid>;
}
