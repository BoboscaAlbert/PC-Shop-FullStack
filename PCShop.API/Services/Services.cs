using Microsoft.IdentityModel.Tokens;
using PCShop.API.DTOs;
using PCShop.API.Models;
using PCShop.API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PCShop.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepo.CreateAsync(user);
        user.Id = id;

        return new AuthResponseDto
        {
            Token = GenerateToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        return new AuthResponseDto
        {
            Token = GenerateToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpirationHours"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync() => _repo.GetAllAsync();
    public Task<Product?> GetProductByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category) =>
        _repo.GetByCategoryAsync(category);
}

public interface IOrderService
{
    Task<OrderResponseDto?> PlaceOrderAsync(int userId, CheckoutDto dto);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;

    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<OrderResponseDto?> PlaceOrderAsync(int userId, CheckoutDto dto)
    {
        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        //never trust frontend
        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product == null || product.Stock < item.Quantity)
                return null;

            var unitPrice = product.Price; // Pretul din DB, nu din frontend
            totalPrice += unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            });
        }

        var order = new Order
        {
            UserId = userId,
            TotalPrice = totalPrice,
            Status = "Pending",
            ShippingAddress = dto.ShippingAddress,
            ShippingCity = dto.ShippingCity,
            ShippingZip = dto.ShippingZip,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems
        };

        var orderId = await _orderRepo.CreateOrderAsync(order);

        return new OrderResponseDto
        {
            OrderId = orderId,
            TotalPrice = totalPrice,
            Status = "Pending",
            CreatedAt = order.CreatedAt
        };
    }
}