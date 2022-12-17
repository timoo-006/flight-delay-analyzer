using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Flight_delay_analyzer;

public class FlightAware
{
    private IWebDriver driver;
    private List<String> delayedFlights = new List<String>();
    
    // Property to get the list of delayed flights
    public List<String> DelayedFlights
    {
        get { return delayedFlights; }
    }

    public FlightAware(string originAirport, string destinationAirport)
    {
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        driver = new ChromeDriver();

        // Navigate to the FlightAware website
        driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport + "&destination=" + destinationAirport);
        
        // Set the window size to square
        driver.Manage().Window.Size = new System.Drawing.Size(1000, 1000);

        FilterFlights();
    }

    public void GetFlights()
    {
        List<IWebElement> flightRows = GetFlightRows();
        List<String> flights = new List<String>();

        foreach (IWebElement flightRow in flightRows)
        {
            List<IWebElement> flightData = flightRow.FindElements(By.TagName("td")).ToList();
            
            // Find the elements that has a anchor tag and get the href attribute
            String flightNumber = flightData[1].FindElement(By.TagName("a")).GetAttribute("href");

            flights.Add(flightNumber);
        }
        
        Console.WriteLine("--- Flights ---");
        Console.Write("Total flights: " + flights.Count);
        Console.WriteLine();

        // Get the delay of each flight
        foreach (String flight in flights)
        {
            GetFlightDelay(flight);
        }
        
        driver.Close();
    }
    
    private void GetFlightDelay(String flight)
    {
        // Open the flight page
        driver.Navigate().GoToUrl(flight);
        
        // Wait for the page to load
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
        
        // Get the delay element
        String delay = driver.FindElement(By.XPath("//div[@class='flightPageDestinationDelayStatus']/span")).Text;
        
        // Find the flight number
        String flightNumber = driver.FindElements(By.TagName("h1")).First().Text;
        
        // Cut off the brackets and the Verspätung text
        try
        {
            delay = delay.Substring(1, delay.Length - 2);
            delay = delay.Substring(0, delay.Length - 11);
        }
        catch (Exception e)
        {
            // do nothing
        }
        
        // If the flight is delayed add it to the list
        if (delay != "(pünktlich)" && !delay.Contains("verfrüht"))
        {
            delayedFlights.Add(delay + " / " + flightNumber);
            Console.WriteLine(delay + " / " + flightNumber);
        }
    }

    private void FilterFlights()
    {
        // Close the Cookie popup.
        driver.FindElement(By.Id("cookieDisclaimerButtonText")).Click();
        
        // Open the filter menu
        driver.FindElement(By.XPath("//div[@id='ffinder-refine']/form/div[2]/a")).Click();
        
        // Apply the filter for only flights that have already landed.
        driver.FindElement(By.XPath("//li[div/label[contains(.,'angekommen')]]/div[2]/a")).Click();
        
        // Apply the filter for only flights that have landed yesterday.
        driver.FindElements(By.XPath("//li[div/label[contains(.,'Gestern')]]/div[2]/a"))[1].Click();
    }

    private List<IWebElement> GetFlightRows()
    {
        IWebElement table = driver.FindElement(By.Id("Results"));
        IWebElement tbody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tbody.FindElements(By.TagName("tr"));
        
        // Filter out all empty rows
        return rows.Where(row => row.Text != "").ToList();
    }
}