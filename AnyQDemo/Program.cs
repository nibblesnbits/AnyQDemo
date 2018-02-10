using AnyQ;
using AnyQ.Formatters;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace AnyQDemo {
    class Program {
        static void Main(string[] args) {

            if (!IsMsmqEnabled()) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error: MSMQ is not running.");
                Console.ReadLine();
            }

            // First, we need an IPayloadFormatter.
            // This tells AnyQ how to format the data when inserting it into the queue
            var payloadFormatter = new JsonPayloadFormatter();
            // Second, we need an IRequestSerializer.
            // This tells AnyQ how to deserialize the data coming back from the queue
            var requestSerializer = new JsonRequestSerializer();
            // Next, we need an IJobQueueFactory.
            // AnyQ.Queues.Msmq provides one that interacts with MSMQ for us.
            var jobQueueFactory = new AnyQ.Queues.Msmq.MsmqJobQueueFactory(payloadFormatter, requestSerializer);

            // Then we instaniate a new JobQueueListener with that IJobQueueFactory
            var listener = new JobQueueListener(jobQueueFactory);
            // Optionally, we can add an IStatusProvider to log the status of each
            // job at each stage in the processing
            listener.AddStatusProvider(new ConsoleStatusProvider());

            // This is our test queue on our local machine.
            // If it does not exist, AnyQ will make it for you.
            const string queueId = @".\private$\test";

            // now we add a JobHandler that will handle jobs sent to the queue
            listener.AddHandler(new TestJobHandler(payloadFormatter, new AnyQ.Jobs.HandlerConfiguration {
                QueueId = queueId,
                QueueName = "Test Queue"
            }));

            // before we send a job,clear the queue if
            // the /clear option is passed
            if (args.Length > 0 && args[0] == "/clear") {
                Console.WriteLine("Purging queue...");
                listener.PurgeQueue(queueId);
            }

            // then we send 2 jobs to the queue
            listener.SendJob(queueId, "test", new Payload {
                Message = "Hello, AnyQ!"
            }, "test message");

            // finally, we tell the JobQueueListener to listen for jobs
            listener.Listen();

            // keep a count of the rest of the messages
            var count = 2;
            using (listener) { // don't forget to dispose of any JobQueueListeners when you're done with them
                while (true) {
                    Thread.Sleep(3000);

                    Console.WriteLine($"Sending job {count}...");
                    listener.SendJob(queueId, "test", new Payload {
                        Message = $"Job {count++}"
                    }, "test message");
                }
            }
        }

        private static bool IsMsmqEnabled() {
            return ServiceController.GetServices()
                .FirstOrDefault(o => o.ServiceName == "MSMQ")
                ?.Status == ServiceControllerStatus.Running;
        }
    }
}