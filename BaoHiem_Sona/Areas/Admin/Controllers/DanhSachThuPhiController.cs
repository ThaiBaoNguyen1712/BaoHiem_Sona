using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using DrawingColor = System.Drawing.Color;
using System.Web.Mvc;
using PagedList;
using BaoHiem_Sona.Models;
using System.Globalization;
using BaoHiem_Sona.Common;

namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class DanhSachThuPhiController : RoleAdminController
    {
        // GET: DanhSachSV
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index(int? page, int? pageSize)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }
            var Invoice = db.GiaoDich
                .OrderByDescending(x => x.ID)
                .Take(20)
                .ToList();
            return View(Invoice.ToPagedList((int)page, (int)pageSize));
        }
        [HttpPost]
        public ActionResult Index(int? page, int? pageSize, long? ID_Invoice, long? MSSV, DateTime? ThoiGianBatDau, DateTime? ThoiGianKetThuc)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }
            var invoices = db.GiaoDich.Where(x =>
                     (ID_Invoice == null || x.ID == ID_Invoice) && // Điều kiện cho ID_Invoice
                     (MSSV == null || x.SinhVien.ID.ToString().Contains(MSSV.ToString())) && // Điều kiện cho MSSV
                     (ThoiGianBatDau == null || x.ThoiGian >= ThoiGianBatDau) && // Điều kiện cho ThoiGianBatDau
                     (ThoiGianKetThuc == null || x.ThoiGian <= ThoiGianKetThuc) // Điều kiện cho ThoiGianKetThuc
                 ).OrderByDescending(x => x.ID).ToList();

            return View(invoices.ToPagedList((int)page, (int)pageSize));
        }


        public ActionResult Detail_Invoice(int ID)
        {
            var IDetail = db.GiaoDich.FirstOrDefault(x => x.ID == ID);
            var bhytExist = db.BHYT.Any(x => x.ID_SV.ToString().Contains(IDetail.ID_SV.ToString()));
            var bhtnExist = db.BHTN.Any(x => x.ID_SV.ToString().Contains(IDetail.ID_SV.ToString()));
            ViewBag.BHYT = bhytExist;
            ViewBag.BHTN = bhtnExist;
            return View(IDetail);
        }
        public ActionResult All_Invoice(int? page, int? pageSize)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }
            var invoices = db.GiaoDich.OrderByDescending(x => x.ThoiGian).ToList();
            return View(invoices.ToPagedList((int)page, (int)pageSize));
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
                worksheet.Cells["U10"].Value = "Ghi Chú";
                worksheet.Cells["A10:U10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A10:U10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                worksheet.Cells["A10:U10"].AutoFitColumns();

                row = 11;
                int i = 1;
                foreach (var item in invoices)
                {
                    worksheet.Cells[row, 1].Value = i++;
                    worksheet.Cells[row, 2].Value = item.SinhVien.MaSV;
                    worksheet.Cells[row, 3].Value = item.SinhVien.HoSV;
                    worksheet.Cells[row, 4].Value = item.SinhVien.TenSV;
                    // Chỗ trống cho MÃ BH
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
                    worksheet.Cells[row, 14].Value = item.ThoiHanDangKy_BHTN;
                    worksheet.Cells[row, 16].Value = item.TienThanhToan;
                    worksheet.Cells[row, 17].Value = item.SinhVien.ThongTinLienHe.Phuong_Xa;
                    worksheet.Cells[row, 18].Value = item.SinhVien.ThongTinLienHe.Quan_Huyen;
                    worksheet.Cells[row, 19].Value = item.SinhVien.ThongTinLienHe.Tinh_TP;
                    worksheet.Cells[row, 20].Value = item.SinhVien.CCCD;
                    worksheet.Cells[row, 21].Value = item.GhiChu;
                    for (int col = 1; col <= 21; col++)
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
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThuPhiBaoHiem_SONADEZI(" + CountBill + ").xlsx");
        }
        public ActionResult BatchInvoiceUpload()
        {
            return View();
        }

     
        [HttpPost]
        public ActionResult SendEmail(DateTime? ThoiGianBatDau, DateTime? ThoiGianKetThuc)
        {
            var invoices = db.GiaoDich.Where(x => (ThoiGianBatDau == null || x.ThoiGian >= ThoiGianBatDau) && x.ThoiGian <= ThoiGianKetThuc).ToList();

            //send mail cho khách hàng
            foreach (var item in invoices)
            {

            // Đọc nội dung từ file HTML mẫu
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/Invoice.html"));

                // Thay thế các thẻ placeholder trong mẫu bằng thông tin thực tế
                contentCustomer = contentCustomer.Replace("{{MaHD}}", Convert.ToString(item.ID));
                if (item.ThoiHanDangKy_BHYT !=null && item.ThoiHanDangKy_BHTN !=null)
                {
                    contentCustomer = contentCustomer.Replace("{{LoaiBH}}", "Bảo hiểm y tế và bảo hiểm tai nạn");
                }    
                else if(item.ThoiHanDangKy_BHYT !=null)
                    {
                        contentCustomer = contentCustomer.Replace("{{LoaiBH}}", "Bảo hiểm y tế");
                    }
                else if (item.ThoiHanDangKy_BHTN != null)
                  {
                    contentCustomer = contentCustomer.Replace("{{LoaiBH}}", "Bảo hiểm tai nạn");
             }
                contentCustomer = contentCustomer.Replace("{{TenSV}}", Convert.ToString(item.SinhVien.HoSV +" "+item.SinhVien.TenSV));
                contentCustomer = contentCustomer.Replace("{{MSSV}}", Convert.ToString(item.SinhVien.MaSV));
                contentCustomer = contentCustomer.Replace("{{Gia}}", ((decimal)item.TienThanhToan).ToString("N0", CultureInfo.InvariantCulture));
                contentCustomer = contentCustomer.Replace("{{ThoiGian}}", item.ThoiGian.ToString());
                  if(item.ThoiHanDangKy_BHYT !=null && item.ThoiHanDangKy_BHTN !=null)
                {
                    contentCustomer = contentCustomer.Replace("{{ThoiHan}}", "<strong>BHYT :</strong> " + Convert.ToString(item.ThoiHanDangKy_BHYT) +
                         " | <strong>BHTN :</strong> " + Convert.ToString(item.ThoiHanDangKy_BHTN));
                }
                else if(item.ThoiHanDangKy_BHYT !=null)
                {
                    contentCustomer = contentCustomer.Replace("{{ThoiHan}}", Convert.ToString(item.ThoiHanDangKy_BHYT));
                }
             else if (item.ThoiHanDangKy_BHTN != null)
              {
                    contentCustomer = contentCustomer.Replace("{{ThoiHan}}", Convert.ToString(item.ThoiHanDangKy_BHTN));
                }
               
                contentCustomer = contentCustomer.Replace("{{Total}}", ((decimal)item.TienThanhToan).ToString("N0", CultureInfo.InvariantCulture));

                // Gửi email
                BaoHiem_Sona.Common.SendEmail.sendEmail("SONADEZI", "Hóa đơn", contentCustomer, item.SinhVien.ThongTinLienHe.Email);

                // Đối với mục đích demo, bạn có thể thay thế "Địa chỉ email khách hàng" bằng địa chỉ email thực tế của khách hàng.
            }
            return RedirectToAction("Index");
        }

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
                            HoSV = worksheet.Cells[rowIndex, 3].Value?.ToString().Trim(),
                            TenSV = worksheet.Cells[rowIndex, 4].Value?.ToString().Trim(),
                            GioiTinh = infoSV.GioiTinh,
                            CCCD = infoSV.CCCD,
                            NgaySinh = Convert.ToDateTime(infoSV.NgaySinh).ToString("dd/MM/yyyy"),
                            TenLop = worksheet.Cells[rowIndex, 9].Value?.ToString().Trim(),
                            LoaiBH = worksheet.Cells[rowIndex, 10].Value?.ToString().Trim(),
                            NgayDongPhi = worksheet.Cells[rowIndex, 11].Value?.ToString().Trim(),
                            SoTienDong = worksheet.Cells[rowIndex, 16].Value?.ToString().Trim(),
                            ThoiHanBHYT = worksheet.Cells[rowIndex, 12].Value?.ToString().Trim(),
                            ThoiHanBHTN = worksheet.Cells[rowIndex, 14].Value?.ToString().Trim(),
                            GhiChu= worksheet.Cells[rowIndex, 21].Value?.ToString().Trim(),
                        };

                        if (!CheckMaSVQuery(rowData.MaSV))
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
        public ActionResult UploadInvoices(List<SinhVien> svList, List<string> Error, List<GiaoDich> gdList, List<string> LoaiBH)
        {
            try
            {
                for (int i = 0; i < svList.Count; i++)
                {
                    var itemsv = svList[i];
                    var itemgd = gdList[i];
                    var err = Error[i];
                    var bh = LoaiBH[i];

                    if (string.IsNullOrEmpty(err))
                    {
                        var student = db.SinhVien.FirstOrDefault(sv => sv.MaSV.Contains(itemsv.MaSV));

                        if (student != null)
                        {
                            var ID_SV = student.ID;

                            long? ID_BHYT = null;
                            long? ID_BHTN = null;

                            if (bh.Contains("Bảo hiểm y tế & Bảo hiểm tai nạn"))
                            {
                                var _BHYT = new BHYT() { ID_SV = ID_SV };
                                var _BHTN = new BHTN() { ID_SV = ID_SV };

                                db.BHYT.Add(_BHYT);
                                db.BHTN.Add(_BHTN);

                                db.SaveChanges();

                                ID_BHYT = _BHYT.ID;
                                ID_BHTN = _BHTN.ID;
                            }
                            else if (bh.Contains("Bảo hiểm y tế"))
                            {
                                var _BHYT = new BHYT() { ID_SV = ID_SV };
                                db.BHYT.Add(_BHYT);
                                db.SaveChanges();
                                ID_BHYT = _BHYT.ID;
                            }
                            else if (bh.Contains("Bảo hiểm tai nạn"))
                            {
                                var _BHTN = new BHTN() { ID_SV = ID_SV };
                                db.BHTN.Add(_BHTN);
                                db.SaveChanges();
                                ID_BHTN = _BHTN.ID;
                            }

                            var GD = new GiaoDich()
                            {
                                ID_SV = ID_SV,
                                ThoiHanDangKy_BHTN = itemgd.ThoiHanDangKy_BHTN,
                                ThoiHanDangKy_BHYT = itemgd.ThoiHanDangKy_BHYT,
                                TienThanhToan = Convert.ToDecimal(itemgd.TienThanhToan),
                                GhiChu = itemgd.GhiChu,
                                ThoiGian = Convert.ToDateTime(itemgd.ThoiGian),
                            };

                            if (bh.Contains("Bảo hiểm y tế & Bảo hiểm tai nạn"))
                            {
                                student.ID_TinhTrang = 3;
                            }
                            else if (bh.Contains("Bảo hiểm y tế"))
                            {
                                student.ID_TinhTrang = 1;
                            }
                            else if (bh.Contains("Bảo hiểm tai nạn"))
                            {
                                student.ID_TinhTrang = 2;
                            }

                            db.GiaoDich.Add(GD);
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

    }

}