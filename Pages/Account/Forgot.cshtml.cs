using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using WebAppIdentity;
using WebRazor.Models;

namespace WebRazor.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private PRN221DBContext dbContext;
        private readonly IConfiguration _config;
        private readonly ISendGridClient _sendGridClient;
        private readonly HtmlEncoder _htmlEncoder;

        public ForgotModel(PRN221DBContext dbContext, IConfiguration configuration,
        ISendGridClient sendGridClient,
        HtmlEncoder htmlEncoder)
        {
            this.dbContext = dbContext;
            this._config = configuration;
            this._sendGridClient = sendGridClient;
            this._htmlEncoder = htmlEncoder;
        }
        [BindProperty]
        public string To { get; set; } //To address    
        
        private MailMessage message;
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var account = dbContext.Accounts.FirstOrDefault(acc => acc.Email.Equals(To));
            if(account != null)
            {
                var password = RandomPassword(10);
                account.Password = password;
                dbContext.Update(account);
                dbContext.SaveChanges();
                SendGridEmailSender sendGrid = new SendGridEmailSender();
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
                var message = new SendGridMessage
                {
                    From = new EmailAddress(
                    email: _config["SendGridSenderEmail"],
                    name: _config["SendGridSenderName"]
                ),
                    Subject = "Confirm Newsletter Signup",
                    HtmlContent = mailbody
                    };

                message.AddTo(new EmailAddress(To, "HaiLuu"));
                try
                {
                    var response = await _sendGridClient.SendEmailAsync(message);
                    
                }
                catch (Exception ex)
                {
                    ViewData["error"] = ex.Message;
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
