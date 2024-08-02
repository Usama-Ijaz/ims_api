using System.Net.Mail;
using System.Net;
using IMS.Repositories.Email;

namespace IMS.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IConfiguration _config;
        string htmlBody = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <title>OTP Verification</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f7f7f7;\r\n        }\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            background-color: #ffffff;\r\n            border-radius: 8px;\r\n            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n        }\r\n        .header {\r\n            text-align: center;\r\n            padding: 10px;\r\n            background-color: #4CAF50;\r\n            color: white;\r\n            border-radius: 8px 8px 0 0;\r\n        }\r\n        .content {\r\n            padding: 20px;\r\n            text-align: center;\r\n        }\r\n        .otp {\r\n            font-size: 24px;\r\n            font-weight: bold;\r\n            color: #333333;\r\n            margin: 20px 0;\r\n        }\r\n        .footer {\r\n            text-align: center;\r\n            font-size: 12px;\r\n            color: #888888;\r\n            margin-top: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            <h1>OTP Verification</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n            <p>Dear User,</p>\r\n            <p>To complete your verification, please use the following One-Time Password (OTP):</p>\r\n            <div class=\"otp\">123456</div> <!-- Replace with the actual OTP -->\r\n            <p>This OTP is valid for 10 minutes. Please do not share it with anyone.</p>\r\n            <p>Thank you for using our service!</p>\r\n        </div>\r\n        <div class=\"footer\">\r\n            &copy; 2024 Your Company Name. All rights reserved.\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>\r\n";
        public EmailService(IEmailRepository emailRepository, IConfiguration config)
        {
            _emailRepository = emailRepository;
            _config = config;

        }
        public async Task<int> SendEmail(string email)
        {
            bool success = true;
            int otpId = 0;
            Random random = new();
            string otp = random.Next(0, 1000000).ToString("D6");
            var smtpClient = new SmtpClient("smtp-relay.brevo.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true,
            };
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress("ijazusama@gmail.com"),
                Subject = "Verify OTP for Registration",
                Body = htmlBody.Replace("123456", otp),
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                success = false;
            }
            if (success)
            {
                otpId = await _emailRepository.InsertOtp(otp);
            }
            return otpId;
        }
    }
}
