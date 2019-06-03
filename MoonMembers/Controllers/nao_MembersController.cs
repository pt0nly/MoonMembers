using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using MoonMembers.Models;
using System.Net;
using System.IO;
using System.Data.SqlClient;

namespace MoonMembers.Controllers
{
    public class MembersController : Controller
    {
        MembersDataAccessLayer objMembers = new MembersDataAccessLayer();

        private void SalvarNaPasta(Image img, string caminho)
        {
            using (System.Drawing.Image novaImagem = new Bitmap(img))
            {
                novaImagem.Save(Server.MapPath(caminho), img.RawFormat);
            };
        }

        private bool IsMemberEmailExist(int memberId, string email)
        {
            //return db.Members.Any(m => m.memberEmail == email && m.memberId != memberId);
            return false;
        }


        #region FrontOffice

        // GET: Members
        public ActionResult Index()
        {
            List<Members> members = new List<Members>();
            members = objMembers.GetAllMembers(1).ToList();

            return View(members);
        }

        #endregion


        #region BackOffice

        /*
         * GET: Membros
         */
        [Route("backoffice/members", Name = "BackMembers")]
        public ActionResult BackofficeIndex()
        {
            List<Members> members = new List<Members>();
            members = objMembers.GetAllMembers(0).ToList();

            return View("Backoffice/Index", members);
        }


        #region Create

        [HttpGet]
        [Route("backoffice/members/create", Name = "BackCreateMember")]
        public ActionResult BackofficeCreate()
        {
            /*
            var model = new MembersViewModel();

            return View("Backoffice/Create", model);
            */
            return View("Backoffice/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/create")]
        public ActionResult BackofficeCreate([Bind] Members member)
        {
            if (IsMemberEmailExist(member.memberId, member.memberEmail))
            {
                ModelState.AddModelError("memberEmail", "Este e-mail já se encontra registado.");
            }

            var imageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            if (member.replacePhoto == null || member.replacePhoto.ContentLength == 0)
            {
                ModelState.AddModelError("replacePhoto", "Este campo é obrigatório");
            }
            else if (!imageTypes.Contains(member.replacePhoto.ContentType))
            {
                ModelState.AddModelError("replacePhoto", "Escolha uma imagem GIF, JPG ou PNG.");
            }

            if (ModelState.IsValid)
            {
                // Salvar a imagem para a pasta e guardar o caminho
                var imagemNome = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
                var extensao = System.IO.Path.GetExtension(member.replacePhoto.FileName).ToLower();

                using (var img = System.Drawing.Image.FromStream(member.replacePhoto.InputStream))
                {
                    member.memberPhoto = String.Format("/Imagens/{0}{1}", imagemNome, extensao);
                    // Salva imagem
                    SalvarNaPasta(img, member.memberPhoto);
                };

                objMembers.AddMember(member);

                return RedirectToAction("BackofficeIndex");
            }

            // Se ocorrer um erro retorna para a página

            return View("Backoffice/Create", member);
        }

        #endregion


        #region Details

        /*
         * GET: Membros/Detail/5
         */
        [HttpGet]
        [Route("backoffice/members/detail/{id?}", Name = "BackMemberDetail")]
        public ActionResult BackofficeDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Members member = objMembers.GetMemberData(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            return View("Backoffice/Detail", member);
        }

        #endregion


        #region Edit

        [HttpGet]
        [Route("backoffice/members/edit/{id?}", Name = "BackMemberEdit")]
        public ActionResult BackofficeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Members member = objMembers.GetMemberData(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            return View("Backoffice/Edit", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/edit/{id}")]
        public ActionResult BackofficeEdit(int id, [Bind] Members member)
        {
            if (id != member.memberId)
            {
                return HttpNotFound();
            }

            if (IsMemberEmailExist(id, member.memberEmail))
            {
                ModelState.AddModelError("memberEmail", "Este e-mail já se encontra registado.");
            }

            if (ModelState.IsValid)
            {
                objMembers.UpdateMember(member);
                /*
                var member = db.Members.Find(id);
                if (member == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                bool replacingPhoto = true;
                var imageTypes = new string[]
                {
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                };

                if (model.replacePhoto == null)
                {
                    replacingPhoto = false;
                }
                else if (model.replacePhoto.ContentLength == 0)
                {
                    ModelState.AddModelError("memberPhoto", "Escolha uma imagem GIF, JPG ou PNG válida.");
                }
                else if (!imageTypes.Contains(model.replacePhoto.ContentType))
                {
                    ModelState.AddModelError("memberPhoto", "Escolha uma imagem GIF, JPG ou PNG.");
                }

                if (ModelState.IsValid)
                {
                    member.memberName = model.memberName;
                    member.memberEmail = model.memberEmail;
                    member.memberBirthdate = model.memberBirthdate;
                    member.memberOrder = model.memberOrder;
                    member.memberStatus = model.memberStatus;

                    // Guarda o caminho da photo anterior
                    string previousPhoto = member.memberPhoto;

                    if (replacingPhoto)
                    {
                        // Salvar a imagem para a pasta e guardar o caminho
                        var imagemNome = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
                        var extensao = System.IO.Path.GetExtension(model.replacePhoto.FileName).ToLower();

                        using (var img = System.Drawing.Image.FromStream(model.replacePhoto.InputStream))
                        {
                            member.memberPhoto = String.Format("/Imagens/{0}{1}", imagemNome, extensao);
                            // Salva imagem
                            SalvarNaPasta(img, member.memberPhoto);
                        };
                    }

                    db.SaveChanges();

                    if (replacingPhoto && previousPhoto != member.memberPhoto)
                    {
                        // Registo foi salvo, já se pode remover photo antiga
                        FileInfo fileOldPhoto = new FileInfo( Server.MapPath(previousPhoto) );
                        if (fileOldPhoto.Exists)
                        {
                            fileOldPhoto.Delete();
                        }
                    }

                    return RedirectToAction("BackofficeEdit");
                }
                */

                return RedirectToAction("BackofficeIndex");
            }

            return View("Backoffice/Edit", member);
        }

        #endregion


        #region Delete

        [HttpGet]
        [Route("backoffice/members/delete/{id?}", Name = "BackMemberDelete")]
        public ActionResult BackofficeDelete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Members member = objMembers.GetMemberData(id);
            if (member == null)
            {
                return HttpNotFound();
            }


            return View("Backoffice/Delete", member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("BackofficeDelete")]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/deleteConfirmed{id?}", Name = "BackMemberDeleteConfirm")]
        public ActionResult BackofficeDeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            objMembers.DeleteMember(id);

            return RedirectToAction("Backoffice/Index");
        }

        #endregion

        #endregion
    }
}
/*
 * Create tblEmployee:
 * Create table tblEmployee(
 *  EmployeeId int IDENTITY(1,1) NOT NULL,
 *  Name varchar(20) NOT NULL,
 *  City varchar(20) NOT NULL,
 *  Department varchar(20) NOT NULL,
 *  Gender varchar(6) NOT NULL
 * )
 * 
 * Create Stored Procedures:
 * 
 * To Insert an Employee Record
 * Create procedure spAddEmployee(
 *  @Name VARCHAR(20),
 *  @City VARCHAR(20),
 *  @Department VARCHAR(20),
 *  @Gender VARCHAR(6)
 * ) as
 * Begin
 *  Insert into tblEmployee (Name, City, Department, Gender)
 *  Values (@Name, @City, @Department, @Gender)
 * End
 * 
 * To Update an Employee Record
 * Create procedure spUpdateEmployee(
 *  @EmpId INTEGER,
 *  @Name VARCHAR(20),
 *  @City VARHCAR(20),
 *  @Department VARCHAR(20),
 *  @Gender VARCHAR(6)
 * ) as
 * Begin
 *  Update tblEmployee
 *      set Name=@Name,
 *          City=@City,
 *          Department=@Department,
 *          Gender=@Gender
 *  where EmployeeId=@EmpId
 * End
 * 
 * To Delete an Employee Record
 * Create procedure spDeleteEmployee(
 *  @EmpId int
 * ) as
 * Begin
 *  Delete from tblEmployee where EmployeeId=@EmpId
 * End
 * 
 * To View all Employee Records
 * Create procedure spGetAllEmployees as
 * Begin
 *  select *
 *  from tblEmployee
 *  order by EmployeeId
 * End
 */