using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project2.Azure_DB;

namespace Project2.Controllers
{
    public class SigninController : Controller
    {
        private readonly CrsInternServcieEntities1 _db;

        public SigninController()
        {
            _db = new CrsInternServcieEntities1();
        }

        // GET: Signin
        public ActionResult Index(string year, string month)
        {
            if (System.Web.HttpContext.Current.User.Identity.Name != null)
            {
                string un = System.Web.HttpContext.Current.User.Identity.Name;
                var result = _db.Signin.Where(x => x.UserName == un).OrderBy(x => x.WorkDate);

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
                    var userName = _db.Signin.Where(x => x.UserName == un);
                    var result1 = userName.Where(x => x.WorkDate.CompareTo(startDate) >= 0 && x.WorkDate.CompareTo(endDate) < 0).OrderBy(x => x.WorkDate).ToList();
                    return View(result1);
                }
                return View(result);
            }

                return HttpNotFound();
        }

        // GET: Signin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Signin/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "SigninId, WorkDate, ArrivedTime, LeaveTime, UserName")] Signin signin)
        {
            if (ModelState.IsValid)
            {
                signin.UserName = System.Web.HttpContext.Current.User.Identity.Name;
                _db.Signin.Add(signin);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(signin);
        }

        // GET: Signin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (SigninAuthentication(id))
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
        

        // POST: Signin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Signin signin = _db.Signin.Find(id);
            _db.Signin.Remove(signin);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Signin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (SigninAuthentication(id))
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

        // POST: Signin/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "SigninId, WorkDate, ArrivedTime, LeaveTime")] Signin signin)
        {
            if (ModelState.IsValid)
            {                
                _db.Entry(signin).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(signin);
        }

        public bool SigninAuthentication(int? id)
        {
            var name = _db.Signin.Where(x => x.SigninId == id).Select(x => x.UserName).SingleOrDefault().ToString();
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