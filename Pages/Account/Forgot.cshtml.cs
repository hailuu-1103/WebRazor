using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Text;
using WebAppIdentity;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private PRN221DBContext dbContext;

        public ForgotModel(PRN221DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public string To { get; set; } //To address    
        
        private string from = "hoanghai.luu.71@gmail.com"; //From address    
        private MailMessage message;
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var account = dbContext.Accounts.FirstOrDefault(acc => acc.Email.Equals(To));
            if(account != null)
            {
                var password = RandomPassword(10);
                account.Password = password;
                dbContext.Update(account);
                dbContext.SaveChanges();
                SendGridEmailSender sendGrid = new SendGridEmailSender();
                message = new MailMessage(from, To);
                string mailbody = "<!DOCTYPE html>\n"
                    + "<html>\n"
                    + "    <body>\n"
                    + "        <h1>Hello World!</h1>\n"
                    + "        <div>\n"
                    + "            Hello, " + account.Email + "\n"
                    + "            <br>\n"
                    + "            <i>** This is an automated message -- please do not reply as you will not receive a response. **</i>\n"
                    + "            <br>\n"
                    + "            This message is in response to your request to reset your account password. Please click the link below and follow the instructions to change your password.\n"
                    + "            <br>\n"
                    + "            Your password is: " + password + "\n"
                    + "            <br>\n"
                    + "            Thank you.\n"
                    + "            <br>\n"
                    + "\n"
                    + "        </div>\n"
                    + "    </body>\n"
                    + "</html>";
                message.Subject = "Forgot password";
                message.Body = mailbody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential("hoanghai.luu.71@gmail.com", "pxnjybofplllsica");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;
                try
                {
                    client.Send(message);
                }

                catch (Exception ex)
                {
                    throw;
                }
            }
            
            return Redirect("/Account/Login");
        }
        private static string CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string RandomPassword(int size)
        {
            string result = "";
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                char chars = CHARACTERS[random.Next(CHARACTERS.Length)];
                result += chars;
            }
            return result;
        }
    }
}
