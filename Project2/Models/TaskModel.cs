using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }

        [Display(Name = "工作內容")]
        public string TaskName { get; set; }

        [Display(Name = "時間長度")]
        public double Duration { get; set; }

        [Display(Name = "名稱")]
        public string UserName { get; set; }

        [Display(Name = "日期")]
        public Nullable<System.DateTime> Date { get; set; }
    }
}