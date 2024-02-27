using System.ComponentModel.DataAnnotations.Schema;

namespace EFAttribute.Domain.Entity
{
    [Table("user")]
    public class pguser
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? sex { get; set; }
        public string? state { get; set; }
        public string? add_time { get; set; }
        public string? verify { get; set; }
        public string? last_login_time { get; set; }
    }
}
