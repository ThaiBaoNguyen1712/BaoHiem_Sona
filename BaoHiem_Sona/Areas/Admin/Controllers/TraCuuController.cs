using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;
namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class TraCuuController : RoleAdminController
    {
        BHYTEntities db = new BHYTEntities();
        // GET: Admin/TraCuu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TraCuuBaoHiem()
        {
            ViewBag_Lop();
            ViewBag_khoa();
            ViewBag_NamHoc();
            ViewBag_TinhTrang();
            return View();
        }
        [HttpPost]
        public ActionResult TraCuuBaoHiem(string SelectedInsurance, string Status, string NamHoc, int? Khoa_ID, int? Lop_ID)
        {
            if (SelectedInsurance == "bhyt")
            {
                var query = db.BHYT.Where(x => x.SinhVien.ID_Lop == Lop_ID && x.SinhVien.Lop.ID_Khoa == Khoa_ID && x.SinhVien.);
                if (NamHoc != null && NamHoc != "")
                {
                    string[] namHocParts = NamHoc.Split('-');
                    DateTime start = DateTime.ParseExact(namHocParts[0].Trim(), "yyyy", CultureInfo.InvariantCulture);
                    DateTime end = DateTime.ParseExact(namHocParts[1].Trim(), "yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => x.SinhVien.Lop.NamBatDau <= start && x.SinhVien.Lop.NamKetThuc >= end);
                }
            
                if (Status != null)
                {
                    // Đặt múi giờ là múi giờ của Việt Nam
                    TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                    DateTime today = TimeZoneInfo.ConvertTime(DateTime.Now, vnTimeZone);

                    if (Status == "ConHan")
                    {
                        query = query.Where(x => x.NgayKetThuc.HasValue && x.NgayKetThuc < today);
                    }
                    else if(Status== "SapHet")
                    {

                        query = query.Where(x =>x.NgayKetThuc.HasValue && x.NgayKetThuc.Value.AddMonths(1) < today);
                    }
                    else if(Status == "HetHan")
                    {
                        query=query.Where(x=>x.NgayKetThuc.HasValue && x.NgayKetThuc > today);
                    }    
                }

                ViewBag.BH = query;
            }
                else if (SelectedInsurance == "bhyt")
                {
                    var query = db.BHYT.Where(x => x.SinhVien.ID_Lop == Lop_ID && x.SinhVien.Lop.ID_Khoa == Khoa_ID && x.SinhVien.);
                    if (NamHoc != null && NamHoc != "")
                    {
                        string[] namHocParts = NamHoc.Split('-');
                        DateTime start = DateTime.ParseExact(namHocParts[0].Trim(), "yyyy", CultureInfo.InvariantCulture);
                        DateTime end = DateTime.ParseExact(namHocParts[1].Trim(), "yyyy", CultureInfo.InvariantCulture);
                        query = query.Where(x => x.SinhVien.Lop.NamBatDau <= start && x.SinhVien.Lop.NamKetThuc >= end);
                    }

                    if (Status != null)
                    {
                        // Đặt múi giờ là múi giờ của Việt Nam
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                        DateTime today = TimeZoneInfo.ConvertTime(DateTime.Now, vnTimeZone);

                        if (Status == "ConHan")
                        {
                            query = query.Where(x => x.NgayKetThuc.HasValue && x.NgayKetThuc < today);
                        }
                        else if (Status == "SapHet")
                        {

                            query = query.Where(x => x.NgayKetThuc.HasValue && x.NgayKetThuc.Value.AddMonths(1) < today);
                        }
                        else if (Status == "HetHan")
                        {
                            query = query.Where(x => x.NgayKetThuc.HasValue && x.NgayKetThuc > today);
                        }
                    }

                ViewBag.BH = query;
            }

            return View();
           
        }



    }
}