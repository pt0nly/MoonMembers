﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        MembersDbContext db;

        public MembersController()
        {
            db = new MembersDbContext();
        }

        private void SalvarNaPasta(Image img, string caminho)
        {
            using (System.Drawing.Image novaImagem = new Bitmap(img))
            {
                novaImagem.Save(Server.MapPath(caminho), img.RawFormat);
            };
        }

        private bool IsMemberEmailExist(int memberId, string email)
        {
            return db.Members.Any(m => m.MemberEmail == email && m.MemberId != memberId);
        }


        #region FrontOffice

        // GET: Members
        public ActionResult Index()
        {
            // Apenas lista os membros activos, e ordenados
            var members = db.Members.Where(model => model.MemberStatus.Equals(1)).OrderBy(model => model.MemberOrder);

            return View("Index", members);
        }

        #endregion


        #region BackOffice

        /*
         * GET: Membros
         */
        [Route("backoffice/members", Name = "BackMembers")]
        public ActionResult BackofficeIndex()
        {
            // Lista todos os membros, e ordenados
            var members = db.Members.OrderBy(model => model.MemberOrder);

            return View("Backoffice/Index", members);
        }


        #region Create

        [HttpGet]
        [Route("backoffice/members/create", Name = "BackCreateMember")]
        public ActionResult BackofficeCreate()
        {
            var model = new Members();

            return View("Backoffice/Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/create")]
        public ActionResult BackofficeCreate(Members model)
        {
            if (IsMemberEmailExist(model.MemberId, model.MemberEmail))
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

            if (model.ReplacePhoto == null || model.ReplacePhoto.ContentLength == 0)
            {
                ModelState.AddModelError("ReplacePhoto", "Este campo é obrigatório");
            }
            else if (!imageTypes.Contains(model.ReplacePhoto.ContentType))
            {
                ModelState.AddModelError("ReplacePhoto", "Escolha uma imagem GIF, JPG ou PNG.");
            }

            if (ModelState.IsValid)
            {
                var member = new Members
                {
                    MemberName = model.MemberName,
                    MemberEmail = model.MemberEmail,
                    MemberBirthdate = model.MemberBirthdate,
                    MemberOrder = 0,
                    MemberStatus = true
                };

                // Salvar a imagem para a pasta e guardar o caminho
                var imagemNome = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
                var extensao = System.IO.Path.GetExtension(model.ReplacePhoto.FileName).ToLower();

                using (var img = System.Drawing.Image.FromStream(model.ReplacePhoto.InputStream))
                {
                    member.MemberPhoto = String.Format("/Imagens/{0}{1}", imagemNome, extensao);
                    // Salva imagem
                    SalvarNaPasta(img, member.MemberPhoto);
                };

                db.Members.Add(member);
                db.SaveChanges();

                return RedirectToAction("BackofficeIndex");
            }

            // Se ocorrer um erro retorna para a página

            return View("Backoffice/Create", model);
        }

        #endregion


        #region Details

        /*
         * GET: Membros/Detail/5
         */
        [Route("backoffice/members/detail/{id?}", Name = "BackMemberDetail")]
        public ActionResult BackofficeDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Members member = db.Members.Find(id);
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

            Members member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            return View("Backoffice/Edit", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/edit/{id}")]
        public ActionResult BackofficeEdit(int id, [Bind] Members model)
        {
            if (IsMemberEmailExist(id, model.MemberEmail))
            {
                ModelState.AddModelError("MemberEmail", "Este e-mail já se encontra registado.");
            }

            if (ModelState.IsValid)
            {
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

                if (model.ReplacePhoto == null)
                {
                    replacingPhoto = false;
                }
                else if (model.ReplacePhoto.ContentLength == 0)
                {
                    ModelState.AddModelError("ReplacePhoto", "Escolha uma imagem GIF, JPG ou PNG válida.");
                }
                else if (!imageTypes.Contains(model.ReplacePhoto.ContentType))
                {
                    ModelState.AddModelError("ReplacePhoto", "Escolha uma imagem GIF, JPG ou PNG.");
                }

                if (ModelState.IsValid)
                {
                    member.MemberName = model.MemberName;
                    member.MemberEmail = model.MemberEmail;
                    member.MemberBirthdate = model.MemberBirthdate;
                    member.MemberOrder = model.MemberOrder;
                    member.MemberStatus = model.MemberStatus;

                    // Guarda o caminho da photo anterior
                    string previousPhoto = member.MemberPhoto;

                    if (replacingPhoto)
                    {
                        // Salvar a imagem para a pasta e guardar o caminho
                        var imagemNome = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
                        var extensao = System.IO.Path.GetExtension(model.ReplacePhoto.FileName).ToLower();

                        using (var img = System.Drawing.Image.FromStream(model.ReplacePhoto.InputStream))
                        {
                            member.MemberPhoto = String.Format("/Imagens/{0}{1}", imagemNome, extensao);
                            // Salva imagem
                            SalvarNaPasta(img, member.MemberPhoto);
                        };
                    }

                    db.SaveChanges();

                    if (replacingPhoto && previousPhoto != member.MemberPhoto)
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
            }

            return View("Backoffice/Edit", model);
        }

        #endregion


        #region Delete

        // GET: Members/Delete/5
        [HttpGet]
        [Route("backoffice/members/delete/{id?}", Name = "BackMemberDelete")]
        public ActionResult BackofficeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Members member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }


            return View("Backoffice/Delete", member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("BackofficeDelete")]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/deleteConfirmed{id}", Name = "BackMemberDeleteConfirm")]
        public ActionResult BackofficeDeleteConfirmed(int id)
        {
            Members member = db.Members.Find(id);
            db.Members.Remove(member);

            db.SaveChanges();

            return RedirectToAction("Backoffice/Index");
        }

        #endregion

        #endregion
    }
}
