using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.User
{
    public enum EUserRole
    {
        [Description("Client")]
        client,
        [Description("Admin")]
        admin,
        [Description("Developer")]
        developer

    }
}
