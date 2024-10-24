Feature: Dispatcher

  Scenario: Publish an event successfully
    Given a valid event
    When PublishAsync is called
    Then it should publish the event
    And it should log the event information

  Scenario: Publish an event with an exception
    Given a valid event
    And the bus throws an exception
    When PublishAsync is called
    Then it should log the initial event information
    And it should log the error
    And it should throw the exception