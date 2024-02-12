using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;
namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class DanhSachLopController : RoleAdminController
    {
        // GET: DanhSachLop
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            var lop = db.Lop.ToList();
            ViewBag_khoa();
            ViewBag_NamHoc();
            ViewBag_Lop();
            return View(lop);
        }
        [HttpPost]
        public ActionResult CreateLop(Lop lop,string startYear,string endYear)
        {
            if (!string.IsNullOrEmpty(startYear) && !string.IsNullOrEmpty(endYear))
            {
               DateTime start = DateTime.ParseExact(startYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
               DateTime end = DateTime.ParseExact(endYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                lop.NamBatDau = start;
                lop.NamKetThuc = end;
            }
            if (lop.HeDaoTao == "none")
            {
                lop.HeDaoTao = null;
            }
            db.Lop.Add(lop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
     
        public ActionResult Edit(int id)
        {
            var lop = db.Lop.FirstOrDefault(x => x.ID == id);
            ViewBag_khoa(lop.ID_Khoa);
            return View(lop);
        }
        [HttpPost]
        public ActionResult Edit(Lop lop,string startYear, string endYear)
        {
            var uLop = db.Lop.FirstOrDefault(x=>x.ID == lop.ID);
            uLop.TenLop = lop.TenLop;
            uLop.TenNganh= lop.TenNganh;
            if(lop.HeDaoTao!="none")
            {
                uLop.HeDaoTao = lop.HeDaoTao;
            }    
         
            if (!string.IsNullOrEmpty(startYear) && !string.IsNullOrEmpty(endYear))
            {
                DateTime start = DateTime.ParseExact(startYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(endYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                uLop.NamBatDau = start;
                uLop.NamKetThuc = end;
            }
            uLop.ID_Khoa=lop.Khoa.ID;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult DeleteLop(int id)
        {
            var lop = db.Lop.FirstOrDefault(x => x.ID == id);
            var hocsinh = db.SinhVien.Where(x => x.ID_Lop == id);

            if (lop != null)
            {
                if (hocsinh.Count() > 0)
                {
                    foreach (var item in hocsinh)
                    {
                        db.SinhVien.Remove(item);
                    }

                    db.SaveChanges(); 
                }

                db.Lop.Remove(lop);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return Redirect("Error");
        }
    }
}