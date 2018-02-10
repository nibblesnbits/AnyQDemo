# AnyQ Demo

A simple demo console app demonstrating the use of [AnyQ.Queues.Msmq](https://www.nuget.org/packages/AnyQ.Queues.Msmq/) with [AnyQ](https://www.nuget.org/packages/AnyQ/).

## Summary

When run, this demo will output to the console *Hello, AnyQ!* and the begin generating a new job every 3 seconds, demonstrating data being sent to MSMQ, received by AnyQ, and passed to a Job Handler.  The `ConsoleStatusProvider` will also output the status of each of the jobs as they are being processed.

This project also contains an example `IRequestSerializer`, the [JsonRequestSerializer](blob/master/AnyQDemo/JsonRequestSerializer.cs).  This class can be used in conjunction with [AnyQ.Formatters.Json](https://www.nuget.org/packages/AnyQ.Formatters.Json/) in any project.

## Building and Running

> Both this project and AnyQ target .NET 4.5.  Therefore you must have Visual Studio 2012 or higher in order to build and run this project.

> If you have Visual Studio 2017 installed, you may run the demo in one step via the `rundemo-vs2017.bat` file.
> To demo consuming a queue from multiple processes, run `runmultidemo-vs2017.bat`.

With Visual Studio 2012 or higher, simply open `AnyQDemo.sln` and press F5 to run.

## Notes

You may pass the `/clear` option to the command line to purge the queue on start.

## Building and Running

Simply clone this repo, open in Visual Studio 2012 (or newer), and run.
