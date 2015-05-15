﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FeedReader.Models;
using Microsoft.AspNet.Identity;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedReader.Controllers
{
    public class FeedController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Feed
        public ActionResult Index()
        {
            return View(db.Feeds.ToList());
        }

        // GET: Feed/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // GET: Feed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Feed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "channel,link")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                feed.user_id = User.Identity.GetUserId();
                db.Feeds.Add(feed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feed);
        }

        public ActionResult showArticles(String url)
        {
            return View(getFeedArticles(url));
        }

        public IEnumerable<FeedArticle> getFeedArticles(String url)
        {
            //Make type IEnumerable so we can iterate on front end
            IEnumerable<FeedArticle> articles = new List<FeedArticle>();

            //try
            //{
            //    XDocument doc = XDocument.Load(url);
            //    // <item> is under the root element of the xml document
            //    var entries = from item in doc.Root.Descendants().Where(i => i.Name.LocalName == "item")
            //                  select new FeedArticle
            //                  {                                  
            //                      link = item.Elements().First(i => i.Name.LocalName == "link").Value,
            //                      desc = item.Elements().First(i => i.Name.LocalName == "description").Value,
            //                      title = item.Elements().First(i => i.Name.LocalName == "title").Value
            //                  };

            //    articles = entries.ToList();
            //}
            try
            {
                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                var entries = from item in feed.Items
                                select new FeedArticle
                                {
                                    title = item.Title.Text,
                                    desc = item.Summary.Text,
                                    link = item.Id
                                };
                articles = entries.ToList();
            }
            catch
            {
                //Dont add to the list
            }
            
            return articles;
        }

        // GET: Feed/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feed/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,channel,link,desc")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feed);
        }

        // GET: Feed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feed feed = db.Feeds.Find(id);
            db.Feeds.Remove(feed);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
