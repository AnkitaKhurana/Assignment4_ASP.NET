using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment4_ASP.NET.Models;
using System.Data.Entity.Infrastructure;

namespace Assignment4_ASP.NET.Controllers
{
    public class AsyncCustomersController : Controller
    {
        private Assignment4_ASPNETContext db = new Assignment4_ASPNETContext();

        // GET: AsyncCustomers
        public async Task<ActionResult> Index()
        {
            return View(await db.Customers.ToListAsync());
        }

        // GET: AsyncCustomers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: AsyncCustomers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AsyncCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,DateOfBirth,RowVersion,Amount")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: AsyncCustomers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: AsyncCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "ID","Name", "DateOfBirth", "RowVersion", "Amount" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var  customerToUpdate = await db.Customers.FindAsync(id);
            if (customerToUpdate == null)
            {
                Customer deletedCustomer = new Customer();
                TryUpdateModel(deletedCustomer, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The customer was deleted by another user.");
                ViewBag.InstructorID = new SelectList(db.Customers, "ID", "FullName", deletedCustomer.ID);
                return View(deletedCustomer);
            }



            if (TryUpdateModel(customerToUpdate, fieldsToBind))
            {
                try
                {
                    db.Entry(customerToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Customer)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The customer was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Customer)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);

                        if (databaseValues.Amount != clientValues.Amount)
                            ModelState.AddModelError("Amount", "Current value: "
                                + databaseValues.Amount);

                        if (databaseValues.DateOfBirth != clientValues.DateOfBirth)
                            ModelState.AddModelError("DateOfBirth", "Current value: "
                                + databaseValues.DateOfBirth);                     

                        if (databaseValues.ID != clientValues.ID)
                            ModelState.AddModelError("ID", "Current value: "
                        + db.Customers.Find(databaseValues.ID).Name);

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        customerToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.InstructorID = new SelectList(db.Customers, "ID", "Name", customerToUpdate.ID);
            return View(customerToUpdate);

        }


        /*
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,DateOfBirth,RowVersion,Amount")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        */

        // GET: AsyncCustomers/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    //ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    //+ "was modified by another user after you got the original values. "
                    //+ "The delete operation was canceled and the current values in the "
                    //+ "database have been displayed. If you still want to delete this "
                    //+ "record, click the Delete button again. Otherwise "
                    //+ "click the Back to List hyperlink.";
                   return RedirectToAction("Index");
                }
                //else
                return HttpNotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(customer);
        }
        
        // POST: AsyncCustomers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<ActionResult> Delete(Customer customer)
        {
            try
            {
                db.Entry(customer).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
              
                return RedirectToAction("Delete", new { concurrencyError = true, id = customer.ID });

            }
            
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(customer);
            }                
           
        }
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        Customer customer = await db.Customers.FindAsync(id);
        //        db.Customers.Remove(customer);
        //        await db.SaveChangesAsync();
        //        //return RedirectToAction("Index");
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return RedirectToAction("Delete", new { concurrencyError = true, id = id });
        //    }
        //    catch (DataException /* dex */)
        //    {
        //        //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
        //        ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");

        //    }
        //    return RedirectToAction("Index");

        //}

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
