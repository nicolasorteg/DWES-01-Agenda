using AgendaContactos.Config;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace AgendaContactos.Test.Config;

[TestFixture]
public class AppConfigTest {
    
    [Test]
    public void Locale_DeberiaRetornarEspana() {
        AppConfig.Locale.Should().NotBeNull();
        AppConfig.Locale.Name.Should().Be("es-ES");
    }

    [Test]
    public void AppName_DeberiaRetornarValorOValorPorDefecto() {
        AppConfig.AppName.Should().NotBeNullOrEmpty();
    }
    
    [Test]
    public void Version_DeberiaRetornarVersionValida() {
        AppConfig.Version.Should().NotBeNullOrEmpty();
    }
    
    [Test]
    public void DropData_DeberiaCoincidirConAppsettings() {
        AppConfig.DropData.Should().BeTrue();
    }

    [Test]
    public void SeedData_DeberiaCoincidirConAppsettings() {
        AppConfig.SeedData.Should().BeTrue();
    }
    
    [Test]
    public void ConnectionString_DeberiaContenerDataSource() {
        AppConfig.ConnectionString.Should().NotBeNullOrEmpty();
        AppConfig.ConnectionString.Should().Contain("Data Source");
    }
    
    [Test]
    public void DataFolder_DeberiaContenerCarpetaData() {
        AppConfig.DataFolder.Should().NotBeNullOrEmpty();
        AppConfig.DataFolder.Should().EndWith("data");
    }

    [Test]
    public void LogMinimumLevel_DeberiaSerDebug() {
        AppConfig.LogMinimumLevel.Should().Be("Debug");
    }

    [Test]
    public void LogFilePath_DeberiaContenerRutaDeLog() {
        AppConfig.LogFilePath.Should().Be("log/log-.txt");
    }

    [Test]
    public void LogRetainedFiles_DeberiaSerCinco() {
        AppConfig.LogRetainedFiles.Should().Be(5);
    }

    [Test]
    public void LogOutputTemplate_NoDeberiaEstarVacio() {
        AppConfig.LogOutputTemplate.Should().NotBeNullOrEmpty();
        AppConfig.LogOutputTemplate.Should().Contain("{Timestamp");
    }
    
    [Test]
    public void CacheSize_DeberiaSerDiez() {
        AppConfig.CacheSize.Should().Be(10);
    }
}