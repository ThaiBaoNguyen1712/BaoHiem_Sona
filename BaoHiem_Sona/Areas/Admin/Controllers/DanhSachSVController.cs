using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using DrawingColor = System.Drawing.Color;
using System.Web.Mvc;
using BaoHiem_Sona.Models;
using BaoHiem_Sona.Common;
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using ExcelDataReader.Log;

namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class DanhSachSVController : RoleAdminController
    {
        // GET: DanhSachSV
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            ViewBag_khoa();
            ViewBag_Lop();
            ViewBag_NamHoc();
            var listClass = db.Lop.ToList();
            return View(listClass);
        }
        [HttpPost]
        public ActionResult Index(int? Khoa_ID, string NamHoc, int? Lop_ID)
        {
            var query = db.Lop.AsQueryable();
            if (NamHoc != null && NamHoc != "")
            {
                string[] namHocParts = NamHoc.Split('-');
                DateTime start = DateTime.ParseExact(namHocParts[0].Trim(), "yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(namHocParts[1].Trim(), "yyyy", CultureInfo.InvariantCulture);
                query = query.Where(x => x.NamBatDau <= start && x.NamKetThuc >= end);
            }
            // Lọc theo khoa nếu Khoa_ID không rỗng
            if (Khoa_ID != null && Khoa_ID != 0)
            {
                query = query.Where(x => x.ID_Khoa == Khoa_ID);
            }
            // Lọc theo Lop_ID nếu Lop_ID không rỗng
            if (Lop_ID != null && Lop_ID != 0)
            {
                query = query.Where(x => x.ID == Lop_ID);
            }
           
            // Lưu giá trị đã chọn để sử dụng trong JavaScript
            ViewBag.SelectedNamHoc = NamHoc;
            ViewBag.SelectedKhoa = Khoa_ID;

            var listClass_Select = query.ToList();
            ViewBag_khoa();
            ViewBag_Lop();
            ViewBag_NamHoc();
            return View(listClass_Select);
        }

        public ActionResult DSSV(int id)
        {
            var query = db.SinhVien.Where(x => x.ID_Lop == id).OrderBy(x => x.TenSV);
            ViewBag_TinhTrang();
            Session["ID_Class"] = id;
            return View(query.ToList());
        }
        [HttpPost]
        public ActionResult DSSV(int? tinhtrang)
        {
            var query = db.SinhVien.AsQueryable(); // Bắt đầu truy vấn từ bảng Lớp

            if (tinhtrang != null)
            {
                query = query.Where(x => x.ID_TinhTrang == tinhtrang); // Lọc theo khoa nếu khoa không rỗng
            }

            var listClass_Select = query.ToList(); // Thực hiện truy vấn và lấy danh sách lớp

            ViewBag_TinhTrang();

            return View(listClass_Select);
        }


        public ActionResult Edit(string id)
        {
            var SV = db.SinhVien.FirstOrDefault(x => x.MaSV == id);
            if (SV != null)
            {
                var BHYT_Infor = db.BHYT.Where(x => x.ID_SV == SV.ID).OrderByDescending(x => x.NgayBatDau).ToList();
                var BHTN_Infor = db.BHTN.Where(x => x.ID_SV == SV.ID).OrderByDescending(x => x.NgayBatDau).ToList();

                if (BHYT_Infor.Count() > 0)
                {
                    ViewBag.BHYT = BHYT_Infor;
                }

                if (BHTN_Infor.Count() > 0)
                {
                    ViewBag.BHTN = BHTN_Infor;
                }
            }

            ViewBag_Lop(SV.ID_Lop);
            ViewBag_TinhTrang(SV.ID_TinhTrang);
            ViewBag_khoa(SV.Lop.Khoa.ID);
            ViewBag_NamHoc();
            return View(SV);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SinhVien sv, HttpPostedFileBase Image, string ID_BHYT, string ID_BHTN, string MaBHYT, string MaBHTN, string NgayHieuLuc_BHYT,
            string NgayHetHan_BHYT, string NgayHieuLuc_BHTN, string NgayHetHan_BHTN)
        {
                // Lấy dữ liệu SinhVien từ cơ sở dữ liệu
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
                    usv.ID_TinhTrang = sv.TinhTrang.ID;
                    usv.ID_Lop = sv.Lop.ID;
                    usv.BHYT_DaCap=sv.BHYT_DaCap;
                    if (Image != null && Image.ContentLength > 0)
                    {
                        long id = sv.ID;

                        string _FileName = "";
                        int Index = Image.FileName.IndexOf(".");
                        _FileName = "HSSV_" + id.ToString() + "." + Image.FileName.Substring(Index + 1);
                        string _path = Path.Combine(Server.MapPath("~/Upload/Students"), _FileName);
                        Image.SaveAs(_path);
                        usv.Image = _FileName;

                    }
                    // Lấy dữ liệu ThongTinLienHe từ cơ sở dữ liệu
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

                    if (MaBHYT != null && NgayHieuLuc_BHYT != null && NgayHetHan_BHYT != null)
                    {
                        long parsedID_BHYT = Convert.ToInt64(ID_BHYT);
                        var BHYT = db.BHYT.FirstOrDefault(x => x.ID == parsedID_BHYT);
                        if (BHYT != null)
                        {
                            BHYT.MaTheBHYT = MaBHYT;
                            BHYT.NgayBatDau = Convert.ToDateTime(NgayHieuLuc_BHYT);
                            BHYT.NgayKetThuc = Convert.ToDateTime(NgayHetHan_BHYT);
                        }

                    }

                    if (MaBHTN != null && NgayHieuLuc_BHTN != null && NgayHetHan_BHTN != null)
                    {
                        long parsedID_BHTN = Convert.ToInt64(ID_BHTN);
                        var BHTN = db.BHTN.FirstOrDefault(x => x.ID == parsedID_BHTN);
                        if (BHTN != null)
                        {
                            BHTN.MaTheBHTN = MaBHTN;
                            BHTN.NgayBatDau = Convert.ToDateTime(NgayHieuLuc_BHTN);
                            BHTN.NgayKetThuc = Convert.ToDateTime(NgayHetHan_BHTN);
                        }
                    }
                    // Lưu các thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                    return RedirectToAction("DSSV", new { id = Session["ID_Class"] });

                }
            return View("Error","Admin");
        }


        public ActionResult Create()
        {
           
            int idClass = (int)Session["ID_Class"];
            var lop = db.Lop.FirstOrDefault(x => x.ID == idClass);
            ViewBag_Lop(lop.ID);
            ViewBag_khoa(lop.ID_Khoa);
            ViewBag_NamHoc();
            return View(lop);
        }
        [HttpPost]
        public ActionResult Create(SinhVien sv, ThongTinLienHe ttlh, HttpPostedFileBase Image)
        {
            ViewBag_Lop();
            db.ThongTinLienHe.Add(ttlh);
            db.SaveChanges();
            int id = ttlh.ID;
            sv.Contact_ID = id;
            sv.ID_TinhTrang = 4;
            Session["ID_Class"] = sv.ID_Lop;
            db.SinhVien.Add(sv);
            db.SaveChanges();

            if (Image != null && Image.ContentLength > 0)
            {
                string maSV = db.SinhVien.ToList().Last().MaSV;
                long id_student = sv.ID;

                string _FileName = "";
                int Index = Image.FileName.IndexOf(".");
                _FileName = "HSSV_" + maSV.ToString() + "." + Image.FileName.Substring(Index + 1);
                string _path = Path.Combine(Server.MapPath("~/Upload/Students"), _FileName);
                Image.SaveAs(_path);

                SinhVien usv = db.SinhVien.FirstOrDefault(x => x.ID == id_student);
                usv.Image = _FileName;
                db.SaveChanges();
            }
            return RedirectToAction("DSSV", new { id = Session["ID_Class"] });
        }

        // XÓA SINH VIÊN
        [HttpPost]
        public ActionResult Delete(int id)
        {
            SinhVien sv = db.SinhVien.FirstOrDefault(x => x.ID == id);
            if (sv != null)
            {
                int contactID = (int)sv.Contact_ID; // Lấy Contact_ID từ SinhVien

                // Xóa ThongTinLienHe dựa trên Contact_ID
                ThongTinLienHe contact = db.ThongTinLienHe.FirstOrDefault(x => x.ID == contactID);
                if (contact != null)
                {
                    db.ThongTinLienHe.Remove(contact);
                }
                //xóa giao dịch
                var trans = db.GiaoDich.Where(x => x.ID_SV == id).ToList();
                if (trans != null)
                {
                    foreach (var i in trans)
                    {
                        db.GiaoDich.Remove(i);
                    }
                }
                // xóa hết bảo hiểm
                var _BHYT = db.BHYT.Where(x => x.ID_SV == id).ToList();
                var _BHTN = db.BHTN.Where(x => x.ID_SV == id).ToList();
                if (_BHYT != null)
                {
                    foreach (var i in _BHYT)
                    {
                        db.BHYT.Remove(i);
                    }
                }
                if (_BHTN != null)
                {
                    foreach (var i in _BHTN)
                    {
                        db.BHTN.Remove(i);
                    }
                }
                // Xóa SinhVien
                db.SinhVien.Remove(sv);
                db.SaveChanges();

                return RedirectToAction("DSSV", new { id = Session["ID_Class"] });
            }
            else
            {
                // Xử lý trường hợp id không hợp lệ
                return RedirectToAction("Error","Admin");
            }
        }

        public ActionResult batchSVUpload()
        { return View(); }


        public ActionResult ExcelFileReader()
        { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcelFileReader(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                // Xử lý lỗi nếu không có tệp được chọn
                return View();
            }

            using (var package = new ExcelPackage(file.InputStream))
            {
                var excelData = new List<ExcelRowData>();
                var excelErrorData = new List<ExcelRowData>();
                var warningList = new List<string>();

                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    int rowIndex = 11; // Bắt đầu từ 11 để bỏ qua hàng tiêu đề

                    while (worksheet.Cells[rowIndex, 1].Value != null)
                    {
                        var rowData = new ExcelRowData
                        {
                            MaSV = worksheet.Cells[rowIndex, 2].Value?.ToString().Trim(),
                            HoSV = worksheet.Cells[rowIndex, 3].Value?.ToString().Trim(),
                            TenSV = worksheet.Cells[rowIndex, 4].Value?.ToString().Trim(),
                            GioiTinh = worksheet.Cells[rowIndex, 5].Value?.ToString().Trim(),
                            CCCD = worksheet.Cells[rowIndex, 6].Value?.ToString().Trim(),
                            NgaySinh = worksheet.Cells[rowIndex, 7].Value?.ToString().Trim(),
                            TenLop = worksheet.Cells[rowIndex, 8].Value?.ToString().Trim(),
                            PhuongXa = worksheet.Cells[rowIndex, 9].Value?.ToString().Trim(),
                            QuanHuyen = worksheet.Cells[rowIndex, 10].Value?.ToString().Trim(),
                            TinhThanhPho = worksheet.Cells[rowIndex, 11].Value?.ToString().Trim(),
                        };

                        if (!CheckMaSVQuery(rowData.MaSV))
                        {
                            rowData.Error = "MaSV";
                        }

                        if (!CheckClassQuery(rowData.TenLop))
                        {
                            excelErrorData.Add(rowData);
                        }
                        else
                        {
                            excelData.Add(rowData);
                        }

                        rowIndex++;
                    }
                }

                ViewBag.Error = excelErrorData;
                ViewBag.ExcelData = excelData;
                ViewBag.WarningList = warningList;

                return View();
            }
        }


        private bool CheckMaSVQuery(string maSvValue)
        {

           var CheckMaSV = db.SinhVien.Where(x=>x.MaSV==maSvValue).ToList();
            if(CheckMaSV.Count>0)
            {
                return false;
            }    
            return true;
        }
        [HttpPost]
        public JsonResult CheckMaSVQueryRefresh(string maSvValue)
        {
            bool isMaSVExists = db.SinhVien.Any(x => x.MaSV == maSvValue);

            // Return boolean result directly
            return Json(!isMaSVExists);
        }

        private bool CheckClassQuery(string className)
        {
            int IdClass = (int)Session["ID_Class"];
            var Checkclass = db.Lop.Where(x=>x.TenLop==className&& x.ID==IdClass).ToList();
            if(Checkclass.Count==0)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadSV(List<SinhVien> svList, List<string> Error)
        {
            try { 
            int IdClass = (int)Session["ID_Class"];
            int lopid = db.Lop.FirstOrDefault(x => x.ID == IdClass).ID;

            for (int i = 0; i < svList.Count; i++)
            {
                var item = svList[i];
                var err = Error[i];

                if (string.IsNullOrEmpty(err))
                {
                    var SV = new SinhVien
                    {
                        MaSV = item.MaSV,
                        HoSV = item.HoSV,
                        TenSV = item.TenSV,
                        GioiTinh = item.GioiTinh,
                        ID_Lop = lopid,
                        CCCD = item.CCCD,
                        ID_TinhTrang = 4,
                        NgaySinh = Convert.ToDateTime(item.NgaySinh)
                    };

                    var ttlh = new ThongTinLienHe
                    {
                        Phuong_Xa = item.ThongTinLienHe.Phuong_Xa,
                        Quan_Huyen = item.ThongTinLienHe.Quan_Huyen,
                        Tinh_TP = item.ThongTinLienHe.Tinh_TP
                    };

                    db.ThongTinLienHe.Add(ttlh);
                    db.SaveChanges(); // Lưu để có ID của ThongTinLienHe

                    int id = ttlh.ID;
                    SV.Contact_ID = id;
                    db.SinhVien.Add(SV);
                }
            }

            db.SaveChanges(); // Lưu thay đổi của tất cả SinhVien
                ViewBag.Error = null;
                ViewBag.ExcelData = null;
                return RedirectToAction("DSSV", new { id = Session["ID_Class"] });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Error", "Admin");
            }
        }

        [HttpPost]
        public ActionResult ExportToExcel(Lop lop,string NamHoc)
        {
            List<Lop> classes = new List<Lop>();
          
            if (lop.ID != null && lop.ID !=0)
            {
                var singleClass = db.Lop.FirstOrDefault(x => x.ID == lop.ID);
                if (singleClass != null)
                {
                    classes.Add(singleClass);
                }
            }
            else if(lop.ID_Khoa != null && NamHoc != null && NamHoc != "")
            {
                string[] namHocParts = NamHoc.Split('-');
                DateTime start = DateTime.ParseExact(namHocParts[0].Trim(), "yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(namHocParts[1].Trim(), "yyyy", CultureInfo.InvariantCulture);
                classes = db.Lop.Where(x => x.NamBatDau.HasValue && x.NamKetThuc.HasValue &&
                x.NamBatDau.Value.Year <= start.Year && x.NamKetThuc.Value.Year >= end.Year && x.ID_Khoa==lop.ID_Khoa).ToList();
            }    
            else if (lop.ID_Khoa != null)
            {
                classes = db.Lop.Where(x => x.ID_Khoa == lop.ID_Khoa).ToList();
            }
            else if (NamHoc !=null && NamHoc !="")
            {
                string[] namHocParts = NamHoc.Split('-');
                DateTime start = DateTime.ParseExact(namHocParts[0].Trim(), "yyyy", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(namHocParts[1].Trim(), "yyyy", CultureInfo.InvariantCulture);
                classes = db.Lop.Where(x => x.NamBatDau.HasValue && x.NamKetThuc.HasValue &&
                 x.NamBatDau.Value.Year <= start.Year && x.NamKetThuc.Value.Year >= end.Year).ToList();
            }
            else
            {
                return HttpNotFound("Vui lòng nhập thông tin");
            }

            if (classes.Count > 0)
            {
                var stream = new MemoryStream();
                using (var xlPackage = new ExcelPackage(stream))
                {
                    foreach (var selectedClass in classes)
                    {
                        var SV = db.SinhVien
                            .Where(x => x.ID_Lop == selectedClass.ID)
                            .OrderBy(x => x.TenSV)
                            .ToList();

                        var worksheet = xlPackage.Workbook.Worksheets.Add(selectedClass.TenLop);

                        // Header styling
                        var headerStyle = worksheet.Cells["A1:K1"].Style;
                        headerStyle.Font.Color.SetColor(DrawingColor.Green);
                        headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerStyle.Fill.BackgroundColor.SetColor(DrawingColor.Lavender);

                        // Headers
                        worksheet.Cells["A1"].Value = "DANH SÁCH HỌC SINH _ SINH VIÊN TRƯỜNG SONADEZI";
                        worksheet.Cells["A2"].Value = "Lớp" + " " + selectedClass.TenLop;

                        worksheet.Cells["A10:K10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells["A10:K10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                     



                        // Column headers
                        worksheet.Cells["A10"].Value = "STT";
                        worksheet.Cells["B10"].Value = "MaSV";
                        worksheet.Cells["C10"].Value = "Họ và tên lót";
                        worksheet.Cells["D10"].Value = "Tên SV";
                        worksheet.Cells["E10"].Value = "Giới tính";
                        worksheet.Cells["F10"].Value = "CCCD";
                        worksheet.Cells["G10"].Value = "Ngày Sinh";
                        worksheet.Cells["H10"].Value = "Lớp";
                        worksheet.Cells["I10"].Value = "Phường/Xã";
                        worksheet.Cells["J10"].Value = "Quận/Huyện";
                        worksheet.Cells["K10"].Value = "Tỉnh/Thành phố";

                        int row = 11;
                        int i = 1;
                        foreach (var item in SV)
                        {
                            // Populate the worksheet with data
                            worksheet.Cells[row, 1].Value = i++;
                            worksheet.Cells[row, 2].Value = item.MaSV;
                            worksheet.Cells[row, 3].Value = item.HoSV;
                            worksheet.Cells[row, 4].Value = item.TenSV;
                            worksheet.Cells[row, 5].Value = item.GioiTinh;
                            worksheet.Cells[row, 6].Value = item.CCCD;
                            worksheet.Cells[row, 7].Value = Convert.ToDateTime(item.NgaySinh).ToString("dd/MM/yyyy");
                            worksheet.Cells[row, 8].Value = item.Lop.TenLop;
                            worksheet.Cells[row, 9].Value = item.ThongTinLienHe.Phuong_Xa;
                            worksheet.Cells[row, 10].Value = item.ThongTinLienHe.Quan_Huyen;
                            worksheet.Cells[row, 11].Value = item.ThongTinLienHe.Tinh_TP;

                            for (int col = 1; col <= 11; col++)
                            {
                                worksheet.Cells[row, col].AutoFitColumns();
                            }

                            row++;
                        }
                    }

                    xlPackage.Workbook.Properties.Title = "Danh sách SV";
                    xlPackage.Workbook.Properties.Author = "Administrator";
                    xlPackage.Save();
                }

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSach_HS_SV.xlsx");
            }

            return HttpNotFound("Không tìm thấy lớp phù hợp");
        }
    }
}