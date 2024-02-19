namespace test_api_rest.Models
{
    public class UserAccessData
    {
        public string login { get; set; }

        public string AesKey { get; set; }

        public Tokens Tokens { get; set; }
    }
}
