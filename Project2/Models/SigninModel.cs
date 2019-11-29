using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class SigninModel
    {
        public int SigninId { get; set; }

        [Display(Name = "日期")]
        public System.DateTime WorkDate { get; set; }

        [Display(Name = "簽到時間")]
        public System.TimeSpan ArrivedTime { get; set; }

        [Display(Name = "簽退時間")]
        public System.TimeSpan LeaveTime { get; set; }

        [Display(Name = "名稱")]
        public string UserName { get; set; }

        [Display(Name = "確認")]
        public bool CheckValue { get; set; }
    }
}