using MediatR;

namespace BCImplementatie.Application.Commands.StartGebruik
{
    public record StopGebruikCommand(Guid Id) : IRequest;
}

