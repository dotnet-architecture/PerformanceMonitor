# .NET Core PerformanceMonitor

## Architecture Overview
The Performance Monitor application allows .NET Core 2.1 developers to track application performance metrics via a web application. The application consists of three major components: performance data collection, data storage and handling, and data presentation. 

### Data Collection
The first step of the process, data collection, is performed by a C# class library function that simply needs to be included in the beginning of the user's application code. The function will trigger application performance reading on the user's machine, and periodically send packets of data to an Azure SQL database. To utilize this service, include the PerfMonitor library and write the following at the start of the tracked application's Main method or equivalent:

```cs
Monitor monitor = new Monitor(String application_name, int sampling_rate, int transmission_rate);
monitor.Record();
```

All arguments for _Monitor_ class instantiation are optional - the default sampling rate is one second and the default transmission rate (rate at which data is sent to the server) is five seconds. If a rate is specified, the arguments should be provided in milliseconds and the sampling rate should be lower than the transmission rate for expected performance. Providing an application name will allow an application with multiple processes to have its processes grouped within the performance monitor's tracking. To do so, simply run performance monitoring for each process simultaneously, with each Monitor instantiation specifying the same application name.

This will trigger performance metric tracking that is done on the user's machine through two channels. The first of these channels is the _System.Diagnostics_ namespace, which is used to fetch the current process to be tracked. This process can then have its CPU and memory usage fetched via the _Process.TotalProcessorTime_ and _Process.WorkingSet64_ fields. The CPU usage reported by the monitor tool represents the percentage of total CPU on the machine, accounting for the number of logical cores present (which is detected via the _System.Environment_ class).

The other channel for data collection is the TraceEvent library (repo found here: https://github.com/Microsoft/perfview/tree/master/src/TraceEvent). A new TraceEvent session is initialized upon starting the program, and delegates are added for responses to certain exception, GC, contention, JIT, and HTTP request events (request events will only be triggered by interaction with ASP.NET Core applications). A list of tracked events and how they are collected can be found further on in this document within the technical detail section. Handling events via TraceEvent is not done with a controlled sampling rate, since event responses are triggered live as events are discovered by the event parsers.

Data collection is (by default) performed roughly every second, by looping through a function that performs a check of the current time via _System.DateTime.Now_. Each time a piece of data is recorded, a class instance is created for the datapoint (each form of data has its own class, with fields containing relevant information for the data). This class instance is added to a Collection of the data type. Every five seconds (by default), the collected data is aggregated into a _Metric_List_ class instance - which is simply a class whose fields consist of the Collections of each monitored data type. This _Metric_List_ instance is then serialized into a JSON string, and is sent in the body of a POST request to the monitor application's web API via the _System.Net.Http.HttpClient_ class. After the data is sent, the Collections containing the datapoints are cleared and the process begins again.

### Data Storage
The data collected is currently being hosted on a SQL database running through Docker. This allows for local testing of the application. In the future, the database will be moved to AzureSQL. This will allow the final product to run with minimal setup from the user. Startup.cs holds the location of the connection string for the server, and can be changed as necessary.

Entity framework was used to manage the sending and fetching of data from the server. There are object-specific controllers for the fetching of data, and there is an all-purpose controller for sending the data in a single packet. Entity framework largely simplifies querying a SQL server, as no commands need to be written. The primary used POST request is POST/api/v1/General/ALL, which allows for pushing all the currently collected data to the server. The receiving of data is done by the specific page being used. Primarily, /api/v1/CPU/Daterange is being used to receive data from the server for CPU usage information. /api/v1/MEM/Daterange will be used to receive data for memory usage information. Data is sent and received in JSON form.


### Data Presentation
The web application is built using ASP.NET CORE and Razor Pages to create a dynamic application. For each metric that the Performance Monitor tracks, it has a separate Razor Page that makes the individual Http requests to the server. The Razor Page then communicates with the generic MetricService.cs, which deserializes the JSON objects to objects that correlate to a specific metric type. For now, the data is fetched on the onGet() method of each Razor Page, meaning that the data is fetched and displayed when the page is refreshed. For the future, the application will utilize SignalR so that data can be fetched continuously without requiring the user to refresh the page. 

## Usage Specifics
### Data Collection

#### Testing
The _MonitorTest_ project within the solution contains C# classes that are each used to test a particular type of metric collection. The project's executable file is Program.cs, which contains only a Main method. In order to run a test for a particular metric, uncomment the metric's test line within Main - for example, to test collection of GC events, uncomment the line reading "GCTest.Test();". Then run _MonitorTest_, and GC events will be triggered for debugging. As it stands, only one test file can be run at a time (if multiple lines are uncommented, only the first test to appear in the code will run).

__IMPORTANT__: When running _MonitorTest_, a console window will pop up and display the program's output. In order to safely terminate the program, press Ctrl+C before closing the window. If Ctrl+C is not pressed, the session allowing TraceEvent to run and collect events will not terminate. The next run of _MonitorTest_ would attempt to recreate the same session, and an error would be triggered. If you forget to Ctrl+C and run into this error, open up your machine's terminal or command prompt and run the command "logman stop MySession -ets". This will close the session, and you will be able to run _MonitorTest_ again.

## Functionality Specifics
### Data Collection
The actual code that performs data collection is Monitor.cs, within the PerfMonitor C# class library. It specifies the _Monitor_ class, includes all necessary C# libraries, and contains the functions that perform data collection and interfacing with the other components of the PerformanceMonitor project.

The performance metrics that are monitored by the PerformanceMonitor are as follows:

__1.__ CPU usage
__2.__ Memory usage
__3.__ HTTP requests
  * Method
  * Path
  * Duration
  * Frequency
__4.__ Exceptions
  * Type
  * Frequency
__5.__ Garbage Collection
  * Type
  * Duration
  * Frequency
__6.__ Contention
  * Duration
  * Frequency
__7.__ JIT events
  * Method
  * Count

#### Recording Metrics (System.Diagnostics.Process class)
As mentioned above, CPU and memory usage are fetched via the _System.Diagnostics.Process_ class. A new _Monitor_ class instance must be created for each process that a user would like to monitor, because the data is fetched by first calling Process.GetCurrentProcess();, which will return a _Process_ class instance. This class instance contains a number of fields that are used to track CPU and memory performance. These metrics will be sampled as often as the sampling rate dictates. This is done by looping through a while(true) loop in the Record() function, and comparing a TimeSpan between the last sample and the current time to the sampling rate.

##### CPU
Tracking CPU usage requires a small amount of overhead for calculation and comparison of a few variables. The total time that a machine's logical processors spend running the user's process is described by _Process.TotalProcessorTime_, and this is what is used to calculate the percentage of time that the processor (accounting for all logical cores) has spent running the user's code. The time between samplings in recording using DateTime objects, and the amount of time is multiplied by the number of logical cores (fetched by _System.Environment.ProcessorCount_) to determine the total amount of time that the processors can allocate to work during the interval. These values are used to generate a percentage of time spent running the process.

Like all other metrics that are collected, there is a unique class used by the project components in order to standardize data representation and make JSON (de)serialization easier for data sharing. All classes contain a field for an application name (String that the user can specify), process descriptor (String that's generated from the process name and ID), and timestamp (DateTime specifying the instance the data was collected). The CPU class also contains a "usage" field, which is a double containing the percentage of CPU usage for a given sample. This class, like all other shared classes in the project, exists within the Shared class library.

##### Memory
Memory usage sampling is much more straightforward than CPU sampling - the process' memory usage can be fetched with a single fetching of the process' _WorkingSet64_ property. The memory usage class looks essentially identical to the CPU class, except the usage is stored as a long (the return value of the _WorkingSet64_ property).

#### Recording Metrics (TraceEvent library)
The rest of the collected metrics are discovered via the TraceEvent library. As mentioned before, these events are not sampled at regular intervals. Instead, delegate actions are set up in response to the discovery of events as they occur, through an independent task established upon calling _Monitor.Record()_. It's worth noting that due to data structure locking, all event tracing is paused for the duration of HTTP requests to the database which stores the performance metrics. This means that certain events, and certain CPU/memory usage data points, may go missing periodically (typically around startup).

The following event collection descriptions will mention terms and processes specific to TraceEvent, so  please refer to the TraceEvent programmer's guide (https://github.com/Microsoft/dotnet-samples/blob/master/Microsoft.Diagnostics.Tracing/TraceEvent/docs/TraceEvent.md) in order to gain a better understanding of how the library works.

##### HTTP Requests
HTTP request tracking is a feature specifically for use with ASP.NET Core applications. HTTP request events for ASP.NET Core apps are not native to the TraceEvent library, so a few lines of code must be provided in order to enable tracking with certain features:

```cs
session.EnableProvider(new Guid("2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5"), TraceEventLevel.Informational, 0x80);
var AspSourceGuid = TraceEventProviders.GetEventSourceGuidFromName("Microsoft-AspNetCore-Hosting");
session.EnableProvider(AspSourceGuid);
```

This will allow the Dynamic event parser to detect, among other events, HTTP request Starts and Stops. The first provider enabling line of code above also allows activity ID's to be associated with these events such that request Starts can be associated with their corresponding Stop (which allows request duration to be calculated). Furthermore, Start events contain within their event message the method and path of the HTTP request. In order to access this information, some string parsing has to be done with the event message that can be referenced in the code.

The HTTP request class contains - in addition to the application name, timestamp, and process - the HTTP event's type (String containing either "Start" or "Stop"), method (String only associated with Starts), path (String only associated with Starts), and activity ID (GUID).

##### Exceptions
Exceptions, and all of the non-HTTP TraceEvent-tracked events, are detected by the same event parser (the ClrTraceEventParser). In order to enable event detection for all of the events we want within the parser's umbrella, keywords have to be referenced for the event types we're interested in when enabling the provider:

```cs
session.EnableProvider(ClrTraceEventParser.ProviderGuid, TraceEventLevel.Verbose, (0x8000 | 0x4000 | 0x1 | 0x10));
```

Using this style, any number of keywords (found in the TraceEvent repo) can be referenced for the parser's detection. In this case, we enable the detection of Exception, GC, Contention, and JIT events. Actually setting up a delegate response to event detection is detailed below, with the delegation for ExceptionStart events as the example:

```cs
// subscribe to all exception start events
clrParser.ExceptionStart += delegate (ExceptionTraceData data)
{
  // if exception was in user process, do stuff
  if (data.ProcessID == myProcess.Id)
  {
    // Stuff to be done
  }
};
```

The exception class contains all typical class fields in addition to a "type" field (String containing exception type).

##### Garbage Collection
For garbage collection, delegates must be set up for a number of different events since the GC "type" is not specified within a general GC event like it is for exceptions. For this reason, the data collector is set up to record GCStart, GCStop, GCAllocationTick, GCCreateConcurrentThread, GCSuspendEEStart, GCRestartEEStop, and GCTriggered events. Using the timestamps and matching thread ID's of GCSuspendEEStart and GCRestartEEStop events, it's possible to determine how much time was set aside for performing garbage collection within the user's process. In addition to the typical class fields, the GC class contains a "type" field (String detailing event name) and thread ID field (int).

##### Contention and JIT
Both contention and JIT events are very straightforward in how they are tracked. ContentionStart and ContentionStop events are correlated to determine time per contention, and JIT events are recorded every time a method within the process is jitted. The contention class includes a "type" field (String - "Start" or "Stop") and the JIT class includes the jitted method's name (String). Method jitting will typically be heaviest upon startup and then fall off in frequency.
