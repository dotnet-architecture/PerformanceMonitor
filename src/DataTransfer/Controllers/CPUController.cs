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
    public class CPUController : ControllerBase
    {
        public PerformanceDataContext _MetricContext;

        public CPUController(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getCPUDataByTimerange(DateTime start, DateTime end, int id )
        {
            List<CPU_Usage> data = await _MetricContext.CPU_Usage.Where(d => (d.timestamp.ToUniversalTime() > start.ToUniversalTime() && d.timestamp.ToUniversalTime() < end.ToUniversalTime() && id == d.AppId)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }



        [HttpGet]
        [Route("CPU")]
        [ProducesResponseType( (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByTime(DateTime d, Session sess)
        {

            CPU_Usage point =  await _MetricContext.CPU_Usage.SingleOrDefaultAsync(cpu => (cpu.timestamp.ToUniversalTime() == d.ToUniversalTime() && sess.Id == cpu.AppId));

            return Ok(point);
        }

        [HttpGet]
        [Route("CPUBYUSAGE")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByUsage(float usage, Session sess)
        {
            var point = await _MetricContext.CPU_Usage.SingleOrDefaultAsync(cpu => cpu.usage == usage && sess.Id == cpu.AppId);
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
                _MetricContext.CPU_Usage.Add(point);
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
                timestamp = c.timestamp.ToUniversalTime(),
               
            };
            _MetricContext.CPU_Usage.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { date = point.timestamp }, null);
        }

    }
}
