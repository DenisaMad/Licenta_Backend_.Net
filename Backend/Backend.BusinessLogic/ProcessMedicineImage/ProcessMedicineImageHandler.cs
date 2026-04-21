using Backend.DataAbstraction;
using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.ProcessMedicineImage
{
    public class ProcessMedicineImageHandler : IRequestHandler<ProcessMedicineImageRequest, ProcessMedicineImageResponse>
    {
        private IDjangoService _djangoService;
        private readonly IMongoDataBase mongoDatabase;
        public ProcessMedicineImageHandler(IDjangoService djangoService, IMongoDataBase mongoDatabase)
        {
            _djangoService = djangoService;
            this.mongoDatabase = mongoDatabase;
        }
        public async Task<ProcessMedicineImageResponse> Handle(ProcessMedicineImageRequest request, CancellationToken cancellationToken)
        {
            var bytes = await ExtractBytesFromImage(request);
            var dto = await this._djangoService.GetMedicinesFromImage(bytes);

            if (dto != null && dto.Medicines != null)
            {
                foreach (var medicine in dto.Medicines)
                {
                    medicine.TakenMorning = medicine.CountMorning == 0;
                    medicine.TakenNoon = medicine.CountAfterNon == 0;
                    medicine.TakenEvening = medicine.CountNight == 0;
                }
            }

            var user = await this.mongoDatabase.GetCollection<Domain.User.User>().Find(Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.UserId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            user.UserMedicine = dto;
            await this.mongoDatabase.GetCollection<Domain.User.User>().ReplaceOneAsync(Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.UserId), user, cancellationToken: cancellationToken);
            return new ProcessMedicineImageResponse
            {
                Success = true
            };
        }
        private async Task<byte[]> ExtractBytesFromImage(ProcessMedicineImageRequest request)
        {
            var file = request.File;
            Console.WriteLine($"FileName: {file.FileName}");
            Console.WriteLine($"ContentType: {file.ContentType}");
            Console.WriteLine($"Length: {file.Length} bytes");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
