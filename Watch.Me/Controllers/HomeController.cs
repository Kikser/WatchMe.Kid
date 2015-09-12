using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Watch.Me.Models;
using Watch.Me.Models.Entities;
using Watch.Me.Models.ViewModels;

namespace Watch.Me.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        int _mostUsedTag;

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            bool isAdmin = User.IsInRole("admin");

            ////if logged user is admin it is redirected to admin panel
            //if (!isAdmin)
            //{
            //    return RedirectToAction("Home", "AdminVideos");
            //}


            //Takes the most recent 6 videos 
            var recentVideos = _dbContext.Videos
                .Where(v => v.IsApproved.Equals(true))
                .OrderByDescending(o => o.DateCreated)
                .Take(6)
                .Select(x => new DisplayedVideosViewModel()
                {
                    Id = x.Id,
                    Url = x.Url,
                    VideoTitle = x.VideoTitle
                }).ToList();


            //Videos that have most likes are most popular
            var mostLikedVideo = _dbContext.EmotionsAboutVideos
                .GroupBy(video => video.Id)
                .Select(x => new
                {
                    VideoId = x.FirstOrDefault().Video.Id,
                    Url = x.FirstOrDefault().Video.Url,
                    VideoTitle = x.FirstOrDefault().Video.VideoTitle,
                    Count = x.Count(l => l.Love)
                }).OrderByDescending(o => o.Count)
                .Take(3)
                .ToList();

            var mostLikeVideoList = new List<DisplayedVideosViewModel>();
            foreach (var item in mostLikedVideo)
            {
                var tempList = new DisplayedVideosViewModel()
                {
                    Id = item.VideoId,
                    Url = item.Url,
                    VideoTitle = item.VideoTitle
                };
                mostLikeVideoList.Add(tempList);
            }


            //in case is logged user recommended tab in index page will be displayed
            if (userId != null)
            {
                //in case logged user is using the app, in the view there will be recommended vidoes tab
                var recentThreeVideos = recentVideos.Take(3).ToList();

                //find the most searched tags by logged user
                var mostSearched = _dbContext.TagLogs
                    .Where(u => u.ApplicationUser.Id == userId)
                    .GroupBy(x => x.Tag.Id)
                    .Select(s => new
                    {
                        Tag = s.Key,
                        Count = s.Count(t => t.Searched),
                    }).OrderByDescending(o => o.Count)
                    .Take(1);

                //takes only the most searched tag
                var firstOrDefault = mostSearched.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    _mostUsedTag = firstOrDefault.Tag;
                }

                //finds recommended vidoes having that tag in many-many relation
                var recommendVidoes = _dbContext.Videos
                    .Where(v => v.IsApproved.Equals(true)
                                && v.Tags.Any(tag => tag.Id == _mostUsedTag))
                    .OrderByDescending(o => o.DateCreated)
                    .Take(3)
                    .Select(x => new DisplayedVideosViewModel()
                    {
                        Id = x.Id,
                        Url = x.Url,
                        VideoTitle = x.VideoTitle
                    }).ToList();


                //all avaiable tags used in video upload
                var availableTags = _dbContext.Tags.GroupBy(g => g.Description)
                    .Select(s => new AvailableTags()
                    {
                        TagDescription = s.FirstOrDefault().Description,
                        TagId = s.FirstOrDefault().Id
                    }).ToList();


                var userData = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

                var allVideos = new DisplayedVideosViewModel()
                {
                    PopularVideos = mostLikeVideoList,
                    RecentVideos = recentThreeVideos,
                    ReccomendedVideos = recommendVidoes,
                    LoggedUser = true,
                    AvailableTags = availableTags,
                    PictureUrl = userData.UserPictures.PictureUrl,
                    UserEmail = userData.Email
                };
                return View(allVideos);
            }

                //in case it is anonymous user
            else
            {
                var allVideos = new DisplayedVideosViewModel()
                {
                    PopularVideos = mostLikeVideoList,
                    RecentVideos = recentVideos,
                    LoggedUser = false
                };
            return View(allVideos);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        [HttpPost]
        public ActionResult PostAVideo(string videoTitle, string videoUrl, string[] selectedTags)
        {
            //each video has many tags 
            var tags = new List<Tag>();
            //in order withs to be embeded in the player
            var embededString = videoUrl.Replace("watch?v=", "embed/");


            using (_dbContext)
            {
                foreach (string element in selectedTags)
                {
                    var currentElement = Int32.Parse(element);
                    var tempTag = _dbContext.Tags.Find(currentElement);
                    tags.Add(tempTag);
                }

                string currentUserId = User.Identity.GetUserId();
                var video = new Video()
                {
                    VideoTitle = videoTitle,
                    Url = embededString,
                    IsApproved = false,
                    DateCreated = DateTime.Now,
                    ApplicationUser = _dbContext.Users.FirstOrDefault(x => x.Id == currentUserId),
                    Tags = tags
                };

                _dbContext.Videos.Add(video);
                _dbContext.SaveChanges();

            }
            

            return Json(new { success = true, responseText = "The video has been uploaded" }, JsonRequestBehavior.AllowGet);
        }
    }





}