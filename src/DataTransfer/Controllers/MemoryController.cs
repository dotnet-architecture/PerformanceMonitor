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
    public class MemoryController : ControllerBase
    {
        public MetricContext _MetricContext;

        public MemoryController(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getMEMDataByTimerange(DateTime start, DateTime end)
        {
            List<Mem_Usage> data = await _MetricContext.MEM_Data.Where(d => (d.timestamp > start && d.timestamp < end)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("MEM")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMEMDataByTime(DateTime d)
        {
            var point = await _MetricContext.MEM_Data.SingleOrDefaultAsync(cpu => cpu.timestamp == d);
            return Ok(point);
        }

        [HttpGet]
        [Route("MEMBYUSAGE")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMEMDataByUsage(float usage)
        {
            var point = await _MetricContext.MEM_Data.SingleOrDefaultAsync(cpu => cpu.usage == usage);
            return Ok(point);
        }
        [HttpPost]
        [Route("MEMJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateMEMDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (Mem_Usage point in met.mem)
            {
                _MetricContext.MEM_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("MEM Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateMEMDatapoint([FromBody]Mem_Usage c)
        {
            Mem_Usage point = new Mem_Usage
            {
                usage = c.usage,
                timestamp = c.timestamp
            };
            _MetricContext.MEM_Data.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("MEM Data Created", new { date = point.timestamp }, null);
        }

    }
}
