using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Me.Models.ViewModels
{
    public class ApproveTagsPerVideo
    {
        public int MovieId { get; set; }
        public string VideoTitle { get; set; }
        public List<TagsPerVideo> TagsPerVideos { get; set; } 
    }
}