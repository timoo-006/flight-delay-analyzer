namespace Flight_delay_analyzer;

public class Flight
{
    public string flightNumber;
    public string delay;

    public Flight(string flightNumber, string delay)
    {
        this.flightNumber = flightNumber;
        this.delay = delay;
    }
}