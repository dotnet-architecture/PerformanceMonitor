﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Services;
using WebApplication.Interfaces;
using System.Net.Http;
using DataTransfer;

namespace WebApplication
{
    public class FetchDataService
    {
        public static async Task<List<T>> getUpdatedData<T>(DateTime oldStamp, DateTime newStamp)
        {
            IMetricService<T> _metricService = new MetricService<T>();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58026/");

            String dateRange = convertDateTime(oldStamp) + "&end=" + convertDateTime(newStamp);

            String type = "";

            Console.WriteLine(typeof(T));

            if (typeof(T).ToString().Equals("DataTransfer.CPU_Usage"))
            {
                type = "CPU";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Mem_usage"))
            {
                type = "Memory";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Exceptions"))
            {
                type = "Exceptions";
            }
            else if (typeof(T).ToString().Equals("DataTransfer.Http_Request"))
            {
                type = "HttpRequest";
            }
            else
            {
                type = "test";
            }

            HttpResponseMessage response = await client.GetAsync("api/v1/" + type + "/Daterange?start=" + dateRange);
            _metricService.updateUsingHttpResponse(response);

            List<T> addOn = new List<T>();

            if (response.IsSuccessStatusCode)
            {
                addOn = await _metricService.getServiceUsage();
            }

            Console.WriteLine(type + " " + addOn.Count + " " + addOn);
            return addOn;

        }

        // Converting DateTIme to a string that Http request will accept
        public static String convertDateTime(DateTime d)
        {
            String s = "";
            s += d.Year.ToString("D4") + "-" + d.Month.ToString("D2") + "-"
                + d.Day.ToString("D2") + "T" + d.Hour.ToString("D2") + "%3A" +
                d.Minute.ToString("D2") + "%3A" + d.Second.ToString("D2") + "." +
                d.Millisecond.ToString("D3");
            return s;
        }

    }
}
