using SentientGeek_assesment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SentientGeek_assesment.Controllers
{
    public class HomeController : Controller
    {
        EmployeeDataContext objDataContext = new EmployeeDataContext();
        public ActionResult Index()
        {
            return View(objDataContext.employees.ToList());
        }

        public ActionResult GetData()
        {
            List<Employee> EmployeeList = objDataContext.employees.ToList();
            return Json(new { data = EmployeeList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult create(Employee objEmp)
        {
            objDataContext.employees.Add(objEmp);
            objDataContext.SaveChanges();
            return Json(new { success = true, message = "Saved Successfully", JsonRequestBehavior.AllowGet });
        }
        public ActionResult Edit(string id)
        {
            int empId = Convert.ToInt32(id);
            var emp = objDataContext.employees.Find(empId);
            return View(emp);
        }
        [HttpPost]
        public ActionResult Edit(Employee objEmp)
        {
            var data = objDataContext.employees.Find(objEmp.EmpId);
            if (data != null)
            {
                data.EmpName = objEmp.EmpName;
                data.Address = objEmp.Address;
                data.Emailid = objEmp.Emailid;
                data.MobileNo = objEmp.MobileNo;
            }
            objDataContext.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully", JsonRequestBehavior.AllowGet });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == 0)
                return View(new Employee());
            else
            {
                return View(objDataContext.employees.Find(id));
            }
        }

        [HttpPost]
        public ActionResult Delete(Employee obj)
        {
            var emp = objDataContext.employees.Find(obj.EmpId);
            objDataContext.employees.Remove(emp);
            objDataContext.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully", JsonRequestBehavior.AllowGet });
        }
       
        [HttpPost]
        public ActionResult Deletebulk(string[] empids)
        {
            if (empids != null)
            {
                foreach (string ids in empids)
                {
                    Employee obj = objDataContext.employees.Find(Convert.ToInt64(ids));
                    objDataContext.employees.Remove(obj);
                }
                objDataContext.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully", JsonRequestBehavior.AllowGet });
            }
            else {
                return Json(new { success = true, message = "No value Selected", JsonRequestBehavior.AllowGet });
            }
        }
    }
}