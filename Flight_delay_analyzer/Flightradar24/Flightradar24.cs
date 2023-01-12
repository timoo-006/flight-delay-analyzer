using System.Drawing;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;

namespace Flight_delay_analyzer.Flightradar24;

public class Flightradar24
{
    private readonly IWebDriver _driver;

    public Flightradar24(string originAirport, string destinationAirport, IWebDriver driver)
    {
        _driver = driver;

        try
        {
            // Navigate to the Flightradar24 website
            _driver.Navigate().GoToUrl("https://www.flightradar24.com/data/flights");
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not navigate to Flightradar24 website: " + e);

            // Close the browser
            _driver.Quit();
            Environment.Exit(1);
        }
        // Close the Cookie popup.
        _driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
        
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        EnterOriginAndDestinationAirport(originAirport, destinationAirport);

        // Set the window size to square
        _driver.Manage().Window.Size = new Size(1000, 1000);
    }

    // Property to get the list of delayed flights
    public List<Flight> DelayedFlights { get; } = new();

    public void EnterOriginAndDestinationAirport(string originAirport, string destinationAirport)
    {
        var searchAirportFrom = _driver.FindElement(By.Id("searchAirportFrom"));
        searchAirportFrom.Click();
        searchAirportFrom.SendKeys(originAirport);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        searchAirportFrom.SendKeys(Keys.Down + Keys.Enter);
        
        var searchAirportTo = _driver.FindElement(By.Id("searchAirportTo"));
        searchAirportTo.Click();
        searchAirportTo.SendKeys(destinationAirport);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        searchAirportTo.SendKeys(Keys.Down + Keys.Enter);
    }
    
    public void GetFlights()
    {
        var flightRows = GetFlightsFromHtml();

        // Check if there are any flights
        if (flightRows.Count == 0)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("No flights found, maybe the airport codes are wrong? Or there are no flights today?");
            Console.WriteLine("========================================\n");
            return;
        }

        var flights = flightRows.Select(flightRow => flightRow.FindElements(By.TagName("div")).ToList()).Select(flightData => flightData[1].FindElement(By.TagName("strong")).Text).ToList();

        // Removing duplicate flight numbers
        var flightsNumbers = flights.Distinct().ToList();
        
        // Get the delay of each flight
        foreach (var flight in flightsNumbers) GetFlightDelay(flight);
        
        _driver.Close();
    }

    public void GetFlightDelay(string flight)
    {
        var delay = "";
        var flightNumber = "";
        
        // Open the flight page
        _driver.Navigate().GoToUrl("https://www.flightradar24.com/data/flights/" + flight);

        // Wait for the page to load
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        
        var playButtonRows = GetUrlFromHtml();
        var playButtons = playButtonRows.Select(flightRow => flightRow.FindElements(By.TagName("td")).ToList()).Select(flightData => flightData[1].FindElement(By.TagName("a")).GetAttribute("href")).ToList();

        foreach (var button in playButtons)
        {
            _driver.Navigate().GoToUrl(button);
            try
            {
                // Get the delay element
                delay = _driver.FindElement(By.Id("txt-flight-delay-average")).Text;

                // Find the flight number
                flightNumber = flight;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("Could not get the delay of the flight: " + flightNumber + " (" + flight + ")");
                Console.WriteLine("Error: " + e);
                Console.WriteLine("========================================\n");

                return;
            }
        }
        
        DelayedFlights.Add(new Flight(flightNumber, delay));
    }

    public List<IWebElement> GetFlightsFromHtml()
    {
        var suggestions = _driver.FindElement(By.ClassName("tt-suggestions"));
        var rows = suggestions.FindElements(By.ClassName("tt-suggestion"));
        
        return rows.ToList();
        
    }
    
    public List<IWebElement> GetUrlFromHtml()
    {
        var table = _driver.FindElement(By.Id("tbl-datatable"));
        var tbody = table.FindElement(By.TagName("tbody"));
        var rows = tbody.FindElements(By.TagName("tr"));
        
        return rows.ToList();
    }
}
