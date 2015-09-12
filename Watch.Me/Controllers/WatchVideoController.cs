using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
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
                        CommentBy = comment.ApplicationUser.UserName,
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
                    Tags =
                        _dbContext.Tags.Where(tag => tag.Videos.Any(a => a.Id == x.Id && a.IsApproved))
                            .Select(all => new TagsPerVideo()
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

        public ActionResult AddEmoteToVideo(int type, int videoId)
        {
            string userId = User.Identity.GetUserId();

            var emotion = new EmotionsAboutVideo();
            using (_dbContext)
            {
                emotion.Video = _dbContext.Videos.Find(videoId);
                if (userId != null)
                {
                    emotion.ApplicationUser = _dbContext.Users.Find(userId);
                }
                if (type == 1)
                {
                    emotion.Love = true;
                }
                else
                {
                    emotion.Dislike = true;
                    emotion.Love = false;
                }
                _dbContext.EmotionsAboutVideos.Add(emotion);
                _dbContext.SaveChanges();

                
                var emotions = _dbContext.EmotionsAboutVideos.Where(x => x.Video.Id == videoId)
                    .GroupBy(g => g.Video)
                    .Select(x => new
                    {
                        NumberOfLikes = x.Count(c => c.Love),
                        NumberOfDislikes = x.Count(d => d.Dislike != null)
                    }).FirstOrDefault();

                WatchVideoViewModel result = new WatchVideoViewModel();

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

                var video = _dbContext.Videos.FirstOrDefault(x => x.Id == videoId);
                var tags =
                    _dbContext.Tags.Where(t => t.Videos.Any(v => v.Id == videoId)).Select(tag => new TagsPerVideo()
                    {
                        IdTag = tag.Id,
                        TagDescription = tag.Description
                    }).ToList();

                result.VideoTitle = video.VideoTitle;
                result.Tags = tags;
                

                return PartialView("~/Views/Home/_LikesDislikes.cshtml", result);
            }

        }

        public ActionResult InsertComment(int videoId, string commentText)
        {
            using (_dbContext)
            {
                var vidoes = new List<Video>();
                var video = _dbContext.Videos.Find(videoId);
                vidoes.Add(video);

                string userId = User.Identity.GetUserId();
                var comment = new Comment()
                {
                    ApplicationUser = _dbContext.Users.Find(userId),
                    CommentText = commentText,
                    DateCreated = DateTime.Now,
                    Videos = vidoes
                };



                _dbContext.Comments.Add(comment);
                _dbContext.SaveChanges();

                var comments = _dbContext.Comments.Where(x => x.Videos.Any(v => v.Id == videoId))
                    .Select(com => new CommentsPerVideo()
                    {
                        Id = com.Id,
                        DateCreated = com.DateCreated,
                        CommentText = com.CommentText,
                        CommentBy = com.ApplicationUser.UserName,
                    }).OrderByDescending(o => o.DateCreated).ToList();

                WatchVideoViewModel result = new WatchVideoViewModel();
                result.Comments = comments;

                return PartialView("~/Views/Home/_Comments.cshtml", comments);
            }


        }
    }

}

