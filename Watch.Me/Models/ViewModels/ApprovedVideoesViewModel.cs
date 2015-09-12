using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Me.Models.ViewModels
{
    public class ApprovedVideoesViewModel
    {
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
        public int NumberLikes { get; set; }
        public int NumberDislikes { get; set; }
    }
}