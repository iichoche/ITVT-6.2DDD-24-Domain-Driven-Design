using MediatR;

namespace BCImplementatie.Application.Commands.CareNeeds
{
    public record DeleteCareNeedCommand(int Id) : IRequest;
}