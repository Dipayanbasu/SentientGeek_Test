using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SentientGeek_assesment.Models
{
    public class EmployeeDataContext : DbContext
    {
        public EmployeeDataContext()
              : base("name=ConString")
        {
        }
        public DbSet<Employee> employees { get; set; }
    }
}