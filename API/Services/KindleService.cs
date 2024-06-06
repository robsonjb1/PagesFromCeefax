// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Mail;
using API.Architecture;
using API.Magazine;
using System.Diagnostics;
using API.PageGenerators;
using Serilog;

namespace API.Services;

public interface IKindleService
{
    public string Publish(string email);
}

public class KindleService : IKindleService
{
    private readonly ISystemConfig _config;

    public KindleService(ISystemConfig config)
    {
        _config = config;
    }

    public string Publish(string email)
    {
        try
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            KindleContent c = new KindleContent();
            KindleSpectator ks = new KindleSpectator(c);
            string filename = ks.Generate();

            // Send e-mail to Kindle?
            if(email == _config.SpecFromAddress)
            {
                SendEMail(filename);
            }
            
            return $"Done in {Math.Round(s.ElapsedMilliseconds / 1000d)} seconds.";
        }
        catch (Exception ex)
        {
            string errorMsg = $"KINDLE BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}";
            Log.Fatal(errorMsg);
            Log.CloseAndFlush();
            return errorMsg;
        }
    }
 
    private void SendEMail(string filename)
    {
        var smtp = new SmtpClient
        {
            Host = _config.SpecHost,
            Port = _config.SpecPort,
            EnableSsl = _config.SpecEnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.SpecFromUsername, _config.SpecFromPassword)
        };

        Attachment attachment = new Attachment(filename);
        using (var message = new MailMessage(
            new MailAddress(_config.SpecFromAddress, _config.SpecName),
            new MailAddress(_config.SpecToAddress, _config.SpecName)
        )
        {
            Subject = "Spectator upload",
            Body = ""
        })
        {
            message.Attachments.Add(attachment);
            smtp.Send(message);
        }
    }   
}