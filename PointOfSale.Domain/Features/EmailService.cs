using FluentEmail.Core;
using PointOfSale.Interfaces;
using System.Threading.Tasks;

namespace PointOfSale.Domain.Features
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<bool> SendAsync(string toEmail, string subject, string body)
        {
            var response = await _fluentEmail
            .To(toEmail)
            .Subject(subject)
            .Body(body, isHtml: true) //  This tells FluentEmail it's HTML
            .SendAsync();

            return response.Successful;
        }
    }
}
