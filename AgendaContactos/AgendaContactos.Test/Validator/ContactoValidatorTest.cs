using AgendaContactos.Errors.Contacto;
using AgendaContactos.Model;
using AgendaContactos.Validator.Contactos;
using FluentAssertions;

namespace AgendaContactos.Test.Validator;

/// <summary>
/// Clase que almacena los test del validador de contactos.
/// Usa patrón AAA.
/// </summary>
[TestFixture]
[TestOf(typeof(ContactoValidator))]
public class ContactoValidatorTest {


    /// <summary>
    /// Almacena validaciones a contactos válidos
    /// </summary>
    [TestFixture] public class CasosCorrectos {

        // instanciacion del validador
        private ContactoValidator _validator;
        [SetUp]
        public void SetUp() {
            _validator = new ContactoValidator();
        }

        [Test]
        public void Validar_ContactoValido_RetornaSuccess() {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Telefono = "123456789",
                Email = "carlitosalc@gmail.com",
                Alias = "Carlitos"
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsSuccess.Should().BeTrue();
        }
        
        // casos correctos forzando varios ejemplos por campo, rozando valores límite
        [Test]
        [TestCase("+34123456789")]
        [TestCase("123 456 789")]
        [TestCase("123-456-789")]
        public void Validar_TelefonoValido_RetornaSuccess(string telefono) {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Telefono = telefono,
                Email = "carlitosalc@gmail.com",
                Alias = "Carlitos"
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsSuccess.Should().BeTrue();
        }
    }
    
    /// <summary>
    /// Almacena validaciones a contactos inválidos
    /// </summary>
    [TestFixture] public class CasosIncorrectos {
        
        // instanciacion del validador
        private ContactoValidator _validator;
        [SetUp]
        public void SetUp() {
            _validator = new ContactoValidator();
        }

        [Test]
        [TestCase("N")]
        [TestCase("NicolasOrtegaFe")]
        public void Validar_NombreInvalido_RetornaFailure(string nombre) {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = nombre,
                Telefono = "123456789",
                Email = "carlitosalc@gmail.com",
                Alias = "Carlitos"
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsFailure.Should().BeTrue();
            
            res.Error.Should().BeOfType<ContactoError.Validation>();
            
            (res.Error as ContactoError.Validation)?.Errores.Should()
                .Contain("El Nombre es obligatorio y debe tener entre 2-14 caracteres.");
        }
        
        [Test]
        [TestCase("")]
        [TestCase("Carlos Alcaraz Garfi")]
        public void Validar_AliasInvalido_RetornaFailure(string alias) {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Telefono = "123456789",
                Email = "carlitosalc@gmail.com",
                Alias = alias
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.Validation>();
            
            (res.Error as ContactoError.Validation)?.Errores.Should()
                .Contain("El Alias es obligatorio y debe tener entre 1-19 caracteres.");
        }
        
        [Test]
        [TestCase("aaaaaaaaaa")]
        [TestCase("12345678")]
        [TestCase("1234567891234561")]
        public void Validar_TelefonoInvalido_RetornaFailure(string telefono) {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Telefono = telefono,
                Email = "carlitosalc@gmail.com",
                Alias = "Carlitos"
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsFailure.Should().BeTrue();
            
            res.Error.Should().BeOfType<ContactoError.Validation>();
            
            (res.Error as ContactoError.Validation)?.Errores.Should()
                .Contain("El Teléfono es obligatorio y solo permite prefijo y números sin - ni caracteres especiales.");
        }
        
        [Test]
        [TestCase("")]
        [TestCase("xxx@xxx")]
        [TestCase("a@.a")]
        public void Validar_EmailInvalido_RetornaFailure(string correo) {
            
            // arrange
            var c = new Contacto {
                Id = 1,
                Nombre = "Carlos",
                Telefono = "123-456-789",
                Email = correo,
                Alias = "Carlitos"
            };
            
            // act
            var res = _validator.Validar(c);
            
            // assert
            res.IsFailure.Should().BeTrue();
            
            res.Error.Should().BeOfType<ContactoError.Validation>();
            
            (res.Error as ContactoError.Validation)?.Errores.Should()
                .Contain("El Email es obligatorio y tiene que seguir el formato 'xxx@xxx.xxx'");
        }
    }
}