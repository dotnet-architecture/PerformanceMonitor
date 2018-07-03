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
    public class ContentionController : ControllerBase
    {
        public MetricContext _MetricContext;

        public ContentionController(MetricContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("Daterange")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> getContentionDataByTimerange(DateTime start, DateTime end)
        {
            List<Contention> data = await _MetricContext.Contention_Data.Where(d => (d.timestamp.ToUniversalTime() > start.ToUniversalTime() && d.timestamp.ToUniversalTime() < end.ToUniversalTime())).ToListAsync();
            string jsonOfData = JsonConvert.SerializeObject(data);
            return Ok(jsonOfData);
        }



        [HttpGet]
        [Route("Contention")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentionDataByTime(DateTime d)
        {

            Contention point = await _MetricContext.Contention_Data.SingleOrDefaultAsync(cont => (cont.timestamp.ToUniversalTime() == d.ToUniversalTime()));

            return Ok(point);
        }

        [HttpGet]
        [Route("ContentionBYUSAGE")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentionDataByType(string type)
        {
            var point = await _MetricContext.Contention_Data.SingleOrDefaultAsync(cont => cont.type == type);
            return Ok(point);
        }


        [HttpPost]
        [Route("ContJSON")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateContentionDatapointFromJSON([FromBody]string j)
        {
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);
            foreach (Contention point in met.contentions)
            {
                _MetricContext.Contention_Data.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("Contention Data Created", new { obj = j }, null);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCPUDatapoint([FromBody]Contention c)
        {
            Contention point = new  Contention
            {
                type = c.type,
                timestamp = c.timestamp.ToUniversalTime()
            };
            _MetricContext.Contention_Data.Add(point);
            await _MetricContext.SaveChangesAsync();
            return CreatedAtAction("CPU Data Created", new { date = point.timestamp }, null);
        }

    }
}
