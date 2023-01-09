using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flight_delay_analyzer;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using Xunit;

namespace Flight_Delay_Analyzer_Integration_Tests.Steps;


[Binding]
public sealed class FlightAwareStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    private FlightAware _flightAware;
    private bool _wereFlightsFound;

    public FlightAwareStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"the user enters a flight route")]
    public void GivenTheUserEntersAFlightRoute()
    {
        var driver = CreateDriver();
        _flightAware = new FlightAware("JFK", "LAX", driver);
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

    [When(@"the flights on this route are requested")]
    public void WhenTheFlightsOnThisRouteAreRequested()
    {
        _flightAware.GetFlights();

        // Check if flights were found
        if (_flightAware.FlightList.Count == 0)
        {
            _wereFlightsFound = false;
        } else
        {
            _wereFlightsFound = true;
        }
    }

    [Then(@"the user gets all the flights on this route that landed in the last 24 hours\. Including the delay or if the flight is early\.")]
    public void ThenTheUserGetsAllTheFlightsOnThisRouteThatLandedInTheLastHoursIncludingTheDelayOrIfTheFlightIsEarly()
    {
        // Check if flights were found
        if (_wereFlightsFound)
        {
            // Verify that the flights is not empty
            Assert.NotEmpty(_flightAware.FlightList);
        }
    }

    [Then(@"when no flights were found on this route, the user gets a message that no flights were found on this route\.")]
    public void ThenWhenNoFlightsWereFoundOnThisRouteTheUserGetsAMessageThatNoFlightsWereFoundOnThisRoute()
    {
        // Check if flights were found
        if (!_wereFlightsFound)
        {
            // Verify that the flights is empty
            Assert.Empty(_flightAware.FlightList);
        }
    }
}