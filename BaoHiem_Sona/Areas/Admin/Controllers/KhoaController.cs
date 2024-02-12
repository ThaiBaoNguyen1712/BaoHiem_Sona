using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;
namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class KhoaController : RoleAdminController
    {
        // GET: Admin/Khoa
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            var khoa = db.Khoa.ToList();
            return View(khoa);
        }
        public JsonResult Edit(int id)
        {
            var khoa = db.Khoa
                .Where(x => x.ID == id)
                .Select(x => new { x.ID, x.TenKhoa })
                .FirstOrDefault();

            return Json(khoa, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(Khoa khoa)
        {
            var ukhoa = db.Khoa.FirstOrDefault(x=>x.ID == khoa.ID);
            ukhoa.TenKhoa=khoa.TenKhoa;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Create(Khoa khoa)
        {
            if(khoa ==null)
            {
                return View("Error");
            }
            db.Khoa.Add(khoa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var khoa = db.Khoa.FirstOrDefault(x => x.ID == id);

            // Kiểm tra xem có lớp nào thuộc khoa này không
            var lop = db.Lop.Where(x => x.ID_Khoa == id).ToList();

            if (lop.Any())
            {
                foreach (var i in lop)
                {
                    // Gán ID_Khoa = null cho từng lớp
                    i.ID_Khoa = null;
                }
            }

            db.Khoa.Remove(khoa);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}