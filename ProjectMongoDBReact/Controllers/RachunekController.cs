﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectMongoDBReact.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectMongoDBReact.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RachunekController : ControllerBase
    {
        private readonly RachunekService _rachunekService;

        public RachunekController(RachunekService rachunekService)
        {
            _rachunekService = rachunekService;
        }

        [HttpPut("{id:length(24)}")]
        [Route("MakeBill/{id}")]
        public IActionResult MakeBill(string id,DateTime dateTime)
        {
            _rachunekService.PayBill(id, dateTime);

            return NoContent();
        }
    }
}
