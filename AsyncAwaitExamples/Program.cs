using System.Threading.Tasks;

namespace AsyncAwaitExamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var example = new Example();

            //await example.ExecuteTaskAsync();
            example.ExecuteParallel();
        }
    }
}
