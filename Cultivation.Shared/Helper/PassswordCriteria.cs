namespace Cultivation.Shared.Helper
{
    public class PasswordCriteria
    {
        public int RequiredLength { get; set; } = 6;
        public int RequiredUniqueChars { get; set; } = 1;
        public bool RequireDigit { get; set; } = true;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public bool RequireUppercase { get; set; } = false;
    }
}
