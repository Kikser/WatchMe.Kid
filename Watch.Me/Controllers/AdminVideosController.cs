using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Me.Models;
using Watch.Me.Models.Entities;
using Watch.Me.Models.ViewModels;

namespace Watch.Me.Controllers.Admin
{
    public class AdminVideosController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        // GET: Admin

        public ActionResult Home()
        {
            return View("~/Views/Admin/AdminHome.cshtml");
        }

        public ActionResult Index()
        {
            using (_dbContext)
            {
                ApproveVideoViewModel result = new ApproveVideoViewModel();

                var model = _dbContext.Videos.Where(e => e.IsApproved == false)
                    .Select(x => new ApproveVideoViewModel
                    {
                        Id = x.Id,
                        DateCreated = x.DateCreated,
                        Url = x.Url,
                        VideoTitle = x.VideoTitle,
                        Username = x.ApplicationUser.UserName,
                        Tags = _dbContext.Tags.Where(t => t.Videos.Any(v => v.Id == x.Id))
                            .Select(tag => new TagsPerVideo()
                            {
                                IdTag = tag.Id,
                                TagDescription = tag.Description
                            }).ToList()
                    }).OrderByDescending(o => o.DateCreated).ToList();


                return View("~/Views/AdminVideos/UnApprovedVideoes.cshtml", model);
            }
        }


        //approves the video
        public ActionResult ApproveVideo(int videoId)
        {
            using (_dbContext)
            {
                var model = _dbContext.Videos.FirstOrDefault(x => x.Id == videoId);
                if (model != null)
                {
                    model.IsApproved = true;
                    model.ApprovedOn = DateTime.Now.ToShortDateString();
                    _dbContext.SaveChanges();
                }

            }
            return RedirectToAction("Index");
        }


        public ActionResult ApproveTag(int videoId)
        {          
            using (_dbContext)
            {
                var tagsForVideo = _dbContext.Tags.
                    Where(t => t.Videos.Any(v => v.Id == videoId))
                    .Select(x => new TagsPerVideo()
                    {
                        IdTag = x.Id,
                        TagDescription = x.Description,
                    }).ToList();


                var tagsPerVideo = new ApproveTagsPerVideo()
                {
                    TagsPerVideos = tagsForVideo,
                    VideoTitle = _dbContext.Videos.Find(videoId).VideoTitle,
                    MovieId = _dbContext.Videos.Find(videoId).Id
                };

                if (tagsPerVideo.TagsPerVideos == null)
                {
                    return View();
                }


                return View(tagsPerVideo);
            }
            
           
            

        }

        public ActionResult RemoveTag(int tagId, int videoId )
        {
            using (_dbContext)
            {
                var video = _dbContext.Videos.Find(videoId);
                var tag = _dbContext.Tags.Find(tagId);

                _dbContext.Entry(video).Collection("Tags").Load();
                video.Tags.Remove(tag);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        


        //public ActionResult AddTagPerVideo(int videoId)
        //{
        //    var availableTags = _dbContext.Tags.GroupBy(g => g.Description)
        //            .Select(s => new AvailableTags()
        //            {
        //                TagDescription = s.FirstOrDefault().Description,
        //                TagId = s.FirstOrDefault().Id
        //            }).ToList();

        //    return View(availableTags);
        //}

    }
}