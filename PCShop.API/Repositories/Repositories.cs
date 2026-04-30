using Dapper;
using PCShop.API.Data;
using PCShop.API.Models;

namespace PCShop.API.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
    Task<bool> EmailExistsAsync(string email);
}

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _db;

    public UserRepository(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { Email = email });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username", new { Username = username });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = _db.CreateConnection();
        var sql = @"INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, CreatedAt)
                    VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.QuerySingleAsync<int>(sql, user);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var conn = _db.CreateConnection();
        var count = await conn.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE Email = @Email", new { Email = email });
        return count > 0;
    }
}

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
}

public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _db;

    public ProductRepository(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Product>("SELECT * FROM Products WHERE Stock > 0");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Product>(
            "SELECT * FROM Products WHERE Category = @Category AND Stock > 0",
            new { Category = category });
    }
}

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
}

public class OrderRepository : IOrderRepository
{
    private readonly IDbConnectionFactory _db;

    public OrderRepository(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<int> CreateOrderAsync(Order order)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            var orderSql = @"INSERT INTO Orders (UserId, TotalPrice, Status, ShippingAddress, ShippingCity, ShippingZip, PhoneNumber, CreatedAt)
                             VALUES (@UserId, @TotalPrice, @Status, @ShippingAddress, @ShippingCity, @ShippingZip, @PhoneNumber, @CreatedAt);
                             SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var orderId = await conn.QuerySingleAsync<int>(orderSql, order, transaction);

            foreach (var item in order.Items)
            {
                item.OrderId = orderId;
                var itemSql = @"INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, UnitPrice)
                                VALUES (@OrderId, @ProductId, @ProductName, @Quantity, @UnitPrice);";
                await conn.ExecuteAsync(itemSql, item, transaction);

                await conn.ExecuteAsync(
                    "UPDATE Products SET Stock = Stock - @Qty WHERE Id = @Id",
                    new { Qty = item.Quantity, Id = item.ProductId }, transaction);
            }

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        using var conn = _db.CreateConnection();
        return await conn.QueryAsync<Order>(
            "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY CreatedAt DESC",
            new { UserId = userId });
    }
}