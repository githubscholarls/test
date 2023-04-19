namespace WebApiTest
{
    public static class Consts
    {
        #region Schemes

        public const string Cookies = nameof(Cookies);
        public const string CookiesAuth = nameof(CookiesAuth);
        public const string UrlTokenScheme = nameof(UrlTokenScheme);
        public const string UrlTokenScheme2 = nameof(UrlTokenScheme2);
        public const string Bearer = nameof(Bearer);

        #endregion


        #region Role

        public const string Admin = nameof(Admin);
        public const string User = nameof(User);

        #endregion

        #region Policy

        public const string CustomPolicy = nameof(CustomPolicy);
        public const string AssertionAdminPolicy = nameof(AssertionAdminPolicy);

        #endregion
    }
}
