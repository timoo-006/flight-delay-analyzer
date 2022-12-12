using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Flight_delay_analyzer;

public class FlightAware
{
    private IWebDriver driver;

    public FlightAware(string originAirport, string destinationAirport)
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport + "&destination=" + destinationAirport);
    }

    public List<IWebElement> GetFlights()
    {
        IWebElement table = driver.FindElement(By.Id("Results"));
        IWebElement tbody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tbody.FindElements(By.TagName("tr"));
        
        // Filter out all empty rows
        return rows.Where(row => row.Text != "").ToList();
    }
}