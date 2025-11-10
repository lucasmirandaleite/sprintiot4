using Microsoft.AspNetCore.Authorization;

namespace MottuRFID.API.Auth
{
    /// <summary>
    /// Requirement para autorização via API Key
    /// </summary>
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
    }
}
