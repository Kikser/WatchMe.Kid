using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Me.Models;
using Watch.Me.Models.Entities;
using Watch.Me.Models.ViewModels;

namespace Watch.Me.Controllers
{
    public class WatchVideoController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        // GET: WatchVideo
        public ActionResult Index(int videoId)
        {
            var video = _dbContext.Videos.FirstOrDefault(x => x.Id == videoId);
            WatchVideoViewModel result = new WatchVideoViewModel();
            //if (video != null)
            //{
                //all comments for given videos
                var comments =
                    _dbContext.Comments.Where(x => x.Videos.Any(v => v.Id == videoId))
                        .Select(comment => new CommentsPerVideo()
                        {
                            Id = comment.Id,
                            DateCreated = comment.DateCreated,
                            CommentText = comment.CommentText,
                            CommentBy = comment.ApplicationUser.UserName
                        }).OrderByDescending(o => o.DateCreated).ToList();

            

            //all tags for given video
                var tags =
                    _dbContext.Tags.Where(t => t.Videos.Any(v => v.Id == videoId)).Select(tag => new TagsPerVideo()
                    {
                        IdTag = tag.Id,
                        TagDescription = tag.Description
                    }).ToList();

                var emotions = _dbContext.EmotionsAboutVideos.Where(x => x.Video.Id == videoId)
                    .GroupBy(g => g.Video)
                    .Select(x => new
                    {
                        NumberOfLikes = x.Count(c => c.Love),
                        NumberOfDislikes = x.Count(d => d.Dislike != null)
                    }).FirstOrDefault();
            
            
            var mostLikedVideosList = _dbContext.Videos.Select(x => new EachVideoForSideVideoesViewModel()
            {
                Id = x.Id,
                Url = x.Url,
                VideoTitle = x.VideoTitle,
                Tags = _dbContext.Tags.Where(tag => tag.Videos.Any(a => a.Id == x.Id)).Select(all => new TagsPerVideo()
                {
                    IdTag = all.Id,
                    TagDescription = all.Description
                }).ToList()
            }).Take(10).ToList();
            
            


                result = new WatchVideoViewModel()
                {
                    Id = video.Id,
                    Url = video.Url,
                    VideoTitle = video.VideoTitle,
                    DateCreated = video.DateCreated,
                    ApplicationUserId = video.ApplicationUser.Id,
                    UserName = video.ApplicationUser.UserName,
                    Comments = comments,
                    Tags = tags,
                    SideVideos = mostLikedVideosList
                };


            if (emotions != null)
            {
                result.NumberLike = emotions.NumberOfLikes;
                result.NumberDislike = emotions.NumberOfDislikes;
            }
            else
            {
                result.NumberLike = 0;
                result.NumberDislike = 0;
            }
            
            return View("~/Views/Home/WatchVideo.cshtml", result);
        }

        public ActionResult MultipleVideoes(int searchType)
        {
            WatchVideoViewModel result = new WatchVideoViewModel();

            if (searchType == 1)
                result.SideVideos = _dbContext.Videos.Select(x => new EachVideoForSideVideoesViewModel()
                {
                    Id = x.Id,
                    Url = x.Url,
                    VideoTitle = x.VideoTitle,
                    Tags = _dbContext.Tags.Where(tag => tag.Videos.Any(a => a.Id == x.Id && a.IsApproved)).Select(all => new TagsPerVideo()
                    {
                        IdTag = all.Id,
                        TagDescription = all.Description
                    }).ToList()
                }).Take(10).ToList();


            if (searchType == 2)
            {
                result.SideVideos = _dbContext.Videos.Select(x => new EachVideoForSideVideoesViewModel()
                {
                    Id = x.Id,
                    Url = x.Url,
                    VideoTitle = x.VideoTitle,
                    DateCreate = x.DateCreated,
                    Tags = _dbContext.Tags.Where(tag => tag.Videos.Any(a => a.Id == x.Id && a.IsApproved))
                    .Select(all => new TagsPerVideo()
                    {
                        IdTag = all.Id,
                        TagDescription = all.Description
                    }).ToList()
                }).OrderByDescending(o => o.DateCreate).Take(10).ToList();
            }

            return PartialView("~/Views/Home/_MultipleVideoesPartialView.cshtml", result);

            //todo recomended videos
            //todo add in post videoId
        }
    }
}

