using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BaoHiem_Sona.Models;
namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class RoleAdminController : Controller
    {
        // GET: RoleAdmin
        BHYTEntities db = new BHYTEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["User_ID"] == null || Session["IsAdmin"] == null)
            {
                filterContext.Result = new RedirectToRouteResult
                    (new RouteValueDictionary(new { area = "Admin", controller = "LoginForAdmin", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }
        [HttpPost]
        public JsonResult GetLopList_byKhoa(int? selectedKhoa, string startYear, string endYear)
        {
            try
            {
                DateTime? start = null;
                DateTime? end = null;
                if (!string.IsNullOrEmpty(startYear) && !string.IsNullOrEmpty(endYear))
                {
                    start = DateTime.ParseExact(startYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                    end = DateTime.ParseExact(endYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                }

                var query = db.Lop.AsQueryable();

                if (selectedKhoa.HasValue)
                {
                    query = query.Where(x => x.ID_Khoa == selectedKhoa);
                }

                if (start.HasValue && end.HasValue)
                {
                    query = query.Where(x => x.NamBatDau.HasValue && x.NamKetThuc.HasValue &&
                                         x.NamBatDau.Value.Year <= start.Value.Year && x.NamKetThuc.Value.Year >= end.Value.Year);
                }

                var LopList = query.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.TenLop
                }).ToList();

                return Json(LopList);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ModelState.AddModelError("startYear", "Đã xảy ra lỗi khi chuyển đổi năm.");
                return Json(new { error = "Đã xảy ra lỗi khi chuyển đổi năm." });
            }
        }

        [HttpPost]
        public JsonResult GetLopList_byNamHoc(string startYear, string endYear, int? selectedKhoa)
        {
            try
            {
                DateTime? start = null;
                DateTime? end = null;

                if (!string.IsNullOrEmpty(startYear) && !string.IsNullOrEmpty(endYear))
                {
                    start = DateTime.ParseExact(startYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                    end = DateTime.ParseExact(endYear.Trim(), "yyyy", CultureInfo.InvariantCulture);
                }

                var query = db.Lop.AsQueryable();

                if (selectedKhoa.HasValue)
                {
                    query = query.Where(x => x.ID_Khoa == selectedKhoa);
                }

                if (start.HasValue && end.HasValue)
                {
                    query = query.Where(x => x.NamBatDau.HasValue && x.NamKetThuc.HasValue &&
                                         x.NamBatDau.Value.Year <= start.Value.Year && x.NamKetThuc.Value.Year >= end.Value.Year);
                }

                var LopList = query.Select(x => new SelectListItem
                {
                    Value = x.ID.ToString(),
                    Text = x.TenLop
                }).ToList();

                return Json(LopList);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ModelState.AddModelError("startYear", "Đã xảy ra lỗi khi chuyển đổi năm.");
                return Json(new { error = "Đã xảy ra lỗi khi chuyển đổi năm." });
            }
        }
        public void ViewBag_khoa(int? selectedid = null)
        {
            ViewBag.Khoa = new SelectList(db.Khoa.ToList(), "ID", "TenKhoa", selectedid);
        }
        public void ViewBag_Lop(int? selectedid = null)
        {
            ViewBag.Lop = new SelectList(db.Lop.ToList(), "ID", "TenLop", selectedid);
        }
        public void ViewBag_NamHoc(int? selectedid = null)
        {
            var namhocFromDb = db.NamHoc.ToList();

            var namhoc = namhocFromDb
               .Where(x => x.StartYear.HasValue && x.EndYear.HasValue)
               .OrderByDescending(x => x.StartYear.Value.Year)
               .Select(x => $"{x.StartYear.Value.Year} - {x.EndYear.Value.Year}")
               .ToList();
            ViewBag.NamHoc = new SelectList(namhoc, selectedid);
        }
        public void ViewBag_TinhTrang(int? selectedid = null)
        {
            ViewBag.TinhTrang = new SelectList(db.TinhTrang.Where(x => new[] { 1, 2, 3, 5, 6, 7 }.Contains(x.ID)).ToList(), "ID", "TenTinhTrang", selectedid);
        }

    }

}