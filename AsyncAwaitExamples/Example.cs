using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExamples
{
    public class Example
    {
        private static int count = 0;

        public string ExecuteParallel()
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"{++count} - Começou aplicação {DateTime.Now:mm-ss}");
            var result2 = string.Empty;
            var allResults = new ConcurrentStack<string>();
            Parallel.ForEach(Enumerable.Range(0, 1000), (index) =>
            {
                //comparar cenário que não é CPU-BOUND
                //Thread.Sleep(100);

                //com o cenário abaixo que é CPU-BOUND, neste caso o CPU vai chegar a 100%
                var result = Enumerable.Range(0, 100000).Select(x => Guid.NewGuid()).ToList();
                var result3 = string.Join(", ", result);
                allResults.Push(result3);
            });

            Console.WriteLine($"{++count} - Finalizou aplicação {DateTime.Now:mm-ss}");
            Console.WriteLine($"Tempo total: {stopwatch.Elapsed.TotalSeconds}");

            return string.Join(", ", allResults);
        }

        public async Task ExecuteTaskAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"{++count} - Começou aplicação {DateTime.Now:mm-ss}");

            var consulta1Task = ConsultaBancoAsync(1000, "resultado do 1");
            var consulta2Task = ConsultaBancoAsync(4000, "resultado do 2");
            var consulta3Task = ConsultaBancoAsync(4000, "resultado do 3");

            var continueWithTask = consulta1Task
                        .ContinueWith((previousTask) => 
                            ConsultaBanco(1000, "fake Resultado" + " resultado do 4"));

            Console.WriteLine("Antes do WhenAll");
            await Task.WhenAll(consulta1Task, consulta2Task, consulta3Task);
            Console.WriteLine("Depois do WhenAll");

            var resultDoContinueWith = await continueWithTask;
            Console.WriteLine(resultDoContinueWith);

            //var resultadoConsulta1 = await consulta1Task;

            //await ConsultaBancoAsync(2000, resultadoConsulta1 + " resultado do 4");

            Console.WriteLine($"{++count} - Finalizou aplicação {DateTime.Now:mm-ss}");
            Console.WriteLine($"Tempo total: {stopwatch.Elapsed.TotalSeconds}");
        }

        public async Task<string> ConsultaBancoAsync(int waitTime, string result = "")
        {
            await Task.Run(() =>
            {
                Thread.Sleep(waitTime);
                Console.WriteLine($"{++count} - {result} {DateTime.Now:mm-ss}");
            });
            return result;
        }

        public string ConsultaBanco(int waitTime, string result = "")
        {
            Thread.Sleep(waitTime);
            Console.WriteLine($"{++count} - {result} {DateTime.Now:mm-ss}");
            return result;
        }
    }
}
