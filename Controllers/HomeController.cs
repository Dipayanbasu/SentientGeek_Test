using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using SentientGeek_assesment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SentientGeek_assesment.Controllers
{
    public class HomeController : Controller
    {
        string mailid = System.Web.Configuration.WebConfigurationManager.AppSettings["emailid"].ToString();
        string emailpassword = System.Web.Configuration.WebConfigurationManager.AppSettings["emailpassword"].ToString();
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
            else
            {
                return Json(new { success = true, message = "No value Selected", JsonRequestBehavior.AllowGet });
            }
        }
      
        #region [Export related section]

        public ActionResult Export()
        {
            string filename = ExportToPdf();
            if (!filename.Contains("error :"))
            {
                string path = Path.Combine(Server.MapPath("~/Files/"), filename);
                if (System.IO.File.Exists(path))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(path);
                    System.IO.File.Delete(path);
                    return File(bytes, "application/octet-stream", filename);
                }
                else
                {
                    TempData["error"] = "File not found.";
                    return RedirectToAction("index");
                }
            }
            else
            {
                TempData["error"] = filename;
                return RedirectToAction("index");
            }

        }

        public string ExportToPdf()
        {
            try
            {
                List<Employee> EmployeeList = objDataContext.employees.ToList();
                DataTable dt = ToDataTable(EmployeeList);
                Document document = new Document();
                string filename = "EmployeeList_" + DateTime.Now.ToString("dd_MM_yy_fff") + ".pdf";
                string filepath = Path.Combine(Server.MapPath("~/Files/"), filename);

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filepath, FileMode.Create));
                document.Open();
                iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.COURIER_BOLD, 7);
                iTextSharp.text.Font font4 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
                PdfPTable table = new PdfPTable(dt.Columns.Count);

                //float[] widths = new float[] {};
                //table.SetWidths(widths);
                table.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell(new Phrase("Employee List", font5));
                cell.Colspan = dt.Columns.Count;
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                foreach (DataColumn c in dt.Columns)
                {
                    //table.AddCell(new Phrase(c.ColumnName, font5));
                    PdfPCell cl = new PdfPCell(new Phrase(c.ColumnName, font5));
                    cl.BackgroundColor = new BaseColor(System.Drawing.Color.Cyan);
                    table.AddCell(cl);
                }

                foreach (DataRow r in dt.Rows)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            table.AddCell(new Phrase(r[i].ToString(), font4));
                        }

                    }
                }
                document.Add(table);
                document.Close();
                return filename;
            }

            catch (Exception ex)
            {
                return "error : " + ex.Message.ToString();
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public ActionResult ExporttoExcel()
        {
            string filename = Exportexcel();
            if (!filename.Contains("error :"))
            {
                string path = Path.Combine(Server.MapPath("~/Files/"), filename);
                if (System.IO.File.Exists(path))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(path);
                    System.IO.File.Delete(path);
                    return File(bytes, "application/octet-stream", filename);
                }
                else
                {
                    TempData["error"] = "File not found.";
                    return RedirectToAction("index");
                }
            }
            else
            {
                TempData["error"] = filename;
                return RedirectToAction("index");
            }

        }

        public string Exportexcel()
        {
            try
            {
                List<Employee> data = objDataContext.employees.ToList();

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromCollection(data, true);
                int totalCols = workSheet.Dimension.End.Column;
                var headerCells = workSheet.Cells[1, 1, 1, totalCols];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightCyan);
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                string filename = "EmployeeList_" + DateTime.Now.ToString("dd_MM_yy_fff") + ".xlsx";
                string filepath = Path.Combine(Server.MapPath("~/Files/"), filename);

                //Write the file to the disk
                FileInfo fi = new FileInfo(filepath);
                excel.SaveAs(filepath);
                return filename;
            }

            catch (Exception ex)
            {
                return "error : " + ex.Message.ToString();
            }

        }

        #endregion
        public bool sendMail(string gMailAccount, string password, string from, string to, string subject, string message, string attachmentpath)
        {
            MailMessage msg = new MailMessage();
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            try
            {
                NetworkCredential loginInfo = new NetworkCredential(gMailAccount, password);
            
                msg.From = new MailAddress(from);
                msg.To.Add(new MailAddress(to));
                //msg.CC.Add(new MailAddress(ccmail));
                msg.Subject = subject;
                msg.Body = message;
                msg.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(attachmentpath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentpath);
                    msg.Attachments.Add(attachment);
                }
               
                //SmtpClient client = new SmtpClient("relay-hosting.secureserver.net", 25);
                client.EnableSsl = true;
                client.UseDefaultCredentials = true;
                client.Credentials = loginInfo;
                client.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally {
                msg.Attachments.Dispose();
                client.Dispose();
            }

        }

        public ActionResult exportsendmail()
        {
            mailsend ms = new mailsend();
            return View(ms);
        }
        [HttpPost]
        public ActionResult exportsendmail(mailsend ms)
        {
            string filepath = "";
            if (ms.reporttype == "pdf") { filepath = ExportToPdf(); }
            else { filepath = Exportexcel(); }
            filepath = Path.Combine(Server.MapPath("~/Files/"), filepath);
            if (sendMail(mailid, emailpassword, mailid, ms.emailid, "Employee List", "Please find the attachment.", filepath) == true)
            {
                System.IO.File.Delete(filepath);
                return Json(new { success = true, message = "Mail Sent Successfully", JsonRequestBehavior.AllowGet });
            }
            else
            {
                System.IO.File.Delete(filepath);
                return Json(new { success = false, message = "Mail Sent Failed.", JsonRequestBehavior.AllowGet });
            }
        }
    }
}