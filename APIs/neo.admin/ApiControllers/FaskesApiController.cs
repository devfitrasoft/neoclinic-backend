using Microsoft.AspNetCore.Mvc;
using neo.admin.Data.Enterprise;
using neo.admin.Facades;
using neo.admin.Models;
using neo.admin.Services;
using Shared.Common;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace neo.admin.ApiControllers
{
    [ApiController, Route("api/v1/faskes")]
    public class FaskesApiController : ControllerBase
    {
        private readonly IRegistrationFacade _registFacade;
        public FaskesApiController(ILoggerFactory loggerFactory, IConfiguration cfg, EnterpriseDbContext edb,
            MailService mailService, ICaptchaValidatorService captchaValidator) =>
            _registFacade = new RegistrationFacade(loggerFactory, cfg, edb, mailService, captchaValidator);

        [HttpGet, Route("search/{noFaskes}")]
        public async Task<IActionResult> SearchAsync([FromRoute] string noFaskes, CancellationToken ct)
        {
            var faskes = await _registFacade.GetFaskesByCodeAsync(noFaskes, ct);

            if (faskes == null)
                return StatusCode(StatusCodes.Status204NoContent, new FaskesInfoResponseModel()
                {
                    Success = false,
                    Message = "Couldn't find data faskes for the requested nomor faskes"
                });

            var data = new FaskesInfoResponseDataModel(
                faskes.Id, faskes.Name, faskes.Email, faskes.Phone,
                faskes.Address, faskes.IsActive, faskes.CorporateId, faskes.Name
            );

            return Ok(new FaskesInfoResponseModel()
            {
                Success = true,
                Data = new List<FaskesInfoResponseDataModel>() { data }
            });
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> RegistrationAsync([FromQuery] string otp, [FromBody] RegisterFaskesRequest req, CancellationToken ct)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(req);

            if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
                return StatusCode(StatusCodes.Status400BadRequest, new RegisterFaskesResponseModel()
                {
                    Success = false,
                    Message = StringParser.ValidationErrorMessageBuilder(validationResults)
                });

            var res = await _registFacade.RegisterAsync(otp, req, ct);
            return StatusCode(StatusCodes.Status201Created, res);
        }

        [HttpGet, Route("activate/{username}")]
        public async Task<IActionResult> ActivateAsync([FromRoute] string username, CancellationToken ct)
        {
            bool isActivated = await _registFacade.ActivateFaskesAsync(username, ct);

            if (isActivated)
            {
                return Ok(new CommonAPIBodyResponseModel()
                {
                    Success = true
                });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonAPIBodyResponseModel()
                {
                    Success = false,
                    Message = "One or some of the object couldn't be saved/updated to the database"
                });
            }
        }
    }
}
