using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace PerfMonitor.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CPUController : ControllerBase
    {
        public CPUContext _CPUContext;

        public CPUController(CPUContext context)
        {
            _CPUContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getCPUDataByTimerange(DateTime start, DateTime end)
        {
            List<CPU_Usage> data = await _CPUContext.CPU_Data.Where(d => (d.timestamp > start && d.timestamp < end)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }



        [HttpGet]
        [Route("CPU")]
        [ProducesResponseType( (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByTime(DateTime d)
        {
            var point = await _CPUContext.CPU_Data.SingleOrDefaultAsync(cpu => cpu.timestamp == d);
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
                _CPUContext.CPU_Data.Add(point);
            }
            await _CPUContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapoint([FromBody]CPU_Usage c)
        {
            CPU_Usage point = new CPU_Usage
            {
                usage = c.usage,
                timestamp = c.timestamp
            };
            _CPUContext.CPU_Data.Add(point);
            await _CPUContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { date = point.timestamp }, null);
        }

    }
}
