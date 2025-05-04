using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagment.Application.SystemConfiguration;
using UserManagment.Common.DTO.ConfigurationDto;
using UserManagment.Common.DTO.MailConfigurationDto;
using UserManagment.Common.Helpers;
using static UserManagment.Application.SystemConfiguration.DomainFormatHandler;
using static UserManagment.Application.SystemConfiguration.MaxTrialsLoginHandler;
using static UserManagment.Application.SystemConfiguration.PasswordExpirationPeriodHandler;

namespace UserManagment.API.SystemConfiguration
{
    [ApiController]
    [Route("/api/configuration")]
    [ApiExplorerSettings(GroupName = "System Configuration")]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(IMediator mediator, ILogger<ConfigurationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [RequiresPolicy("MaxTrialConfiguration")]
        [HttpPost]
        [Route("MaxTrialsLogin")]
        public async Task<IActionResult> MaxTrialsLogin(MaxTrialsLoginDto request)
        {
            _logger.LogInformation("in YourController, MaxTrialsLogin API called with request: {@Request}", request);

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }

        [RequiresPolicy("MaxTrialConfiguration")]
        [HttpGet]
        [Route("MaxTrialsLogin")]
        public async Task<IActionResult> MaxTrialsLogin()
        {
            _logger.LogInformation("in YourController, MaxTrialsLogin GET API called");

            return await ResponseHelper.HandleRequestAsync(
                new MaxTrialsLoginRequest(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [RequiresPolicy("PasswordExpirationConfiguration")]
        [HttpPost]
        [Route("PasswordExpirationPeriod")]
        public async Task<IActionResult> PasswordExpirationPeriod(PasswordExpirationPeriodDto request)
        {
            _logger.LogInformation("in YourController, PasswordExpirationPeriod POST API called with request: {@Request}", request);

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }

        [RequiresPolicy("PasswordExpirationConfiguration")]
        [HttpGet]
        [Route("PasswordExpirationPeriod")]
        public async Task<IActionResult> PasswordExpirationPeriod()
        {
            _logger.LogInformation("in YourController, PasswordExpirationPeriod GET API called");

            return await ResponseHelper.HandleRequestAsync(
                new PasswordExpirationPeriodRequest(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

       [RequiresPolicy("DomainFormatConfiguration")]
        [HttpPost]
        [Route("DomainFormat")]
        public async Task<IActionResult> DomainFormat(DomainFormatDto request)
        {
            _logger.LogInformation("in YourController, DomainFormat POST API called with request: {@Request}", request);

            return await ResponseHelper.HandleRequestAsync(
                request,
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }
        [HttpGet]
        [Route("GetDomainFormat")]
        public async Task<IActionResult> DomainFormat()
        {
            _logger.LogInformation("in YourController, DomainFormat GET API called");

            return await ResponseHelper.HandleRequestAsync(
                new DomainFormat(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [RequiresPolicy("MailConfiguration")]
        [HttpPut("UpdateMailSettings")]
        public async Task<IActionResult> UpdateSetting(List<ConfigurationDTo> configDtos)
        {
            _logger.LogInformation("in YourController, UpdateMailSettings PUT API called with configurations: {@configDtos}", configDtos);

            return await ResponseHelper.HandleRequestAsync(
                new MailConfigurationRequest(configDtos),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }

        [RequiresPolicy("EmailTemplatesConfiguration")]
        [HttpPost("UpdateEmailTemplate")]
        public async Task<IActionResult> UpdateEmailTemplate(UpdateEmailTemplateDto updateEmailTemplateDto)
        {
            _logger.LogInformation("in YourController, UpdateEmailTemplate POST API called with ConfigKey: {ConfigKey}", updateEmailTemplateDto.ConfigKey);

            return await ResponseHelper.HandleRequestAsync(
                new EmailTemplateRequest(
                    updateEmailTemplateDto.ConfigKey,
                    updateEmailTemplateDto.ConfigType,
                    updateEmailTemplateDto.NewTemplateBody),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.UpdationSuccessfully
            );
        }

        [RequiresPolicy("MailConfiguration")]
        [HttpGet("GetMailSettings")]
        public async Task<IActionResult> GetEmailConfigurations()
        {
            _logger.LogInformation("in YourController, GetEmailConfigurations GET API called");

            return await ResponseHelper.HandleRequestAsync(
                new EmailConfigurationGetAll(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }

        [RequiresPolicy("MailConfiguration")]
        [HttpGet("GetEmailTemplate/{ConfigKey}")]
        public async Task<IActionResult> GetEmailTemplates(string ConfigKey)
        {
            _logger.LogInformation("in YourController, GetEmailTemplates GET API called with ConfigKey: {ConfigKey}", ConfigKey);

            return await ResponseHelper.HandleRequestAsync(
                new GetEmailTemplateRequest(ConfigKey),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }
        
        [RequiresPolicy("MailConfiguration")]
        [HttpGet("GetEmailKeys")]
        public async Task<IActionResult> GetEmailKeys()
        {
            _logger.LogInformation("in YourController, GetEmailKeys GET API called");

            return await ResponseHelper.HandleRequestAsync(
                new GetEmailKeysRequest(),
                async (req) => await _mediator.Send(req),
                SuccessResponseMessage.RetrievedSuccessfully
            );
        }
    }
}
