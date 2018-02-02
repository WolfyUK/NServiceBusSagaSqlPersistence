using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Saga
{
    public class FailSaga : SqlSaga<SagaData>, IAmStartedByMessages<Command>
    {
        protected override string CorrelationPropertyName => nameof(SagaData.CorrelationId);

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<Command>(c => c.CorrelationId);
        }

        public Task Handle(Command message, IMessageHandlerContext context)
        {
            Data.Value = DateTime.UtcNow.ToLongDateString();
            return Task.CompletedTask;
        }
    }

    public class Command : ICommand
    {
        public Guid CorrelationId { get; set; }
    }

    public class SagaData : ContainSagaData
    {
        public Guid CorrelationId { get; set; }
        public string Value { get; set; }
    }
}
