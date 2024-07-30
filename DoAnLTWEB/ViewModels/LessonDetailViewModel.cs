using DoAnLTWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnLTWEB.ViewModels
{
    public class LessonDetailViewModel
    {
        public BaiGiang baiGiang { get; set; }
        public List<BaiGiang> listBG { get; set; }
    }
}