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

        private static object lock_obj = new object();

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
            Session info = await _MetricContext.Session.SingleOrDefaultAsync(s => (s.application == met.session.application && s.process == met.session.process));
            if (null == info)
            {
                lock (lock_obj)
                {
                    Task<Session> obj =  _MetricContext.Session.SingleOrDefaultAsync(s => (s.application == met.session.application && s.process == met.session.process));
                    obj.Wait();
                    info = obj.Result;
                    if (null == info)
                    {
                        _MetricContext.Session.Add(met.session);
                        _MetricContext.SaveChangesAsync();
                        info = met.session;
                    }
                }
              
            }

            //Adds datapoints to respective tables
            foreach (CPU_Usage point in met.cpu)
            {
                point.AppId = info.Id;
                _MetricContext.CPU_Usage.Add(point);
            }
            foreach (Mem_Usage point in met.mem)
            {
                point.AppId = info.Id;
                _MetricContext.MemData.Add(point);
            }
            foreach (Exceptions point in met.exceptions)
            {
                point.AppId = info.Id;
                _MetricContext.Exceptions.Add(point);
            }
            foreach (Http_Request point in met.requests)
            {
                point.AppId = info.Id;
                _MetricContext.Http_Request.Add(point);
            }
            foreach (Jit point in met.jit)
            {
                point.AppId = info.Id;
                _MetricContext.Jit.Add(point);
            }
            foreach (Contention point in met.contentions)
            {
                point.AppId = info.Id;
                _MetricContext.Contention.Add(point);
            }
            foreach (GC point in met.gc)
            {
                point.AppId = info.Id;
                _MetricContext.GC.Add(point);
            }
            await _MetricContext.SaveChangesAsync();
            return Ok(j);
        }
        [HttpGet]
        [Route("DELETE")]
        public async Task<IActionResult> DeleteAllDatapoints()
        {
            _MetricContext.Session.RemoveRange(_MetricContext.Session);
            await _MetricContext.SaveChangesAsync();
            return Ok();
        }
    }

}
