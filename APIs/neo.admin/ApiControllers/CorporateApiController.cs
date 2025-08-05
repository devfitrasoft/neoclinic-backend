using Microsoft.AspNetCore.Mvc;
using neo.admin.Data.Enterprise;
using neo.admin.Facades;
using neo.admin.Models;
using neo.admin.Services;

namespace neo.admin.ApiControllers
{
    [ApiController, Route("api/v1/corpo")]
    public class CorporateApiController : ControllerBase
    {
        private readonly IRegistrationFacade _registFacade;
        public CorporateApiController(ILoggerFactory loggerFactory, IConfiguration cfg, EnterpriseDbContext edb,
            MailService mailService, ICaptchaValidatorService captchaValidator) =>
            _registFacade = new RegistrationFacade(loggerFactory, cfg, edb, mailService, captchaValidator);

        [HttpGet, Route("search")]
        public async Task<IActionResult> SearchByNameAsync([FromQuery] string qry, CancellationToken ct)
        { 
            var corpoList = await _registFacade.SearchCorpoByNameAsync(qry, ct);

            if (corpoList == null)
                return StatusCode(StatusCodes.Status204NoContent, new CorporateInfoResponseModel()
                {
                    Success = false,
                    Message = "No corporate data available"
                });


            return Ok(new CorporateInfoResponseModel()
            {
                Success = true,
                Data = corpoList
            });
        }
    }
}
