namespace   TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Contains detailed information about a house
    /// </summary>
    public class House
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DateTimeZone { get; set; }

        public DateTime DateCreated { get; set; }

        public string TemperatureUnit { get; set; }

        public bool InstallationCompleted { get; set; }

        public object Partner { get; set; }

        public bool SimpleSmartScheduleEnabled { get; set; }

        public double AwayRadiusInMeters { get; set; }

        public string License { get; set; }

        public bool ChristmasModeEnabled { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public Address Address { get; set; }

        public Geolocation Geolocation { get; set; }
    }
}