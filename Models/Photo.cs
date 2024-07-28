using System.Text.Json.Serialization;

namespace api_dotnet_mssql_base.Models
{
	public class Photo
	{
    [JsonPropertyName("ID")]
		public int ID { get; set; }
    [JsonPropertyName("Filename")]
		public required string Filename { get; set; }
    [JsonPropertyName("Timestamp")]
		public DateTime Timestamp { get; set; }
    [JsonPropertyName("Day")]
		public DateTime Day { get; set; }
    [JsonPropertyName("Altitude")]
		public double Altitude { get; set; }
    [JsonPropertyName("Lng")]
		public double Lng { get; set; }
    [JsonPropertyName("Lat")]
		public double Lat { get; set; }
    [JsonPropertyName("CameraLng")]
		public double CameraLng { get; set; }
    [JsonPropertyName("CameraLat")]
		public double CameraLat { get; set; }
    [JsonPropertyName("Pitch")]
		public double Pitch { get; set; }
    [JsonPropertyName("Zoom")]
		public double Zoom { get; set; }
    [JsonPropertyName("Bearing")]
		public double Bearing { get; set; }
    [JsonPropertyName("Enabled")]
		public bool Enabled { get; set; }
	}
}
