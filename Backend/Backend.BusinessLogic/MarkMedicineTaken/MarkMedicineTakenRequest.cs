using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.MarkMedicineTaken
{
    public class MarkMedicineTakenRequest : IRequest<MarkMedicineTakenResponse>
    {
        public string UserId { get; set; }
        public string MedicineName { get; set; }
        public string TimeOfDay { get; set; } // "Morning", "Noon", "Evening"
        public bool IsTaken { get; set; } = true;
    }
}
