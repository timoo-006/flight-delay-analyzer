using Flight_delay_analyzer;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

JSONReadAndWrite storage = new JSONReadAndWrite();
JSONInputs jsonItems = new JSONInputs();
var jsonInputs = storage.ReadFlightsOutFromJSON(jsonItems.OriginAirport, jsonItems.DestinationAirport, jsonItems.dateOfFlight);

foreach (var jsonInput in jsonInputs)
{
    var driver = CreateDriver();
    var flightAware = new FlightAware(jsonInput.OriginAirport, jsonInput.DestinationAirport, driver);
    flightAware.GetFlights();
    storage.StoreFlightsIntoJSON(flightAware.FlightList);
    storage.AnalyzeResults(flightAware.FlightList);
}


IWebDriver CreateDriver()
{
    // Create a new instance of the chrome driver.
    new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
    var options = new ChromeOptions();
    options.AddArgument("--lang=de");
    var chromeDriver = new ChromeDriver(options);
    return chromeDriver;
}