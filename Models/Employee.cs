using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SentientGeek_assesment.Models
{
    public class Employee
    {
        [Key]
        public Int64 EmpId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(100)]
        public string EmpName { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is Required")]
        [MaxLength(300)]
        public string Address { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email ID is Required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(100)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect Email Format")]
        public string Emailid { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "MobileNo is Required")]
        [MaxLength(20)]
        public string MobileNo { get; set; }
    }

    public class mailsend
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email ID is Required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(100)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect Email Format")]
        public string emailid { get; set; }

        [Display(Name = "Report Type")]
        public string reporttype { get; set; }
    }
}