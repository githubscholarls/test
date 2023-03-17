namespace EFAttribute.Domain.Entity
{
    public class wechat
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }

        //多余的影子外键
        //public int userId { get; set; }

        public user user { get; set; }
    }
}
