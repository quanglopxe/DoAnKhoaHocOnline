using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnLTWEB.Models
{
    public class MyCourses
    {
        QuanLyKhoaHocDataContext db = new QuanLyKhoaHocDataContext();
        public string sMaKH { get; set; }
        public string sTenKH { get; set; }
        public string sMota { get; set; }
        public string sGV { get; set; }
        public string sStartDate { get; set; }
        public string sEndDate { get; set; }
        public decimal dDonGia { get; set; }
        public string sTenMH { get; set; }
        public string sAnh { get; set; }
        public int iSoLuong { get; set; }
        public decimal dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }

        public MyCourses(string MaKH)
        {
            sMaKH = MaKH;
            KhoaHoc kh = db.KhoaHocs.Single(s => s.MaKH == sMaKH);
            sTenKH = kh.TenKH;
            sMota = kh.MoTaKH;
            sGV = kh.GiangVien;
            sStartDate = DateTime.Now.ToString("dd/MM/yyyy");
            sEndDate = DateTime.Now.AddMonths(6).ToString("dd/MM/yyyy");
            dDonGia = decimal.Parse(kh.GiaKhoaHoc.ToString());
            sTenMH = kh.TenMonHoc;
            sAnh = kh.Picture;
            iSoLuong = 1;
        }
    }
}