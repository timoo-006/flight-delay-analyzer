using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Flight_delay_analyzer;

public class FlightAware
{
    private IWebDriver driver;

    public FlightAware(string originAirport, string destinationAirport)
    {
        driver = new ChromeDriver();
        driver.Navigate().GoToUrl("https://de.flightaware.com/live/findflight?origin=" + originAirport + "&destination=" + destinationAirport);
        FilterOnlyPastFlights(); 
    }

    public void GetFlightData()
    {
        List<IWebElement> flightRows = GetFlights();
        
        foreach (IWebElement flightRow in flightRows)
        {
            List<IWebElement> flightData = flightRow.FindElements(By.TagName("td")).ToList();
            
            // Find the elements that has a anchor tag and get the href attribute.
            string flightNumber = flightData[1].FindElement(By.TagName("a")).GetAttribute("href");
            
            Console.WriteLine(flightNumber);
        }
    }

    private void FilterOnlyPastFlights()
    {
        // Close the Cookie popup.
        driver.FindElement(By.Id("cookieDisclaimerButtonText")).Click();
        
        // Apply the filter for only flights that have already landed.
        driver.FindElement(By.XPath("//div[@id='ffinder-refine']/form/div[2]/a")).Click();
        driver.FindElement(By.XPath("//li[div/label[contains(.,'angekommen')]]/div[2]/a")).Click();
    }

    private List<IWebElement> GetFlights()
    {
        IWebElement table = driver.FindElement(By.Id("Results"));
        IWebElement tbody = table.FindElement(By.TagName("tbody"));
        IList<IWebElement> rows = tbody.FindElements(By.TagName("tr"));
        
        // Filter out all empty rows
        return rows.Where(row => row.Text != "").ToList();
    }
}