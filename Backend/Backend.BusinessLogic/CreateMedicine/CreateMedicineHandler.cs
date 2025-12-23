using Backend.DataAbstraction.Security;
using Backend.Domain.MedicineDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.CreateMedicine
{
    public  class CreateMedicineHandler
    {
        private readonly IMongoDataBase _db;
        private static readonly HttpClient _http = new();

        public CreateMedicineHandler(IMongoDataBase db)
        {
            _db = db;
        }

        public async Task<CreateMedicineResponse> Handle(CreateMedicineRequest request, CancellationToken cancellationToken)
        {
            var medicine = await GetResultsAsync(request.Name);

            var medicineLabel = new MedicineLabel
            {
                Labels = medicine,
                MedicineName = request.Name,
                Prescription = new Prescription
                {
                    NumberOfCapsules = 3,
                    NumberOfTimesPerDay = 3
                }
            };

            await _db.GetCollection<MedicineLabel>().InsertOneAsync(medicineLabel, cancellationToken: cancellationToken);

            return new CreateMedicineResponse
            {
                MedicineName = request.Name,
                MedicineData = medicine,
                Success = true
            };
        }

        private static async Task<Medicine> GetResultsAsync(string name)
        {
            var url = $"https://api.fda.gov/drug/label.json?search=openfda.substance_name:%22{name.ToUpper()}%22&limit=5";

            var response = await _http.GetFromJsonAsync<Medicine>(
                url,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return response!;
        }
    }
}
