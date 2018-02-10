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

            // now we add a JobHandlerLocator that manages the 3 queues we need
            listener.AddHandlerLocator(new DemoJobHandlerLocator(payloadFormatter));

            // clear the queues so we have a clean starting point.
            listener.PurgeQueue(queueId);
            listener.PurgeQueue(queueId + ":retry");
            listener.PurgeQueue(queueId + ":failed" );

            // then we send a job to the queue
            listener.SendJob(queueId, "test", null, "Test Job");

            // finally, we tell the JobQueueListener to listen for jobs
            listener.Listen();
            
            using (listener) { // don't forget to dispose of any JobQueueListeners when you're done with them
                Console.ReadLine();
            }
        }

        private static bool IsMsmqEnabled() {
            return ServiceController.GetServices()
                .FirstOrDefault(o => o.ServiceName == "MSMQ")
                ?.Status == ServiceControllerStatus.Running;
        }
    }
}