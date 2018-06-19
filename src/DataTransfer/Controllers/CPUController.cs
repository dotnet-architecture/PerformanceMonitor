using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monitor. ;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            var data = await _CPUContext.CPU_Data.Where(d => (d.date > start && d.date < end)).ToListAsync();
                
            return Ok(data);
        }

        [HttpGet]
        [Route("CPU")]
        [ProducesResponseType( (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCPUDataByTime(DateTime d)
        {
            var point = await _CPUContext.CPU_Data.SingleOrDefaultAsync(cpu => cpu.date == d);
            return Ok(point);
        }

        //[HttpPost]
        //[ProducesResponseType((int)HttpStatusCode.Created)]
        //public async Task<IActionResult> createCPUDatapointFromJSON([FromBody] string JSON)
        //{
        //    //parse string
        //    CPU point = new CPU(JSON);
        //    _CPUContext.Add(point);
        //    await _CPUContext.SaveChangesAsync();
        //    return CreatedAtAction("CPU Datapoint Created", new { date = point.date }, null);
        //}

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapoint([FromBody]CPU c)
        {
            CPU point = new CPU
            {
                CPU_Usage = c.CPU_Usage,
                date = c.date
            };
            _CPUContext.Add(point);
            await _CPUContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { date = point.date }, null);
        }

    }
}
