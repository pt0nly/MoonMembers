using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using MoonMembers.Models;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
using System.Drawing.Imaging;
using PagedList;

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
        public ActionResult Index(int? page)
        {
            // 6 Elementos por página
            int pageSize = 6;
            int pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            // Apenas lista os membros activos, e ordenados
            IPagedList<Members> members = db.Members
                        .Where(mdl => mdl.MemberStatus == true)
                        .OrderBy(mdl => mdl.MemberOrder)
                        .ToPagedList(pageIndex, pageSize);

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
            var members = db.Members.OrderBy(mdl => mdl.MemberOrder);

            return View("Backoffice/Index", members);
        }

        [HttpPost]
        public ActionResult UpdateStatusOrder(List<Members> model)
        {
            // Update code to update MemberOrder
            foreach(var item in model)
            {
                var status = db.Members.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                if (status == null)
                {
                    status.MemberOrder = item.MemberOrder;
                }

                db.SaveChanges();
            }


            return Content("asdas");
        }

        #region Create

        [HttpGet]
        [Route("backoffice/members/create", Name = "BackCreateMember")]
        public ActionResult BackofficeCreate()
        {
            var model = new MembersViewModel();

            return View("Backoffice/Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/create")]
        public ActionResult BackofficeCreate(MembersViewModel model)
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
                    member.MemberPhoto = String.Format("/Imagens/Membros/{0}{1}", imagemNome, extensao);
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

            return View("Backoffice/Edit", (MembersViewModel)member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/edit/{id}")]
        public ActionResult BackofficeEdit(int id, [Bind] MembersViewModel model)
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
                            member.MemberPhoto = String.Format("/Imagens/Membros/{0}{1}", imagemNome, extensao);
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
        [Route("backoffice/members/deleteConfirmed/{id?}", Name = "BackMemberDeleteConfirm")]
        public ActionResult BackofficeDeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Members member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            string pathPhoto = Server.MapPath(member.MemberPhoto);

            db.Members.Remove(member);
            db.SaveChanges();

            FileInfo file = new FileInfo(pathPhoto);
            if (file.Exists)
            {
                file.Delete();
            }

            return RedirectToRoute("BackMembers");
        }

        #endregion


        #region ImageEncoding

        private string ImageToBase64(string filePath)
        {
            string base64String = null;

            using (Image image = Image.FromFile(filePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, image.RawFormat);
                    byte[] imageBytes = ms.ToArray();

                    base64String = Convert.ToBase64String(imageBytes);
                }
            }

            return base64String;
        }

        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);

            return Image.FromStream(ms, true);
        }

        #endregion


        #region XML

        [HttpGet]
        [Route("backoffice/members/exportxml", Name = "BackMemberExportXml")]
        public FileStreamResult ExportXml()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                // Lista todos os membros, e ordenados
                var members = db.Members.OrderBy(model => model.MemberOrder);
                XDocument doc = new XDocument();

                XElement rootNode = new XElement("Members");
                foreach (var item in members)
                {
                    // Converte imagem para base64, para ser incluída no XML
                    string photoFile = ImageToBase64( Server.MapPath(item.MemberPhoto) );

                    XElement itemNode = new XElement("member",
                        new XElement("MemberId", item.MemberId),
                        new XElement("MemberName", item.MemberName),
                        new XElement("MemberEmail", item.MemberEmail),
                        new XElement("MemberBirthdate", item.MemberBirthdate),
                        new XElement("MemberPhoto", item.MemberPhoto),
                        new XElement("MemberOrder", item.MemberOrder),
                        new XElement("MemberStatus", item.MemberStatus),
                        new XElement("MemberPhoto",
                            new XAttribute("path", item.MemberPhoto),
                            new XCData(photoFile)
                        )
                    );

                    rootNode.Add(itemNode);
                }
                doc.Add(rootNode);

                doc.WriteTo(xw);
            }
            ms.Position = 0;

            return File(ms, "application/xml", "ClubMembers.xml");
        }

        public ActionResult ImportXML()
        {
            return Content("Import XML");
        }

        #endregion

        #endregion
    }
}
