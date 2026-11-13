// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

namespace CovidPass.Core.External.Auth
{
    public interface IDirectoryService
    {
        bool ValidateUser(string domain, string username, string password);
        DirectoryUserInfo GetUserInfo(string serviceAccount, string servicePassword, string username, string employeeNumber);
        bool IsUserLocked(string domain, string username, string password, string login);
    }
    public class DirectoryService : IDirectoryService
    {
        private readonly ILogger<DirectoryService> _logger;

        public DirectoryService(ILogger<DirectoryService> logger)
        {
            _logger = logger;
        }

        public DirectoryUserInfo GetUserInfo(string serviceAccount, string servicePassword, string username, string employeeNumber)
        {
            //to get all :  "*", "+"
            var attrs = new string[] { "attribute_1", "attribute_2", "attribute_3", "attribute_4", "attribute_5", "attribute_6", "attribute_7" };
            try
            {
                using (var connection = new LdapConnection { SecureSocketLayer = true })
                {
                    connection.Connect("YOUR_HOST", 1111);
                    connection.Bind($"uid={serviceAccount},ou=internal,ou=people,dc=entdir,dc=your_org_name,dc=org", servicePassword);
                    if (!connection.Bound)
                        throw new Exception($"Can't login to LDAP lookup service (login: {serviceAccount})");

                    var filter = $"(&(employeeNumber={employeeNumber}))";
                    var attribute = "employeeNumber";
                    var searchByUsername = false;
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        searchByUsername = true;
                        //if username is an email
                        if (username.Contains("@"))
                            attribute = "mail";
                        else
                            attribute = "cn";
                        filter = $"(&({attribute}={username}))";
                    }

                    var result = connection.Search("dc=entdir,dc=your_org_name,dc=org", LdapConnection.ScopeSub, filter, attrs, false).ToList();

                    var item = result.FirstOrDefault(x =>
                    {
                        var value = x.GetAttribute(attribute)?.StringValue;
                        return string.Equals(value, searchByUsername ? username : employeeNumber, StringComparison.InvariantCultureIgnoreCase);
                    });

                    var itemAttrs = item?.GetAttributeSet();
                    return item == null ? null : new DirectoryUserInfo
                    {
                        Username = GetAttribute("cn", itemAttrs),
                        EmployeeNumber = GetAttribute("employeeNumber", itemAttrs),
                        FirstName = GetAttribute("givenName", itemAttrs),
                        LastName = GetAttribute("sn", itemAttrs),
                        Email = GetAttribute("mail", itemAttrs),
                        DepartmentName = GetAttribute("departmentName", itemAttrs),
                        DepartmentNumber = GetAttribute("departmentNumber", itemAttrs)
                    };
                }
            }
            catch (LdapException e)
            {
                _logger.LogError($"Error when try to get information about user({username}) - " + e);
            }
            return null;
        }

        public string GetAttribute(string name, LdapAttributeSet collection)
        {
            return collection.ContainsKey(name) ? collection[name]?.StringValue : string.Empty;
        }

        public bool IsUserLocked(string domain, string username, string password, string login)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, domain, username, password))
                using (var user = UserPrincipal.FindByIdentity(context, login))
                {
                    if (user == null)
                    {
                        _logger.LogWarning($"Cannot validate user <<{login}>>");
                        return false;
                    }
                    return user.IsAccountLockedOut();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return false;
        }

        public bool ValidateUser(string domain, string username, string password)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, domain))
                {
                    return context.ValidateCredentials(username, password);
                }
            }
            catch (PrincipalServerDownException ex)
            {
                _logger.LogError("Login failed" + ex.ToString());
                return false;
            }
        }
    }
}
