# Advanced AnyQ Demo

This project demonstrates one possible approach to adding more resiliency to job processing.

## The `ResilientJobHandler` Classes

The `ResilientJobHandler` class and it's descendants in the project demonstrate a way to handle automatic but conditional redirects
to "failure queues" in order to retry jobs.  The `ResilientJobHandler` class itself simply adds the abstract `CanRetry()` method
needed by it's children.  

## TODO
