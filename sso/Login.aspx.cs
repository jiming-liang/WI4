using System;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.DirectoryServices;
using Newtonsoft.Json;
using System.Configuration;
public partial class Login : Page {
    public  string site_key = ConfigurationManager.AppSettings["site_key"];
    public string api_key = ConfigurationManager.AppSettings["api_key"];
    public string UserId = ConfigurationManager.AppSettings["userId"];
    public string PWD = ConfigurationManager.AppSettings["pwd"];
    public string AD = ConfigurationManager.AppSettings["AD"];
    // http://desksso.enscoplc.com/login
    //http://desksso-dev.enscoplc.com/sso/test.aspx
    //https://payzonekpiqa.enscoplc.com:81/test.aspx
    //
    static byte[] Encrypt(string json, byte[] Key, byte[] IV) {
        byte[] encrypted;

        using (AesManaged aesAlg = new AesManaged()) {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            // Create a decryptor to perform the stream transform
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream()) {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        swEncrypt.Write(json);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

     byte[] EncryptionKey() {
        byte[] key;
        byte[] salt = Encoding.UTF8.GetBytes(api_key + site_key);

        using (SHA1 sha1 = new SHA1CryptoServiceProvider()) {
            key = sha1.ComputeHash(salt);
            Array.Resize(ref key, 16);
        }

        return key;
    }

     byte[] Signature(string multipass) {
        byte[] signature;

        using (HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(api_key))) {
            using (MemoryStream msHmac = new MemoryStream(Encoding.UTF8.GetBytes(multipass))) {
                signature = hmac.ComputeHash(msHmac);
            }
        }

        return signature;
    }

    /**
     * This is totally experimental and untested code copied from
     * http://www.codeproject.com/Articles/18102/Howto-Almost-Everything-In-Active-Directory-via-C#35
     */
    DirectoryEntry Authenticate(string userName, string password, string domain) {
        DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain, userName, password);

        //  DirectoryEntry entry = new DirectoryEntry(domain,  userName, password);

        object nativeObject = entry.NativeObject;
        return entry;
    }
    bool  Authenticate2(string userName, string password, string domain) {
        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "ensco.ws", this.UserId, this.PWD)) { //, "dc=company,dc=com")) {


            bool b = pc.ValidateCredentials(userName, password);
            //this.Response.Write(b.ToString()); this.Response.End(); return null;
            return b;
            
          //UserPrincipal up = UserPrincipal.FindByIdentity(pc, userName);
          //  return up;
        }
    }
    UserPrincipal Authenticate22(string userName, string password, string domain) {


        var pc = new PrincipalContext (ContextType.Domain,
                                   "hco-ad01",
                                   "DC=ensco,DC=ws",
                                   ContextOptions.SimpleBind,
                                   this.UserId,
                                   this.PWD);



            bool isValid = pc.ValidateCredentials(userName, password);
            if (!isValid)
                return null;
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, userName);
        this.Response.Write(up.Name);
        this.Response.End();
            return up;

    }
    protected void Page_Load(object sender, EventArgs e) {
        this.Password.TextMode = System.Web.UI.WebControls.TextBoxMode.Password;
        // this.Submit();
       // this.test1();

    }
    DirectoryEntry  GetDirectoryEntry(string name ) {
        string[] arr = new string[] { "sAMAccountName", "givenName", "sn", "mail" };
        DirectoryEntry entry = new DirectoryEntry("LDAP://"+this.AD+"/dc=ensco,dc=ws", "sa-ppointnp", "d3^el0pmint");// CN=users,DC=enscoplc,DC=com");
        DirectorySearcher searcher1 = new DirectorySearcher(entry);
       // string name = "011311";
        searcher1.Filter = "(&(objectClass=user)(sAMAccountName=" + name + "))";
        SearchResult r = searcher1.FindOne();
        string s = "<br><table border=1 id=TableSearchAD width=50% cellspacing=0 cellpadding=0 style='border-collapse:collapse;'>";

        DirectoryEntry de = r.GetDirectoryEntry();
        return de;
    }
    protected void Submit() {
        try {
            string userId = this.UserName.Text.Trim();
            string pwd = this.Password.Text.Trim();

            if (! this.Authenticate2(userId, pwd, "")) {
                this.DisplayError();
                return;
            }
            DirectoryEntry de = this.GetDirectoryEntry(userId);
            if (de == null) {
                this.DisplayError();
                return;
            }
            string name = de.Properties["givenName"].Value.ToString() + " " + de.Properties["sn"].Value.ToString();
            var json = JsonConvert.SerializeObject(new Dictionary<string, string>(){
                    {"uid", de.Guid.ToString()},
                    {"expires", DateTime.UtcNow.AddMinutes(10).ToString("o")},
                    {"customer_email", de.Properties["mail"].Value.ToString()},
                    {"customer_name", name }
                });
            var json2 = JsonConvert.SerializeObject(new Dictionary<string, string>(){
                    {"uid", de.Guid.ToString()},
                    {"expires", DateTime.UtcNow.AddMinutes(10).ToString("o")},
                    {"customer_email", "tkumar@enscoplc.com"},
                    {"customer_name", "Emily Zong" }
                });
            this.WriteLog(de);
           this.Desk(json);

        } catch (Exception ex) {
            this.Response.Write(ex.Message + "\n" + ex.StackTrace);
            this.DisplayError();
        }
    }
    void WriteLog(DirectoryEntry de) {
        string s= de.Guid.ToString() +" "+ de.Properties["mail"].Value.ToString()+" "+ de.Properties["givenName"].Value.ToString() + " " + de.Properties["sn"].Value.ToString();
        this.WriteLog(DateTime.Now.ToString());
        this.WriteLog(s);
    }
    void WriteLog(string s) {
        s +=  Environment.NewLine;
        string f = this.Server.MapPath("log.txt");
        System.IO.File.AppendAllText(f, s);
    }
    void Desk(string  json) {
        using (AesManaged myAes = new AesManaged()) {
            byte[] encrypted = Encrypt(json, EncryptionKey(), myAes.IV);

            Debug.WriteLine("    Prepend the IV to the encrypted data");
            byte[] combined = new byte[myAes.IV.Length + encrypted.Length];
            Array.Copy(myAes.IV, 0, combined, 0, myAes.IV.Length);
            Array.Copy(encrypted, 0, combined, myAes.IV.Length, encrypted.Length);

            Debug.WriteLine("    Base64 encode the encrypted data");
            var multipass = Convert.ToBase64String(combined);

            Debug.WriteLine("    Build an HMAC-SHA1 signature using the encoded string and your api key");
            byte[] encrypted_signature = Signature(multipass);
            var signature = Convert.ToBase64String(encrypted_signature);

            Debug.WriteLine("    Finally, URL encode the multipass and signature");
            multipass = Uri.EscapeDataString(multipass);
            signature = Uri.EscapeDataString(signature);

            Debug.WriteLine("== Finished ==");
            string s = string.Format("https://{0}.desk.com/customer/authentication/multipass/callback?multipass={1}&signature={2}", site_key, multipass, signature);
            this.Response.Write(s);
            this.WriteLog(s);
            this.Response.Redirect(s);
        }
    }
    void DisplayError() {
        string s = "Your User ID and/or Password are invalid.";
        this.Msg.Text = s;
    }
    protected void Button1_Click(object sender, EventArgs e) {
        this.Msg.Text = "";
        this.Submit();
    }
}


