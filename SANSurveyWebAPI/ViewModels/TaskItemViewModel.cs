using DoSurveyApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoSurveyApp.ViewModels
{
    public class TaskItemViewModel
    {
        public int? Id { get; set; }

        [Required, StringLength(255)]
        public string ShortName { get; set; }

        [StringLength(255)]
        public string LongName { get; set; }

        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> Date { get; set; }


        public string Title
        {
            get
            {
                return Id != 0 ? "Edit Task" : "New Task";
            }

        }

        public TaskItemViewModel()
        {
            Id = 0;
            CreatedDate = DateTime.Now;
            //Status = TaskStatus.Active.ToString();
        }
        public TaskItemViewModel(TaskItem taskItem)
        {

            Id = taskItem.Id;
            ShortName = taskItem.ShortName;
            LongName = taskItem.LongName;
            Status = taskItem.Status;
            CreatedDate = taskItem.CreatedDate;

            LastUpdatedDate = DateTime.Now;

        }



    }
}