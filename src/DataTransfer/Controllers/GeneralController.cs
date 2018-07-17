using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace DataTransfer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class General : ControllerBase
    {
        public PerformanceDataContext _MetricContext;

        public General(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapointFromJSON([FromBody]string j)
        {
            //Deserializes json
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);

            //Adds Application to app table
            _MetricContext.Session.Add(met.session);
            //Adds datapoints to respective tables
            foreach (CPU_Usage point in met.cpu)
            {
                _MetricContext.CPU_Usage.Add(point);
            }
            foreach (Mem_Usage point in met.mem)
            {
                _MetricContext.MemData.Add(point);
            }
            foreach (Exceptions point in met.exceptions)
            {
                _MetricContext.Exceptions.Add(point);
            }
            foreach (Http_Request point in met.requests)
            {
                _MetricContext.Http_Request.Add(point);
            }
            foreach (Jit point in met.jit)
            {
                _MetricContext.Jit.Add(point);
            }
            foreach (Contention point in met.contentions)
            {
                _MetricContext.Contention.Add(point);
            }
            foreach (GC point in met.gc)
            {
                _MetricContext.GC.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { obj = j }, null);
        }
    }
}
