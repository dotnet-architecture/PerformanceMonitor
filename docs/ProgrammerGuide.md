# .NET Core Performance Monitor

## Architecture Specifics
### Data Storage
The data collected is currently being hosted on a SQL database running through Docker. This allows for local testing of the application. In the future, the database will be moved to AzureSQL. This will allow the final product to run with minimal setup from the user. Startup.cs holds the location of the connection string for the server, and can be changed as necessary.

Entity framework was used to manage the sending and fetching of data from the server. There are object-specific controllers for the fetching of data, and there is an all-purpose controller for sending the data in a single packet. Entity framework largely simplifies querying a SQL server, as no commands need to be written. The primary used POST request is POST/api/v1/General/ALL, which allows for pushing all the currently collected data to the server. The receiving of data is done by the specific page being used. Primarily, /api/v1/CPU/Daterange is being used to receive data from the server for CPU usage information. /api/v1/MEM/Daterange will be used to receive data for memory usage information. Data is sent and received in JSON form.

## Functionality Specifics
### Data Collection

The actual code that performs data collection is Monitor.cs, within the PerfMonitor C# class library. It specifies the _Monitor_ class, includes all necessary C# libraries, and contains the functions that perform data collection and interfacing with the other components of the PerformanceMonitor project.
The performance metrics that are monitored by the PerformanceMonitor are as follows:

1. __CPU usage__

2. __Memory usage__
 
3. __HTTP requests__
  * Method
  * Path
  * Duration
  * Frequency

4. __Exceptions__
  * Type
  * Frequency

5. __Garbage Collection__
  * Type
  * Duration
  * Frequency

6. __Contention__
  * Duration
  * Frequency

7. __JIT events__
  * Method
  * Count
 
#### Recording Metrics (System.Diagnostics.Process class)
As mentioned above, CPU and memory usage are fetched via the _System.Diagnostics.Process_ class. A new _Monitor_ class instance must be created for each process that a user would like to monitor, because the data is fetched by first calling Process.GetCurrentProcess();, which will return a _Process_ class instance. This class instance contains a number of fields that are used to track CPU and memory performance. These metrics will be sampled as often as the sampling rate dictates. This is done by looping through a while(true) loop in the Record() function, and comparing a TimeSpan between the last sample and the current time to the sampling rate.
 
##### CPU
Tracking CPU usage requires a small amount of overhead for calculation and comparison of a few variables. The total time that a machine's logical processors spend running the user's process is described by _Process.TotalProcessorTime_, and this is what is used to calculate the percentage of time that the processor (accounting for all logical cores) has spent running the user's code. The time between samplings in recording using DateTime objects, and the amount of time is multiplied by the number of logical cores (fetched by _System.Environment.ProcessorCount_) to determine the total amount of time that the processors can allocate to work during the interval. These values are used to generate a percentage of time spent running the process.
 
Like all other metrics that are collected, there is a unique class used by the project components in order to standardize data representation and make JSON (de)serialization easier for data sharing. All classes contain a field for a recording session instance (_Session_ class instance, which will be covered in the Data Transmission subsection below) and timestamp (DateTime specifying the instance the data was collected). The CPU class also contains a "usage" field, which is a double containing the percentage of CPU usage for a given sample. This class, like all other shared classes in the project, exists within the Shared class library.
 
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
 
The HTTP request class contains - in addition to the session instance and timestamp - the HTTP event's type (String containing either "Start" or "Stop"), method (String only associated with Starts), path (String only associated with Starts), and activity ID (GUID).
 
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
Both contention and JIT events are very straightforward in how they are tracked. ContentionStart and ContentionStop events are correlated by activity ID to determine time per contention, and JIT events are recorded every time a method within the process is jitted. The contention class includes a "type" field (String - "Start" or "Stop") as well as an activity ID field (GUID), and the JIT class includes the jitted method's name (String). Method jitting will typically be heaviest upon startup and then fall off in frequency.
 
#### Data Transmission
There are two more shared classes within the project, in addition to the classes for each metric type: a _Session_ class and a _Metric_List_ class.
 
##### The _Session_ class
The _Session_ class is meant to contain information unique to a user's process and michine that will help the user 1. identify and recognize unique processes within a single application, and 2. understand performance metrics in the context of the local machine's environment. The class has six fields: "application" (String containing the user-specified application name), "process" (String composed of the process' name - for example, "dotnet" - and unique ID), "sampleRate" (int - milliseconds between CPU and memory measurements), "sendRate" (int - milliseconds between data transmissions to server), "processorCount" (int - number of logical processors on machine), "os" (String describing the machine's operating system), and "Id" (int - internal key used to uniquely identify the session).
 
##### The _Metric_List_ class
The _Metric_List_ class is meant to be used for data packaging and efficient sharing between the different components of the project. Its fields are: "session", "cpu", "mem", "exceptions", "requests", "contentions", "gc", and "jit". The session field contains an instance of the _Session_ class for the current process - this will not change throughout the running of a single process. Each of the fields corresponding to a performance metric type is a Collection of class instances for the given type.
 
### Data Presentation
 
The home page lists all current sessions (with the application name and process name) calling the FetchDataService class to get a list of all sessions, which is then used to create the table seen on the home page. The user is then prompted to input the specific session that they want to examine. If the sessions with the specified application name and process name is not found, an error pops up and the user is prompted to re-enter the information. Afterwards, another call to the API determines the session that the user is interested in based off the users' input. This session and its id is used to look up all further data. 
 
For each metric that the Performance Monitor tracks, it has a separate Razor Page that makes individual Http requests to the server through the generic class FetchDataService. Based off of the type of metric, the FetchDataService constructs an HTTP request that then returns a list of data (only of the type that was requested) by utilizing the generic MetricService class, which deserializes the JSON objects (this is the response that is received when performing an HTTP request) to objects that correlate to a specific metric type. The data is fetched on the onGet() method of each Razor Page, meaning that the data is fetched and displayed when the page is refreshed.
 
The design of the web application is specified in the .cshtml files of each Razor Page. There is a shared _Layout.cshtml page that dictates the design of the sidebar. The sidebar points towards all the different Razor pages metrics. The contents of the individual metric Razor pages then solely are responsible for the metric content portion of the web page. For each metric page, a log of data is presented in a table. A refresh button is also given so that the users can see the most current data.

## Testing
### Data Collection

The _MonitorTest_ project within the solution contains C# classes that are each used to test a particular type of metric collection. The project's executable file is Program.cs, which contains only a Main method. In order to run a test for a particular metric, uncomment the metric's test line within Main - for example, to test collection of GC events, uncomment the line reading "GCTest.Test();". Then run _MonitorTest_, and GC events will be triggered for debugging. As it stands, only one test file can be run at a time (if multiple lines are uncommented, only the first test to appear in the code will run).

__IMPORTANT__: When running _MonitorTest_, a console window will pop up and display the program's output. In order to safely terminate the program, press Ctrl+C before closing the window. If Ctrl+C is not pressed, the session allowing TraceEvent to run and collect events will not terminate. The next run of _MonitorTest_ would attempt to recreate the same session, and an error would be triggered. If you forget to Ctrl+C and run into this error, open up your machine's terminal or command prompt and run the command "logman stop MySession -ets". This will close the session, and you will be able to run _MonitorTest_ again.
