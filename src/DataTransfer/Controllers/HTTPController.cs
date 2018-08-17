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
    public class HTTPController : ControllerBase
    {
        public PerformanceDataContext _MetricContext;

        public HTTPController(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        [EnableCors("AllowAllOrigins")]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getHTTPDataByTimerange(DateTime start, DateTime end, int id)
        {
            List<Http_Request> data = await _MetricContext.Http_Request.Where(d => (d.timestamp > start && d.timestamp < end && id == d.AppId)).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }

        [HttpGet]
        [Route("HTTP")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHTTPDataByTime(DateTime d, Session sess)
        {
            var point = await _MetricContext.Http_Request.SingleOrDefaultAsync(cpu => cpu.timestamp == d && sess.Id == cpu.AppId);
            return Ok(point);
        }

        [HttpPost]
        [Route("HTTPJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateHTTPDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (Http_Request point in met.requests)
            {
                _MetricContext.Http_Request.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("HTTP Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateHTTPDatapoint([FromBody]Http_Request c)
        {
            Http_Request point = new Http_Request
            {
                type = c.type,
                method = c.method,
                path = c.path,
                timestamp = c.timestamp
            };
            _MetricContext.Http_Request.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("HTTP Data Created", new { date = point.timestamp }, null);
        }

    }
}
