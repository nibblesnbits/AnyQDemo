using AnyQ;
using AnyQ.Formatters;
using System;
using System.Threading;

namespace AnyQDemo {
    class Program {
        static void Main(string[] args) {

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

            // then we send 2 jobs to the queue
            listener.SendJob(queueId, "test", new Payload {
                Message = "Hello, AnyQ!"
            }, "test message");
            listener.SendJob(queueId, "test", new Payload {
                Message = "Hello, again."
            }, "test message");

            // finally, we tell the JobQueueListener to listen for jobs
            listener.Listen();

            using (listener) { // don't forget to dispose of any JobQueueListeners when you're done with them
                while (true) {
                    Thread.Sleep(3000);
                    Console.WriteLine("Sending another job...");
                    listener.SendJob(queueId, "test", new Payload {
                        Message = "Hello, again."
                    }, "test message");
                }
            }
        }
    }
}