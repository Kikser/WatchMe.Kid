using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Watch.Me.Models.ViewModels
{
    public class ApproveVideoViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        public string VideoTitle { get; set; }

        [DisplayName("VideoUrl")]
        public string Url { get; set; }

        [DisplayName("Posted on")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Uploaded by")]
        public string Username { get; set; }
        public List<TagsPerVideo> Tags { get; set; } 


    }
}