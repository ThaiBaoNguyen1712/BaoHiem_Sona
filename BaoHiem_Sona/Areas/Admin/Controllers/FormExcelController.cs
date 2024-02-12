using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaoHiem_Sona.Models;
using ExcelDataReader;
using OfficeOpenXml;
using DrawingColor = System.Drawing.Color;


namespace BaoHiem_Sona.Areas.Admin.Controllers
{
    public class FormExcelController : RoleAdminController
    {
        BHYTEntities db = new BHYTEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FormSV() {
            var stream = new MemoryStream();
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("SV");

                var customstyle = xlPackage.Workbook.Styles.CreateNamedStyle("CustomStyle");
                customstyle.Style.Font.UnderLine = true;
                customstyle.Style.Font.Color.SetColor(DrawingColor.Red);

                var startRow = 5;
                var row = startRow;

                worksheet.Cells["A1"].Value = "Danh sách học sinh SONADEZI";
                using (var r = worksheet.Cells["A1:C1"])
                {
                    r.Merge = true;
                    r.Style.Font.Color.SetColor(DrawingColor.Green);
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(DrawingColor.Lavender);

                }

                worksheet.Cells["A10:K10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A10:K10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                worksheet.Cells["A10:K10"].AutoFitColumns();

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

                xlPackage.Workbook.Properties.Title = "Danh sách SV";
                xlPackage.Workbook.Properties.Author = "Administrator";
                xlPackage.Save();
            }
            stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSach_HS_SV.xlsx");
           
        }
        [HttpPost]
        public ActionResult FormThuPhi()
        {
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
                worksheet.Cells["A10:U10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A10:U10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                worksheet.Cells["A10:U10"].AutoFitColumns();

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
                
                xlPackage.Workbook.Properties.Title = "Danh sách bảo hiểm học sinh SONADEZI";
                xlPackage.Workbook.Properties.Author = "Administrator";
                xlPackage.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachThuPhiBaoHiem_SONADEZI.xlsx");
        }
        [HttpPost]
        public ActionResult FormCapMa()
        {
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
                worksheet.Cells["A10:V10"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A10:V10"].Style.Fill.BackgroundColor.SetColor(DrawingColor.Yellow);
                worksheet.Cells["A10:V10"].AutoFitColumns();

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

                xlPackage.Workbook.Properties.Title = "Danh sách bảo hiểm học sinh SONADEZI";
                xlPackage.Workbook.Properties.Author = "Administrator";
                xlPackage.Save();
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachBaoHiem_HSSV_SONADEZI.xlsx");
        }
    }
}