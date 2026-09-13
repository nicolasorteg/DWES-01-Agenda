using AgendaContactos.Entity;
using AgendaContactos.Mapper;
using AgendaContactos.Model;
using FluentAssertions;

namespace AgendaContactos.Test.Mapper;

/// <summary>
/// Clase que almacena los test del mapper
/// </summary>
[TestFixture]
[TestOf(typeof(ContactoMapper))]
public class ContactoMapperTest {

    [TestFixture] 
    public class CasosCorrectos {

        private Contacto _contacto = null!;
        private ContactoEntity _contactoEntity = null!;

        [SetUp]
        public void SetUp() {
            
            _contacto = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Alias = "Carlitos",
                Telefono = "677412345",
                Email = "carlos@gmail.com"
            };
            
            _contactoEntity = new ContactoEntity {
                Id = 1,
                Nombre = "Carlos",
                Alias = "Carlitos",
                Telefono = "677412345",
                Email = "carlos@gmail.com"
            };
        }

        [Test]
        public void ToModelFromEntity_DeberiaSerSuccess() {
            var res = _contactoEntity.ToModel();

            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Nombre.Should().Be("Carlos");
            res.Alias.Should().Be("Carlitos");
            res.Telefono.Should().Be("677412345");
            res.Email.Should().Be("carlos@gmail.com");
        }
        
        [Test]
        public void ToEntityFromModel_DeberiaSerSuccess() {
            var res = _contacto.ToEntity();

            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Nombre.Should().Be("Carlos");
            res.Alias.Should().Be("Carlitos");
            res.Telefono.Should().Be("677412345");
            res.Email.Should().Be("carlos@gmail.com");
        }
        
        [Test]
        public void ToModelFromEntities_DeberiaSerSuccess() {
            var entities = new List<ContactoEntity> { _contactoEntity, _contactoEntity, _contactoEntity };

            var res = entities.ToModel();

            res.Should().HaveCount(3);
        }
        

    }

}