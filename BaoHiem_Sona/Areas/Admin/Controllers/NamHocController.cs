using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;


namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class NamHocController : RoleAdminController
    {
        // GET: Admin/NamHoc
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            var namhoc = db.NamHoc.OrderByDescending(x=>x.StartYear).ToList();
            return View(namhoc);
        }
        [HttpPost]
        public ActionResult Create(string StartYearHand)
        {
            try
            {
                // Chuyển đổi chuỗi năm thành đối tượng DateTime
                DateTime startYear = DateTime.ParseExact(StartYearHand, "yyyy", CultureInfo.InvariantCulture);

                // Tạo đối tượng NamHoc và thiết lập StartYear và EndYear
                NamHoc namhoc = new NamHoc()
                {
                    StartYear = startYear,
                    EndYear = startYear.AddYears(1)
                };

                // Thêm vào cơ sở dữ liệu và lưu thay đổi
                db.NamHoc.Add(namhoc);
                db.SaveChanges();

                // Chuyển hướng đến trang Index hoặc nơi bạn muốn
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ModelState.AddModelError("StartYearHand", "Đã xảy ra lỗi khi chuyển đổi năm.");
                return View(); // hoặc trả về view với thông báo lỗi
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var nam= db.NamHoc.FirstOrDefault(x=>x.ID==id);
            if (nam!=null)
            {
                db.NamHoc.Remove(nam);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}