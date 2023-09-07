using MediaTrackerApiGateway.Data;
using MediaTrackerApiGateway.Models;

namespace MediaTrackerApiGateway.Controllers;

public class UserInformationController
{
    private readonly IUserInformationRepository _userInformationRepository;

    public UserInformationController(IUserInformationRepository userInformationRepository)
    {
        _userInformationRepository = userInformationRepository;
    }

    public async Task<ServiceResponse<UserInformation>> GetUserIdByToken(string token)
    {
        var serviceResponse = new ServiceResponse<UserInformation>
        {
            Data = await _userInformationRepository.GetUserIdByToken(token)
        };

        if (serviceResponse.Data is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "No entry found";
        }
        return serviceResponse;
    }
}
