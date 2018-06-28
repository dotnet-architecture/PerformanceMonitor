# PerformanceMonitor Data Collection
Portion of Performance Monitor responsible for collection of application performance counters

## Architecture Overview
The Performance Monitor application allows .NET Core 2.1 developers to track application performance metrics via a web application. The application consists of three major components: performance data collection, data storage and handling, and data presentation. 

The first step of the process, data collection, is performed by a C# class library function that simply needs to be included in the beginning of the user's application code. The function will trigger application performance reading on the user's machine, and periodically send packets of data to an Azure SQL database. To utilize this service, include the PerfMonitor library and write the following at the start of the tracked application's Main method or equivalent:

```cs
Monitor monitor = new Monitor();
monitor.Record();
```

This will trigger performance metric tracking that is done on the user's machine through a number of channels. The first of these channels is the System.Diagnostics namespace, which is used to fetch the current process to be tracked. This process can then have its CPU and memory usage fetched via the Process.TotalProcessorTime and Process.WorkingSet64 fields. The CPU usage reported by the monitor tool represents the percentage of total CPU on the machine, accounting for the number of logical cores present (which is detected via the System.Environment class).

[//]: # (Description of other data collection would go here)

Data collection is performed roughly every second, by looping through a function that performs a check of the current time via System.DateTime.Now. Each time a piece of data is recorded, a class instance is created for the datapoint (each form of data has its own class, with fields containing relevant information for the data). This class instance is added to a Collection of the data type. Every five seconds, the collected data is aggregated into a Metric_List class instance - which is simply a class whose fields consist of the Collections of each monitored data type. This Metric_List instance is then serialized into a JSON string, and is sent in the body of a POST request to the monitor application's web API via the System.Net.Http.HttpClient class. After the data is sent, the Collections containing the datapoints are cleared and the process begins again.