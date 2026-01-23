using Backend.CommonDomain;

namespace Backend.DataAbstraction
{
  public interface IDjangoService
  {
    public Task<UserMedicineDTO> GetMedicinesFromImage(byte[] imageBytes);
  }
}
