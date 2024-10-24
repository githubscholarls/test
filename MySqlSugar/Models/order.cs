using SqlSugar;

namespace MySqlSugar.Models
{
    public class order
    {
        public long id { get; set; }
        [SugarColumn(ColumnName ="cust_id")]
        public long? huiyuan_id { get; set; }

        public decimal? money { get; set; }

        public DateTime? addtime { get; set; }
    }
}
