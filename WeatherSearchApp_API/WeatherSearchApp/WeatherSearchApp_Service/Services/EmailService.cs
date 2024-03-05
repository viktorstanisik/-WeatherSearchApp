using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using WeatherSearchApp_Domain.DTOModels;
using WeatherSearchApp_Domain.EntityModels;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;
using WeatherSearchApp_Shared.AppConstants;
using WeatherSearchApp_Shared.HelperMethods;

namespace WeatherSearchApp_Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;

        public EmailService(IConfiguration configuration, IDistributedCache distributedCache)
        {
            _configuration = configuration;
            _distributedCache = distributedCache;
        }

        public async Task RegisterAccountConfirmationLink(User user)
        {
            var urlLink = _configuration["Authorization:LinkUrlConfirmRegisteredAccount"];
            Uri uri = new Uri(urlLink);

            UserEmailOptions userEmailOptions = new();

            userEmailOptions.ToEmail = user.Email;
            userEmailOptions.Subject = "Confirm Your Account";


            string hashedToken = HashUserUrl(user);

            string linkUrl = $"{uri}racl={hashedToken}";

            userEmailOptions.Body = $"<p> Click on this link : {linkUrl} to confirm your account </p>";

            //we are setting hashedToken to be as Id to find the user by that hashed token
            await _distributedCache.SetStringAsync(hashedToken, user.Email, CacheExpiration());

            await SendEmail(userEmailOptions);
        }

        public async Task<ServiceResponse<bool>> GenerateAndSendSecurityCodeForLogin(string email)
        {
            try
            {
                var response = new ServiceResponse<bool>() { Success = false };

                var hasError = await ValidateEmail(email);

                if(!hasError.Success)
                {
                    response.ErrorMessage = hasError.ErrorMessage;
                    response.Success = hasError.Success;
                    return response;
                }


                UserEmailOptions userEmailOptions = new();

                int codeLength = Convert.ToInt32(_configuration["Authorization:SecurityCodeLength"]);

                var generatedCode = Methods.GenerateSecurityCode(codeLength);

                userEmailOptions.ToEmail = email;

                userEmailOptions.Subject = "Confirm Your Security Code";

                var securityCodeExpiration = _configuration["Caching:SecurityCodeTime"];

                userEmailOptions.Body = $"<p> Your security code is: <strong>{generatedCode}</strong> Your code expires in {securityCodeExpiration} minutes. Please don't reply.</p>";

                //we are setting hashedToken to be as Id to find the user by that hashed token
                await _distributedCache.SetStringAsync(generatedCode, email, CacheExpiration(true));

                await SendEmail(userEmailOptions);

                response.Data = true;
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage();
            mail.Subject = userEmailOptions.Subject;
            mail.Body = userEmailOptions.Body;
            mail.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);
            mail.From = new MailAddress(_configuration["SMTPConfig:SenderAddress"], _configuration["SMTPConfig:SenderDisplayName"]);
            mail.To.Add(userEmailOptions.ToEmail);

            NetworkCredential networkCredential = new NetworkCredential(_configuration["SMTPConfig:UserName"], _configuration["SMTPConfig:Password"]);

            SmtpClient smtpClient = new SmtpClient()
            {
                UseDefaultCredentials = Convert.ToBoolean(_configuration["SMTPConfig:UseDefaultCredentials"]),
                Credentials = networkCredential,
                EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]),
                Host = _configuration["SMTPConfig:Host"],
                Port = Convert.ToInt32(_configuration["SMTPConfig:Port"])
            };

            mail.BodyEncoding = Encoding.Default;
            try
            {
                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public string HashUserUrl(User user)
        {
            string secretKey = _configuration["Authorization:SecretKey"];
            string urlToHash = user.Email + user.Id + secretKey;

            return Methods.GenerateSha512Hash(urlToHash);
        }

        private DistributedCacheEntryOptions CacheExpiration(bool isForSecurityCode = false)
        {

            string cacheKey = isForSecurityCode ? "Caching:SecurityCodeTime" : "Caching:LinkExpirationTime";

            double cacheTimeExpiration = Convert.ToDouble(_configuration[cacheKey] ?? (isForSecurityCode ? "5" : "12"));

            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(cacheTimeExpiration)
            };

            return options;
        }

        private async Task<ServiceResponse<bool>> ValidateEmail(string email)
        {
            var response = new ServiceResponse<bool>() { Success = false };

            email = email.Trim();

            Regex emailRegex = new(_configuration["RegexValidation:EmailRegex"]);

            if (!emailRegex.IsMatch(email))
            {
                response.ErrorMessage = ErrorMessages.InvalidUser;
                return response;
            }

            response.Success = true;
            return await Task.FromResult(response);
        }


    }
}
