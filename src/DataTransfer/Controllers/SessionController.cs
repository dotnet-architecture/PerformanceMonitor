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
    public class SessionController : Controller
    {
        public PerformanceDataContext _MetricContext;

        public SessionController(PerformanceDataContext context)
        {
            _MetricContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("SESSION")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMEMDataByTime(int id)
        {
            var point = await _MetricContext.Session.SingleOrDefaultAsync(sess => (sess.Id == id));
            return Ok(point);
        }
    }
}