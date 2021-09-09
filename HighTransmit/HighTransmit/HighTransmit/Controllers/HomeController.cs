using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HighTransmit.Data;
using HighTransmit.Data.MessageQueue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HighTransmit.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        #region Fileds
        private readonly MseeageProduction allSituationService = ServiceLocator.Current.GetInstance<MseeageProduction>();
        #endregion

        [HttpPost]
        [HttpPatch]
        public IActionResult Index([FromBody] JsonElement jsonElement)
        {
            //创建订单队列
            object result = allSituationService.MessageSend(jsonElement);
            return Ok("ok");
        }
    }
}
