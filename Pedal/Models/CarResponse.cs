using Pedal.Entities;
using Pedal.Entities.Enums;

namespace Pedal.Models
{
    public class CarResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfProduction { get; set; }
        public EngineType Engine { get; set; }
        public TransmissionType Transmission { get; set; }
        public int Mileage { get; set; }
        public int Horsepower { get; set; }
        public List<Passions> Passions { get; set; } = new List<Passions>();
        public List<CarCulture> CarCultures { get; set; } = new List<CarCulture>();
        public List<string> PictureURLs { get; set; }
    }
}
