using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.MedicineDomain
{
    public class Result
    {
        public List<string> spl_product_data_elements { get; set; }
        public List<string> active_ingredient { get; set; }
        public List<string> purpose { get; set; }
        public List<string> indications_and_usage { get; set; }
        public List<string> warnings { get; set; }
        public List<string> do_not_use { get; set; }
        public List<string> ask_doctor { get; set; }
        public List<string> ask_doctor_or_pharmacist { get; set; }
        public List<string> stop_use { get; set; }
        public List<string> pregnancy_or_breast_feeding { get; set; }
        public List<string> keep_out_of_reach_of_children { get; set; }
        public List<string> dosage_and_administration { get; set; }
        public List<string> storage_and_handling { get; set; }
        public List<string> inactive_ingredient { get; set; }
        public List<string> questions { get; set; }
        public List<string> package_label_principal_display_panel { get; set; }

        public string set_id { get; set; }
        public string effective_time { get; set; }
        public string version { get; set; }

        public OpenFda openFda { get; set; }
    }

}
