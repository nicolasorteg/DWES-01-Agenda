using CSharpFunctionalExtensions;
using FluentAssertions;
using AgendaContactos.Cache.Common;
using AgendaContactos.Errors.Common;
using AgendaContactos.Errors.Contacto;
using AgendaContactos.Model;
using AgendaContactos.Repository.Common;
using AgendaContactos.Service;
using AgendaContactos.Validator.Common;
using Moq;

namespace AgendaContactos.Test.Service;

[TestFixture]
[TestOf(typeof(ContactoService))]
public class ContactoServiceTest {

    private Mock<IContactoRepository> _contactoRepositoryMock;
    private Mock<IValidator<Contacto>> _contactoValidatorMock;
    private Mock<ICache<int, Contacto>> _cacheMock;
    private ContactoService _contactoService;
    private Contacto _contactoPrueba;

    [SetUp]
    public void Setup() {

        _contactoRepositoryMock = new Mock<IContactoRepository>();
        _contactoValidatorMock = new Mock<IValidator<Contacto>>();
        _cacheMock = new Mock<ICache<int, Contacto>>();
        _contactoService = new ContactoService(_contactoRepositoryMock.Object, _contactoValidatorMock.Object, _cacheMock.Object);

        // objeto de prueba
        _contactoPrueba = new Contacto {
            Id = 1,
            Nombre = "Ana",
            Telefono = "612345678",
            Alias = "ana",
            Email = "ana@mail.com"
        };
    }

    [Test]
    public void BuscarContactoPorId_SiEstaEnCache_DeberiaDevolverloSinLlamarAlRepositorio() {
        // arrange
        _cacheMock.Setup(c => c.Get(_contactoPrueba.Id)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.BuscarContactoPorId(_contactoPrueba.Id);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
        _cacheMock.Verify(c => c.Get(_contactoPrueba.Id), Times.Once);
        _contactoRepositoryMock.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void BuscarContactoPorId_SiNoEstaEnCache_DeberiaBuscarEnRepoYGuardarEnCache() {
        // arrange
        _cacheMock.Setup(c => c.Get(_contactoPrueba.Id)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.BuscarContactoPorId(_contactoPrueba.Id);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
        _cacheMock.Verify(c => c.Get(_contactoPrueba.Id), Times.Once);
        _contactoRepositoryMock.Verify(r => r.GetById(_contactoPrueba.Id), Times.Once);
        _cacheMock.Verify(c => c.Add(_contactoPrueba.Id, _contactoPrueba), Times.Once);
    }

    [Test]
    public void BuscarContactoPorId_SiNoExisteEnRepo_DeberiaRetornarNotFound() {
        // arrange
        _cacheMock.Setup(c => c.Get(3)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.GetById(3)).Returns((Contacto?)null);

        // act
        var resultado = _contactoService.BuscarContactoPorId(3);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundById>();
        _cacheMock.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void BuscarContactoPorAlias_SiExiste_DeberiaDevolverContacto() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.BuscarContactoPorAlias(_contactoPrueba.Alias);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
        _contactoRepositoryMock.Verify(r => r.GetByAlias(_contactoPrueba.Alias), Times.Once);
    }

    [Test]
    public void BuscarContactoPorAlias_SiNoExiste_DeberiaRetornarNotFound() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetByAlias("noexiste")).Returns((Contacto?)null);

        // act
        var resultado = _contactoService.BuscarContactoPorAlias("noexiste");

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundByAlias>();
    }

    [Test]
    public void BuscarContactoPorTelefono_SiExiste_DeberiaDevolverContacto() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.BuscarContactoPorTelefono(_contactoPrueba.Telefono);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
    }

    [Test]
    public void BuscarContactoPorTelefono_SiNoExiste_DeberiaRetornarNotFound() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetByTelefono("000000000")).Returns((Contacto?)null);

        // act
        var resultado = _contactoService.BuscarContactoPorTelefono("000000000");

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundByTelefono>();
    }

    [Test]
    public void ObtenerContactosPaginados_DeberiaLlamarAlRepositorioConParametros() {
        // arrange
        var listaContactos = new List<Contacto> { _contactoPrueba };
        _contactoRepositoryMock.Setup(r => r.GetAll(1, 5)).Returns(listaContactos);

        // act
        var resultado = _contactoService.ObtenerContactosPaginados(1, 5);

        // assert
        resultado.Should().BeEquivalentTo(listaContactos);
        _contactoRepositoryMock.Verify(r => r.GetAll(1, 5), Times.Once);
    }

    [Test]
    public void ObtenerContactosPaginados_SiNoHayContactos_DeberiaRetornarListaVacia() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetAll(1, 5)).Returns(new List<Contacto>());

        // act
        var resultado = _contactoService.ObtenerContactosPaginados();

        // assert
        resultado.Should().BeEmpty();
        _contactoRepositoryMock.Verify(r => r.GetAll(1, 5), Times.Once);
    }

    [Test]
    public void CrearContacto_SiPasaTodasLasReglas_DeberiaInsertarYActualizarCache() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.SetupSequence(r => r.GetByAlias(_contactoPrueba.Alias))
            .Returns((Contacto?)null)       // comprobación de duplicado
            .Returns(_contactoPrueba);      // dentro del .Tap tras crear
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.Create(_contactoPrueba)).Returns(true);

        // act
        var resultado = _contactoService.CrearContacto(_contactoPrueba);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
        _contactoRepositoryMock.Verify(r => r.Create(_contactoPrueba), Times.Once);
        _cacheMock.Verify(c => c.Add(_contactoPrueba.Id, _contactoPrueba), Times.Once);
    }

    [Test]
    public void CrearContacto_SiFallaLaValidacion_DeberiaFrenarInmediatamente() {
        // arrange
        var listadoErrores = new List<string>();
        var errores = new ContactoError.Validation(listadoErrores);
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Failure<Contacto, DomainError>(errores));

        // act
        var resultado = _contactoService.CrearContacto(_contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(errores);
        _contactoRepositoryMock.Verify(r => r.GetByTelefono(It.IsAny<string>()), Times.Never);
        _contactoRepositoryMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void CrearContacto_SiTelefonoYaExiste_DeberiaFrenarPorTelefonoRepetido() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.CrearContacto(_contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.TelefonosRepetidos>();
        _contactoRepositoryMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Never);
        _cacheMock.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void CrearContacto_SiAliasYaExiste_DeberiaFrenarPorAliasRepetido() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns(_contactoPrueba);

        // act
        var resultado = _contactoService.CrearContacto(_contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.AliasRepetidos>();
        _contactoRepositoryMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void CrearContacto_SiFallaAlGuardar_DeberiaRetornarValidationError() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns((Contacto?)null);
        _contactoRepositoryMock.Setup(r => r.Create(_contactoPrueba)).Returns(false);

        // act
        var resultado = _contactoService.CrearContacto(_contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.Validation>();
    }

    [Test]
    public void ActualizarContacto_SiExiste_DeberiaModificarYGuardarEnCache() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.Update(_contactoPrueba.Id, _contactoPrueba)).Returns(true);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        _contactoRepositoryMock.Verify(r => r.Update(_contactoPrueba.Id, _contactoPrueba), Times.Once);
        _cacheMock.Verify(c => c.Add(_contactoPrueba.Id, _contactoPrueba), Times.Once);
    }

    [Test]
    public void ActualizarContacto_SiNoExisteEnBd_DeberiaFrenarYRetornarNotFound() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns((Contacto?)null);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundById>();
        _contactoRepositoryMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void ActualizarContacto_SiTelefonoPerteneceAOtroContacto_DeberiaFrenarPorTelefonoRepetido() {
        // arrange
        var otroContacto = _contactoPrueba with { Id = 2 };
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(otroContacto);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.TelefonosRepetidos>();
        _contactoRepositoryMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }
    
    [Test]
    public void ActualizarContacto_SiTelefonoNoPerteneceANadie_DeberiaPermitirActualizar() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns((Contacto?)null); // 👈 rama null
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns((Contacto?)null);       // 👈 rama null
        _contactoRepositoryMock.Setup(r => r.Update(_contactoPrueba.Id, _contactoPrueba)).Returns(true);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        _cacheMock.Verify(c => c.Add(_contactoPrueba.Id, _contactoPrueba), Times.Once);
    }

    [Test]
    public void ActualizarContacto_SiAliasPerteneceAOtroContacto_DeberiaFrenarPorAliasRepetido() {
        // arrange
        var otroContacto = _contactoPrueba with { Id = 2 };
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns(otroContacto);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.AliasRepetidos>();
        _contactoRepositoryMock.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }
    
    [Test]
    public void ActualizarContacto_SiFallaAlActualizarEnRepositorio_DeberiaRetornarNotFound() {
        // arrange
        _contactoValidatorMock.Setup(v => v.Validar(_contactoPrueba)).Returns(Result.Success<Contacto, DomainError>(_contactoPrueba));
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByTelefono(_contactoPrueba.Telefono)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.GetByAlias(_contactoPrueba.Alias)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.Update(_contactoPrueba.Id, _contactoPrueba)).Returns(false);

        // act
        var resultado = _contactoService.ActualizarContacto(_contactoPrueba.Id, _contactoPrueba);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundById>();
        _cacheMock.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Never);
    }

    [Test]
    public void EliminarContacto_SiExiste_DeberiaQuitarDeRepoYDeCache() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.Delete(_contactoPrueba.Id)).Returns(true);

        // act
        var resultado = _contactoService.EliminarContacto(_contactoPrueba.Id);

        // assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().Be(_contactoPrueba);
        _contactoRepositoryMock.Verify(r => r.Delete(_contactoPrueba.Id), Times.Once);
        _cacheMock.Verify(c => c.Remove(_contactoPrueba.Id), Times.Once);
    }

    [Test]
    public void EliminarContacto_SiNoExiste_DeberiaRetornarNotFound() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns((Contacto?)null);

        // act
        var resultado = _contactoService.EliminarContacto(_contactoPrueba.Id);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundById>();
        _cacheMock.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
    }
    
    [Test]
    public void EliminarContacto_SiRepositorioFallaAlEliminar_DeberiaRetornarNotFound() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.GetById(_contactoPrueba.Id)).Returns(_contactoPrueba);
        _contactoRepositoryMock.Setup(r => r.Delete(_contactoPrueba.Id)).Returns(false);

        // act
        var resultado = _contactoService.EliminarContacto(_contactoPrueba.Id);

        // assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<ContactoError.NotFoundById>();
        _cacheMock.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void EliminarTodosLosContactos_SiEsExitoso_DeberiaPurgarLaCache() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.DeleteAll()).Returns(true);

        // act
        var resultado = _contactoService.EliminarTodosLosContactos();

        // assert
        resultado.Should().BeTrue();
        _contactoRepositoryMock.Verify(r => r.DeleteAll(), Times.Once);
        _cacheMock.Verify(c => c.Clear(), Times.Once);
    }

    [Test]
    public void EliminarTodosLosContactos_SiFalla_NoDeberiaLimpiarCache() {
        // arrange
        _contactoRepositoryMock.Setup(r => r.DeleteAll()).Returns(false);

        // act
        var resultado = _contactoService.EliminarTodosLosContactos();

        // assert
        resultado.Should().BeFalse();
        _cacheMock.Verify(c => c.Clear(), Times.Never);
    }
}