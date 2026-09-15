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
        var contacto = new Contacto { Nombre = "Ana", Telefono = "612345678", Alias = "ana", Email = "ana@mail.com" };
        var resultado = _repository.Create(contacto);
        resultado.Should().BeTrue();
    }
    
}