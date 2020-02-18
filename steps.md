## Initial Project
- Disable SSL redirect for ngrok
- Add `OrdersDbContext.cs` 
- Add `Microsoft.EntityFrameworkCore.SqlServer` nuget
- Add `Microsoft.EntityFrameworkCore.Tools` nuget

## Setup DB Context
- Make `OrdersDbContext` Inherit `:DbContext`
- Add `constructor` for `OrdersDbContext`

``` c#
public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
            : base(options)
        {
        }
```

- Create Order Model
``` c#

   public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; }

        public string ProductName { get; set; }

        public int Qty { get; set; }

        public string SalesPerson { get; set; }
    }
```


- Add Orders DbSet to `OrdersDbContext`
``` c#
public DbSet<Order> Orders { get; set; }
```


- Inject Db into services
``` c#

 services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

```

- Setup Migrations

``` terminal
Add-Migration Init

Update-Database
```



- Setup Basic Controller
- Create `OrdersController.cs`


``` c#

  public OrdersDbContext DbContext { get; }

        public OrdersController(OrdersDbContext dbContext)
        {
            DbContext = dbContext;
        }

        [HttpGet]
        public IQueryable<Order> GetOrders()
        {
            return DbContext.Orders;
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Now;
            DbContext.Orders.Add(order);

            DbContext.SaveChanges();

            return Ok();
        }

```


## Authententication

- Basic API Key


- Create `ApiKeyAuthenticationOptions.cs`
```
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "API Key";
    public string Scheme => DefaultScheme;
    public string AuthenticationType = DefaultScheme;
}
```

- Add `AuthenticationBuilderExtensions.cs`
``` c#
public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
    {
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.DefaultScheme, options);
    }
}
```

- Create ApiKeyAuthenticationHandler
``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace zapier_bham_dotnet
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ProblemDetailsContentType = "application/problem+json";
    
        private const string ApiKeyHeaderName = "x-api-key";
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
            
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            

            if (providedApiKey == "1234567890abc")
            {
                var claims = new List<Claim>
                { };

                

                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }

           
            return AuthenticateResult.Fail("Invalid API Key provided.");
        }

    }
}
```

- Add ` [Authorize]` to `OrdersController`

- Add Authentication Service
``` c#

  services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                })
                .AddApiKeySupport(options => {});
```

- Add Use Auth
``` c#
            app.UseAuthentication();
            app.UseAuthorization();
```