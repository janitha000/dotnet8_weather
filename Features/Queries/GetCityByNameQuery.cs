using MediatR;

public record GetCityByNameQuery(string Name) : IRequest<City?>;