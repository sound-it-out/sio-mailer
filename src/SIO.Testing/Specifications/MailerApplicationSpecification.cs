using System;
using System.Threading.Tasks;
using SIO.Testing.Abstractions;
using Xunit;

namespace SIO.Testing.Specifications
{
    public abstract class MailerApplicationSpecification<TStartup> : IAsyncLifetime, IClassFixture<MailerWebApplicationFactory<TStartup>>
        where TStartup: class
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

    public abstract class MailerApplicationSpecification<TStartup, TResult> : IAsyncLifetime, IClassFixture<MailerWebApplicationFactory<TStartup>>
        where TStartup : class
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
