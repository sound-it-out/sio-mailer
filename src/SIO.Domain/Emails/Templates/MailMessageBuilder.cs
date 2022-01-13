using System.Net.Mail;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Templates
{
    internal abstract class MailMessageBuilder<TEvent> : IMailMessageBuilder<TEvent>
        where TEvent : IEvent
    {
        private readonly ILogger<MailMessageBuilder<TEvent>> _logger;
        protected readonly IRazorViewBuilder _razorViewBuilder;
        private readonly SmtpOptions _smtpOptions;

        public MailMessageBuilder(ILogger<MailMessageBuilder<TEvent>> logger,
            IRazorViewBuilder razorViewBuilder,
            IOptionsSnapshot<SmtpOptions> optionsSnapshot)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (razorViewBuilder == null)
                throw new ArgumentNullException(nameof(razorViewBuilder));
            if (optionsSnapshot == null)
                throw new ArgumentNullException(nameof(optionsSnapshot));

            _logger = logger;
            _razorViewBuilder = razorViewBuilder;
            _smtpOptions = optionsSnapshot.Value;
        }

        protected abstract Task<string[]> GenerateRecipentsAsync(TEvent @event);
        protected abstract Task<string> GenerateSubjectAsync(TEvent @event);
        protected virtual Task<string> GenerateBodyAsync(TEvent @event) => _razorViewBuilder.BuildAsync(typeof(TEvent).Name, @event);
        protected virtual Task InitializeAsync(TEvent @event) => Task.CompletedTask;

        public async Task<MailMessage> BuildAsync(TEvent @event)
        {
            await InitializeAsync(@event);

            var message = new MailMessage
            {
                From = new MailAddress(_smtpOptions.From),
                Subject = await GenerateSubjectAsync(@event),
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Body = await GenerateBodyAsync(@event)
            };

            foreach (var recipient in await GenerateRecipentsAsync(@event))
                message.To.Add(new MailAddress(recipient));

            return message;
        }
    }

    internal sealed class MailMessageBuilder : IMailMessageBuilder
    {
        private static Lazy<MethodInfo> _buildAsyncMethod = new Lazy<MethodInfo>(() => typeof(MailMessageBuilder)
            .GetMethod(nameof(InternalBuildAsync), BindingFlags.NonPublic | BindingFlags.Instance));

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MailMessageBuilder(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<MailMessage> BuildAsync(IEvent @event)
            => (Task<MailMessage>)_buildAsyncMethod.Value
                .MakeGenericMethod(@event.GetType())
                .Invoke(this, new object[] { @event });         
        

        private async Task<MailMessage> InternalBuildAsync<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var builder = scope.ServiceProvider.GetService<IMailMessageBuilder<TEvent>>();

                if (builder == null)
                    throw new Exception();

                return await builder.BuildAsync(@event);
            }
        }
    }
}
