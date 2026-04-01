using Backend.CommonDomain;
using Backend.DataAbstraction;
using System.Text.Json.Serialization;

namespace Backend.Services
{
  public class DjangoResponse
  {
    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("total_lines")]
    public int TotalLines { get; set; }

    [JsonPropertyName("positive_lines")]
    public int PositiveLines { get; set; }

    [JsonPropertyName("results")]
    public List<DjangoMedicineResult> Results { get; set; }
  }

  public class DjangoMedicineResult
  {
    [JsonPropertyName("drug")]
    public string Drug { get; set; }

    [JsonPropertyName("dose")]
    public string Dose { get; set; }

    [JsonPropertyName("raw_line")]
    public string RawLine { get; set; }
  }
  public class DjangoService : IDjangoService
  {
    private readonly HttpClient _http;

    public DjangoService()
    {
      _http = new HttpClient();
    }

    public async Task<UserMedicineDTO> GetMedicinesFromImage(byte[] imageBytes)
    {
      var content = new ByteArrayContent(imageBytes);
      content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

      var response = await _http.PostAsync("http://localhost:8000/api/predict_by_byte", content);

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception("Django API failed");
      }

      var json = await response.Content.ReadAsStringAsync();

      var djangoResponse = System.Text.Json.JsonSerializer.Deserialize<DjangoResponse>(json, new System.Text.Json.JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      return new UserMedicineDTO
      {
        Date = DateTime.Now,
        DoctorName = "Auto-detected",
        PatientName = "Unknown",
        Medicines = djangoResponse.Results.Select(MapToMedicine).ToList()
      };
    }

    private MedicineDTO MapToMedicine(DjangoMedicineResult r)
    {
      var parts = r.Dose.Split('/');

      return new MedicineDTO
      {
        MedicineName = r.Drug,
        CountMorning = int.Parse(parts[0]),
        CountAfterNon = int.Parse(parts[1]),
        CountNight = int.Parse(parts[2]),
        StartDate = DateTime.Now,
        EndDate = DateTime.Now.AddDays(7)
      };
    }

  }
}
