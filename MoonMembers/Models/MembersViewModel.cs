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
        public string memberName { get; set; }

        [Required(ErrorMessage = "O e-mail do produto é obrigatorio", AllowEmptyStrings = false)]
        [Display(Name = "E-mail do Membro")]
        [StringLength(120)]
        public string memberEmail { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = "Fotografia")]
        public HttpPostedFileBase memberPhoto { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória", AllowEmptyStrings = false)]
        [Display(Name = "Data de Nascimento")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime memberBirthdate { get; set; }

        public int memberOrder { get; set; }

        public bool memberStatus { get; set; }
    }
}