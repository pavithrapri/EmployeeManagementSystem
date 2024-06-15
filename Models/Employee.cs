using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DemoEmployeeDb.Models 
{
    public class Employee
    {
        public string Id { get; set; }
        public string DType { get; set; }
        public string CivilId { get; set; }
        public string FileNumber { get; set; }
        public string FullName { get; set; }
        public string JobName { get; set; }
        public string GeneralDepartment { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Address { get; set; }
        public string Tell { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Photo { get; set; }
        public List<Vacation> Vacations { get; set; }
        public List<Overtime> Overtimes { get; set; }
        public List<Doctor> Doctors { get; set; }
        public List<Sanction> Sanctions { get; set; }
    }
}
