namespace MoonMembers.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("members")]
    public partial class Members
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Nome")]
        [StringLength(120)]
        public string MemberName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        [StringLength(120)]
        public string MemberEmail { get; set; }

        [Display(Name = "Fotografia")]
        public string MemberPhoto { get; set; }

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
    }
}
