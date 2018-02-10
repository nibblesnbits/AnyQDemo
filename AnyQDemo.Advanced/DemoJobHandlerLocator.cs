using AnyQ.Formatters;
using AnyQ.Jobs;
using AnyQDemo.Handlers;
using System;
using System.Collections.Generic;

namespace AnyQDemo {
    class DemoJobHandlerLocator : IJobHandlerLocator {

        private readonly Dictionary<string, JobHandler> _handlers;

        public DemoJobHandlerLocator(IPayloadFormatter formatter) {
            var tracker = new ConditionalJobRetryTracker(r => r.Retries >= 5);

            // this base handler will fail every job
            var mainHandler = new FailingJobHandler();
            // this wrapped handler will retry 5 times
            var retryHandler = new RetryResilientJobHandler(mainHandler, tracker, ":failed");
            // this wrapped handler will retry every 5 seconds, and give up after 30 seconds
            var failedHandler = new FailedJobHandler(mainHandler, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));

            _handlers = new Dictionary<string, JobHandler> {
                { mainHandler.Configuration.QueueId, mainHandler },
                { retryHandler.Configuration.QueueId, retryHandler },
                { failedHandler.Configuration.QueueId, failedHandler },
            };
        }

        public IEnumerable<JobHandler> GetHandlers() {
            return _handlers.Values;
        }

        public bool TryGetHandlerByQueueId(string queueId, out JobHandler handler) {
            return _handlers.TryGetValue(queueId, out handler);
        }
    }
}
