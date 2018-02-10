using AnyQ.Jobs;
using AnyQ.Queues.Status;
using System;

namespace AnyQDemo {
    class ConsoleStatusProvider : IStatusProvider {
        public void WriteStatus(JobStatus status) {
            if (status.Status.Equals(JobStatus.Failed)) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                // If an exception is caught by JobQueueListener, the exception details can be found in
                // JobStatus.Details
                Console.WriteLine(
                    $"{status.JobName} ({status.JobId}) reported a status of {status.Status}:{Environment.NewLine}{status.Details}");
                Console.ResetColor();
                return;
            }

            Console.WriteLine(
                $"{status.JobName} ({status.JobId}) reported a status of {status.Status}" +
                // if a job is redirected, the Details contains information about the new queue
                (!string.IsNullOrWhiteSpace(status.Details) ? $" --- Details: {status.Details}" : string.Empty));
        }
    }
}
