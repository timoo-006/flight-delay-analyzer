using Flight_delay_analyzer;
using Flight_delay_analyzer.FlightAware;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

public static class Program
{
    public static void StoreDelays()
    {
        JSONReadAndWrite storage = new JSONReadAndWrite();
        JSONInputs jsonItems = new JSONInputs();
        var jsonInputs = storage.ReadFlightsOutFromJSON(jsonItems.OriginAirport, jsonItems.DestinationAirport, jsonItems.dateOfFlight);

        foreach (var jsonInput in jsonInputs)
        {
            var driver = CreateDriver();
            var flightAware = new FlightAware(jsonInput.OriginAirport, jsonInput.DestinationAirport, driver);
            flightAware.GetFlights();
            storage.StoreFlightsIntoJSON(flightAware.FlightList);
            storage.AnalyzeResults(flightAware.FlightList, jsonInput.OriginAirport, jsonInput.DestinationAirport, jsonInput.dateOfFlight);
        } 
    }


    static IWebDriver CreateDriver()
    {
        // Create a new instance of the chrome driver.
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        var options = new ChromeOptions();
        options.AddArgument("--lang=de");
        var chromeDriver = new ChromeDriver(options);
        return chromeDriver;
    }   
    
    
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n ===================================== ");
            Console.WriteLine("A. Start the Program");
            Console.WriteLine("B. Exit the Program"); 
            Console.Write("Choose an option: ");

            string? option = Console.ReadLine()?.ToUpper();

            switch (option)
            {
                case "A":
                    Console.WriteLine("Starting the Program...");
                    StoreDelays();
                    break;
                case "B":
                    Console.WriteLine("Exiting the Program...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            } 
        }
    }
}