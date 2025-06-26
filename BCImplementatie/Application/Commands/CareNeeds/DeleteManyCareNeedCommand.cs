using MediatR;

namespace BCImplementatie.Application.Commands.CareNeeds
{
    public record DeleteManyCareNeedsCommand(int[] Ids) : IRequest;
}