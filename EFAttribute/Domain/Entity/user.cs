namespace EFAttribute.Domain.Entity
{
    public class user
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? sex { get; set; }
        public string? state { get; set; }
        public string? add_time { get; set; }
        public string? verify { get; set; }
        public string? lastLoginTime { get; set; }

        #region userPosition Table

        public string? bornAddress { get; set; }
        public string? schoolAddress { get; set; }
        public string? homeAddress { get; set; }
        public string? nowAddress { get; set; }
        #endregion
    }
}
