using AnyQ.Jobs;
using AnyQ.Queues.Status;
using System;

namespace AnyQDemo {
    class ConsoleStatusProvider : IStatusProvider {
        public void WriteStatus(JobStatus status) {
            Console.WriteLine($"Job {status.JobId} reported a status of {status.Status}");
        }
    }
}
