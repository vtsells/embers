using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Security;
using System.DirectoryServices.Protocols;

namespace ITMS.ADAPI
{
    public class ADIntegrator
    {
        private PrincipalContext AD { get; }
        private string DomainName { get; }
        public ADIntegrator(string domainName)
        {
            DomainName = domainName;
            AD = new PrincipalContext(ContextType.Domain,DomainName, 
                                     "DC="+DomainName.Split('.')[0]+",DC="+ DomainName.Split('.')[1],
                                     ContextOptions.Negotiate);
        }
        public static bool ValidateCredentials(string username, SecureString password, string domainName)
        {
            bool valid = false;
            var creds = new NetworkCredential(username, password, domainName);
            var identifier = new LdapDirectoryIdentifier(domainName);
            LdapConnection connection = null;
            try
            {
                connection = new LdapConnection(identifier, creds, AuthType.Kerberos);
                connection.Bind();
                valid = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if(connection != null) connection.Dispose();
            return valid;
        }
        public UserPrincipal GetOneUser(string samName)
        {
            UserPrincipal user = new UserPrincipal(AD)
            {
                SamAccountName = samName
            };
            var searcher = new PrincipalSearcher(user);
            return (UserPrincipal)searcher.FindOne();
        }
        public UserPrincipal GetOneUser(string firstName, string lastName)
        {
            UserPrincipal user = new UserPrincipal(AD)
            {
                GivenName = firstName,
                Surname = lastName
            };
            var searcher = new PrincipalSearcher(user);
            return (UserPrincipal)searcher.FindOne();
        }
        public PrincipalSearchResult<Principal> GetAllUsers()
        {
            UserPrincipal users = new UserPrincipal(AD)
            {
                SamAccountName = "*"
            };
            return new PrincipalSearcher(users).FindAll();
        }
        public enum UserProperties
        {
            manager, title, telephoneNumber, facsimileTelephoneNumber, mobile, physicalDeliveryOfficeName, postOfficeBox, streetAddress, l, st, postalCode, co
        }
        public string GetProperty(UserProperties property, DirectoryEntry entry)
        {
            string prop = Enum.GetName(typeof(UserProperties), property);
            if (entry.Properties[prop].Value == null) return null;
            return entry.Properties[prop].Value.ToString();
        }
        public void EditUserProperty(UserProperties property, string value, DirectoryEntry entry)
        {
            string prop = Enum.GetName(typeof(UserProperties), property);
            entry.Properties[prop].Value = value;
            entry.CommitChanges();

        }
    }
}
