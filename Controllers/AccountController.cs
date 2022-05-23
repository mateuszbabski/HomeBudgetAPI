using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.Controllers
{
    [Route("/api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, 
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }
        
        [HttpPost("register")]
        public ActionResult RegisterUser(RegisterUserModel dto)
        {
            _logger.LogInformation("Register new user controller method invoked");
            _accountService.RegisterUser(dto);

            return Ok();
        }

        [HttpPost("login")]
        public  ActionResult Login(LoginUserModel dto)
        {
            _logger.LogInformation("Login user controller method invoked");
            string token = _accountService.GenerateJwt(dto);

            return Ok(token);
        }


    }
}
