# .NET Core PerformanceMonitor

## Architecture Overview

![Architecture Diagram](Architecture.png)

The Performance Monitor application allows .NET Core 2.1 developers to track application performance metrics via a web application. The application consists of three major components: performance data collection, data storage and handling, and data presentation. 

### Data Collection
The first step of the process, data collection, is performed by a C# class library function that simply needs to be included in the beginning of the user's application code. The function will trigger application performance reading on the user's machine, and periodically send packets of data to be presented on the web application. To utilize this service, include the PerfMonitor library and write the following at the start of the tracked application's Main method or equivalent:

```cs
Monitor monitor = new Monitor(String application_name, int sampling_rate, int transmission_rate);
monitor.Record();
```

All arguments for _Monitor_ class instantiation are optional - the default sampling rate is one sample per second and the default transmission rate (rate at which data is sent to the server) is five seconds. If a rate is specified, the arguments should be provided in milliseconds between sample/transmission and the sampling rate value should be smaller than the transmission rate value for expected performance. 

Providing an application name will allow an application with multiple processes to have its processes grouped within the performance monitor's tracking. To do so, simply run performance monitoring for each process simultaneously, with each Monitor instantiation specifying the same application name.

This will trigger performance metric tracking that is done on the user's machine through two channels. The first of these channels is the _System.Diagnostics_ namespace, which is used to fetch information unique to the current process. This data includes CPU and memory usage, which is sampled at the specified or default rate.

The other channel for data collection is the TraceEvent library (repo found here: https://github.com/Microsoft/perfview/tree/master/src/TraceEvent). Using TraceEvent, the monitor can monitor certain exception, GC, contention, JIT, and HTTP request events (request events will only be triggered by interaction with ASP.NET Core applications). Handling events via TraceEvent is not done with a controlled sampling rate, since event responses are triggered live as events are discovered by the event parsers.

### Data Presentation
The web application is built using ASP.NET CORE and Razor Pages to create a dynamic application. To start accessing the data, users must first specify the application and process name that they want to examine (collectively, the application and process are called sessions) on the homepage. The homepage also lists all the sessions that the database has data of. Once the user enters the information, the user is given feedback on whether or not a session with the entered information exists. If that session does not exist, an error is thrown and the user is prompted to re-enter the information. If the session does exist, the user can then use the sidebar to view data gathered from their session.

## Usage Specifics