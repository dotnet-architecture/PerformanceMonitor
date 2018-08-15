# How to Implement a New Metric in .NET Core Performance Monitor

## Data Presentation

Once the custom class for this metric has been created, create a new Razor page under the Pages/Metrics folder. This will automatically create both a cshtml and cshtml.cs file. To add a link to this Razor page on the sidebar, add the following line within Pages/Shared/_Layout.csthml.

```cs
<li><a asp-page="/Metrics/<your razor page file>"><new metric></a></li>
```

To request data like the other metrics, one can follow the Jit.cshtml.cs file. Within the cshtml.cs file, update the newStamp wihtin the OnGet() method to ensure that you are getting the current data. Use the FetchDataService.getData method and pass in the oldStamp and newStamp arguments to retrieve data and add it to the list of data points. 

To show a table of the data, one can follow the html code in the Jit.cshtml file. If the table is to be updated continuoulsy without requiring the user to hit the refresh button, follow the JS code in Jit.cshtml and use the JS fetch API. If a graph is to be included, follow the JS code in CPU_Memory.cshtml to create a graph that will update continuously.
