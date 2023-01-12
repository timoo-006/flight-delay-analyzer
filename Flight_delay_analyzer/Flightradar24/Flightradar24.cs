using System.Drawing;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;

namespace Flight_delay_analyzer.Flightradar24;

public class Flightradar24
{
    private readonly IWebDriver _driver;

    public Flightradar24(/*string originAirport, string destinationAirport, */ IWebDriver driver)
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

        // Set the window size to square
        _driver.Manage().Window.Size = new Size(1000, 1000);
    }

    // Property to get the list of delayed flights
    public List<Flight> DelayedFlights { get; } = new();

    public void EnterOriginAndDestinationAirport(string originAirport, string destinationAirport)
    {
        var searchAirportFrom = _driver.FindElement(By.Id("searchAirportFrom"));
        searchAirportFrom.Click();
        searchAirportFrom.SendKeys(originAirport + Keys.Down + Keys.Enter);
        
        var searchAirportTo = _driver.FindElement(By.Id("searchAirportTo"));
        searchAirportFrom.Click();
        searchAirportFrom.SendKeys(destinationAirport + Keys.Down + Keys.Enter);
    }
    
    public void GetFlights(string originAirport, string destinationAirport)
    {
        EnterOriginAndDestinationAirport(originAirport, destinationAirport);
        for (int i = 1; i > 0; i++)
        {
            try
            {
                _driver.FindElement(By.XPath("/html/body/div[6]/div/section/article/section/div[3]/div[1]/div[2]/span[2]/span/div/span/div[i]")).Click();
                
            }
            catch (Exception e)
            {
                i = 0;
            }
        }

        var flightRows = GetFlightsFromHtml();

        // Check if there are any flights
        if (flightRows.Count == 0)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("No flights found, maybe the airport codes are wrong? Or there are no flights today?");
            Console.WriteLine("========================================\n");
            return;
        }

        var flights = flightRows.Select(flightRow => flightRow.FindElements(By.TagName("td")).ToList()).Select(flightData => flightData[1].FindElement(By.TagName("a")).GetAttribute("href")).ToList();

        // Get the delay of each flight
        foreach (var flight in flights) GetFlightDelay(flight);

        _driver.Close();
    }

    public void GetDelay()
    {
        
    }

    public void GetFlightDelay(string flight)
    {
        var delay = "";
        var flightNumber = "";

        // Open the flight page
        _driver.Navigate().GoToUrl(flight);

        // Wait for the page to load
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        try
        {
            // Get the delay element
            delay = _driver.FindElement(By.XPath("//div[@class='flightPageDestinationDelayStatus']/span")).Text;

            // Find the flight number
            flightNumber = _driver.FindElements(By.TagName("h1")).First().Text;
        }
        catch (Exception e)
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine("Could not get the delay of the flight: " + flightNumber + " (" + flight + ")");
            Console.WriteLine("Error: " + e);
            Console.WriteLine("========================================\n");

            return;
        }

        DelayedFlights.Add(new Flight(flightNumber, delay));
    }

    public void FilterFlights()
    {
        // Close the Cookie popup.
        _driver.FindElement(By.Id("cookieDisclaimerButtonText")).Click();

        // Open the filter menu
        _driver.FindElement(By.XPath("//div[@id='ffinder-refine']/form/div[2]/a")).Click();

        // Apply the filter for only flights that have already landed.
        _driver.FindElement(By.XPath("//fieldset[@id='Status']/ul/li[div/label[contains(.,'angekommen')]]/div/a"))
            .Click();

        // Apply the filter for only flights that have landed yesterday.
        _driver.FindElement(By.XPath("//fieldset[@id='Arrive']/ul/li[div/label[contains(.,'Gestern')]]/div/a")).Click();
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