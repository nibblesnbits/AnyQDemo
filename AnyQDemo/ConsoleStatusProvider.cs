using AnyQ.Jobs;
using AnyQ.Queues.Status;
using System;

namespace AnyQDemo {
    class ConsoleStatusProvider : IStatusProvider {
        public void WriteStatus(JobStatus status) {
            if (status.Status.Equals(JobStatus.Failed)) {
                Console.ForegroundColor = ConsoleColor.Red;
                // If an exception is caught by JobQueueListener, the exception details can be found in
                // JobStatus.Details
                Console.WriteLine(
                    $"{status.JobName} ({status.JobId}) reported a status of {status.Status}:{Environment.NewLine}{status.Details}");
                Console.ResetColor();
            }

            Console.WriteLine($"{status.JobName} ({status.JobId}) reported a status of {status.Status}");
            if (status.Status.Equals(JobStatus.Complete)) {
                Console.WriteLine(); // just so the output is a bit nicer.
            }
        }
    }
}
