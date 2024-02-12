using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using DrawingColor = System.Drawing.Color;
using System.Web.Mvc;
using ExcelDataReader;
using PagedList;
using BaoHiem_Sona.Models;
using Microsoft.Ajax.Utilities;
using System.Globalization;
using BaoHiem_Sona.Common;

namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class DanhSachPhatController : RoleAdminController
    {
        // GET: DanhSachSV
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            ViewBag_khoa();
            ViewBag_NamHoc();
            ViewBag_Lop();
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
            var students = db.SinhVien.Where(x => x.ID_Lop == id && (x.ID_TinhTrang == 1
            || x.ID_TinhTrang == 2 || x.ID_TinhTrang == 3
            || x.ID_TinhTrang == 5 || x.ID_TinhTrang == 6 || x.ID_TinhTrang == 7)).OrderBy(x => x.TenSV).ToList();

            var ID_students = students.Select(s => s.ID).ToList();
            var bhytExist = db.BHYT.Where(x => ID_students.Contains((int)x.ID_SV)).Select(x => x.MaTheBHYT).ToList();
            var bhtnExist = db.BHTN.Where(x => ID_students.Contains((int)x.ID_SV)).Select(x => x.MaTheBHTN).ToList();
            var IDstudent_BHYT = db.BHYT.Select(x => x.ID_SV).ToList();
            var IDstudent_BHTN = db.BHTN.Select(x => x.ID_SV).ToList();

            ViewBag.Students = students;
            ViewBag_TinhTrang();
            Session["ID_Class"] = id;
            return View();
        }
        [HttpPost]
        public ActionResult DSSV(int? tinhtrang)
        {
            var id = (int)Session["ID_Class"]; // Lấy ID_Class từ Session
            var query = db.SinhVien.Where(x => x.ID_Lop == id);

            if (tinhtrang != null)
            {
                query = query.Where(x => x.ID_TinhTrang == tinhtrang);
            }
            else
            {
                query = query.Where(x => x.ID_TinhTrang == 1
                    || x.ID_TinhTrang == 2 || x.ID_TinhTrang == 3
                    || x.ID_TinhTrang == 5 || x.ID_TinhTrang == 6 || x.ID_TinhTrang == 7).OrderBy(x => x.TenSV).AsQueryable();
            }


            var ID_students = query.Select(s => s.ID).ToList();
            var bhytExist = db.BHYT.Where(x => ID_students.Contains((int)x.ID_SV)).Select(x => x.MaTheBHYT).ToList();
            var bhtnExist = db.BHTN.Where(x => ID_students.Contains((int)x.ID_SV)).Select(x => x.MaTheBHTN).ToList();
            var IDstudent_BHYT = db.BHYT.Select(x => x.ID_SV).ToList();
            var IDstudent_BHTN = db.BHTN.Select(x => x.ID_SV).ToList();

            ViewBag.Students = query.ToList();
            ViewBag_TinhTrang();
            return View();
        }


        public ActionResult Provide_BHYT()
        {
            int id = (int)Session["ID_Class"];
            var students = db.SinhVien.Where(x => x.ID_Lop == id && (x.ID_TinhTrang == 1 || x.ID_TinhTrang == 3 || x.ID_TinhTrang == 6)).OrderBy(x => x.TenSV).ToList();

            return View(students);
        }
        [HttpPost]
        public ActionResult Provide_BHYT(List<int> studentID, List<string> BHYTCode, DateTime? ThoiGianBatDau)
        {
            for (int i = 0; i < studentID.Count; i++)
            {
                int id = studentID[i];
                string code = BHYTCode[i];
                if (id != 0 && code != "")
                {
                    //lưu mã thẻ
                    var _bhyt = db.BHYT.FirstOrDefault(x => x.ID_SV == id && x.MaTheBHYT==null);
                    _bhyt.MaTheBHYT = code;

                    // Lấy thời hạn đăng ký để tính toán thẻ có hiệu lực & hết hiêu lực
                    var giaodich = db.GiaoDich.FirstOrDefault(x => x.ID_SV == id);

                    if (ThoiGianBatDau != null)
                    {
                        if (giaodich != null)
                        {
                            if (giaodich.ThoiHanDangKy_BHYT != null)
                            {
                                string combinedExpire = string.Join(" ", giaodich.ThoiHanDangKy_BHYT);


                                // Sử dụng LINQ để trích xuất số
                                int months = int.Parse(new string(combinedExpire.Where(char.IsDigit).ToArray()));


                                _bhyt.NgayBatDau = Convert.ToDateTime(ThoiGianBatDau);
                                _bhyt.NgayKetThuc = Convert.ToDateTime(ThoiGianBatDau).AddMonths(months);
                                _bhyt.NgayCap = DateTime.Now;
                            }
                        }
                    }

                    //check mã thẻ BHYT và BHTN đã tồn tại hay chưa
                    var checkBHTN_Exist = db.BHTN.Where(x => x.ID_SV == id && string.IsNullOrEmpty(x.MaTheBHTN)).Any();
                    var checkBHTN_Valid = db.BHTN.Where(x => x.ID_SV == id && string.IsNullOrEmpty(x.MaTheBHTN)).Any();

                    //Chuyển trạng thái sinh viên
                    var student = db.SinhVien.FirstOrDefault(x => x.ID == id);
                    if (student.ID_TinhTrang == 1 || student.ID_TinhTrang == 6)
                    {
                        student.ID_TinhTrang = 5;
                    }
                    else if (student.ID_TinhTrang == 3 && checkBHTN_Exist && !checkBHTN_Valid)
                    {
                        student.ID_TinhTrang = 5;
                    }
                    else if (student.ID_TinhTrang == 3 && checkBHTN_Valid)
                    {
                        student.ID_TinhTrang = 7;
                    }
                    else
                    {
                        return RedirectToAction("Error", "Admin");
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DSSV", new { id = (int)Session["ID_Class"] });
        }
        public ActionResult Provide_BHTN()
        {
            int id = (int)Session["ID_Class"];
            var students = db.SinhVien.Where(x => x.ID_Lop == id && (x.ID_TinhTrang == 2 || x.ID_TinhTrang == 3 || x.ID_TinhTrang == 7)).OrderBy(x => x.TenSV).ToList();

            return View(students);
        }
        [HttpPost]
        public ActionResult Provide_BHTN(List<int> studentID, List<string> BHTNCode, DateTime? ThoiGianBatDau)
        {
            for (int i = 0; i < studentID.Count; i++)
            {
                int id = studentID[i];
                string code = BHTNCode[i];
                if (id != 0 && code != "")
                {
                    //lưu mã thẻ
                    var _bhtn = db.BHTN.FirstOrDefault(x => x.ID_SV == id && x.MaTheBHTN== null);
                    _bhtn.MaTheBHTN = code;

                    // Lấy thời hạn đăng ký để tính toán thẻ có hiệu lực & hết hiêu lực
                    var giaodich = db.GiaoDich.FirstOrDefault(x => x.ID_SV == id);

                    if (ThoiGianBatDau != null)
                    {
                        if (giaodich != null)
                        {
                            if (giaodich.ThoiHanDangKy_BHTN != null)
                            {
                                string combinedExpire = string.Join(" ", giaodich.ThoiHanDangKy_BHTN);


                                // Sử dụng LINQ để trích xuất số
                                int months = int.Parse(new string(combinedExpire.Where(char.IsDigit).ToArray()));


                                _bhtn.NgayBatDau = Convert.ToDateTime(ThoiGianBatDau);
                                _bhtn.NgayKetThuc = Convert.ToDateTime(ThoiGianBatDau).AddMonths(months);
                                _bhtn.NgayCap = DateTime.Now;
                            }
                        }
                    }
                    //check mã thẻ BHYT và BHTN đã tồn tại hay chưa
                    var checkBHYT_Exist = db.BHYT.Where(x => x.ID_SV == id).Any();
                    var checkBHYT_Valid = db.BHYT.Where(x => x.ID_SV == id && string.IsNullOrEmpty(x.MaTheBHYT)).Any();
                    //Chuyển trạng thái sinh viên
                    var student = db.SinhVien.FirstOrDefault(x => x.ID == id);
                    if (student.ID_TinhTrang == 2 || student.ID_TinhTrang == 7)
                    {
                        student.ID_TinhTrang = 5;
                    }
                    else if (student.ID_TinhTrang == 3 && checkBHYT_Exist && !checkBHYT_Valid)
                    {
                        student.ID_TinhTrang = 5;
                    }
                    else if (student.ID_TinhTrang == 3 && checkBHYT_Valid)
                    {
                        student.ID_TinhTrang = 6;
                    }
                    else
                    {
                        return RedirectToAction("Error", "Admin");
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("DSSV", new { id = (int)Session["ID_Class"] });
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

            return View(SV);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SinhVien sv, HttpPostedFileBase Image, string ID_BHYT, string ID_BHTN, string MaBHYT, string MaBHTN, string NgayHieuLuc_BHYT,
             string NgayHetHan_BHYT, string NgayHieuLuc_BHTN, string NgayHetHan_BHTN)
        {
            if (ModelState.IsValid)
            {
                // Lấy dữ liệu SinhVien từ cơ sở dữ liệu
                SinhVien usv = db.SinhVien.FirstOrDefault(x => x.ID == sv.ID);

                if (usv != null)
                {
                    // Cập nhật thông tin SinhVien
                    usv.ID_TinhTrang = sv.TinhTrang.ID;

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
                else
                {
                    return RedirectToAction("Error", "Admin"); // Xử lý khi không tìm thấy SinhVien
                }
            }

            // Nếu ModelState không hợp lệ, trả về view chỉnh sửa để người dùng sửa lại.
            return View(sv);
        }


        public ActionResult Create()
        {
            ViewBag_Lop();
            ViewBag_khoa();
            ViewBag_NamHoc();
            return View();
        }
        [HttpPost]
        public ActionResult Create(SinhVien sv, ThongTinLienHe ttlh, HttpPostedFileBase Image)
        {
            ViewBag_Lop();
            db.ThongTinLienHe.Add(ttlh);
            db.SaveChanges();
            int id = ttlh.ID;
            sv.Contact_ID = id;
            sv.ID_TinhTrang = 2;
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

        public ActionResult Delete_BHYT(int id, string Ma_SV)
        {
            var bhyt = db.BHYT.FirstOrDefault(x => x.SinhVien.MaSV == Ma_SV);

            if (bhyt != null)
            {
                bhyt.MaTheBHYT = null;
                bhyt.NgayBatDau = null;
                bhyt.NgayKetThuc = null;
                var sv = db.SinhVien.FirstOrDefault(x => x.MaSV == Ma_SV);
                sv.ID_TinhTrang = 1;
                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = Ma_SV.Trim() });
        }
        public ActionResult Delete_BHTN(int id, string Ma_SV)
        {
            var bhtn = db.BHTN.FirstOrDefault(x => x.ID == id && x.SinhVien.MaSV == Ma_SV);

            if (bhtn != null)
            {
                bhtn.MaTheBHTN = null;
                bhtn.NgayBatDau = null;
                bhtn.NgayKetThuc = null;
                var sv = db.SinhVien.FirstOrDefault(x => x.MaSV == Ma_SV);
                sv.ID_TinhTrang = 2;

                db.SaveChanges();
            }

            // Quay lại trang Edit/masoSV

            return RedirectToAction("Edit", new { id = Ma_SV.Trim() });
        }

        public ActionResult LichSuCapThe()
        {
           
            var lichsu_BHYT = db.BHYT.Where(x => !string.IsNullOrEmpty(x.MaTheBHYT)).OrderByDescending(x => x.NgayCap).ToList();
            var lichsu_BHTN = db.BHTN.Where(x => !string.IsNullOrEmpty(x.MaTheBHTN)).OrderByDescending(x => x.NgayCap).ToList();

            ViewBag.BHYT = lichsu_BHYT.ToList();
            ViewBag.BHTN = lichsu_BHTN.ToList();

            return View();
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
                        var maSV = worksheet.Cells[rowIndex, 2].Value?.ToString().Trim();
                        var infoSV = db.SinhVien.FirstOrDefault(x => x.MaSV == maSV);
                        var rowData = new ExcelRowData
                        {
                            MaSV = worksheet.Cells[rowIndex, 2].Value?.ToString().Trim(),
                            HoSV = infoSV.HoSV,
                            TenSV = infoSV.TenSV,
                            MaBHYT = worksheet.Cells[rowIndex, 5].Value?.ToString().Trim(),
                            MaBHTN = worksheet.Cells[rowIndex, 6].Value?.ToString().Trim(),
                            GioiTinh = infoSV.GioiTinh,
                            CCCD = infoSV.CCCD,
                            NgaySinh = Convert.ToDateTime(infoSV.NgaySinh).ToString("dd/MM/yyyy"),
                            TenLop = infoSV.Lop.TenLop,
                            LoaiBH = worksheet.Cells[rowIndex, 10].Value?.ToString().Trim(),
                            ThoiHanBHYT = worksheet.Cells[rowIndex, 12].Value?.ToString().Trim(),
                            NgayHieuLuc_BHYT= worksheet.Cells[rowIndex, 13].Value?.ToString().Trim(),
                            ThoiHanBHTN = worksheet.Cells[rowIndex, 14].Value?.ToString().Trim(),
                            NgayHieuLuc_BHTN = worksheet.Cells[rowIndex, 15].Value?.ToString().Trim(),
                            GhiChu = worksheet.Cells[rowIndex, 21].Value?.ToString().Trim(),
                        };

                        if (!CheckMaSVQuery(rowData.MaSV))
                        {
                            excelErrorData.Add(rowData);
                        }

                        //if (!CheckClassQuery(rowData.TenLop))
                        //{
                        //    excelErrorData.Add(rowData);
                        //}
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

            var CheckMaSV = db.SinhVien.Where(x => x.MaSV == maSvValue).ToList();
            if (CheckMaSV.Count > 0)
            {
                return true;
            }
            return false;
        }
        [HttpPost]
        public JsonResult CheckMaSVQueryRefresh(string maSvValue)
        {
            bool isMaSVExists = db.SinhVien.Any(x => x.MaSV == maSvValue);

            // Return boolean result directly
            return Json(!isMaSVExists);
        }

        //private bool CheckClassQuery(string className)
        //{
        //    int IdClass = (int)Session["ID_Class"];
        //    var Checkclass = db.Lop.Where(x => x.TenLop == className && x.ID == IdClass).ToList();
        //    if (Checkclass.Count == 0)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCode(List<SinhVien> svList, List<string> Error, List<BHYT> BHYTList, List<string> LoaiBH, List<BHTN> BHTNList, List<GiaoDich> gdList)
        {
            try
            {
                for (int i = 0; i < svList.Count; i++)
                {
                    var itemsv = svList[i];
                    var bhytItem = BHYTList[i];
                    var bhtnItem = BHTNList[i];
                    var itemgd = gdList[i];
                    var err = Error[i];
                    var bh = LoaiBH[i];

                    if (string.IsNullOrEmpty(err))
                    {
                        var student = db.SinhVien.FirstOrDefault(sv => sv.MaSV.Contains(itemsv.MaSV));

                        if (student != null)
                        {
                            var id_SV = student.ID;

                            var bhyt = db.BHYT.FirstOrDefault(x => x.ID_SV == id_SV && x.MaTheBHYT == null);
                            var bhtn = db.BHTN.FirstOrDefault(x => x.ID_SV == id_SV && x.MaTheBHTN == null);

                            if (bhyt != null && bhytItem.MaTheBHYT != null && bhyt.MaTheBHYT == null)
                            {
                                bhyt.MaTheBHYT = bhytItem.MaTheBHYT;
                                if (bhytItem.NgayBatDau != null)
                                {
                                    bhyt.NgayBatDau = Convert.ToDateTime(bhytItem.NgayBatDau);
                                    bhyt.NgayCap = DateTime.Now;
                                    if (itemgd.ThoiHanDangKy_BHYT != null)
                                    {
                                        string combinedExpire = string.Join(" ", itemgd.ThoiHanDangKy_BHYT);
                                        int months_bhyt = int.Parse(new string(combinedExpire.Where(char.IsDigit).ToArray()));
                                        bhyt.NgayKetThuc = Convert.ToDateTime(bhytItem.NgayBatDau).AddMonths(months_bhyt);
                                    }
                                }

                                changeStatus(id_SV);
                                db.SaveChanges();
                            }

                            if (bhtn != null && bhtnItem.MaTheBHTN != null && bhtn.MaTheBHTN == null)
                            {
                                bhtn.MaTheBHTN = bhtnItem.MaTheBHTN;
                                if (bhtnItem.NgayBatDau != null)
                                {
                                    bhtn.NgayBatDau = Convert.ToDateTime(bhtnItem.NgayBatDau);
                                    bhtn.NgayCap = DateTime.Now;
                                    if (itemgd.ThoiHanDangKy_BHTN != null)
                                    {
                                        string combinedExpire_2 = string.Join(" ", itemgd.ThoiHanDangKy_BHTN);
                                        int months_bhtn = int.Parse(new string(combinedExpire_2.Where(char.IsDigit).ToArray()));
                                        bhtn.NgayKetThuc = Convert.ToDateTime(bhtnItem.NgayBatDau).AddMonths(months_bhtn);
                                    }
                                }

                                changeStatus(id_SV);
                                db.SaveChanges();
                            }
                        }
                    }
                }

                db.SaveChanges(); // Lưu thay đổi của tất cả SinhVien
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Error", "Admin");
            }
        }


       
        public void changeStatus(long id_sv)
        {
            try
            {
                var sv = db.SinhVien.FirstOrDefault(x => x.ID == id_sv);
                var bhyt_notExist = db.BHTN.Where(x => x.ID_SV == id_sv && string.IsNullOrEmpty(x.MaTheBHTN)).Any();
                var bhtn_notExist = db.BHTN.Where(x => x.ID_SV == id_sv && string.IsNullOrEmpty(x.MaTheBHTN)).Any();

                if (sv.ID_TinhTrang == 1 || sv.ID_TinhTrang == 6)
                {
                    sv.ID_TinhTrang = 5;
                }
                else if (sv.ID_TinhTrang == 2 || sv.ID_TinhTrang == 7)
                {
                    sv.ID_TinhTrang = 5;
                }
                else if (sv.ID_TinhTrang == 3 && bhtn_notExist)
                {
                    sv.ID_TinhTrang = 7;
                }
                else if (sv.ID_TinhTrang == 3 && bhyt_notExist)
                {
                    sv.ID_TinhTrang = 6;
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("Error", "Admin");
            }
        }

        [HttpPost]
        public ActionResult ExportToExcel(DateTime? ThoiGianBatDau, DateTime? ThoiGianKetThuc)
        {
            var invoices = db.GiaoDich.Where(x => (ThoiGianBatDau == null || x.ThoiGian >= ThoiGianBatDau) && x.ThoiGian <= ThoiGianKetThuc).ToList();
            // Tiếp tục xử lý dữ liệu SV
            var CountBill = invoices.Count();
            var stream = new MemoryStream();
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("SV");

                var customstyle = xlPackage.Workbook.Styles.CreateNamedStyle("CustomStyle");
                customstyle.Style.Font.UnderLine = true;
                customstyle.Style.Font.Color.SetColor(DrawingColor.Red);

                var startRow = 5;
                var row = startRow;

                worksheet.Cells["A1"].Value = "Danh sách bảo hiểm học sinh SONADEZI";
                using (var r = worksheet.Cells["A1:C1"])
                {
                    r.Merge = true;
                    r.Style.Font.Color.SetColor(DrawingColor.Green);
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(DrawingColor.Lavender);

                }
                worksheet.Cells["A10"].Value = "STT";
                worksheet.Cells["B10"].Value = "MaSV";
                worksheet.Cells["C10"].Value = "Họ và tên lót";
                worksheet.Cells["D10"].Value = "Tên SV";
                worksheet.Cells["E10"].Value = "Mã Số BHYT";
                worksheet.Cells["F10"].Value = "Mã Số BHTN";
                worksheet.Cells["G10"].Value = "Giới tính";
                worksheet.Cells["H10"].Value = "Ngày Sinh";
                worksheet.Cells["I10"].Value = "Lớp";
                worksheet.Cells["J10"].Value = "Loại BH";
                worksheet.Cells["K10"].Value = "Ngày đóng phí";
                worksheet.Cells["L10"].Value = "Thời hạn BHYT";
                worksheet.Cells["M10"].Value = "Ngày hiệu lực BHYT";
                worksheet.Cells["N10"].Value = "Thời hạn BHTN";
                worksheet.Cells["O10"].Value = "Ngày hiệu lực BHTN";
                worksheet.Cells["P10"].Value = "Số tiền đóng";
                worksheet.Cells["Q10"].Value = "Phường/Xã";
                worksheet.Cells["R10"].Value = "Quận/Huyện";
                worksheet.Cells["S10"].Value = "Tỉnh/Thành phố";
                worksheet.Cells["T10"].Value = "CCCD";
                worksheet.Cells["U10"].Value = "Tình trạng";
                worksheet.Cells["V10"].Value = "Ghi Chú";
                worksheet.Cells["A10:V10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A10:V10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                worksheet.Cells["A10:V10"].AutoFitColumns();

                row = 11;
                int i = 1;
                foreach (var item in invoices)
                {
                    var BHYT = db.BHYT.FirstOrDefault(x => x.ID_SV == item.SinhVien.ID);
                    var BHTN = db.BHTN.FirstOrDefault(x => x.ID_SV == item.SinhVien.ID);
                    worksheet.Cells[row, 1].Value = i++;
                    worksheet.Cells[row, 2].Value = item.SinhVien.MaSV;
                    worksheet.Cells[row, 3].Value = item.SinhVien.HoSV;
                    worksheet.Cells[row, 4].Value = item.SinhVien.TenSV;
                    if (BHYT != null && BHYT.MaTheBHYT != null)
                    {
                        worksheet.Cells[row, 5].Value = BHYT.MaTheBHYT;
                    }
                    if (BHTN != null && BHTN.MaTheBHTN != null)
                    {
                        worksheet.Cells[row, 6].Value = BHTN.MaTheBHTN;
                    }
                    worksheet.Cells[row, 7].Value = item.SinhVien.GioiTinh;
                    worksheet.Cells[row, 8].Value = Convert.ToDateTime(item.SinhVien.NgaySinh).ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 9].Value = item.SinhVien.Lop.TenLop;

                    var bhytExists = db.BHYT.Any(x => x.ID_SV.ToString().Contains(item.ID_SV.ToString()));
                    var bhtnExists = db.BHTN.Any(x => x.ID_SV.ToString().Contains(item.ID_SV.ToString()));

                    if (bhytExists && bhtnExists)
                    {
                        worksheet.Cells[row, 10].Value = "Bảo hiểm y tế & Bảo hiểm tai nạn";
                    }
                    else if (bhytExists)
                    {
                        worksheet.Cells[row, 10].Value = "Bảo hiểm y tế";
                    }
                    else if (bhtnExists)
                    {
                        worksheet.Cells[row, 10].Value = "Bảo hiểm tai nạn";
                    }
                    worksheet.Cells[row, 11].Value = Convert.ToDateTime(item.ThoiGian).ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 12].Value = item.ThoiHanDangKy_BHYT;

                    if (BHYT != null && BHYT.NgayBatDau != null)
                    {
                        worksheet.Cells[row, 13].Value = Convert.ToDateTime(BHYT.NgayBatDau).ToString("dd/MM/yyyy");
                    }
                    worksheet.Cells[row, 14].Value = item.ThoiHanDangKy_BHTN;

                    if (BHTN != null && BHTN.NgayBatDau != null)
                    {
                        worksheet.Cells[row, 15].Value = Convert.ToDateTime(BHTN.NgayBatDau).ToString("dd/MM/yyyy");
                    }
                    worksheet.Cells[row, 16].Value = item.TienThanhToan;
                    worksheet.Cells[row, 17].Value = item.SinhVien.ThongTinLienHe.Phuong_Xa;
                    worksheet.Cells[row, 18].Value = item.SinhVien.ThongTinLienHe.Quan_Huyen;
                    worksheet.Cells[row, 19].Value = item.SinhVien.ThongTinLienHe.Tinh_TP;
                    worksheet.Cells[row, 20].Value = item.SinhVien.CCCD;
                    worksheet.Cells[row, 21].Value = item.SinhVien.TinhTrang.TenTinhTrang;
                    worksheet.Cells[row, 22].Value = item.GhiChu;

                    for (int col = 1; col <= 22; col++)
                    {
                        worksheet.Cells[row, col].AutoFitColumns();
                    }
                    row++;
                }
                xlPackage.Workbook.Properties.Title = "Danh sách bảo hiểm học sinh SONADEZI";
                xlPackage.Workbook.Properties.Author = "Administrator";
                xlPackage.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachBaoHiem_HSSV_SONADEZI(" + CountBill + ").xlsx");
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
     


    }
}