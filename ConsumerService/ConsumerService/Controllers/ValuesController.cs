using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsumerService.Controllers
{
    [Route("api/consumer")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("calltimeout")]
        public async Task<ActionResult<IEnumerable<string>>> CallTimeOut()
        {
            try
            {
                var client = httpClientFactory.CreateClient("producerService");
                var response = await client.GetAsync("api/producer/timeout");
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [HttpGet("callerror")]
        public async Task<ActionResult<IEnumerable<string>>> CallError()
        {
            try
            {
                var client = httpClientFactory.CreateClient("producerService");
                var response = await client.GetAsync("api/producer/error");
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        [HttpGet("callsuccess")]
        public async Task<ActionResult<IEnumerable<string>>> CallSuccess()
        {
            try
            {
                var client = httpClientFactory.CreateClient("producerService");
                var response = await client.GetAsync("api/producer/success");
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
    }
}
