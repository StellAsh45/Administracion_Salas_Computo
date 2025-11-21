using AutoMapper;
using Moq;
using Services;
using Xunit;

namespace ServicesTest
{
    public class AsignarEstadoTest
    {
        [Fact]
        public async Task SetEstado_ThrowsOnEmptyEstado()
        {
            // Arrange
            var repoMock = new Mock<Infrastructure.Repositories.IComputadorRepository>();
            var mapperMock = new Mock<IMapper>();
            var svc = new ComputadorService(repoMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => svc.SetEstado(Guid.NewGuid(), ""));
        }
    }
}

