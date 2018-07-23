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
    public class General : ControllerBase
    {
        public PerformanceDataContext _MetricContext;

        public General(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]  
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateGeneralDatapointFromJSON([FromBody]string j)
        {
            //Deserializes json
            Metric_List met = new Metric_List();
            met = JsonConvert.DeserializeObject<Metric_List>(j);

            //Adds Application to app table

            if( null == _MetricContext.Session.SingleOrDefaultAsync(s => (s.Id == met.session.Id)))
            {
                _MetricContext.Session.Add(met.session);
                await _MetricContext.SaveChangesAsync();
            }
            //Adds datapoints to respective tables
            foreach (CPU_Usage point in met.cpu)
            {
                point.AppId = met.session.Id;
                _MetricContext.CPU_Usage.Add(point);
            }
            foreach (Mem_Usage point in met.mem)
            {
                point.AppId = met.session.Id;
                _MetricContext.MemData.Add(point);
            }
            foreach (Exceptions point in met.exceptions)
            {
                point.AppId = met.session.Id;
                _MetricContext.Exceptions.Add(point);
            }
            foreach (Http_Request point in met.requests)
            {
                point.AppId = met.session.Id;
                _MetricContext.Http_Request.Add(point);
            }
            foreach (Jit point in met.jit)
            {
                point.AppId = met.session.Id;
                _MetricContext.Jit.Add(point);
            }
            foreach (Contention point in met.contentions)
            {
                point.AppId = met.session.Id;
                _MetricContext.Contention.Add(point);
            }
            foreach (GC point in met.gc)
            {
                point.AppId = met.session.Id;
                _MetricContext.GC.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return Ok(j);
        }
    }
}
