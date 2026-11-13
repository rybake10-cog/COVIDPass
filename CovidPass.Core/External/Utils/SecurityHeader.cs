// © 2020 Partners HealthCare Technology & Innovation Department
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace CovidPass.Core.External.Utils
{

    public class SecurityHeader : MessageHeader
    {
        private readonly UsernameToken _usernameToken;

        public SecurityHeader(string username, string password)
        {
            _usernameToken = new UsernameToken(username, password);
        }
        public override bool MustUnderstand => true;

        public override string Name => "Security";

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(UsernameToken));
            serializer.Serialize(writer, _usernameToken);
        }
    }

    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class UsernameToken
    {
        public UsernameToken() { }

        public UsernameToken(string username, string password)
        {
            Username = username;
            Password = new Password { Value = password };
        }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public Password Password { get; set; }
    }

    public class Password
    {
        public Password()
        {
            Type = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
        }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
