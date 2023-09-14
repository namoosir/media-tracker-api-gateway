// using System.Text.Json;
// using MediaTrackerApiGateway.Models;
// using StackExchange.Redis;

// namespace MediaTrackerApiGateway.Data;

// public class UserInformationRepository : IUserInformationRepository
// {
//     private readonly IConnectionMultiplexer _redis;

//     public UserInformationRepository(IConnectionMultiplexer redis)
//     {
//         _redis = redis;
//     }

//     public async Task<bool> CreateUserInformation(UserInformation userInformation)
//     {
//         if (userInformation is null)
//         {
//             throw new ArgumentOutOfRangeException(nameof(userInformation));
//         }

//         var db = _redis.GetDatabase();
//         var serialUserInformation = JsonSerializer.Serialize(userInformation);
//         return await db.StringSetAsync(
//             $"UserInformation:{userInformation.Token}",
//             serialUserInformation
//         );
//     }

//     public async Task<bool> DeleteUserInformation(string token)
//     {
//         var db = _redis.GetDatabase();
//         return await db.KeyDeleteAsync($"UserInformation:{token}");
//     }

//     public async Task<UserInformation?> GetUserIdByToken(string token)
//     {
//         var db = _redis.GetDatabase();
//         var userInformation = await db.StringGetAsync($"UserInformation:{token}");

//         if (!string.IsNullOrEmpty(userInformation))
//         {
//             return JsonSerializer.Deserialize<UserInformation>(userInformation);
//         }

//         return null;
//     }
// }
