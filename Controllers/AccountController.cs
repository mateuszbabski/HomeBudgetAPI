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

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser(RegisterUserModel dto)
        {
            _accountService.RegisterUser(dto);

            return Ok();
        }

        [HttpPost("login")]
        public  ActionResult Login(LoginUserModel dto)
        {
            string token = _accountService.GenerateJwt(dto);

            return Ok(token);
        }


    }
}
