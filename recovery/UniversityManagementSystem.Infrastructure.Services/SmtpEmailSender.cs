using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Infrastructure.Services;

public class SmtpEmailSender : IEmailSenderService
{
	private readonly IConfiguration _configuration;

	private readonly ILogger<SmtpEmailSender> _logger;

	public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody)
	{
		string host = _configuration["Smtp:Host"];
		if (string.IsNullOrWhiteSpace(host))
		{
			return true;
		}
		try
		{
			int port = (int.TryParse(_configuration["Smtp:Port"], out var p) ? p : 587);
			bool enableSsl = !string.Equals(_configuration["Smtp:EnableSsl"], "false", StringComparison.OrdinalIgnoreCase);
			string username = _configuration["Smtp:Username"];
			string password = _configuration["Smtp:Password"];
			string fromEmail = _configuration["Smtp:FromEmail"] ?? username ?? string.Empty;
			string fromName = _configuration["Smtp:FromName"] ?? "University Management System";
			using SmtpClient client = new SmtpClient(host, port)
			{
				EnableSsl = enableSsl,
				Credentials = (string.IsNullOrEmpty(username) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(username, password))
			};
			MailAddress from = new MailAddress(fromEmail, fromName);
			MailAddress to = new MailAddress(toEmail, string.IsNullOrWhiteSpace(toName) ? toEmail : toName);
			using MailMessage message = new MailMessage(from, to)
			{
				Subject = subject,
				Body = htmlBody,
				IsBodyHtml = true
			};
			await client.SendMailAsync(message);
			return true;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "Failed to send email to {To} (subject: {Subject})", toEmail, subject);
			return false;
		}
	}
}
