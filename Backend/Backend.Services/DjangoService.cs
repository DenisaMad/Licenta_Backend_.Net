using Backend.CommonDomain;
using Backend.DataAbstraction;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services
{
  public class DjangoService : IDjangoService
  {
    public Task<UserMedicineDTO> GetMedicinesFromImage(byte[] imageBytes)
    {
      //API CALL LA DJANGO->TRIMITE IMAGE BYTES->PRIMIM UserMedicineDTO
      return Task.FromResult(new UserMedicineDTO
      {
        Date = DateTime.Now,
        DoctorName = "Doctor-Chat-Gpt-license",
        PatientName = "Patient-00",
        Medicines = new List<MedicineDTO>
        {
          new MedicineDTO{
            MedicineName = "Paracetamol",
            CountAfterNon = 1,
            CountMorning = 1,
            CountNight = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5)
          },
          new MedicineDTO{
            MedicineName = "Ibuprofen",
            CountAfterNon = 0,
            CountMorning = 1,
            CountNight = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
          },
          new MedicineDTO{
            MedicineName = "Amoxicillin",
            CountAfterNon = 1,
            CountMorning = 0,
            CountNight = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10)
        },
          new MedicineDTO{
            MedicineName = "Cetirizine",
            CountAfterNon = 1,
            CountMorning = 1,
            CountNight = 0,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14)
          }
        }
      });
    }
  }
}
