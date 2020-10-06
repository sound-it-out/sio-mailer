using System;
using System.Threading.Tasks;
using Xunit;

namespace SIO.Testing.Abstractions
{
    public abstract class MailerApplicationSpecification<TStartup> : IClassFixture<MailerWebApplicationFactory<TStartup>>
        where TStartup: BaseStartup
    {
        protected readonly MailerWebApplicationFactory<TStartup> _mailerWebApplicationFactory;
        protected abstract Task Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }

        public MailerApplicationSpecification(MailerWebApplicationFactory<TStartup> mailerWebApplicationFactory)
        {
            if (mailerWebApplicationFactory == null)
                throw new ArgumentNullException(nameof(mailerWebApplicationFactory));

            _mailerWebApplicationFactory = mailerWebApplicationFactory;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                await Given();
            }
            catch (Exception e)
            {
                if (ExceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }

    public abstract class MailerApplicationSpecification<TStartup, TResult> : IClassFixture<MailerWebApplicationFactory<TStartup>>
        where TStartup : BaseStartup
    {
        protected readonly MailerWebApplicationFactory<TStartup> _mailerWebApplicationFactory;
        protected abstract Task<TResult> Given();
        protected abstract Task When();
        protected Exception Exception { get; private set; }
        protected ExceptionMode ExceptionMode { get; set; }
        protected TResult Result { get; private set; }

        public MailerApplicationSpecification(MailerWebApplicationFactory<TStartup> mailerWebApplicationFactory)
        {
            if (mailerWebApplicationFactory == null)
                throw new ArgumentNullException(nameof(mailerWebApplicationFactory));

            _mailerWebApplicationFactory = mailerWebApplicationFactory;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                Result = await Given();
            }
            catch (Exception e)
            {
                if (ExceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }
}
