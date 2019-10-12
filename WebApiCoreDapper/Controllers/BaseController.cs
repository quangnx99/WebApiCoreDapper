using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCoreDapper.Controllers
{
    public class BaseController:ControllerBase
    {
        private protected ILogger logger;
        public BaseController(ILogger logger)
        {
            this.logger = logger;
        }
    }
}
