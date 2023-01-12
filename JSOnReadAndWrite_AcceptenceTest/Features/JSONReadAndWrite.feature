Feature: JSONReadAndWrite

Scenario: Return List of OriginAirport and DestinationAirport
	Given an OriginAirport and DestinationAirport,
	When the method ReadFlightsOutFromJSON(originAirport, destinationAirport, dateOfFlight) is called,
	Then the method should return a list of JSONInputs
	
Scenario: Throw exception because of invalid Inputs
	Given a false OriginAirport and DestinationAirport,
	When the method ReadFlightsOutFromJSONoriginAirport, destinationAirport, dateOfFlight is called,
	Then the method should throw an exception