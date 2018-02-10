using System;
using System.Collections.Concurrent;

namespace AnyQDemo.Handlers {
    public interface IJobRetryTracker {
        void AddOrUpdateJob(string jobId, string jobName);
        bool HasJobFailed(string jobId, string jobName);
        void RemoveFailedJob(string jobId, string jobName);
    }

    /// <summary>
    /// A basic in-memory tracker for failed jobs
    /// </summary>
    public class ConditionalJobRetryTracker : IJobRetryTracker {
        private readonly ConcurrentDictionary<string, RetryRecord> _retries = new ConcurrentDictionary<string, RetryRecord>();
        private readonly Func<RetryRecord, bool> _condition;

        public ConditionalJobRetryTracker(Func<RetryRecord, bool> condition) {
            _condition = condition;
        }

        public void AddOrUpdateJob(string jobId, string jobName) {
            var record = _retries.GetOrAdd(jobName, _ => new RetryRecord(DateTime.UtcNow));
            record.Retries += 1;
        }

        public bool HasJobFailed(string jobId, string jobName) {
            if (_retries.TryGetValue(jobName, out var record)) {
                if (_condition(record)) {
                    if (_retries.TryRemove(jobName, out var removedRecord)) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Terminal failure for job '{jobName}' ({jobId}). Failed attempts: {removedRecord.Retries}.");
                        Console.ResetColor();
                    }
                    return true;
                }
            }
            return false;
        }

        public void RemoveFailedJob(string jobId, string jobName) {
            if (_retries.TryRemove(jobName, out var removedRecord)) {
                Console.WriteLine($"Failed job '{jobName}'. Succeeded after {removedRecord.Retries} attempts.");
            }
        }
    }

    public class RetryRecord {
        private readonly DateTime _startDate;

        public RetryRecord(DateTime startDate) {
            _startDate = startDate;
        }
        public int Retries { get; set; }
        public DateTime Started => _startDate;
    }
}
