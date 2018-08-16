# How to Implement a New Metric in the .NET Core Performance Monitor

Due to the current implementation of this tool, the addition of another metric's tracking requires changes to each of the three components of the system. This process can be streamlined by following the instructions below for each domain of the application.

## Data Collection

To discover which metrics the monitor can actually trace, a look is required at the _System.Diagnostics_ namespace [here](https://docs.microsoft.com/es-es/dotnet/api/system.diagnostics?view=netcore-2.1) and/or the TraceEvent library [here](https://github.com/Microsoft/perfview/blob/master/documentation/TraceEvent/TraceEventLibrary.md). In this repository's ProgrammerGuide.md there are a few examples of how events can be subscribed to through TraceEvent, for reference.

What's most important to add to the code base in order to support other metrics is an abstraction of the data, which is currently performed through the custom classes that exist in the shared directory. Storing event data into a custom class makes the information associated with an event more readily digestible and more easily accessible.

It's also important to remember that TraceEvent runs in parallel with the rest of the code in the monitor, so any actions performed with the data fetched from TraceEvent must be kept thread-safe. This is currently done by sharing a lock object between the code that interacts with the shared collections that store event instances, and it's recommended that a similar approach is taken with any added metrics.

## Data Presentation

Once the custom class for this metric has been created, create a new Razor page under the Pages/Metrics folder. This will automatically create both a cshtml and cshtml.cs file. To add a link to this Razor page on the sidebar, add the following line within Pages/Shared/_Layout.csthml.

```cs
<li><a asp-page="/Metrics/<your razor page file>"><new metric></a></li>
```

To request data like the other metrics, one can follow the Jit.cshtml.cs file. Within the cshtml.cs file, update the newStamp wihtin the OnGet() method to ensure that you are getting the current data. Use the FetchDataService.getData method and pass in the oldStamp and newStamp arguments to retrieve data and add it to the list of data points. 

To show a table of the data, one can follow the html code in the Jit.cshtml file. If the table is to be updated continuoulsy without requiring the user to hit the refresh button, follow the JS code in Jit.cshtml and use the JS fetch API. If a graph is to be included, follow the JS code in CPU_Memory.cshtml to create a graph that will update continuously.
