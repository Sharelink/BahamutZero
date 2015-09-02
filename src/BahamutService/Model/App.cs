using BahamutCommon;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BahamutService.Model
{
    public interface IAppUserDBConnectionConfig
    {
        string DBConnectionString { get; set; }
        string DBName { get; set; }
        string TableName { get; set; }
    }

    public class AppUserDBConnectionConfig:IAppUserDBConnectionConfig
    {
        public string DBConnectionString { get; set; }
        public string DBName { get; set; }
        public string TableName { get; set; }
    }

    [Table("BahamutDB.App")]
    public partial class App
    {
        [Column(TypeName = "uint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Key]
        public string Appkey { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(1073741823)]
        public string Document { get; set; }

        [StringLength(16777215)]
        public string Description { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }
    }

    public partial class App
    {
        [NotMapped]
        public dynamic DocumentModel
        {
            get { return BahamutCommon.DocumentModel.ToDocumentObject(this.Document); }
            set { this.Document = BahamutCommon.DocumentModel.ToDocument(value); }
        }
    }
}
