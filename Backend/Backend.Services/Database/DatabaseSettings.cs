using Backend.DataAbstraction.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Database
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get ; set ; }
        public string DatabaseName { get; set; }
    }
}
