using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using User_Service.Attributes;

namespace User_Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly ManagementApi managementApi;
	private readonly UserDeletionProducerRmq userDeletionProducer;

	public UsersController(ManagementApi managementApi, UserDeletionProducerRmq userDeletionProducer)
	{
		this.managementApi = managementApi;
		this.userDeletionProducer = userDeletionProducer;
	}

	[Authorize]
	[OwnUserRequirement]
	[HttpDelete("{userId}")]
	public async Task<IActionResult> DeleteData(string userId)
	{
		var response = await managementApi.SendRequest(HttpMethod.Delete, $"v2/users/{userId}");
		if (response.IsSuccessStatusCode)
		{
			//Notify listeners that user has been deleted. E.g. to delete the books of the user
			userDeletionProducer.MessageUserDeleted(userId);
			
			return Ok("User deleted successfully");
		}
		else
		{
			return BadRequest("An error occurred while deleting the user.");
		}
	}
}