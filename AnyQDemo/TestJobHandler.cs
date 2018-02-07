using AnyQ.Formatters;
using AnyQ.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQDemo {
    public class TestJobHandler : JobHandler {
        private readonly IPayloadFormatter _payloadFormatter;
        private readonly HandlerConfiguration _handlerConfiguration;

        public TestJobHandler(IPayloadFormatter payloadFormatter, HandlerConfiguration handlerConfiguration) {
            _payloadFormatter = payloadFormatter;
            _handlerConfiguration = handlerConfiguration;
        }

        public override HandlerConfiguration Configuration => _handlerConfiguration;

        public override bool CanProcess(ProcessingRequest request) {
            return true;
        }

        public override Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken) {
            var payload = _payloadFormatter.Read<Payload>(request.JobRequest.Payload);
            Console.WriteLine(payload.Message);
            OnProcessingCompleted(request);
            return Task.FromResult(false);
        }
    }
}
