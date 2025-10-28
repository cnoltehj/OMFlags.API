using FluentAssertions;
using Moq;
using OMFlags.Application.Countries;
using OMFlags.Domain.Contracts; // ICountryService
using OMFlags.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

public class GetCountriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCountries_OrderedByName()
    {
        // Arrange
        var mock = new Mock<ICountryService>();
        mock.Setup(s => s.GetCountries(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Country { Id = 2, Name = "Zimbabwe", FlagUrl = "https://flagcdn.com/zw.png" },
                new Country { Id = 1, Name = "Albania",  FlagUrl = "https://flagcdn.com/al.png" }
            });

        var handler = new GetCountriesQueryHandler(mock.Object);

        // Act
        var result = await handler.Handle(new GetCountriesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Albania");
        result[1].Name.Should().Be("Zimbabwe");
        mock.Verify(s => s.GetCountries(It.IsAny<CancellationToken>()), Times.Once);
    }
}
