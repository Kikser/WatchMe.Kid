using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Watch.Me.Models.Entities;

namespace Watch.Me.Models.ViewModels
{
    public class WatchVideoViewModel
    {
        public int Id { get; set; }
        public string VideoTitle { get; set; }
        public string Url { get; set; }
        public DateTime DateCreated { get; set; }

        //UserData
        public string ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string UserPicture { get; set; }

        //Emotions
        public int NumberLike { get; set; }
        public int NumberDislike { get; set; }

        //Comments
        public List<CommentsPerVideo> Comments { get; set; }
        public List<TagsPerVideo> Tags { get; set; }

        public List<EachVideoForSideVideoesViewModel> SideVideos { get; set; } 
        
    }
}