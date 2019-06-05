using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using MoonMembers.Models;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using PagedList;
using System.Text.RegularExpressions;

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

            ViewData["menu"] = "Membros";

            return View("Index", members);
        }

        #endregion


        #region BackOffice

        [Route("backoffice/members", Name = "BackMembers")]
        public ActionResult BackofficeIndex()
        {
            // Lista todos os membros, e ordenados
            var members = db.Members.OrderBy(mdl => mdl.MemberOrder);

            ViewData["menu"] = "Membros";

            return View("Backoffice/Index", members);
        }


        #region Ajax

        [Route("backoffice/members/getlist", Name = "BackMemberGetList")]
        public PartialViewResult GetList()
        {
            // Lista todos os membros, e ordenados
            var members = db.Members.OrderBy(mdl => mdl.MemberOrder);

            return PartialView("Backoffice/_MemberList", members);
        }

        [HttpPost]
        [Route("backoffice/members/updateOrder", Name = "BackMemberUpdateOrder")]
        public ActionResult UpdateOrder(List<Members> model)
        {
            // Update code to update MemberOrder
            foreach (var item in model)
            {
                var status = db.Members.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                if (status != null)
                {
                    status.MemberOrder = item.MemberOrder;
                }

                db.SaveChanges();
            }

            return Content("OK");
        }

        #endregion


        #region Create

        [HttpGet]
        [Route("backoffice/members/create", Name = "BackCreateMember")]
        public ActionResult BackofficeCreate()
        {
            var model = new MembersViewModel();

            ViewData["menu"] = "Membros";

            return View("Backoffice/Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/create")]
        public ActionResult BackofficeCreate(MembersViewModel model)
        {
            string strEmailReg = @"^([\w-\.]+)@((\[[0-9]{2,3}\.[0-9]{2,3}\.[0-9]{2,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{2,3})(\]?)$";

            if (model.MemberEmail == null || !Regex.IsMatch(model.MemberEmail, strEmailReg))
            {
                ModelState.AddModelError("memberEmail", "Introduza um e-mail válido.");
            }

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

                ViewData["menu"] = "Membros";

                return RedirectToAction("BackofficeIndex");
            }

            ViewData["menu"] = "Membros";

            // Se ocorrer um erro retorna para a página
            return View("Backoffice/Create", model);
        }

        #endregion


        #region Details

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

            ViewData["menu"] = "Membros";

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

            ViewData["menu"] = "Membros";

            return View("Backoffice/Edit", (MembersViewModel)member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("backoffice/members/edit/{id}")]
        public ActionResult BackofficeEdit(int id, [Bind] MembersViewModel model)
        {
            string strEmailReg = @"^([\w-\.]+)@((\[[0-9]{2,3}\.[0-9]{2,3}\.[0-9]{2,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{2,3})(\]?)$";

            if (model.MemberEmail == null || !Regex.IsMatch(model.MemberEmail, strEmailReg))
            {
                ModelState.AddModelError("memberEmail", "Introduza um e-mail válido.");
            }

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

                    ViewData["menu"] = "Membros";

                    return RedirectToAction("BackofficeIndex");
                }
            }

            ViewData["menu"] = "Membros";

            return View("Backoffice/Edit", model);
        }

        #endregion


        #region Delete

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

            ViewData["menu"] = "Membros";

            return View("Backoffice/Delete", member);
        }

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

            ViewData["menu"] = "Membros";

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
                        new XElement("MemberOrder", item.MemberOrder),
                        new XElement("MemberStatus", item.MemberStatus),
                        new XElement("MemberPhoto",
                            new XAttribute("extension", System.IO.Path.GetExtension( item.MemberPhoto ).ToLower()  ),
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

        [HttpPost]
        [Route("backoffice/members/importxml", Name = "BackMemberImportXml")]
        public ActionResult ImportXML(HttpPostedFileBase xmlFile)
        {
            if (xmlFile.ContentType.Equals("application/xml") || xmlFile.ContentType.Equals("text/xml"))
            {
                var xmlPath = Server.MapPath("~/FileUpload/" + xmlFile.FileName);
                xmlFile.SaveAs(xmlPath);

                // Apagar todos os dados e fotos
                foreach(var member in db.Members)
                {
                    FileInfo file = new FileInfo( Server.MapPath(member.MemberPhoto) );
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                db.Members.RemoveRange(db.Members);
                db.SaveChanges();


                // Ler ficheiro XML e importar os dados para a base de dados
                XDocument doc = XDocument.Load(xmlPath);
                
                foreach(var xmlMember in doc.Descendants("member"))
                {
                    Members member = new Members
                    {
                        MemberId = Convert.ToInt32(xmlMember.Element("MemberId").Value),
                        MemberName = xmlMember.Element("MemberName").Value,
                        MemberEmail = xmlMember.Element("MemberEmail").Value,
                        MemberBirthdate = Convert.ToDateTime(xmlMember.Element("MemberBirthdate").Value),
                        MemberOrder = Convert.ToInt32(xmlMember.Element("MemberOrder").Value),
                        MemberStatus = Convert.ToBoolean(xmlMember.Element("MemberStatus").Value),
                        MemberPhoto = ""
                    };

                    // Guardar a imagem num ficheiro, caso tenha dados
                    if (xmlMember.Element("MemberPhoto").Value.Length > 0)
                    {
                        var imagemNome = String.Format("{0:yyyyMMdd-HHmmssfff}", DateTime.Now);
                        var extensao = xmlMember.Element("MemberPhoto").Attribute("extension").Value;

                        member.MemberPhoto = String.Format("/Imagens/Membros/{0}{1}", imagemNome, extensao);

                        // Salva imagem
                        SalvarNaPasta(Base64ToImage(xmlMember.Element("MemberPhoto").Value), member.MemberPhoto);
                    }

                    // Guarda o registo
                    db.Members.Add(member);
                }
                db.SaveChanges();

                // Remove o ficheiro XML
                FileInfo xmlInfo = new FileInfo(xmlPath);
                if (xmlInfo.Exists)
                {
                    xmlInfo.Delete();
                }

                ViewBag.Success = "XML importado com sucesso!";
            }
            else
            {
                ViewBag.Error = "Ficheiro inválido (Upload apenas ficheiro XML)!";
            }

            return RedirectToRoute("BackMembers");
        }

        #endregion

        #endregion
    }
}
