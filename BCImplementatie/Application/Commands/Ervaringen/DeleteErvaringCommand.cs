using MediatR;

namespace BCImplementatie.Application.Commands.Ervaringen
{
    public record DeleteErvaringCommand(int Id) : IRequest;
}

