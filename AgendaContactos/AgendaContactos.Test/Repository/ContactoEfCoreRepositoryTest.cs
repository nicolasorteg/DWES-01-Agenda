using AgendaContactos.Entity;
using AgendaContactos.Model;
using AgendaContactos.Repository.EfCore;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Test.Repository;

[TestFixture]
[TestOf(typeof(ContactoEfCoreRepository))]
public class ContactoEfCoreRepositoryTest {
    
    private AppDbContext _context;
    private ContactoEfCoreRepository _repository;
    private SqliteConnection _connection;

    [SetUp]
    public void SetUp() {
        _connection = new SqliteConnection("Data Source=:memory:"); // bd en memoria para test  
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new ContactoEfCoreRepository(_context);
    }
    
    [TearDown]
    public void TearDown() {
        _context.Dispose();
        _connection.Dispose(); // al cerrar la conexión la bd desaparece
    }
    
    [Test]
    public void Create_ContactoValido_DevuelveTrue() {
        var contacto = new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" };
        var resultado = _repository.Create(contacto);
        resultado.Should().BeTrue();
    }
    
    [Test]
    public void GetByAlias_AliasExistente_DevuelveContacto() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" });
        var resultado = _repository.GetByAlias("ana");
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Ana");
    }
    
    [Test]
    public void GetByAlias_AliasInexistente_DevuelveNull() {
        var resultado = _repository.GetByAlias("noexiste");
        resultado.Should().BeNull();
    }

    [Test]
    public void CreateAndUpdate_ContactoExistente_DevuelveTrue() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" });
        var resultado = _repository.Update(1        , new Contacto { Nombre = "X", Telefono = "1", Alias = "x", Email = "x@x.com" });
        resultado.Should().BeTrue();
    }
    
    [Test]
    public void Update_ContactoInexistente_DevuelveFalse() {
        var resultado = _repository.Update(999, new Contacto { Nombre = "X", Telefono = "1", Alias = "x", Email = "x@x.com" });
        resultado.Should().BeFalse();
    }
    
    [Test]
    public void Delete_ContactoExistente_DevuelveTrue() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" });
        var creado = _repository.GetByAlias("ana")!;
        var resultado = _repository.Delete(creado.Id);
        resultado.Should().BeTrue();
    }
    
    [Test]
    public void Delete_ContactoInexistente_DevuelveFalse() {
        var resultado = _repository.Delete(999);
        resultado.Should().BeFalse();
    }
    
    [Test]
    public void GetById_IdExistente_DevuelveContacto() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" });
        var creado = _repository.GetByAlias("ana")!;

        var resultado = _repository.GetById(creado.Id);

        resultado.Should().NotBeNull();
        resultado.Nombre.Should().Be("Ana");
    }

    [Test]
    public void GetById_IdInexistente_DevuelveNull() {
        var resultado = _repository.GetById(999);
        resultado.Should().BeNull();
    }

    [Test]
    public void GetByTelefono_TelefonoExistente_DevuelveContacto() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "612345679", Alias = "ana", Email = "ana@mail.com" });

        var resultado = _repository.GetByTelefono("612345679");

        resultado.Should().NotBeNull();
        resultado.Alias.Should().Be("ana");
    }

    [Test]
    public void GetByTelefono_TelefonoInexistente_DevuelveNull() {
        var resultado = _repository.GetByTelefono("000000000");
        resultado.Should().BeNull();
    }

    [Test]
    public void GetAll_ConVariosContactos_DevuelvePaginaCorrecta() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "611111111", Alias = "ana", Email = "ana@mail.com" });
        _repository.Create(new Contacto { Nombre = "Beto", Telefono = "622222222", Alias = "beto", Email = "beto@mail.com" });
        _repository.Create(new Contacto { Nombre = "Carla", Telefono = "633333333", Alias = "carla", Email = "carla@mail.com" });

        var pagina1 = _repository.GetAll(pagina: 1, tamPagina: 2).ToList();
        var pagina2 = _repository.GetAll(pagina: 2, tamPagina: 1).ToList();

        pagina1.Should().HaveCount(2);
        pagina2.Should().HaveCount(1);
    }
    
    [Test]
    public void DeleteAll_ConContactosCreados_EliminaTodos() {
        _repository.Create(new Contacto { Nombre = "Ana", Telefono = "611111111", Alias = "ana", Email = "ana@mail.com" });
        _repository.Create(new Contacto { Nombre = "Beto", Telefono = "622222222", Alias = "beto", Email = "beto@mail.com" });

        var resultado = _repository.DeleteAll();

        resultado.Should().BeTrue();
        _repository.Count().Should().Be(0);
    }
    
    [Test]
    public void GetById_BaseDatosFalla_DevuelveNull() {
        _connection.Close();

        var resultado = _repository.GetById(1);

        resultado.Should().BeNull();
    }
    
    [Test]
    public void GetAll_BaseDatosFalla_DevuelveNull() {
        _connection.Close();

        var resultado = _repository.GetAll();

        resultado.Should().BeEmpty();
    }
    
    [Test]
    public void Create_BaseDatosFalla_DevuelveFalse() {
        _connection.Close();

        var resultado = _repository.Create(new Contacto { Nombre = "Ana", Telefono = "611111111", Alias = "ana", Email = "ana@mail.com" });

        resultado.Should().BeFalse();
    }
    
    [Test]
    public void Update_BaseDatosFalla_DevuelveFalse() {
        _connection.Close();

        var resultado = _repository.Update(1, new Contacto { Nombre = "Ana", Telefono = "611111111", Alias = "ana", Email = "ana@mail.com" });

        resultado.Should().BeFalse();
    }
    
    [Test]
    public void Delete_BaseDatosFalla_DevuelveFalse() {
        _connection.Close();

        var resultado = _repository.Delete(1);

        resultado.Should().BeFalse();
    }
    
    [Test]
    public void DeleteAll_BaseDatosFalla_DevuelveFalse() {
        _connection.Close();

        var resultado = _repository.DeleteAll();

        resultado.Should().BeFalse();
    }
    
    [Test]
    public void Count_BaseDatosFalla_DevuelveCero() {
        _connection.Close();

        var resultado = _repository.Count();

        resultado.Should().Be(0);
    }
    
    [Test]
    public void GetByAlias_BaseDatosFalla_DevuelveNull() {
        _connection.Close();

        var resultado = _repository.GetByAlias("Nick");

        resultado.Should().BeNull();
    }
    
    [Test]
    public void GetByTelefono_BaseDatosFalla_DevuelveNull() {
        _connection.Close();

        var resultado = _repository.GetByTelefono("612345678");

        resultado.Should().BeNull();
    }
}