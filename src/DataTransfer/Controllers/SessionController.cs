﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetSessionDataByTime(int id)
        {
            var point = await _MetricContext.Session.SingleOrDefaultAsync(sess => (sess.Id == id));
            return Ok(point);
        }

        [HttpGet]
        [Route("SESSIONBYAPPANDPRO")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSessionDataByAppAndProcess(string app, string pro)
        {
            var point = await _MetricContext.Session.SingleOrDefaultAsync(sess => (sess.application == app && sess.process == pro));
            return Ok(point);
        }

        [HttpGet]
        [Route("RETURNALL")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllpoints()
        {
            var point = await _MetricContext.Session.ToListAsync();
            return Ok(point);
        }

        [HttpPost]
        [Route("GETSESSION")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult GetSession([FromForm] string application, [FromForm] string process)
        {
            var sessions = _MetricContext.Session
                .Where(t => t.application == application && t.process == process);

            if (sessions == null)
            {
                return NotFound(); 
            }

            return Ok(sessions);

            /*
             * return Ok(new {
             *     application = application, 
             *     process = process,
             *     sessionsReturend =  sessions, //foreach....<p>session<p>
             */
        }
    }
}