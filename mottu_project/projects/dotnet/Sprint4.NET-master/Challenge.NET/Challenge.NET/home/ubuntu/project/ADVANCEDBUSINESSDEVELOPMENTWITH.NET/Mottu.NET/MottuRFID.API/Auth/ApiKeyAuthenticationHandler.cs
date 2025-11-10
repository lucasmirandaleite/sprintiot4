using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MottuRFID.API.Auth
{
    /// <summary>
    /// Handler para autenticação via API Key
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string API_KEY_HEADER_NAME = "X-API-Key";
        private const string VALID_API_KEY = "mottu-rfid-api-key-2024"; // Em produção, usar configuração segura

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder
            )
            
            : base(options, logger, encoder)
        
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Verificar se o header da API Key está presente
            if (!Request.Headers.ContainsKey(API_KEY_HEADER_NAME))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key não fornecida"));
            }

            var apiKey = Request.Headers[API_KEY_HEADER_NAME].ToString();

            // Validar a API Key
            if (string.IsNullOrEmpty(apiKey) || apiKey != VALID_API_KEY)
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key inválida"));
            }

            // Criar claims para o usuário autenticado
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "ApiUser"),
                new Claim(ClaimTypes.NameIdentifier, "api-user"),
                new Claim("ApiKey", apiKey)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

