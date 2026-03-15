using Microsoft.AspNetCore.Mvc;
using Pedal.Services;
using Pedal.Entities;
using Pedal.Models;
using Pedal.Extensions;

namespace Pedal.Controllers
{
    [ApiController]
    [Route("api/cars")]
    public class CarController : ControllerBase
    {
        private readonly CarService carService;
        private readonly TokenService tokenService;

        public CarController(CarService carsService, TokenService tokenService)
        {
            carService = carsService;
            this.tokenService = tokenService;
        }

        [HttpGet]
        public async Task<CarResponse[]> Get() =>
            (await carService.GetCarsAsync()).ToResponse().ToArray();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<CarResponse>> Get(string id)
        {
            var car = await carService.GetCarByIdAsync(id);

            if (car is null)
            {
                return NotFound();
            }

            return car.ToResponse();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CarRequest car)
        {
            var createdCar = await carService.SignUpAsync(car);

            return Ok(createdCar.ToResponse());
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var car = await carService.LogInAsync(request.Email, request.Password);
                var token = tokenService.GenerateToken(car.Id, car.Email);
                return Ok(new LoginResponse { Token = token, CarId = car.Id, Email = car.Email });
            }
            catch (InvalidOperationException)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            catch (InvalidDataException)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update([FromBody] Car updatedCar)
        {
            var newCar = await carService.UpdateCarInfoAsync(updatedCar);

            return Ok(newCar.ToResponse());
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {

            await carService.DeleteCarAsync(id);
            return Ok();
        }
    }
}
