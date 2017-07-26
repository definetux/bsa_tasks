using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
    class AsyncExamples
    {
        #region Asynchronous Programming
        public string Download(int delay = 5000)
        {
            Thread.Sleep(delay);
            using (var client = new WebClient())
            {
                return client.DownloadString("http://www.google.com");
            }
        }

        public async Task<string> DownloadAsync (int delay = 5000)
        {
            await Task.Delay(delay);
            using (var client = new WebClient())
            {
                //return await client.DownloadStringTaskAsync("https://wwww.google.com");
                return await client.DownloadStringTaskAsync("https://www.google.com");
            }
        }

        public async Task CalculateAsync(int delay = 5000)
        {
            await Task.Delay(delay);
            await Task.Run(() =>
            {
                //Console.Write(Enumerable.Range(1, 100000000).Where(x => x % 2 == 0).Select(x => x * 2).Sum());
                Console.Write(Enumerable.Range(1, 10000).Where(x => x % 2 == 0).Select(x => x * 2).Sum());
            });
        }

        public async Task Method()
        {
            await Task.Delay(500);
            await Task.Run(() => Console.WriteLine("Async"))
                        .ContinueWith((t) => Console.WriteLine("Task finished"));
        }
        #endregion

        #region ContinueWith
        public Task ContinueWithChain()
        {
            var client = new HttpClient();
            return client.GetAsync("https://www.google.com")
                .ContinueWith(t => t.Result.Content.ReadAsStringAsync()
                                                   .ContinueWith(k => k.Result)
                                                   .ContinueWith(r => Console.WriteLine(r.Result)));
        }
        #endregion

        #region TaskCompletionSource
        public Task<double> UseBackgroundWorker()
        {
            var tcs = new TaskCompletionSource<double>();

            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, e) =>
            {
                Thread.Sleep(2000);
                e.Result = 42;
            };
            backgroundWorker.RunWorkerCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                }
                else
                {
                    tcs.SetResult((double)e.Result);
                }
            };

            return tcs.Task;
        }
        #endregion

        #region Cancellation
        public Task WithCancellation(CancellationToken token)
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    Console.WriteLine("Task is working");
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, token);
        }
        #endregion

        #region Synchronization
        private object l = new object();

        public async Task<string> WithLock()
        {
            Task<string> t;
            lock(l)
            {
                using (var client = new HttpClient())
                    t = client.GetStringAsync("https://www.google.com");
            }
            return await t;
        }

        private SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public async Task<string> WithMutex()
        {
            await _mutex.WaitAsync();
            try
            {
                using (var client = new HttpClient())
                    return await client.GetStringAsync("https://www.google.com");
            }
            finally
            {
                _mutex.Release();
            }
        }

        AsyncLock _asyncLock = new AsyncLock();

        public async Task<string> WithAsyncLock()
        {
            using (await _asyncLock.LockAsync())
            using (var client = new HttpClient())
                return await client.GetStringAsync("https://www.google.com");
        }
        #endregion

        #region Exceptions
        public void HandleExceptions()
        {
            try
            {
                var result = string.Empty;
                using (var client = new HttpClient())
                    result = client.GetStringAsync("https://wwww.google.com").Result;
            }
            catch (AggregateException ae)
            {
                ae.Handle((x) =>
                {
                    if (x is HttpRequestException)
                    {
                        Console.WriteLine("HandleException.");
                        return false;
                    }
                    return false;
                });
            }
        }
        #endregion
    }
}
