using Project2.Azure_DB;
using Project2.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;

namespace Project2.Controllers
{

    public class WorkListController : Controller
    {
        private readonly CrsInternServcieEntities1 _db;

        public WorkListController()
        {
            _db = new CrsInternServcieEntities1();
        }

        [HttpGet]
        public ActionResult Index(string year, string month)
        {            
            if (System.Web.HttpContext.Current.User.Identity.Name != null)
            {               
                string un = System.Web.HttpContext.Current.User.Identity.Name;                                              
                var result = _db.Task.Where(x => x.UserName == un).OrderBy(x => x.Date);

                if (month != null)
                {
                    var lastMonth = Convert.ToInt32(month) - 1;
                    var nextYear = 0;
                    if (month == "12")
                    {
                        nextYear =+ 1;
                    }
                    else
                    {
                        nextYear = Convert.ToInt32(year);
                    }
                    DateTime endDate = new DateTime(nextYear, Convert.ToInt32(month), 20, 23, 59, 59);
                    DateTime startDate = new DateTime(Convert.ToInt32(year), lastMonth, 21, 0, 0, 0);
                    var yearNum = Convert.ToInt32(year);
                    var monthNum = Convert.ToInt32(month);
                    var userName = _db.Task.Where(x => x.UserName == un);
                    var result1 = userName.Where(x => x.Date.CompareTo(startDate) >= 0 && x.Date.CompareTo(endDate) < 0).OrderBy(x => x.Date).ToList();
                    return View(result1);
                }
                return View(result);
            }

            return HttpNotFound();
        }

        // GET: WorkList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WorkList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskName, Duration, UserName")]Task task)
        {
            if (ModelState.IsValid)
            {
                task.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                task.Date = DateTime.Now.Date;
                _db.Task.Add(task);
                _db.SaveChanges();
                return RedirectToAction("Index", "WorkList");
            }

            return View(task);
        }

        // GET: WorkList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (TaskAuthentication(id))
            {
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
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: WorkList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = _db.Task.Find(id);
            _db.Task.Remove(task);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: WorkList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (TaskAuthentication(id))
            {
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
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: WorkList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId, TaskName, Duration")] Task task)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(task).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        public bool TaskAuthentication(int? id)
        {
            var name = _db.Task.Where(x => x.TaskId == id).Select(x => x.UserName).SingleOrDefault().ToString();
            if (name == System.Web.HttpContext.Current.User.Identity.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}