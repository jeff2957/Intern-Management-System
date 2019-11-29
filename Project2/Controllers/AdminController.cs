using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project2.Models;
using Project2.Azure_DB;
using System.Net;
using System.Data.Entity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data.Entity.Infrastructure;

namespace Project2.Controllers
{
    public class AdminController : Controller
    {
        private readonly CrsInternServcieEntities1 _db;
        private static string internUrl;
        private static string yearUrl;
        private static string monthUrl;
        public AdminController()
        {
            _db = new CrsInternServcieEntities1();
        }
        // GET: Admin
        public ActionResult AdminIndex(string year, string month, string internName)

        {
            //if (AdminAuthentication())
            //{
                AdminViewModel viewModel = new AdminViewModel();
                viewModel.PartialTask = GetTasks();
                viewModel.PartialSignin = GetSignins();


                if (year != null && month != null && internName != null)
                {
                    var monthNum = Convert.ToInt32(month);
                    var yearNum = Convert.ToInt32(year);

                    var userName = _db.Task.Where(x => x.UserName == internName);
                    var taskResult = userName.Where(x => x.Date.Month == monthNum && x.Date.Year == yearNum).OrderBy(x => x.Date).ToList();

                    var userName1 = _db.Signin.Where(x => x.UserName == internName);
                    var signinResult = userName1.Where(x => x.WorkDate.Month == monthNum && x.WorkDate.Year == yearNum).OrderBy(x => x.WorkDate).ToList();

                    viewModel.PartialTask = taskResult;
                    viewModel.PartialSignin = signinResult;

                    internUrl = internName;
                    yearUrl = year;
                    monthUrl = month;

                    HttpCookie cookie = new HttpCookie("filterCookie");
                    cookie.Values.Add("internUrl", internName);
                    cookie.Values.Add("yearUrl", year);
                    cookie.Values.Add("monthUrl", month);
                    Response.Cookies.Add(cookie);

                    return View(viewModel);
                }

                return View(viewModel);
            //}

            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private List<Task> GetTasks()
        {
            List<Task> tasks = new List<Task>();
            return tasks;
        }

        public List<Signin> GetSignins()
        {
            List<Signin> signins = new List<Signin>();
            return signins;
        }

        public FileContentResult DownloadCSV()
        {
            string csv = "Charlie, Chaplin, Chuckles" + Environment.NewLine  + "abc";
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "InternReport.csv");
        }

        // GET: Admin/worklistEdit/5
        public ActionResult WorklistEdit(int? id)
        {
            //if (AdminAuthentication())
            //{
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Task task = _db.Task.Find(id);
                if (task == null)
                {
                    return HttpNotFound();
                }

                return View(task);
            //}

            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Admin/worklistEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorklistEdit([Bind(Include = "TaskId, TaskName, Duration")] Task task)
        {
            if (ModelState.IsValid)
            {
                task.UserName = internUrl;
                _db.Entry(task).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("AdminIndex", new { year = yearUrl, month = monthUrl, internName = internUrl });
            }
            return View(task);
        }

        // GET: Admin/signinEdit/5
        public ActionResult SigninEdit(int? id)
        {
            //if (AdminAuthentication())
            //{
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Signin signin = _db.Signin.Find(id);

                if (signin == null)
                {
                    return HttpNotFound();
                }
                return View(signin);
            //}
            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Admin/signinEdit/5
        [HttpPost]
        public ActionResult SigninEdit([Bind(Include = "SigninId, WorkDate, ArrivedTime, LeaveTime, CheckValue")] Signin signin)
        {
            if (ModelState.IsValid)
            {
                signin.UserName = internUrl;
                _db.Entry(signin).State = EntityState.Modified;
                _db.Configuration.ValidateOnSaveEnabled = false;
                _db.SaveChanges();
                return RedirectToAction("AdminIndex", new { year = yearUrl, month = monthUrl, internName = internUrl});
            }
            return View(signin);
        }

        // GET: Admin/worklistDelete/5
        public ActionResult WorklistDelete(int? id)
        {
            //if (AdminAuthentication())
            //{
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Task task = _db.Task.Find(id);
                if (task == null)
                {
                    return HttpNotFound();
                }
                return View(task);
            //}

            //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Admin/worklistDelete/5
        [HttpPost, ActionName("WorklistDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult WorklistDeleteConfirmed(int id)
        {
            Task task = _db.Task.Find(id);
            _db.Task.Remove(task);
            _db.SaveChanges();
            return RedirectToAction("AdminIndex", new { year = yearUrl, month = monthUrl, internName = internUrl });
        }

        // GET: Admin/signinDelete/5
        public ActionResult SigninDelete(int? id)
        {
            if (AdminAuthentication())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Signin signin = _db.Signin.Find(id);
                if (signin == null)
                {
                    return HttpNotFound();
                }
                return View(signin);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // POST: Admin/singinDelete/5
        [HttpPost, ActionName("SigninDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SigninDeleteConfirmed(int? id)
        {
            Signin signin = _db.Signin.Find(id);
            _db.Signin.Remove(signin);
            _db.SaveChanges();
            return RedirectToAction("AdminIndex", new { year = yearUrl, month = monthUrl, internName = internUrl });
        }

        public bool AdminAuthentication()
        {
            if (System.Web.HttpContext.Current.User.Identity.Name.Contains("AnYun") || System.Web.HttpContext.Current.User.Identity.Name.Contains("Ryuk"))
            {
                return true;
            }
            return false;
        }

        public void ExportToExcel()
        {
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("InternReport");
            HttpResponse response = System.Web.HttpContext.Current.Response;

            var internName = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["internName"];
            var month = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["month"];
            var year = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["year"];

            ws.Cells["A1:P1"].Merge = true;
            ws.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells["A1:P1"].Value = "雲馥數位科技股份有限公司";
            ws.Cells["A2:I2"].Merge = true;
            ws.Cells["A2:I2"].Value = "簽到表";

            ws.Cells["A3:B3"].Merge = true;
            ws.Cells["C3:I3"].Merge = true;
            ws.Cells["L3:P3"].Merge = true;
            ws.Cells["J3:K3"].Merge = true;
            ws.Cells["A3:B3"].Value = "實習生";
            ws.Cells["C3"].Value = internName;
            ws.Cells["C3:D3"].Value = System.Web.HttpContext.Current.User.Identity.Name;
            ws.Cells["J3:K3"].Value = "指導人";

            ws.Cells["A4:P4"].Merge = true;

            //set all cells text-center
            ws.Cells[1, 1, 39, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 39, 16].Style.Font.Name = "微軟正黑體";

            //prepare excel column
            ws.Cells["A5:B5"].Value = "日期";
            ws.Cells["A5:B5"].Merge = true;
            ws.Cells["A5:B5"].Style.Font.Bold = true;
            ws.Cells["C5:D5"].Merge = true;
            ws.Cells["C5:D5"].Value = "上班簽到時間";
            ws.Cells["C5:D5"].Style.Font.Bold = true;
            ws.Cells["E5:F5"].Merge = true;
            ws.Cells["E5:F5"].Value = "下班簽到時間";
            ws.Cells["E5:F5"].Style.Font.Bold = true;
            ws.Cells["G5"].Value = "確認人";
            ws.Cells["G5"].Style.Font.Bold = true;
            ws.Cells["H5:I5"].Merge = true;
            ws.Cells["H5:I5"].Value = "總計";
            ws.Cells["H5:I5"].Style.Font.Bold = true;
            ws.Cells["G37"].Style.Font.Bold = true;
            ws.Cells["H37:I37"].Merge = true;
            ws.Cells["C39"].Value = "主管簽名:";
            ws.Cells["D39:E39"].Merge = true;
            ws.Cells["F39:G39"].Merge = true;
            ws.Cells["F39:G39"].Value = "實習生簽名:";
            ws.Cells["H39:I39"].Merge = true;
            ws.Cells["J2:P2"].Merge = true;
            ws.Cells["J2"].Value = "工作內容";
            ws.Cells["J5"].Value = "序號";
            ws.Cells["K5:O5"].Merge = true;
            ws.Cells["K5:O5"].Value = "項目";
            ws.Cells["P5"].Value = "學習天";
            ws.Cells["J5:J37"].Style.Border.Left.Style = ExcelBorderStyle.Thick;
            ws.Cells["H37:I37"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells["P37"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            ws.Cells["A2:P2"].Style.Font.Bold = true;
            ws.Cells["J5:P5"].Style.Font.Bold = true;

            //merge C6:D6 to C35:D35
            for (int i = 6; i < 36; i++)
            {
                ws.Cells[string.Format("A{0:G}:B{0:G}", i, i)].Merge = true;
                ws.Cells[string.Format("C{0:G}:D{0:G}", i, i)].Merge = true;
                ws.Cells[string.Format("E{0:G}:F{0:G}", i, i)].Merge = true;
                ws.Cells[string.Format("H{0:G}:I{0:G}", i, i)].Merge = true;
            }

            //merge K5:O5 to K34:O34
            for (int i = 6; i < 36; i++)
            {
                ws.Cells[string.Format("K{0:G}:O{0:G}", i, i)].Merge = true;
            }

            ws.Cells["A6:A35"].AutoFitColumns();

            //print Signin db data to excel
            var nextYearNum = 0;
            if (month == "12")
            {
                nextYearNum = Convert.ToInt32(year) + 1;
            }
            else
            {
                nextYearNum = Convert.ToInt32(year);
            }

            var lastMonthNum = Convert.ToInt32(month) - 1;
            DateTime endDate = new DateTime(nextYearNum, Convert.ToInt32(month), 20, 23, 59, 0);
            DateTime startDate = new DateTime(Convert.ToInt32(year), lastMonthNum, 21, 00, 00, 0);

            var results = _db.Signin.Where(x => x.WorkDate.CompareTo(startDate) >= 0 && x.WorkDate.CompareTo(endDate) < 0 && x.UserName == internName).Distinct().ToList();

            for (int i = 0; i < results.Count(); i++)
            {
                ws.Cells[6 + i, 1].Value = results[i].WorkDate.Date.ToString("yyyy/MM/dd");
                ws.Cells[6 + i, 3].Value = string.Format("{0:00}:{1:00}", results[i].ArrivedTime.Hours, results[i].ArrivedTime.Minutes);
                ws.Cells[6 + i, 5].Value = string.Format("{0:00}:{1:00}", results[i].LeaveTime.Hours, results[i].LeaveTime.Minutes);
                ws.Cells[6 + i, 8].Value = Math.Round(results[i].LeaveTime.TotalHours - results[i].ArrivedTime.TotalHours, 0) - 1.0;
                ws.Cells["H37"].Formula = "=SUM(H6:H36)";

                if (results[i].CheckValue.Equals(true))
                {
                    ws.Cells[6 + i, 7].Value = System.Web.HttpContext.Current.User.Identity.Name;
                }
            }

            //print Worklist db date to excel
            var results1 = _db.Task.Where(x => x.Date.CompareTo(startDate) >= 0 && x.Date.CompareTo(endDate) < 0 && x.UserName == internName).Distinct().ToList();
            for (int i = 0; i < results1.Count(); i++)
            {
                ws.Cells[6 + i, 10].Value = (i+1).ToString();
                ws.Cells[6 + i, 11].Value = results1[i].TaskName;
                ws.Cells[6 + i, 16].Value = (int)results1[i].Duration;
                ws.Cells["P37"].Formula = "=SUM(P6:P36)";
            }



            response.Clear();
            response.AddHeader("content-disposition", "attachment: filename = " + "InternReport.xlsx");
            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.BinaryWrite(pck.GetAsByteArray());
            response.End();
        }

    }
}
