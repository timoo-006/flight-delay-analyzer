using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Flight_delay_analyzer;

public class FlightAware
{
    private readonly IWebDriver _driver;
    
    // Property to get the list of delayed flights
    public List<Flight> DelayedFlights { get; set; } = new();

    public FlightAware(string originAirport, string destinationAirport)
    {
        try
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            _driver = new ChromeDriver();
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "There was an error with Initializing Selenium. Make Sure you have the correct Version of Chrome installed. (" +
                e.Message + ")");
            return;
        }

        try
        {
            // Navigate to the FlightAware website
            _driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport +
                                       "&destination=" + destinationAirport);
        }
        catch (Exception e)
        {
            Console.WriteLine("There was an error with navigating to the FlightAware website. (" + e.Message + ")");
            return;
        }

        // Set the window size to square
        _driver.Manage().Window.Size = new Size(1000, 1000);

        FilterFlights();
    }

    public void GetFlights()
    {
        var flightRows = GetFlightRows();
        var flights = new List<string>();

        foreach (var flightRow in flightRows)
        {
            List<IWebElement> flightData = flightRow.FindElements(By.TagName("td")).ToList();

            // Find the elements that has a anchor tag and get the href attribute
            var flightNumber = flightData[1].FindElement(By.TagName("a")).GetAttribute("href");

            flights.Add(flightNumber);
        }

        // TODO: Remove the Console.WriteLine
        Console.WriteLine("--- Flights ---");
        Console.Write("Total flights: " + flights.Count);
        Console.WriteLine();

        // Get the delay of each flight
        foreach (var flight in flights) GetFlightDelay(flight);

        _driver.Close();
    }

    private void GetFlightDelay(string flight)
    {
        // Open the flight page
        _driver.Navigate().GoToUrl(flight);

        // Wait for the page to load
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));

        // Get the delay element
        var delay = _driver.FindElement(By.XPath("//div[@class='flightPageDestinationDelayStatus']/span")).Text;

        // Find the flight number
        var flightNumber = _driver.FindElements(By.TagName("h1")).First().Text;
        
        // TODO: Remove the Console.WriteLine
        DelayedFlights.Add(new Flight(flightNumber, delay));
        Console.WriteLine(flightNumber + " - " + delay);
    }

    private void FilterFlights()
    {
        // Close the Cookie popup.
        _driver.FindElement(By.Id("cookieDisclaimerButtonText")).Click();

        // Open the filter menu
        _driver.FindElement(By.XPath("//div[@id='ffinder-refine']/form/div[2]/a")).Click();

        // Apply the filter for only flights that have already landed.
        _driver.FindElement(By.XPath("//li[div/label[contains(.,'angekommen')]]/div[2]/a")).Click();

        // Apply the filter for only flights that have landed yesterday.
        _driver.FindElements(By.XPath("//li[div/label[contains(.,'Gestern')]]/div[2]/a"))[1].Click();
    }

    private List<IWebElement> GetFlightRows()
    {
        var table = _driver.FindElement(By.Id("Results"));
        var tbody = table.FindElement(By.TagName("tbody"));
        var rows = tbody.FindElements(By.TagName("tr"));

        // Filter out all empty rows
        return rows.Where(row => row.Text != "").ToList();
    }
}