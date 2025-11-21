using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Services;
using Services.Models.ModelosComputador;
using Services.Models.ModelosUsuario;
using Domain;
using Xunit;

namespace ServicesTest
{
    public class ComputadorUsuarioTests
    {
        [Fact]
        public async Task GetComputadores_ReturnsMappedList()
        {
            // Arrange
            var repoMock = new Mock<Infrastructure.Repositories.IComputadorRepository>();
            var mapperMock = new Mock<IMapper>();

            var domainList = new List<Computador>
            {
                new Computador { Id = Guid.NewGuid(), Nombre = "PC1", Estado = "Disponible" }
            };
            repoMock.Setup(r => r.GetComputadores()).ReturnsAsync(domainList);

            var expected = new List<ModeloComputador>
            {
                new ModeloComputador { Id = domainList[0].Id, Nombre = "PC1", Estado = "Disponible" }
            };
            mapperMock.Setup(m => m.Map<IList<ModeloComputador>>(It.IsAny<IList<Computador>>())).Returns(expected);

            var svc = new ComputadorService(repoMock.Object, mapperMock.Object);

            // Act
            var result = await svc.GetComputadores();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expected[0].Id, result[0].Id);
        }
    }
}
