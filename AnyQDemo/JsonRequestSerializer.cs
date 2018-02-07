using AnyQ.Jobs;
using AnyQ.Queues;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace AnyQDemo {
    internal class JsonRequestSerializer : IRequestSerializer {
        public JobRequest Deserialize(Stream stream) {
            return Deserialize<JobRequest>(stream);
        }

        public Stream Serialize(JobRequest request) {
            return new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)));
        }

        private TResult Deserialize<TResult>(Stream responseStream) {
            using (var sr = new StreamReader(responseStream)) {
                using (var reader = new JsonTextReader(sr)) {
                    return new JsonSerializer().Deserialize<TResult>(reader);
                }
            }
        }
    }
}
