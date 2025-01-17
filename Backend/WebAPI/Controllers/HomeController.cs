﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/message")]
    public class HomeController : ControllerBase
    {
        private readonly MessengeChatTestContext messengeChat;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public HomeController(MessengeChatTestContext messenge, ILogger<HomeController> logger,
            IWebHostEnvironment environment)
        {
            this.messengeChat = messenge;
            this._logger = logger;
            this._environment = environment ?? throw new ArgumentNullException(nameof(environment));

        }

        [HttpPost]
        [Route("image")]
        public ActionResult PostImage()
        {
            Account account = new Account(
              "uit-information",
              "758822263555998",
              "aW_PQSGalL-ITWJUaux-cos-JEA");
            Cloudinary _cloudinary = new Cloudinary(account);



            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {

                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            PublicId = "tutuanle/image/upload/" + file.FileName.Split('.')[0]

                        };

                        var uploadResult = _cloudinary.Upload(uploadParams);

                        return Ok(uploadResult.Url);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                return new StatusCodeResult(500);
            }
            return Ok(1);
        }


        [HttpPost]
        [Route("video")]
        public ActionResult PostVideo()
        {
            Account account = new Account(
              "uit-information",
              "758822263555998",
              "aW_PQSGalL-ITWJUaux-cos-JEA");
            Cloudinary _cloudinary = new Cloudinary(account);



            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {

                        var uploadParams = new VideoUploadParams()
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            PublicId = "tutuanle/record/upload/" + file.FileName.Split('.')[0]

                        };

                        var uploadResult = _cloudinary.Upload(uploadParams);

                        return Ok(uploadResult.Url);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error");
                return new StatusCodeResult(500);
            }
            return Ok(1);
        }


        [HttpGet("{userId}")]

        public IActionResult GetUser(int userId)
        {
            var modelFriend = (from f in messengeChat.Friends
                               join u in messengeChat.Users
                               on f.UserId equals u.UserId
                               select new FriendModel
                               {
                                   FriendId = f.FriendId,
                                   UserId = u.UserId,
                                   status = (from us in messengeChat.Users
                                             where us.UserId == f.UserIdfriend
                                             select us.Active
                                            ).SingleOrDefault() == 1 ? true : false,
                                   FriendKey = f.FriendKey,
                                   Name = (from us in messengeChat.Users
                                           where us.UserId == f.UserIdfriend
                                           select us.FirstName + " " + us.LastName).SingleOrDefault(),
                                   ImgURL = (from us in messengeChat.Users
                                             where us.UserId == f.UserIdfriend
                                             select us.ImgURL).SingleOrDefault(),
                                   DateSend = (from ms in messengeChat.Messagings
                                               where ms.FriendId == f.FriendKey
                                               orderby ms.DateSent descending
                                               select ms.DateSent
                                                ).SingleOrDefault().ToString("H:mm", CultureInfo.InvariantCulture),
                                   CountUnRead = (from ms in messengeChat.Messagings
                                                  where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                                  select ms
                                                ).ToList().Count,
                                   IdMessageNew = (from ms in messengeChat.Messagings
                                                   where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                                   select ms.MessageId
                                                ).ToList(),
                                   MessageNew = (from ms in messengeChat.Messagings
                                                 where ms.FriendId == f.FriendKey
                                                 orderby ms.DateSent descending
                                                 select ms.Content
                                                ).SingleOrDefault(),

                                   sortDate = (from ms in messengeChat.Messagings
                                               where ms.FriendId == f.FriendKey
                                               orderby ms.DateSent descending
                                               select ms.DateSent
                                                ).SingleOrDefault(),
                                   ColorSeen = (from ms in messengeChat.Messagings
                                                where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                                select ms
                                                ).ToList().Count == 0 ? "#C7C7C7" : "#000000"
                               });


            return Ok(modelFriend.ToList());
        }

        [HttpGet]
        public IActionResult GetFriend()
        {

            var MessageNew = (from ms in messengeChat.Messagings
                              where ms.FriendId == "1221"
                              orderby ms.DateSent descending
                              select ms.Content);
            return Ok(MessageNew.FirstOrDefault());
        }

        [HttpGet("CountMessaging")]

        public IActionResult GetCountMessaging()
        {
            return Ok(messengeChat.Messagings.Count());
        }

        [HttpGet("getFriend/{userId}")]
        public IActionResult GetFriend(int userId)
        {
            var modelGroupFriend =
                (from f in messengeChat.DetailUserGroups
                 join u in messengeChat.Users
                 on f.AddUserId equals u.UserId
                 //where u.UserId == userId
                 select new FriendModel
                 {
                     FriendId = 0,
                     UserId = u.UserId,
                     status = (from us in messengeChat.Users
                               where us.UserId == f.AddUserId
                               select us.Active
                              ).SingleOrDefault() == 1 ? true : false,
                     FriendKey = f.FriendKey,
                     Name = (from userGroup in messengeChat.UserGroups
                             where userGroup.UserGroupId == f.UserGroupId
                             select userGroup.NickNameowner).FirstOrDefault(),
                     ImgURL = (from us in messengeChat.Users
                               where us.UserId == f.AddUserId
                               select us.ImgURL).SingleOrDefault(),
                     DateSend = (from ms in messengeChat.Messagings
                                 where ms.FriendId == f.FriendKey
                                 orderby ms.DateSent descending
                                 select ms.DateSent
                                  ).SingleOrDefault().ToString("H:mm", CultureInfo.InvariantCulture),
                     CountUnRead = (from ms in messengeChat.Messagings
                                    where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                    select ms
                                  ).ToList().Count,
                     IdMessageNew = (from ms in messengeChat.Messagings
                                     where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                     select ms.MessageId
                                  ).ToList(),
                     MessageNew = (from ms in messengeChat.Messagings
                                   where ms.FriendId == f.FriendKey
                                   orderby ms.DateSent descending
                                   select ms.Content
                                  ).SingleOrDefault(),

                     sortDate = (from ms in messengeChat.Messagings
                                 where ms.FriendId == f.FriendKey
                                 orderby ms.DateSent descending
                                 select ms.DateSent
                                  ).SingleOrDefault(),
                     ColorSeen = (from ms in messengeChat.Messagings
                                  where ms.FriendId == f.FriendKey && ms.DateRead == null && ms.FromUserId != u.UserId
                                  select ms
                                  ).ToList().Count == 0 ? "#5B5A5A" : "#000000",
                 }).ToList();


            return Ok(modelGroupFriend);
        }

        [HttpGet("generateCode")]
        public IActionResult GetCode()
        {
            return Ok(generateUniqueID());
        }

        private string generateUniqueID(int _characterLength = 11)
        {
            StringBuilder _builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(_characterLength)
                .ToList().ForEach(e => _builder.Append(e));
            return _builder.ToString();
        }
    }
}
