using MediatR;
using OMFlags.Domain.Entities;

namespace OMFlags.Application.Countries;

public sealed record GetCountryByNameQuery(string Name): IRequest<CountryDetails?>;
