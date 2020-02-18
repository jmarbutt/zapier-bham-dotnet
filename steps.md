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



