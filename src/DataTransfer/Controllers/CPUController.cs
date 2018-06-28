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
    public class CPUController : ControllerBase
    {
        public MetricContext _MetricContext;

        public CPUController(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getCPUDataByTimerange(DateTime start, DateTime end)
        {
            List<CPU_Usage> data = await _MetricContext.CPU_Data.Where(d => (d.timestamp.ToUniversalTime() > start.ToUniversalTime() && d.timestamp.ToUniversalTime() < end.ToUniversalTime())).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }



        [HttpGet]
        [Route("CPU")]
        [ProducesResponseType( (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByTime(DateTime d)
        {

            CPU_Usage point =  await _MetricContext.CPU_Data.SingleOrDefaultAsync(cpu => (cpu.timestamp.ToUniversalTime() == d.ToUniversalTime()));

            return Ok(point);
        }

        [HttpGet]
        [Route("CPUBYUSAGE")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByUsage(float usage)
        {
            var point = await _MetricContext.CPU_Data.SingleOrDefaultAsync(cpu => cpu.usage == usage);
            return Ok(point);
        }


        [HttpPost]
        [Route("CPUJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach(CPU_Usage point in met.cpu)
            {
                _MetricContext.CPU_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapoint([FromBody]CPU_Usage c)
        {
            CPU_Usage point = new CPU_Usage
            {
                usage = c.usage,
                timestamp = c.timestamp.ToUniversalTime()
            };
            _MetricContext.CPU_Data.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { date = point.timestamp }, null);
        }

    }
}
