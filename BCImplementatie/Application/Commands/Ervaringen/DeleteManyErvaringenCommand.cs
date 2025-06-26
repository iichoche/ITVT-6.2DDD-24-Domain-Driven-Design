using MediatR;

namespace BCImplementatie.Application.Commands.Ervaringen
{
    public record DeleteManyErvaringenCommand(int[] Ids) : IRequest;
}