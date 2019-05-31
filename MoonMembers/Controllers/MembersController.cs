using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoonMembers.Models;

namespace MoonMembers.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
        public ActionResult Index()
        {
            moonMembersEntities DB = new moonMembersEntities();

            List<members> members = DB.members.ToList();


            return View(members);
        }
    }
}