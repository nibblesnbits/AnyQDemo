# AnyQ Demo

A simple demo console app demonstrating the use of [AnyQ.Queues.Msmq](https://www.nuget.org/packages/AnyQ.Queues.Msmq/) with [AnyQ](https://www.nuget.org/packages/AnyQ/).

## Summary

When run, this demo will begin generating a new job every 3 seconds.  This job will wait 5 seconds and output a message to the console, demonstrating data being sent to MSMQ, received by AnyQ, and passed to a Job Handler to be executed asyncronously.  The `ConsoleStatusProvider` will also output the status of each of the jobs as they are being processed.

This project also contains an example `IRequestSerializer`, the [JsonRequestSerializer](AnyQDemo/JsonRequestSerializer.cs).  This class can be used in conjunction with [AnyQ.Formatters.Json](https://www.nuget.org/packages/AnyQ.Formatters.Json/) in any project.

## Building and Running

With Visual Studio 2012 or higher, simply open `AnyQDemo.sln` or `AnyQDemo.Advanced.sln` and press F5 to run.

> If you have Visual Studio 2017 installed, you may run the demo in one step via the `rundemo-vs2017.bat` file.
> To demo consuming a queue from multiple processes, run `runmultidemo-vs2017.bat`.
>
> The `rundemo-advanced-vs2017.bat` file will excute a demo of the more advanced features of AnyQ.

## Notes

You may pass the `/clear` option to the exe to purge the queue on start.
