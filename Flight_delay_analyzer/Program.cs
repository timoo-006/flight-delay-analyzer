using Flight_delay_analyzer.Storage;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace Flight_delay_analyzer;

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
            var flightAware = new FlightAware.FlightAware(jsonInput.OriginAirport, jsonInput.DestinationAirport, driver);
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
            Console.WriteLine("\n===================================== ");
            Console.WriteLine("A. Start the Program");
            Console.WriteLine("B. Exit the Program"); 
            Console.WriteLine("C. Tutorial (Please read before starting the program)");
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
                case "C":
                    Console.WriteLine("============= Tutorial =============");
                    Console.WriteLine("1. Create a JSON file (readFlight.json) in the bin/Debug/net6.0 folder with the following structure:");
                    Console.WriteLine("[");
                    Console.WriteLine("  {");
                    Console.WriteLine("    \"OriginAirport\": \"MUC\",");
                    Console.WriteLine("    \"DestinationAirport\": \"LHR\",");
                    Console.WriteLine("  }");
                    Console.WriteLine("] (You can add as many routes as you want)");
                    Console.WriteLine("2. Run the program");
                    Console.WriteLine("3. The program will create a JSON file (delayedFlights.json) with the results (also in the bin/Debug/net6.0 folder)");
                    Console.WriteLine("4. The program will also output the highest delay and the lowest delay to the console");
                    Console.WriteLine("If you prefer to use FlightRadar (Doesn't work, because the Website is broken) over FlightAware, change the line 21 in the Program.cs. The same goes for the Unit and Acceptance Tests.");
                    Console.WriteLine("=====================================");
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            } 
        }
    }
}