using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PerfMonitor.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JitController : ControllerBase
    {
        public MetricContext _MetricContext;

        public JitController(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getJitDataByTimerange(DateTime start, DateTime end)
        {
            List<Jit> data = await _MetricContext.Jit_Data.Where(d => (d.timestamp > start && d.timestamp < end)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("Jit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetJitDataByTime(DateTime d)
        {
            var point = await _MetricContext.Jit_Data.SingleOrDefaultAsync(cpu => cpu.timestamp == d);
            return Ok(point);
        }

        [HttpPost]
        [Route("JitJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateJitDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (Jit point in met.jit)
            {
                _MetricContext.Jit_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("Jit Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateJitDatapoint([FromBody]Jit c)
        {
            Jit point = new Jit
            {
                method = c.method,
                timestamp = c.timestamp
            };
            _MetricContext.Jit_Data.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("HTTP Data Created", new { date = point.timestamp }, null);
        }

    }
}
