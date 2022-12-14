using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport + "&destination=" + destinationAirport);
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
        
        // Get the delay element
        String delay = driver.FindElement(By.XPath("//div[@class='flightPageDestinationDelayStatus']/span")).Text;
        
        // If the flight is delayed add it to the list
        if (delay != "(pünktlich)" && !delay.Contains("verfrüht"))
        {
            delayedFlights.Add(flight);
            Console.WriteLine(delay);
        }

    }

    private void FilterFlights()
    {
        // Close the Cookie popup.
        driver.FindElement(By.Id("cookieDisclaimerButtonText")).Click();
        
        // Apply the filter for only flights that have already landed.
        driver.FindElement(By.XPath("//div[@id='ffinder-refine']/form/div[2]/a")).Click();
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