namespace Flight_delay_analyzer.FlightAware;

public class Flight
{
    public string delay;
    public string flightNumber;

    public Flight(string flightNumber, string delay)
    {
        this.flightNumber = flightNumber;
        this.delay = delay;
    }
}