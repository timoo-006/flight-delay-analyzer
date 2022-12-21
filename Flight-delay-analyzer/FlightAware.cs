using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Flight_delay_analyzer;

public class FlightAware : IFlightAware
{
    private readonly IWebDriver _driver;

    public FlightAware(string originAirport, string destinationAirport, IWebDriver driver)
    {
        _driver = driver;

        try
        {
            // Navigate to the FlightAware website
            _driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport +
                                       "&destination=" + destinationAirport);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not navigate to FlightAware website: " + e);

            // Close the browser
            _driver.Quit();
            Environment.Exit(1);
        }

        // Set the window size to square
        _driver.Manage().Window.Size = new Size(1000, 1000);
    }

    // Property to get the list of delayed flights
    public List<Flight> DelayedFlights { get; } = new();

    public void GetFlights()
    {
        // Filter the flights
        try
        {
            FilterFlights();
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not filter the flights: " + e);
            Console.WriteLine(
                "Please check if the origin and destination airports are correct. And if there are any flights " +
                "available for the given airports.");
        }

        var flightRows = GetFlightsFromHtml();

        // Check if there are any flights
        if (flightRows.Count == 0)
        {
            Console.WriteLine("No flights found, maybe the airport codes are wrong? Or there are no flights today?");
            return;
        }

        var flights = new List<string>();

        foreach (var flightRow in flightRows)
        {
            List<IWebElement> flightData = flightRow.FindElements(By.TagName("td")).ToList();

            // Find the elements that has a anchor tag and get the href attribute
            var flightNumber = flightData[1].FindElement(By.TagName("a")).GetAttribute("href");

            flights.Add(flightNumber);
        }

        // Get the delay of each flight
        foreach (var flight in flights) GetFlightDelay(flight);

        _driver.Close();
    }

    public void GetFlightDelay(string flight)
    {
        // Open the flight page
        _driver.Navigate().GoToUrl(flight);

        // Wait for the page to load
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

        // Get the delay element
        var delay = _driver.FindElement(By.XPath("//div[@class='flightPageDestinationDelayStatus']/span")).Text;

        // Find the flight number
        var flightNumber = _driver.FindElements(By.TagName("h1")).First().Text;

        DelayedFlights.Add(new Flight(flightNumber, delay));
    }

    public void FilterFlights()
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

    public List<IWebElement> GetFlightsFromHtml()
    {
        var table = _driver.FindElement(By.Id("Results"));
        var tbody = table.FindElement(By.TagName("tbody"));
        var rows = tbody.FindElements(By.TagName("tr"));

        // Filter out all empty rows
        return rows.Where(row => row.Text != "").ToList();
    }
}