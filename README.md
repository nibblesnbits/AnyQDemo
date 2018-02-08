# AnyQ Demo

A simple demo console app demonstrating the use of [AnyQ.Queues.Msmq](https://www.nuget.org/packages/AnyQ.Queues.Msmq/) with [AnyQ](https://www.nuget.org/packages/AnyQ/).

## Summary

When run, this demo will output to the console *Hello, AnyQ!* and *Hello, Again.*, demonstrating data being sent to MSMQ, received by AnyQ, and passed to a Job Handler.  The `ConsoleStatusProvider` will also output the status of each of the two jobs as they are being processed.

This project also contains an example `IRequestSerializer`, the `JsonRequestSerializer`.  This class can be used in conjunction with [AnyQ.Formatters.Json](https://www.nuget.org/packages/AnyQ.Formatters.Json/) in any project.

## Building and Running

Simply clone this repo, open in Visual Studio 2012 (or newer), and run.
