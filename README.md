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

The data collected is currently being hosted on a SQL database running through Docker. This allows for local testing of the application. In the future, the database will be moved to AzureSQL. This will allow the final product to run with minimal setup from the user. Startup.cs holds the location of the connection string for the server, and can be changed as necessary.

Entity framework was used to manage the sending and fetching of data from the server. There are object-specific controllers for the fetching of data, and there is an all-purpose controller for sending the data in a single packet. Entity framework largely simplifies querying a SQL server, as no commands need to be written. The primary used POST request is POST/api/v1/General/ALL, which allows for pushing all the currently collected data to the server. The receiving of data is done by the specific page being used. Primarily, /api/v1/CPU/Daterange is being used to receive data from the server for CPU usage information. /api/v1/MEM/Daterange will be used to receive data for memory usage information. Data is sent and received in JSON form.

The web application is built using ASP.NET CORE and Razor Pages to create a dynamic application. For each metric that the Performance Monitor tracks, it has a separate Razor Page that makes the individual Http requests to the server. The Razor Page then communicates with the generic MetricService.cs, which deserializes the JSON objects to objects that correlate to a specific metric type. For now, the data is fetched on the onGet() method of each Razor Page, meaning that the data is fetched and displayed when the page is refreshed. For the future, the application will utilize SignalR so that data can be fetched continuously without requiring the user to refresh the page. 