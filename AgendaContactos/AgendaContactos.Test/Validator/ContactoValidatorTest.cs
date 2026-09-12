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
            
            // arrange
            res.IsSuccess.Should().BeTrue();
        }
        
        // casos correctos forzando varios ejemplos por campo, rozando valores límite
        [Test]
        [TestCase("+34123456789")]
        [TestCase("123 456 789")]
        public void Validar_MatriculaValida_RetornaSuccess(string telefono) {
            
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
            
            // arrange
            res.IsSuccess.Should().BeTrue();
        }
    }
    
    /// <summary>
    /// Almacena validaciones a contactos inválidos
    /// </summary>
    [TestFixture] public class CasosIncorrectos {
        
    }
}