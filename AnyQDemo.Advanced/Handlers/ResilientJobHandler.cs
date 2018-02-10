using AnyQ.Jobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQDemo.Handlers {
    /// <summary>
    /// Provides the basic funtionality for retrying a job on another queue
    /// </summary>
    public abstract class ResilientJobHandler : JobHandler {
        private readonly string _nextQueue;

        public ResilientJobHandler(string nextQueue) {
            if (string.IsNullOrWhiteSpace(nextQueue)) {
                throw new ArgumentException(nameof(nextQueue));
            }

            _nextQueue = nextQueue;
        }

        /// <summary>
        /// Returns true if a job can be retried
        /// </summary>
        /// <param name="status">Status of the job</param>
        /// <param name="exception">Exception generated on a failure</param>
        protected abstract bool CanRetry(JobStatus status, Exception exception);

        public override IEnumerable<RedirectStrategy> GetRedirectStrategies() {
            yield return RedirectStrategy.Create(CanRetry, s => s.QueueId + _nextQueue);
        }
    }

    /// <summary>
    /// Provides 
    /// </summary>
    public class RetryResilientJobHandler : ResilientJobHandler {

        private readonly ResilientJobHandler _baseHandler;
        private readonly string _nextQueue;
        private readonly HandlerConfiguration _configuration;

        public RetryResilientJobHandler(ResilientJobHandler baseHandler, IJobRetryTracker reqHistoryJobRetryTracker, string nextQueue = null)
            : base(":retry") {

            _nextQueue = nextQueue;
            _baseHandler = baseHandler;
            Tracker = reqHistoryJobRetryTracker;
            _configuration = new HandlerConfiguration {
                QueueId = $"{baseHandler.Configuration.QueueId}:retry",
                QueueName = $"Retry {baseHandler.Configuration.QueueName}"
            };
        }

        public override HandlerConfiguration Configuration => _configuration;

        protected IJobRetryTracker Tracker { get; private set; }

        public override IEnumerable<RedirectStrategy> GetRedirectStrategies() {
            yield return RedirectStrategy.Create(CanRetry, s => {
                if (Tracker.HasJobFailed(s.JobId, s.JobName)) {
                    if (string.IsNullOrWhiteSpace(_nextQueue)) {
                        return null;
                    }
                    return _baseHandler.Configuration.QueueId + _nextQueue;
                }
                return Configuration.QueueId;
            });
        }

        public override Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken) {
            return _baseHandler.ProcessAsync(request, cancellationToken);
        }

        protected override bool CanRetry(JobStatus s, Exception exception) {
            return s.Status == JobStatus.Failed;
        }

        public override bool CanProcess(ProcessingRequest request) {
            Tracker.AddOrUpdateJob(request.JobId, request.Name);
            return _baseHandler.CanProcess(request);
        }
    }

    public class FailedJobHandler : RetryResilientJobHandler {
        private readonly HandlerConfiguration _configuration;

        public FailedJobHandler(ResilientJobHandler baseHandler, TimeSpan throttleInterval, TimeSpan terminalTimeout)
            : base(baseHandler, new ConditionalJobRetryTracker(r => r.Started + terminalTimeout < DateTime.UtcNow)) {

            _configuration = new HandlerConfiguration {
                QueueId = $"{baseHandler.Configuration.QueueId}:failed",
                QueueName = $"Failed {baseHandler.Configuration.QueueName}",
                ThrottleInterval = throttleInterval
            };
        }

        public override HandlerConfiguration Configuration => _configuration;

        protected override void OnProcessingCompleted(ProcessingRequest request, string result = null) {
            Tracker.RemoveFailedJob(request.JobId, request.Name);
            base.OnProcessingCompleted(request, result);
        }
    }
}
