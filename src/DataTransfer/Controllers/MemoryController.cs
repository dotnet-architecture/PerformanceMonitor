using Microsoft.AspNetCore.Cors;
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
        public PerformanceDataContext _MetricContext;

        public MemoryController(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getMEMDataByTimerange(DateTime start, DateTime end, int id)
        {
            List<Mem_Usage> data = await _MetricContext.MemData.Where(d => (d.timestamp > start && d.timestamp < end && id == d.AppId)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("MEM")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMEMDataByTime(DateTime d, Session sess )
        {
            var point = await _MetricContext.MemData.SingleOrDefaultAsync(mem => mem.timestamp == d && sess.Id == mem.AppId);
            return Ok(point);
        }

        [HttpGet]
        [Route("MEMBYUSAGE")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMEMDataByUsage(float usage, Session sess)
        {
            var point = await _MetricContext.MemData.SingleOrDefaultAsync(mem => mem.usage == usage && sess.Id == mem.AppId);
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
                _MetricContext.MemData.Add(point);
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
            _MetricContext.MemData.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("MEM Data Created", new { date = point.timestamp }, null);
        }

    }
}
