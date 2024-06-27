using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;



namespace WebApplication4.EmailService



{
    public class EmailService
    {public async Task TavsiyeGonder(string email,string baslik,string ozet)
        {
            var smptclient = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true




            };
            var mailmessage = new MailMessage()
            {
                From = new MailAddress("noreply@example.com"),
                Subject = $"Movie Recommendation: {baslik}",
                Body = $"<h1>{baslik}</h1><p>{ozet}</p>",
                IsBodyHtml = true,



            };

            mailmessage.To.Add(email);

            await smptclient.SendMailAsync(mailmessage);

        }
    }
}
