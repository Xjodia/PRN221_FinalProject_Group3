using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;

namespace PRN221_FinalProject_Group3.Services.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly MailOptions _options;

    public SmtpEmailSender(IOptions<MailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendVerificationCodeAsync(
        string email,
        string code,
        string purpose,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromAddress, _options.FromName, Encoding.UTF8),
            Subject = $"Mã xác thực {purpose} - StoryNest",
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Body = $"""
                <!doctype html>
                <html lang="vi">
                <body style="font-family:Arial,sans-serif;color:#172033;line-height:1.6">
                    <h2 style="color:#6f62e9">StoryNest</h2>
                    <p>Bạn vừa yêu cầu {WebUtility.HtmlEncode(purpose)}.</p>
                    <p>Mã xác thực của bạn là:</p>
                    <div style="font-size:32px;font-weight:700;letter-spacing:8px;color:#6f62e9">
                        {WebUtility.HtmlEncode(code)}
                    </div>
                    <p>Mã chỉ có hiệu lực trong <strong>5 phút</strong>. Không chia sẻ mã này với bất kỳ ai.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, hãy bỏ qua email.</p>
                </body>
                </html>
                """
        };
        message.To.Add(new MailAddress(email));

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_options.Username, _options.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.Username)
            || string.IsNullOrWhiteSpace(_options.Password)
            || string.IsNullOrWhiteSpace(_options.FromAddress))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình MAIL_USERNAME, MAIL_PASSWORD và MAIL_FROM.");
        }
    }
}
