using MediatR;

namespace BCImplementatie.Application.Commands.StartGebruik
{
    public record DeleteGebruikCommand(Guid Id) : IRequest;
    public record DeleteManyGebruikenCommand(Guid[] Ids) : IRequest;
}