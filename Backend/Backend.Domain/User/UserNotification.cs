using Backend.CommonDomain;

namespace Backend.Domain.User
{
  public class UserNotification
  {
    public List<MedicineDTO> Notifications { get; set; } = new List<MedicineDTO>();
  }
 
}
