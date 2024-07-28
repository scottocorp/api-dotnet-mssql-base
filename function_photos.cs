using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using api_dotnet_mssql_base.Models;

namespace api_dotnet_mssql_base
{
	public class function_photos
	{
		[Function("Create")]
		public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "photos")] 
			HttpRequestData req, 
			FunctionContext context)
		{            
			var photo = await JsonSerializer.DeserializeAsync<Photo>(req.Body);
			// _logger.LogInformation(photo.filename);

			try {
				using SqlConnection connection = new(Environment.GetEnvironmentVariable("SqlConnectionString"));
				connection.Open();
				var query = @$"
					INSERT INTO [photos] 
						(
							Filename, 
							Bearing, 
							Pitch, 
							Zoom
						) 
					VALUES
						(
							'{photo.Filename}', 
							'{photo.Bearing}', 
							'{photo.Pitch}', 
							'{photo.Zoom}'
						)
				";
				SqlCommand command = new(query, connection);
				command.ExecuteNonQuery();
      } catch (Exception e) {
				_logger.LogError(e.ToString());
				return req.CreateResponse(HttpStatusCode.InternalServerError);
			}
			return req.CreateResponse(HttpStatusCode.OK);
		}

		[Function("ReadAll")]
		public async Task<HttpResponseData> ReadAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "photos")] 
			HttpRequestData req)
		{            
			List<Photo> photoList = [];
			try {
				using SqlConnection connection = new(Environment.GetEnvironmentVariable("SqlConnectionString"));
				await connection.OpenAsync();
				var query = @"Select * from photos";
				using SqlCommand command = new SqlCommand(query, connection);
				using var reader = await command.ExecuteReaderAsync();
				while (reader.Read())
				{
					Photo photo = new()
					{
							ID = (int)reader["ID"],
							Filename = (string)reader["Filename"],
							Zoom = (double)reader["Zoom"],
							Pitch = (double)reader["Pitch"],
							Bearing = (double)reader["Bearing"],
					};
					photoList.Add(photo);
				}
			} catch (Exception e) {
				_logger.LogError(e.ToString());
				return req.CreateResponse(HttpStatusCode.InternalServerError);
			}
			if (photoList.Count == 0) {
				return req.CreateResponse(HttpStatusCode.NotFound);
			}

			var response = req.CreateResponse(HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "application/json; charset=utf-8");
			response.WriteString(JsonSerializer.Serialize(photoList));
			return response;
		}

		[Function("Read")]
		public async Task<HttpResponseData> Read([HttpTrigger(AuthorizationLevel.Function, "get", Route = "photos/{ID}")] 
			HttpRequestData req, 
			int ID)
		{            
			Photo? photo = null;
			try {
				using SqlConnection connection = new(Environment.GetEnvironmentVariable("SqlConnectionString"));
				await connection.OpenAsync();
				var query = @"Select * from photos Where ID = @ID";
				using SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@ID", ID);
				using var reader = await command.ExecuteReaderAsync();
				if (await reader.ReadAsync())
				{
					photo = new Photo
					{
						ID = (int)reader["ID"],
						Filename = (string)reader["Filename"],
						Zoom = (double)reader["Zoom"],
						Pitch = (double)reader["Pitch"],
						Bearing = (double)reader["Bearing"],
					};
				}
			} catch (Exception e) {
				_logger.LogError(e.ToString());
				return req.CreateResponse(HttpStatusCode.InternalServerError);
			}
			if (photo == null) {
				return req.CreateResponse(HttpStatusCode.NotFound);
			}

			var response = req.CreateResponse(HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "application/json; charset=utf-8");
			response.WriteString(JsonSerializer.Serialize(photo));
			return response;
		}

		[Function("Delete")]
		public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "photos/{ID}")] 
			HttpRequestData req, 
			int ID)
		{            
			try {
				using SqlConnection connection = new(Environment.GetEnvironmentVariable("SqlConnectionString"));
				await connection.OpenAsync();
				var query = @"Delete from photos Where ID = @ID";
				using SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@ID", ID);
				command.ExecuteNonQuery();
			} catch (Exception e) {
				_logger.LogError(e.ToString());
				return req.CreateResponse(HttpStatusCode.InternalServerError);
			}
			return req.CreateResponse(HttpStatusCode.OK);
		}


		[Function("Update")]
		public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "photos/{ID}")] 
			HttpRequestData req, 
			int ID)
		{
			var photo = await JsonSerializer.DeserializeAsync<Photo>(req.Body);
			try {
				using SqlConnection connection = new(Environment.GetEnvironmentVariable("SqlConnectionString"));
				connection.Open();
				var query = @$"
					UPDATE photos 
					SET 
						Filename = @Filename,
						Timestamp = @Timestamp,
						Day = @Day,
						Altitude = @Altitude,
						Lng = @Lng,
						Lat = @Lat,
						CameraLng = @CameraLng,
						CameraLat = @CameraLat,
						Pitch = @Pitch,
						Zoom = @Zoom,
						Bearing = @Bearing,
						Enabled = @Enabled
					WHERE 
						ID = @key
				";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@Filename", photo.Filename);
				command.Parameters.AddWithValue("@Timestamp", photo.Timestamp);
				command.Parameters.AddWithValue("@Day", photo.Day);
				command.Parameters.AddWithValue("@Altitude", photo.Altitude);
				command.Parameters.AddWithValue("@Lng", photo.Lng);
				command.Parameters.AddWithValue("@Lat", photo.Lat);
				command.Parameters.AddWithValue("@CameraLng", photo.CameraLng);
				command.Parameters.AddWithValue("@CameraLat", photo.CameraLat);
				command.Parameters.AddWithValue("@Pitch", photo.Pitch);
				command.Parameters.AddWithValue("@Zoom", photo.Zoom);
				command.Parameters.AddWithValue("@Bearing", photo.Bearing);
				command.Parameters.AddWithValue("@Enabled", photo.Enabled);
				command.Parameters.AddWithValue("@Key", ID);
				command.ExecuteNonQuery();
			} catch (Exception e) {
				_logger.LogError(e.ToString());
			return req.CreateResponse(HttpStatusCode.InternalServerError);
			}
			return req.CreateResponse(HttpStatusCode.OK);
		}

		private readonly ILogger _logger;

		public function_photos(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<function_photos>();
		}

	}
}
