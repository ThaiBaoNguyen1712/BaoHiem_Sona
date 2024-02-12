using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;

namespace BaoHiem_Sona.Controllers
{
    public class TraCuuBaoHiemController : Controller
    {
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string MaBH, string Select, string MaSV)
        {
            if(Select == "bhyt")
            {
                var bhyt = db.BHYT.FirstOrDefault(x => x.MaTheBHYT.Trim() == MaBH.Trim() && x.SinhVien.MaSV.Trim() == MaSV.Trim());
                if(bhyt != null)
                {
                    ViewBag.TTBH = bhyt;
                    ViewBag.MaThe = bhyt.MaTheBHYT;
                }
                else
                {
                    ViewBag.Status = "Không có dữ liệu. Vui lòng kiểm tra lại !";
                }

            }
            else if(Select == "bhtn")
            {
                var bhtn = db.BHTN.FirstOrDefault(x => x.MaTheBHTN.Trim() == MaBH.Trim() && x.SinhVien.MaSV.Trim() == MaSV.Trim());
                if (bhtn != null)
                {
                    ViewBag.TTBH = bhtn;
                    ViewBag.MaThe = bhtn.MaTheBHTN;
                }
                else
                {
                    ViewBag.Status = "Không có dữ liệu. Vui lòng kiểm tra lại !";
                }

            }
          
            return View();
        }
    }
}