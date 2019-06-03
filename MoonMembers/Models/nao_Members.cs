namespace MoonMembers.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Data.Entity.Spatial;
    using System.Web;

    public class Members
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int memberId { get; set; }

        [Required(ErrorMessage = "O nome do membro é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome do membro")]
        [StringLength(120)]
        public string memberName { get; set; }

        [Required(ErrorMessage = "O email do membro é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Email do membro")]
        [StringLength(120)]
        public string memberEmail { get; set; }

        [Display(Name = "Fotografia do membro")]
        public string memberPhoto { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Fotografia")]
        public HttpPostedFileBase replacePhoto { get; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória", AllowEmptyStrings = false)]
        [Display(Name = "Data de Nascimento")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime memberBirthdate { get; set; }

        public int memberOrder { get; set; }

        public byte memberStatus { get; set; }
    }
}
