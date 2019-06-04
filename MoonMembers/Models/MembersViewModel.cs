using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MoonMembers.Models
{
    public class MembersViewModel
    {
        public Int32 MemberId { get; set; }

        [Required(ErrorMessage = "O nome do membro é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome do Membro")]
        [StringLength(120)]
        public string MemberName { get; set; }

        [Required(ErrorMessage = "O e-mail do produto é obrigatorio", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        [StringLength(120)]
        public string MemberEmail { get; set; }

        [Display(Name = "Fotografia")]
        public string MemberPhoto { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Fotografia")]
        public HttpPostedFileBase ReplacePhoto { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória", AllowEmptyStrings = false)]
        [Display(Name = "Data de Nascimento")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MemberBirthdate { get; set; }

        public int MemberOrder { get; set; }

        public bool MemberStatus { get; set; }

        /*
         * Esta função serve para converter do Model "Members" para o Model "MembersViewModel"
         */
        public static explicit operator MembersViewModel(Members v)
        {
            MembersViewModel model = new MembersViewModel
            {
                MemberId = v.MemberId,
                MemberName = v.MemberName,
                MemberEmail = v.MemberEmail,
                MemberBirthdate = v.MemberBirthdate,
                MemberPhoto = v.MemberPhoto,
                MemberOrder = v.MemberOrder,
                MemberStatus = v.MemberStatus
            };
            /*
            MembersViewModel model = new MembersViewModel();
            model.MemberId = v.MemberId;
            model.MemberName = v.MemberName;
            model.MemberEmail = v.MemberEmail;
            model.MemberBirthdate = v.MemberBirthdate;
            model.MemberPhoto = v.MemberPhoto;
            model.MemberOrder = v.MemberOrder;
            model.MemberStatus = v.MemberStatus;
            */

            return model;
        }
    }
}