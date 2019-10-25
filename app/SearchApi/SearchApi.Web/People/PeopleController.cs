using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody]PersonSearchRequest searchRequest)
        {
            return await Task.FromResult(Ok(searchRequest));
        }
    }
}