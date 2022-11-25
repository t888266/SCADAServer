namespace SCADAServer
{
    using MailSender;
    using System.Threading.Tasks.Dataflow;
    public interface IBGMailSender
    {
        Task SendMail(MailContent content);
    }
    public class MailSenderBackgroundService : BackgroundService,IBGMailSender
    {
        IMailSender mailSender;
        BufferBlock<MailContent> toMail;
        public MailSenderBackgroundService(IMailSender mailSender)
        {
            this.mailSender = mailSender;
            toMail = new BufferBlock<MailContent>();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var mail = await toMail.ReceiveAsync();
                    await mailSender.SendMailAsync(mail);
                }
                catch
                {

                }
            }
        }
        public async Task SendMail(MailContent content)
        {
            await toMail.SendAsync(content);
        }
    }
}
