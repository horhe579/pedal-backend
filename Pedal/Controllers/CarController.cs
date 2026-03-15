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

        public CarController(CarService carsService) =>
            carService = carsService;

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
