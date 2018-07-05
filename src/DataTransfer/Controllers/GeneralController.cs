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
        public MetricContext _MetricContext;

        public General(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (CPU_Usage point in met.cpu)
            {
                _MetricContext.CPU_Data.Add(point);
            }
            foreach(Mem_Usage point in met.mem)
            {
                _MetricContext.MEM_Data.Add(point); 
            }
            foreach(Exceptions point in met.exceptions)
            {
                _MetricContext.Exception_Data.Add(point);
            }
            foreach(Http_Request point in met.requests)
            {
                _MetricContext.HTTP_Data.Add(point);
            }
            foreach(Jit point in met.jit)
            {
                _MetricContext.Jit_Data.Add(point);
            }
            foreach (Contention point in met.contentions)
            {
                _MetricContext.Contention_Data.Add(point);
            }
            foreach (GC point in met.gc)
            {
                _MetricContext.GC_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { obj = j }, null);
        }
    }
}
