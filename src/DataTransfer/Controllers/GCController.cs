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
    public class GCController : ControllerBase
    {
        public MetricContext _MetricContext;

        public GCController(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getGCDataByTimerange(DateTime start, DateTime end)
        {
            List<GC> data = await _MetricContext.GC_Data.Where(d => (d.timestamp > start && d.timestamp < end)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("GC")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetGCDataByTime(DateTime d)
        {
            var point = await _MetricContext.GC_Data.SingleOrDefaultAsync(cpu => cpu.timestamp == d);
            return Ok(point);
        }

        [HttpPost]
        [Route("GCJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateGCDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (GC point in met.gc)
            {
                _MetricContext.GC_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("GC Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateGCDatapoint([FromBody]GC c)
        {
            GC point = new GC
            {
                type = c.type,
                timestamp = c.timestamp
            };
            _MetricContext.GC_Data.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("GC Data Created", new { date = point.timestamp }, null);
        }

    }
}
