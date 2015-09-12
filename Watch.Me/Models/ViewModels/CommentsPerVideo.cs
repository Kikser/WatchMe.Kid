using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Watch.Me.Models.ViewModels
{
    public class CommentsPerVideo
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string CommentText { get; set; }
        public string CommentBy { get; set; }
        public string UserPicture { get; set; }     //used in Comment as poster picture
    }
}