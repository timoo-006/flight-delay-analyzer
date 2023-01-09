Feature: FlightAware
The User enters a flight route and gets all the flights on this route that landed in the last 24 hours. Including the delay or if the flight is early.


Scenario: Get Flights on a Route
    Given the user enters a flight route
    When the flights on this route are requested
    Then the user gets all the flights on this route that landed in the last 24 hours. Including the delay or if the flight is early.
    Then when no flights were found on this route, the user gets a message that no flights were found on this route.