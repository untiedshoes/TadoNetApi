namespace   TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Contains detailed information about a house
    /// </summary>
    public class House
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string DateTimeZone { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public string TemperatureUnit { get; set; } = string.Empty;

        public bool InstallationCompleted { get; set; }

        public object? Partner { get; set; }

        public bool SimpleSmartScheduleEnabled { get; set; }

        public double AwayRadiusInMeters { get; set; }

        public string License { get; set; } = string.Empty;

        public bool ChristmasModeEnabled { get; set; }

        public IncidentDetection? IncidentDetection { get; set; }

        public ContactDetails ContactDetails { get; set; } = new();

        public Address Address { get; set; } = new();

        public Geolocation Geolocation { get; set; } = new();
    }
}