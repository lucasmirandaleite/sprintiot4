using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace MottuRFID.API.Auth
{
    public class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            // Implementar a lógica de autorização aqui, se necessário.
            // Por enquanto, apenas permitir o acesso.
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
