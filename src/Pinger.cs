using System;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pinger
{
   class Pinger
   {
      private readonly string _logFile;
      private CancellationTokenSource _cts = new CancellationTokenSource();
      private Task _pingTask;

      public Pinger()
      {
         _logFile = Path.Combine(AssemblyDirectory, "pinger.log");
      }

      public void Start()
      {
         _pingTask = Task.Factory.StartNew(Run, _cts.Token);
      }

      public void Stop()
      {
         _cts.Cancel();
         _pingTask.Wait();
      }

      public void Run()
      {
         var ping = new Ping();

         Log(_logFile, "Starting");

         while (true)
         {
            try
            {
               var pingReply = ping.Send("www.google.com");

               Log(_logFile, "Ping completed\t" + pingReply.RoundtripTime);
            }
            catch (PingException pingException)
            {
               var inner = pingException.InnerException;

               if (inner != null)
               {
                  Log(_logFile, "Ping failed\t" + inner.Message);
               }
               else
               {
                  Log(_logFile, "Ping failed\t" + pingException.Message);
               }
            }
            catch (Exception e)
            {
               Log(_logFile, "Ping failed\t" + e.Message);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            if (_cts.Token.IsCancellationRequested)
            {
               Log(_logFile, "Stopping");
               return;
            }

         }
      }

      private static void Log(string logFile, string message)
      {
         string completeMessage = String.Format("{0}\t{1}\r\n",
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), message);

         File.AppendAllText(logFile, completeMessage);
      }


      private static string AssemblyDirectory
      {
         get
         {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
         }
      }
   }
}
