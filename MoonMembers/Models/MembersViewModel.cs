using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MoonMembers.Models
{
    public class MembersViewModel
    {
        public Int32 MemberId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome")]
        [StringLength(120)]
        public string MemberName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatorio", AllowEmptyStrings = false)]
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

        [Display(Name = "Ordem")]
        public int MemberOrder { get; set; }

        [Display(Name = "Activo")]
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

            return model;
        }
    }
}