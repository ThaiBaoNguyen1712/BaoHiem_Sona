using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;

namespace BaoHiem_Sona.Controllers
{
   
    public class HomeController : BaseController
    {
      
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            // Tắt cache buộc refresh khi load lại trang
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            long id_user = (long)Session["User_ID"];
            var user = db.SinhVien.FirstOrDefault(x => x.ID_User == id_user);

            if (user != null && user.MaSV != null && user.HoSV != null && user.TenSV != null)
            {
                Session["MSSV"] = user.MaSV;
                Session["HoTen"] = user.HoSV + " " + user.TenSV;
            }

            var tb = db.ThongBaoGui.Where(x => x.ID_User == id_user).OrderByDescending(x=>x.ID).ToList();
            var checkAdmin = db.User.FirstOrDefault(x => x.ID == id_user);

          
            if (checkAdmin.ChucNang == "admin")
            {
                Session["IsAdmin"] = true;
            }
            else
            {
                // Nếu không phải admin, đặt Session["IsAdmin"] về null để tránh lỗi
                Session["IsAdmin"] = null;
            }

            return View(tb);
        }

        public ActionResult ChiTiet(int id)
        {
            var tb = db.ThongBao.FirstOrDefault(x => x.ID == id);
            return View(tb);
        }

        public ActionResult Info()
        {
            long idUser = (long)Session["User_ID"];
             var ttsv = db.SinhVien.FirstOrDefault(x=>x.ID_User== idUser);
            if (ttsv != null)
            {
                var BHYT_Infor = db.BHYT
                 .Where(x => x.ID_SV == ttsv.ID)
                 .OrderByDescending(x => x.NgayBatDau.HasValue)
                 .ThenByDescending(x => x.NgayBatDau)
                 .ToList();

                var BHTN_Infor = db.BHTN
                    .Where(x => x.ID_SV == ttsv.ID)
                    .OrderByDescending(x => x.NgayBatDau.HasValue)
                    .ThenByDescending(x => x.NgayBatDau)
                    .ToList();

                if (BHYT_Infor.Count() > 0)
                {
                    ViewBag.BHYT = BHYT_Infor;
                }

                if (BHTN_Infor.Count() > 0)
                {
                    ViewBag.BHTN = BHTN_Infor;
                }
            }
            return View(ttsv);
        }
        public ActionResult Edit()
        {
            long idsv = (long)Session["User_ID"];
            var ttsv = db.SinhVien.FirstOrDefault(x => x.ID_User == idsv);

            return View(ttsv);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SinhVien sv, HttpPostedFileBase Image)
        {
                SinhVien usv = db.SinhVien.FirstOrDefault(x => x.ID == sv.ID);

                if (usv != null)
                {
                    // Cập nhật thông tin SinhVien
                    usv.MaSV = sv.MaSV;
                    usv.HoSV = sv.HoSV;
                    usv.TenSV = sv.TenSV;
                    usv.GioiTinh = sv.GioiTinh;
                    usv.CCCD = sv.CCCD;
                    usv.NgaySinh = sv.NgaySinh;
                    usv.BHYT_DaCap = sv.BHYT_DaCap;

                    if (Image != null && Image.ContentLength > 0)
                    {
                        long id = sv.ID;
                        string _FileName = "HSSV_" + id.ToString() + Path.GetExtension(Image.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Upload/Students"), _FileName);

                        Image.SaveAs(_path);
                        usv.Image = _FileName;
                    }

                    ThongTinLienHe uttlh = db.ThongTinLienHe.FirstOrDefault(x => x.ID == usv.Contact_ID);

                    if (uttlh != null)
                    {
                        // Cập nhật thông tin ThongTinLienHe
                        uttlh.Tinh_TP = sv.ThongTinLienHe.Tinh_TP;
                        uttlh.Quan_Huyen = sv.ThongTinLienHe.Quan_Huyen;
                        uttlh.Phuong_Xa = sv.ThongTinLienHe.Phuong_Xa;
                        uttlh.SoNha = sv.ThongTinLienHe.SoNha;
                        uttlh.Email = sv.ThongTinLienHe.Email;
                        uttlh.SDT = sv.ThongTinLienHe.SDT;
                    }

                    db.SaveChanges();
                 
                }
            return RedirectToAction("Info");
        }

        public ActionResult ThongBao()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public JsonResult ChangeStatusTB(int id)
        {
            long id_user = (long)Session["User_ID"];
            var tbg = db.ThongBaoGui.FirstOrDefault(x => x.ID_ThongBao == id && x.ID_User== id_user);
            tbg.isRead = "Đã đọc";
            db.SaveChanges();
            return Json(new { isRead = tbg.isRead });
        }
    
        public JsonResult countNoti()
        {
            var tbCount = GetUnreadNotificationCount();
            return Json(tbCount, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetBtnUpdate()
        {
            var setting = db.Setting.FirstOrDefault(x => x.ID == 1);
            return Json(setting.Allow, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DoiMK(string txtPW, string txtPW1,string txtPW2)
        {
            if(txtPW1.Trim()==txtPW2.Trim())
            {
                long userId = (long)Session["User_ID"];
                var checkpass = db.User.FirstOrDefault(x => x.ID == userId);
                if (txtPW.Trim() == checkpass.Password.Trim())
                {
                    checkpass.Password = txtPW1.Trim();
                    db.SaveChanges();
                    ViewBag.Status = "Đổi thành công!";
                 
                }
                else
                {
                    ViewBag.Status = "Mật khẩu bạn nhập chưa đúng!";
                }
            }
            else
            {
                ViewBag.Status = "Mật khẩu nhập lại chưa đúng!";
            }
            var result = new { Status = ViewBag.Status };

            return Json(result);
        }
    }
}