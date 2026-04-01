namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Temperature object for Celsius and Fahrenheit.
    /// </summary>
    public class Temperature
    {
        public double? Celsius { get; set; }
        public double? Fahrenheit { get; set; }
    }
}