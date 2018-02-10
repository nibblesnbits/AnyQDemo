using AnyQ.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQDemo.Handlers {
    public class FailingJobHandler : ResilientJobHandler {
        private readonly HandlerConfiguration _handlerConfiguration;

        public FailingJobHandler(): base(":retry") {
            _handlerConfiguration = new HandlerConfiguration {
                QueueId = @".\private$\test",
                QueueName = "Failing Queue",
            };
        }

        public override HandlerConfiguration Configuration => _handlerConfiguration;

        public override bool CanProcess(ProcessingRequest request) => true;

        public override Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken) {
            // we call OnProcessingFailed() instead of throwing here
            // to make debugging easiser (VS won't break on the exception this way)
            OnProcessingFailed(request, new Exception("Test Exception"));
            return Task.FromResult(false);
        }

        protected override bool CanRetry(JobStatus status, Exception exception) {
            return status.Status == JobStatus.Failed;
        }
    }
}
