using Flight_delay_analyzer;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

var driver = CreateDriver();

var flightAware = new FlightAware("EGLL", "LSZH", driver);
flightAware.GetFlights();

Storage storage = new Storage();
storage.StoreFlightsIntoJSON();

IWebDriver CreateDriver()
{
    // Create a new instance of the chrome driver.
    new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
    var options = new ChromeOptions();
    options.AddArgument("--lang=de");
    var chromeDriver = new ChromeDriver(options);
    return chromeDriver;
}