using DoAnLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAnLTWEB.Filter
{
    public class CheckRegisteredCourseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            NguoiDung nguoiDung = (NguoiDung)session["User"];
            string _courseId = (string)session["MaKH"];
            string _lessonId = filterContext.ActionParameters["MaBG"] as string;
            QuanLyKhoaHocDataContext db = new QuanLyKhoaHocDataContext();            

            if (nguoiDung != null && _courseId != null && _lessonId != null)
            {
                HoaDonDky dKyKhoaHoc = db.HoaDonDkies.FirstOrDefault(t => t.MaND == nguoiDung.MaND);
                var listMaKH = session["CourseIds"] as List<string>;
                BaiGiang baiGiang = db.BaiGiangs.FirstOrDefault(t => t.MaBG == _lessonId && t.MaKH == _courseId);

                if (dKyKhoaHoc != null && listMaKH != null && listMaKH.Contains(_courseId) && baiGiang != null)
                {
                    return;
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "controller", "User" },
                            { "action", "Logout" }
                        });
                }
            }
            else
            {                
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "User" },
                        { "action", "Logout" }
                    });
            }
        }
    }
}