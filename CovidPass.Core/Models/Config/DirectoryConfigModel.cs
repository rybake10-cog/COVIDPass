namespace CovidPass.Core.Models.Config
{
    public class DirectoryConfigModel
    {
        public string Domain { get; set; } = "YOUR_DOMAIN_NAME";
        public string Host { get; set; } = "YOUR_HOST";
        public int Port { get; set; } = 1111;
        public string BindDnTemplate { get; set; } = "uid={0},ou=internal,ou=people,dc=entdir,dc=your_org_name,dc=org";
        public string SearchBase { get; set; } = "dc=entdir,dc=your_org_name,dc=org";
    }
}
