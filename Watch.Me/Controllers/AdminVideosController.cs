using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Me.Models;
using Watch.Me.Models.ViewModels;

namespace Watch.Me.Controllers.Admin
{
    public class AdminVideosController : Controller
    {
        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            var model = _dbContext.Videos.Select(x => new ApproveVideoViewModel
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
            });

            return View("~/Views/AdminVideos/UnApprovedVideoes.cshtml", model);
        }
    }
}