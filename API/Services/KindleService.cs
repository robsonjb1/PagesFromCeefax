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
    public string PublishKindle(string email);
}

public class KindleService : IKindleService
{
    private readonly ISystemConfig _config;
    private readonly Object l = new();

    public KindleService(ISystemConfig config)
    {
        _config = config;
    }

    public string PublishKindle(string email)
    {
        try
        {
            lock (l)
            {
                Stopwatch s = new Stopwatch();
                s.Start();

                // Clear temporary working directory
                if (Directory.Exists("KindleTemp")) { Directory.Delete("KindleTemp", true); }
                Directory.CreateDirectory("KindleTemp"); ;

                PublishSpectator(email);
                PublishThurrott(email);

                return $"Done in {Math.Round(s.ElapsedMilliseconds / 1000d)} seconds.";
            }
        }    
        catch (Exception ex)
        {
            string errorMsg = $"KINDLE BUILD ERROR {ex.Message} {ex.InnerException} {ex.Source} {ex.StackTrace}";
            Log.Fatal(errorMsg);
            Log.CloseAndFlush();
            return errorMsg;
        }
    }


    public void PublishSpectator(string email)
    {
        SpectatorContent c = new SpectatorContent(_config);
        SpectatorNews sn = new SpectatorNews(c);
        string filename = sn.Generate();

        // Send e-mail to Kindle?
        if(email == _config.KindleFromAddress)
        {
            SendEMail(filename);
        }
    }

    public void PublishThurrott(string email)
    {
        ThurrottContent c = new ThurrottContent(_config);
        ThurrottNews tn = new ThurrottNews(c);
        string filename = tn.Generate();

        // Send e-mail to Kindle?
        if(email == _config.KindleFromAddress)
        {
            SendEMail(filename);
        }            
    }
 
    private void SendEMail(string filename)
    {
        var smtp = new SmtpClient
        {
            Host = _config.KindleHost,
            Port = _config.KindlePort,
            EnableSsl = _config.KindleEnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.KindleFromUsername, _config.KindleFromPassword)
        };

        Attachment attachment = new Attachment(filename);
        using (var message = new MailMessage(
            new MailAddress(_config.KindleFromAddress, _config.KindleName),
            new MailAddress(_config.KindleToAddress, _config.KindleName)
        )
        {
            Subject = "Kindle upload",
            Body = ""
        })
        {
            message.Attachments.Add(attachment);
            smtp.Send(message);
        }
    }   
}