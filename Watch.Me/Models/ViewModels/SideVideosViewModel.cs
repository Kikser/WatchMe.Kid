using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Me.Models.ViewModels
{
    public class SideVideosViewModel
    {
        public List<EachVideoForSideVideoesViewModel> EachVideoForSideVideoes { get; set; } 
    }

    public class EachVideoForSideVideoesViewModel
    {
        public int Id { get; set; }
        public string VideoTitle { get; set; }
        public string Url { get; set; }
        public List<TagsPerVideo> Tags { get; set; }
    }
    
}