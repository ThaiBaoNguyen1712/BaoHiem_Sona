using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaoHiem_Sona.Common
{
    public class ExcelRowData
    {
        public string MaSV { get; set; }
        public string HoSV { get; set; }
        public string TenSV { get; set; }

        public string GioiTinh { get; set; }
        public string CCCD { get; set; }
        public string NgaySinh { get; set; }
        public string TenLop { get; set; }
        public string PhuongXa { get; set; }
        public string QuanHuyen { get; set; }
        public string TinhThanhPho { get; set; }
        public string LoaiBH { get; set; }
        public string NgayDongPhi {get; set;}
        public string ThoiHanBHYT { get; set; }
        public string ThoiHanBHTN { get; set; }
        public string NgayHieuLuc_BHYT { get; set; }
        public string NgayHieuLuc_BHTN { get; set; }

        public string GhiChu { get; set; }
        public string SoTienDong { get; set; }
        public string Error { get; set; }

        public string MaBHYT { get; set; }
       public string MaBHTN { get; set; }

    }
}