using System;
using System.Linq;
using System.Threading.Tasks;
using DFDS.CapabilityService.WebApi.Application;
using DFDS.CapabilityService.WebApi.Domain.Exceptions;
using DFDS.CapabilityService.WebApi.Infrastructure.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DFDS.CapabilityService.WebApi.Infrastructure.Api
{
	[ApiController]
	[Route("api/v1/capabilities")]
	public class CapabilityController : ControllerBase
	{
		private readonly ICapabilityApplicationService _capabilityApplicationService;

		public CapabilityController(ICapabilityApplicationService capabilityApplicationService)
		{
			_capabilityApplicationService = capabilityApplicationService;
		}

		[HttpGet("")]
		public async Task<IActionResult> GetAllCapabilities()
		{
			var capabilities = await _capabilityApplicationService.GetAllCapabilities();

			return Ok(new CapabilityResponse
			{
				Items = capabilities
					.Select(Capability.Create)
					.ToArray()
			});
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetCapability(string id)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			IActionResult actionResult;
			try
			{
				var capability = await _capabilityApplicationService.GetCapability(capabilityId);

				var dto = CapabilityDetails.Create(capability);
				actionResult = Ok(dto);
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }


			return actionResult;
		}

		[HttpPost("")]
		public async Task<IActionResult> CreateCapability(CapabilityInput input)
		{
			IActionResult actionResult;
			try
			{
				var capability = await _capabilityApplicationService.CreateCapability(input.Name, input.Description);

				var dto = Capability.Create(capability);

				actionResult = CreatedAtAction(
					actionName: nameof(GetCapability),
					routeValues: new {id = capability.Id},
					value: dto
				);
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCapability(string id, [FromBody] CapabilityInput input)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);


			IActionResult actionResult;
			try
			{
				var capability =
					await _capabilityApplicationService.UpdateCapability(capabilityId, input.Name, input.Description);
				var dto = Capability.Create(capability);

				actionResult = Ok(
					value: dto
				);
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCapability(string id)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			IActionResult actionResult;
			try
			{
				await _capabilityApplicationService.DeleteCapability(capabilityId);
				actionResult = Ok();
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}

		[HttpPost("{id}/members")]
		public async Task<IActionResult> AddMemberToCapability(string id, [FromBody] MemberInput input)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			IActionResult actionResult;
			try
			{
				await _capabilityApplicationService.JoinCapability(capabilityId, input.Email);
				actionResult = Ok();
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}

		[HttpDelete("{id}/members/{memberEmail}")]
		public async Task<IActionResult> RemoveMemberFromCapability([FromRoute] string id,
			[FromRoute] string memberEmail)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			IActionResult actionResult;
			try
			{
				await _capabilityApplicationService.LeaveCapability(capabilityId, memberEmail);
				actionResult = Ok();
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}

		[HttpPost("{id}/contexts")]
		public async Task<IActionResult> AddContextToCapability(string id, [FromBody] ContextInput input)
		{
			var capabilityId = Guid.Empty;
			Guid.TryParse(id, out capabilityId);

			IActionResult actionResult;
			try
			{
				await _capabilityApplicationService.AddContext(capabilityId, input.Name);

				actionResult = Ok();
			}
			catch (Exception exception) when (ExceptionToStatusCode.CanConvert(exception, out actionResult)) { }

			return actionResult;
		}
	}
}
