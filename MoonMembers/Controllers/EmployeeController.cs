using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MoonMembers.Models;

namespace MoonMembers.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeDataAccessLayer objEmployee = new EmployeeDataAccessLayer();

        public ActionResult Index()
        {
            List<Employee> lstEmployee = new List<Employee>();
            lstEmployee = objEmployee.GetAllEmployees().ToList();

            return View(lstEmployee);
        }


        #region Create

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] Employee employee)
        {
            if (ModelState.IsValid)
            {
                objEmployee.AddEmployee(employee);

                return RedirectToAction("Index");
            }

            return View(employee);
        }

        #endregion


        #region Edit

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Employee employee = objEmployee.GetEmployeeData(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind]Employee employee)
        {
            if (id != employee.ID)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                objEmployee.UpdateEmployee(employee);

                return RedirectToAction("Index");
            }

            return View(employee);
        }

        #endregion


        #region Details

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Employee employee = objEmployee.GetEmployeeData(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        #endregion


        #region Delete

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Employee employee = objEmployee.GetEmployeeData(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            objEmployee.DeleteEmployee(id);

            return RedirectToAction("Index");
        }

        #endregion
    }
}
