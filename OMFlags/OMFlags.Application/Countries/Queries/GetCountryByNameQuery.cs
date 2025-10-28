using MediatR;
using OMFlags.Domain.Entities;

namespace OMFlags.Application.Countries;

public record GetCountryByNameQuery(string Name): IRequest<CountryDetails?>;
