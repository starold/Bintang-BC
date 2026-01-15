class Program
{
    static void Main()
    {
        Vehicle car = new Car
        {
            Brand = "Toyota",
            HorsePower = 150
        };

        Vehicle motor = new Motorcycle
        {
            Brand = "Yamaha",
            EngineCapacity = 250
        };

        PrintVehicleInfo(car);
        Console.WriteLine();
        PrintVehicleInfo(motor);
    }

    static void PrintVehicleInfo(Vehicle vehicle)
    {
        Console.WriteLine($"Brand: {vehicle.Brand}");
        Console.WriteLine($"Max Speed: {vehicle.GetMaxSpeed()} km/h");
    }
}