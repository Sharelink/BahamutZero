using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BahamutService.Model
{
    [Table("BahamutDB.Account")]
    public partial class Account
    {
        [Column(TypeName = "ubigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal AccountID { get; set; }

        [Required]
        [StringLength(60)]
        public string AccountName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(40)]
        public string Mobile { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        [Required]
        [StringLength(1073741823)]
        public string Password { get; set; }

        [StringLength(1073741823)]
        public string Extra { get; set; }
    }
}
