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
``` c#

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ZapierAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {


            if (!context.HttpContext.Request.Headers.ContainsKey("X-API-KEY"))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            var apiKey = context.HttpContext.Request.Headers["X-API-KEY"][0];

            if (apiKey != "1234567890abc")
            {
                context.Result = new StatusCodeResult((int) HttpStatusCode.Forbidden);
            }

      
        }
    }

```


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

- Add ` [ZapierAuthorize]` to `OrdersController`

- Add `services.AddAuthorization();` & ` app.UseAuthorization();` to startup