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
    public class JitController : ControllerBase
    {
        public PerformanceDataContext _MetricContext;

        public JitController(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getJitDataByTimerange(DateTime start, DateTime end, int id)
        {
            List<Jit> data = await _MetricContext.Jit.Where(d => (d.timestamp > start && d.timestamp < end && id == d.AppId)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("Jit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetJitDataByTime(DateTime d, Session sess)
        {
            var point = await _MetricContext.Jit.SingleOrDefaultAsync(jit => jit.timestamp == d && sess.Id == jit.AppId);
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
                _MetricContext.Jit.Add(point);
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
            _MetricContext.Jit.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("HTTP Data Created", new { date = point.timestamp }, null);
        }

    }
}
