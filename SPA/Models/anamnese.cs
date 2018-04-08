using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class anamnese
    {
        public long UserId { get; set; }
        public long SchId { get; set; }
        public string Profession { get; set; }
        public string Hobbies { get; set; }
        public Nullable<System.DateTime> Date_1 { get; set; }
        [Required]
        public string Doctor_Name { get; set; }
        public Nullable<System.DateTime> Date_2 { get; set; }
        [Required]
        public string Main_Diagnosis { get; set; }
        public string Secondary_Diagnosis { get; set; }
        public string Main_issues { get; set; }
        public string Symptom_Behaviour { get; set; }
        public string History { get; set; }
        public string Previous_Therapies { get; set; }
        public string Az { get; set; }
        public string Weight { get; set; }
        public string Sicknesses { get; set; }
        public string Surgery { get; set; }
        public string Accidents { get; set; }
        public string Dizziness { get; set; }
        public string Medicine { get; set; }
        public string Social_Situation { get; set; }
        public string Exp_Therapy { get; set; }
        public string Immediate_objective { get; set; }
        public string MidTerm_objective { get; set; }
        public string LongTerm_objective { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
    }
}