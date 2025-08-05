using Microsoft.AspNetCore.Mvc;
using neo.preregist.Common;
using neo.preregist.Data.Enterprise;
using neo.preregist.Facades;
using neo.preregist.Models;
using neo.preregist.Services;
using Shared.Common;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace neo.preregist.ApiControllers
{
    [ApiController, Route("api/v1/pre-register")]
    public class PreRegisterApiController : ControllerBase
    {
        private readonly IPreRegistFacade _facade;
        public PreRegisterApiController(ILoggerFactory loggerFactory, IConfiguration cfg,
            EnterpriseDbContext edb, MailService mailService) 
        {
            _facade = new PreRegistFacades(loggerFactory, cfg, edb, mailService);
        }

        [HttpPost, Route("add")]
        public async Task<IActionResult> AddAsync([FromBody] PreRegistRequest req, CancellationToken ct)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(req);

            if (!Validator.TryValidateObject(req, context, validationResults, validateAllProperties: true))
                return StatusCode(StatusCodes.Status400BadRequest, new TokenVerifyResponse()
                {
                    Success = false,
                    Message = StringParser.ValidationErrorMessageBuilder(validationResults)
                });

            var res = await _facade.SaveAndNotify(req, ct);

            if (res.Data.FirstOrDefault() != null)
            {
                return res.Data.FirstOrDefault()?.status switch
                {
                    PreRegistSaveResponse.Created => StatusCode(StatusCodes.Status201Created, res),
                    PreRegistSaveResponse.Updated => Ok(res),
                    PreRegistSaveResponse.Registered => StatusCode(StatusCodes.Status202Accepted, res),
                    PreRegistSaveResponse.Error => StatusCode(StatusCodes.Status500InternalServerError, res),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, res)
                };
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet, Route("verify")]
        public async Task<IActionResult> VerifyAsync([FromQuery] string token, CancellationToken ct)
        {
            var row = await _facade.GetRowByTokenAsync(token, ct);

            if (row == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new TokenVerifyResponse()
                {
                    Success = false,
                    Message = "Token unauthorized"
                }); // Token expired

            //  3. Check if token is expired
            if (!row.otpExpiresAt.HasValue || row.otpExpiresAt.Value < DateTime.UtcNow)
                return StatusCode(StatusCodes.Status401Unauthorized, new TokenVerifyResponse()
                {
                    Success = false,
                    Message = "Token unauthorized"
                }); // Token expired

            //  4. Check if is_registered_web = true
            if (row.isRegistered)
                return StatusCode(StatusCodes.Status409Conflict, new TokenVerifyResponse()
                {
                    Success = false,
                    Message = "Data already registered into the system"
                }); // Account already registered, henche conflict

            //  5. Return essential data
            return Ok(new TokenVerifyResponse()
            {
                Success = true,
                Data = new List<PreRegistData>() { row },
            });
        }
    }
}
