using Flight_delay_analyzer.Storage;
using FluentAssertions;

namespace JSOnReadAndWrite_AcceptenceTest.Steps;

[Binding]
public sealed class JSOnReadAndWriteStepDefinitions
{

    private JSONReadAndWrite jsonReadAndWriteObject;
    private string originAirport = string.Empty;
    private string destinationAirport = string.Empty;
    private DateTime dateOfFlight = DateTime.Now.AddDays(-1);
    private List<JSONInputs> jsonInputsList;
    private Action invalidMethodArguments;

    [Given(@"an OriginAirport and DestinationAirport")]
    public void GivenAValidJsonFile()
    {
        originAirport = "AMS";
        destinationAirport = "FRA";
    }
    
    [When(@"the method ReadFlightsOutFromJSON\(originAirport, destinationAirport, dateOfFlight\) is called,")]
    public void WhenTheMethodReadFlightsOutFromJsonOriginAirportDestinationAirportDateOfFlightIsCalled()
    {
        jsonReadAndWriteObject.ReadFlightsOutFromJSON(originAirport, destinationAirport, dateOfFlight);
    }

    [Then(@"the method should return a list of JSONInputs")]
    public void ThenTheMethodShouldReturnAListOfJsonInputs()
    {
        jsonInputsList = jsonReadAndWriteObject.ReadFlightsOutFromJSON(originAirport, destinationAirport, dateOfFlight);
    }


    [Given(@"a false OriginAirport and DestinationAirport,")]
    public void GivenAFalseOriginAirportAndDestinationAirport()
    {
        originAirport = "AMSSSDD";
        destinationAirport = "FRASA";
    }

    [When(@"the method ReadFlightsOutFromJSONoriginAirport, destinationAirport, dateOfFlight is called,")]
    public void WhenTheMethodReadFlightsOutFromJsoNoriginAirportDestinationAirportDateOfFlightIsCalled()
    {
        invalidMethodArguments = () => jsonReadAndWriteObject.ReadFlightsOutFromJSON(originAirport, destinationAirport, dateOfFlight);
    }

    [Then(@"the method should throw an exception")]
    public void ThenTheMethodShouldThrowAnException()
    {
        invalidMethodArguments.Should().Throw<Exception>();
    }
    
}