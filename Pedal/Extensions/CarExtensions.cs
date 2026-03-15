using Pedal.Entities;
using Pedal.Models;

namespace Pedal.Extensions
{
    public static class CarExtensions
    {
        public static CarResponse ToResponse(this Car car)
        {
            return new CarResponse
            {
                Id = car.Id,
                Email = car.Email,
                Brand = car.Brand,
                Model = car.Model,
                YearOfProduction = car.YearOfProduction,
                Engine = car.Engine,
                Transmission = car.Transmission,
                Mileage = car.Mileage,
                Horsepower = car.Horsepower,
                Passions = car.Passions,
                CarCultures = car.CarCultures,
                PictureURLs = car.PictureURLs
            };
        }

        public static List<CarResponse> ToResponse(this IEnumerable<Car> cars)
        {
            return cars.Select(car => car.ToResponse()).ToList();
        }
    }
}
