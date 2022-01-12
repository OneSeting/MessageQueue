using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HighConcurrency.Data.MessageQueue;
using HighConcurrency.Data.ServiceLocators;
using HighConcurrency.Data.Services.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace HighConcurrency.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        #region Fileds
        private readonly AllSituationService allSituationService = ServiceLocator.Current.GetInstance<AllSituationService>();
        #endregion

        [HttpPost]
        [HttpPatch]
        //[Produces("application/json")]
        public IActionResult Index([FromBody] JsonElement parameter)
        {
            //接收
            object result = allSituationService.GetOrderPage(parameter);
            return Ok(result);
        }

        //[HttpGet]
        //[HttpPatch]
        //public IActionResult PlaceAnOrders([FromBody] JsonElement parameter)
        //{
        //    //下定单
        //    object result = allSituationService.PlaceAnOrders(parameter);
        //    return Ok(result);
        //}

        [HttpGet]
        [HttpPatch]
        public string Say()
        {
            return "hello";
        }
    }
}
