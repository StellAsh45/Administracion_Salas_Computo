using AutoMapper;
using Domain;
using Moq;
using Services;
using Services.Models.ModelosUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ServicesTest
{
    public class AñadirUsuarioTest
    {
        [Fact]
        public async Task AddUsuario_ThrowsWhenEmailExists()
        {
            // Arrange
            var repoMock = new Mock<Infrastructure.Repositories.IUsuarioRepository>();
            var mapperMock = new Mock<IMapper>();
            var usuarioService = new UsuarioService(repoMock.Object, mapperMock.Object);

            var existing = new Usuario { Id = Guid.NewGuid(), Correo = "exist@domain.test" };
            repoMock.Setup(r => r.GetByEmail(It.IsAny<string>())).ReturnsAsync(existing);

            var model = new AñadirModeloUsuario { Correo = "exist@domain.test", Nombre = "X", Contrasena = "p", Rol = "Usuario" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => usuarioService.AddUsuario(model));
        }
    }
}

