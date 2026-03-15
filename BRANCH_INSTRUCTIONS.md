# Branch: PED-jwt-auth-setup

## Goal
Implement JWT authentication infrastructure for the Pedal backend.

## Context
This is a .NET 10 backend for a Tinder-style car matching app. Currently, there's no authentication - any user can access any resource. This branch adds the JWT foundation.

## Tasks

### 1. Add JWT NuGet Packages
Add to `Pedal/Pedal.csproj`:
- `Microsoft.AspNetCore.Authentication.JwtBearer` version 10.0.*
- `System.IdentityModel.Tokens.Jwt` version 8.2.1

### 2. Add JWT Configuration
Add to `Pedal/appsettings.json`:
```json
"JwtSettings": {
  "SecretKey": "PedalSuperSecretKeyForUniversityProjectThatIsAtLeast32CharactersLong!",
  "Issuer": "PedalBackend",
  "Audience": "PedalApp",
  "ExpirationMinutes": 1440
}
```

### 3. Create Model Files

**Pedal/Models/JwtSettings.cs**:
```csharp
namespace Pedal.Models
{
    public class JwtSettings
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}
```

**Pedal/Models/LoginRequest.cs**:
```csharp
namespace Pedal.Models
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
```

**Pedal/Models/LoginResponse.cs**:
```csharp
namespace Pedal.Models
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required string CarId { get; set; }
        public required string Email { get; set; }
    }
}
```

### 4. Create TokenService

**Pedal/Services/TokenService.cs**:
- Create a service that generates JWT tokens
- Constructor should accept `IOptions<JwtSettings>`
- Method: `GenerateToken(string carId, string email)` returns JWT token string
- Token should include claims: "carId", "email", and "jti"
- Use HMACSHA256 signing algorithm
- Token expires based on ExpirationMinutes from settings

### 5. Configure JWT Authentication in Program.cs

After line 17 (after service registrations), add:
1. Register TokenService as singleton
2. Configure JwtSettings from appsettings
3. Add Authentication with JwtBearer scheme
4. Configure TokenValidationParameters with proper settings
5. Add SignalR JWT authentication event handler for query string tokens

In the middleware section (after line 43), add:
1. `app.UseAuthentication();` BEFORE `app.UseAuthorization();`

### 6. Fix CORS Configuration

Replace the dangerous CORS setup (lines 36-40 in Program.cs) with:
```csharp
app.UseCors(builder => builder
    .WithOrigins("http://localhost:3000")  // React frontend
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
```

### 7. Implement Login Endpoint in CarController

Add TokenService to CarController constructor via dependency injection.

Add new endpoint:
```csharp
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
{
    try
    {
        var car = await carService.LogInAsync(request.Email, request.Password);
        var token = tokenService.GenerateToken(car.Id, car.Email);
        return Ok(new LoginResponse { Token = token, CarId = car.Id, Email = car.Email });
    }
    catch (InvalidOperationException ex)
    {
        return Unauthorized(new { message = "Invalid email or password" });
    }
}
```

**Note**: The `LogInAsync` method already exists in CarService.cs (lines 67-79) and validates credentials.

## Verification

After implementation:

1. **Build the project**:
   ```bash
   dotnet build
   ```

2. **Run the application**:
   ```bash
   dotnet run --project Pedal
   ```

3. **Test signup** (should still work without token):
   ```bash
   curl -X POST http://localhost:5000/api/cars \
     -H "Content-Type: application/json" \
     -d '{"email":"test@example.com","password":"Test1234","brand":"BMW","model":"M3","yearOfProduction":2020,"mileage":10000,"horsepower":450,"engine":"PETROL","transmission":"MANUAL","passions":["DRIFT"],"carCultures":["JDM"],"pictureURLs":["https://example.com/car.jpg"]}'
   ```

4. **Test login** (should return JWT token):
   ```bash
   curl -X POST http://localhost:5000/api/cars/login \
     -H "Content-Type: application/json" \
     -d '{"email":"test@example.com","password":"Test1234"}'
   ```

5. **Verify token is returned** in response with carId and email

## Success Criteria
- ✅ Project builds without errors
- ✅ Login endpoint returns JWT token for valid credentials
- ✅ Login endpoint returns 401 for invalid credentials
- ✅ Token contains "carId" and "email" claims
- ✅ CORS allows localhost:3000 only

## Next Branch
After this is merged: `PED-secure-endpoints` (add [Authorize] attributes to protect endpoints)
