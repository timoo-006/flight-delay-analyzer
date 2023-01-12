using Flight_delay_analyzer.FlightAware;
using FluentAssertions;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Flight_Aware_Unit_Tests;

public class FlightAwareTests : IDisposable
{
    private readonly IWebDriver _driver;
    private FlightAware _flightAware;

    public FlightAwareTests()
    {
        // Set up the Chrome driver and create a new FlightAware object with fake airport codes
        _driver = CreateDriver();
        _flightAware = new FlightAware("JFK", "LHR", _driver);
    }

    // Dispose of the driver when the tests are done
    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void FlightAware_GetFlights_ReturnsListOfFlights()
    {
        // Act
        _flightAware.GetFlights();

        // Assert
        _flightAware.FlightList.Should().NotBeEmpty();
    }

    [Fact]
    public void FlightAware_GetFlights_ReturnsEmptyListWhenNoFlightsAreFound()
    {
        // Arrange
        _flightAware = new FlightAware("LSZH", "LSMM", _driver);

        // Act
        _flightAware.GetFlights();

        // Assert
        _flightAware.FlightList.Should().BeEmpty();
    }

    [Fact]
    public void FlightAware_GetFlightDelay_ReturnsCorrectDelay()
    {
        // Arrange
        const string fakeFlight = "https://flightaware.com/live/flight/BAW182/history/20221220/0425Z/KJFK/EGLL";
        const string expectedDelay = "(4 Stunde(n) 23 Minuten Verspätung)";

        // Act
        _flightAware.GetFlightDelay(fakeFlight);
        var actualDelay = _flightAware.FlightList[0].delay;

        // Assert
        actualDelay.Should().Be(expectedDelay);
    }
    
    [Fact]
    public void FlightAware_GetFlights_CallsFilterFlights()
    {
        // Arrange
        var mockFlightAware = new Mock<IFlightAware>();
        mockFlightAware.Setup(fa => fa.FilterFlights());

        // Act
        mockFlightAware.Object.FilterFlights();
        
        // Assert
        mockFlightAware.Verify(fa => fa.FilterFlights(), Times.Once);
    }

    [Fact]
    public void FlightAware_GetFlights_CallsGetFlightsFromHtml()
    {
        // Arrange
        var mockFlightAware = new Mock<IFlightAware>();
        mockFlightAware.Setup(fa => fa.GetFlightsFromHtml());
        
        // Act
        mockFlightAware.Object.GetFlightsFromHtml();
        
        // Assert
        mockFlightAware.Verify(fa => fa.GetFlightsFromHtml(), Times.Once);
    }
    
    [Fact]
    public void FlightAware_GetFlights_CallsGetFlightDelay()
    {
        // Arrange
        var mockFlightAware = new Mock<IFlightAware>();
        mockFlightAware.Setup(fa => fa.GetFlightDelay(It.IsAny<string>()));
        
        // Act
        mockFlightAware.Object.GetFlightDelay(It.IsAny<string>());
        
        // Assert
        mockFlightAware.Verify(fa => fa.GetFlightDelay(It.IsAny<string>()), Times.AtLeastOnce);
    }

    private static IWebDriver CreateDriver()
    {
        // Create a new instance of the chrome driver.
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        var options = new ChromeOptions();
        options.AddArgument("--lang=de");
        var driver = new ChromeDriver(options);
        return driver;
    }
}