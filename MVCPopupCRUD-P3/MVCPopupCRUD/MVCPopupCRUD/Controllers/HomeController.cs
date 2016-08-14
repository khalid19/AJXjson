using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPopupCRUD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetContacts()
        {
            List<Contact> all = null;

            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var contacts = (from a in dc.Contacts
                                join b in dc.Countries on a.CountryID equals b.CountryID
                                join c in dc.States on a.StateID equals c.StateID
                                select new
                                {
                                    a,
                                    b.CountryName,
                                    c.StateName
                                });
                if (contacts != null)
                {
                    all = new List<Contact>();
                    foreach (var i in contacts)
                    {
                        Contact con = i.a;
                        con.CountryName = i.CountryName;
                        con.StateName = i.StateName;
                        all.Add(con);
                    }
                }
            }

            return new JsonResult { Data = all, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //Fetch Country from database
        private List<Country> GetCountry()
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                return dc.Countries.OrderBy(a => a.CountryName).ToList();
            }
        }

        //Fetch State from database
        private List<State> GetState(int countryID)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                return dc.States.Where(a => a.CountryID.Equals(countryID)).OrderBy(a => a.StateName).ToList();
            }
        }

        //return states as json data
        public JsonResult GetStateList(int countryID)
        {
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                return new JsonResult { Data = GetState(countryID), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //Get contact by ID
        public Contact GetContact(int contactID)
        {
            Contact contact = null;
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = (from a in dc.Contacts
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         join c in dc.States on a.StateID equals c.StateID   
                         where a.ContactID.Equals(contactID)
                         select new
                         {
                             a,
                             b.CountryName,
                             c.StateName
                         }).FirstOrDefault();
                if (v != null)
                {
                    contact = v.a;
                    contact.CountryName = v.CountryName;
                    contact.StateName = v.StateName;
                }
                return contact;
            }
        }


        public ActionResult Save(int id = 0)
        {
            List<Country> Country = GetCountry();
            List<State> States = new List<State>();

            if (id > 0)
            {
                var c = GetContact(id);
                if (c != null)
                {
                    ViewBag.Countries = new SelectList(Country, "CountryID", "CountryName", c.CountryID);
                    ViewBag.States = new SelectList(GetState(c.CountryID), "StateID", "StateName", c.StateID);
                }
                else
                {
                    return HttpNotFound();
                }
                return PartialView("Save", c);
            }
            else
            {
                ViewBag.Countries = new SelectList(Country, "CountryID", "CountryName");
                ViewBag.States = new SelectList(States, "StateID", "StateName");
                return PartialView("Save");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Contact c)
        {
            string message = "";
            bool status = false;
            if (ModelState.IsValid)
            {
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {
                    if (c.ContactID > 0)
                    {
                        var v = dc.Contacts.Where(a => a.ContactID.Equals(c.ContactID)).FirstOrDefault();
                        if (v != null)
                        {
                            v.ContactPerson = c.ContactPerson;
                            v.ContactNo = c.ContactNo;
                            v.CountryID = c.CountryID;
                            v.StateID = c.StateID;
                        }
                        else
                        {
                            return HttpNotFound();
                        }
                    }
                    else
                    {
                        dc.Contacts.Add(c);
                    }
                    dc.SaveChanges();
                    status = true;
                    message = "Successfully Saved.";
                }
            }
            else
            {
                message = "Error! Please try again.";
            }

            return new JsonResult { Data = new { status = status, message = message} };
        }

        public ActionResult Delete(int id)
        {
            var c = GetContact(id);
            if (c == null)
            {
                return HttpNotFound();
            }
            return PartialView(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteContact(int id)
        {
            bool status = false;
            string message = "";
            using (MyDatabaseEntities dc = new MyDatabaseEntities())
            {
                var v = dc.Contacts.Where(a => a.ContactID.Equals(id)).FirstOrDefault();
                if (v != null)
                {
                    dc.Contacts.Remove(v);
                    dc.SaveChanges();
                    status = true;
                    message = "Successfully Deleted!";
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return new JsonResult { Data = new { status = status, message = message } };
        }
    }
}
