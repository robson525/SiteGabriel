using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

namespace Rifa
{
    public class Logger
    {
        #region Singleton

        private static Logger _instance;

        public static Logger Instance => _instance ?? (_instance = new Logger());

        private Logger()
        {
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);

            this.CheckFile();

            _timer.Elapsed += delegate { this.Log(); };
            _timer.Interval = 1000;
            _timer.Start();
        }

        #endregion

        public const string LogFolder = @"..\Log";

        private readonly Timer _timer = new Timer();
        private readonly ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();

        private int _count = 0;
        private string _file;

        private void Log()
        {
            _timer.Stop();

            if (_count++ > 60)
            {
                this.CheckFile();
                _count = 0;
            }

            try
            {
                if (_queue.IsEmpty) return;
                using (var file = new FileStream(_file, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        while (_queue.TryDequeue(out var log))
                            writer.WriteLine(log);
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
            finally
            {
                _timer.Start();
            }
        }

        public void WriteNone(string message, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            TaskEx.Run(() => { this.Write("NONE", message, sourceFilePath, sourceLineNumber); });
        }

        public void WriteInfo(string message, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            TaskEx.Run(() => { this.Write("INFO", message, sourceFilePath, sourceLineNumber); });
        }

        public void WriteWarning(string message, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            TaskEx.Run(() => { this.Write("WARNING", message, sourceFilePath, sourceLineNumber); });
        }

        public void WriteError(string message, Exception ex = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            TaskEx.Run(() =>
            {
                message += GetException(ex);
                this.Write("WARNING", message, sourceFilePath, sourceLineNumber);
            });
        }

        private void Write(string type, string message, string sourceFilePath, int sourceLineNumber)
        {
            string log = type + "\t";
            log += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff");
            log += "\t" + Path.GetFileName(sourceFilePath) + ".";
            log += sourceLineNumber + "\t\t|\t";
            log += message;

            _queue.Enqueue(log);
        }

        private static string GetException(Exception ex)
        {
            if (ex == null)
                return "";

            string ret = Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
            ex = ex.InnerException;

            while (ex != null)
            {
                ret += Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                ex = ex.InnerException;
            }

            return ret;
        }

        private void CheckFile()
        {
            string file = Path.Combine(LogFolder, $"{DateTime.Now:yyyyMMdd_HHmm}00.log");

            if (_file == null || new FileInfo(_file).Length > 5 * 1024 * 1024)
            {
                _file = file;
                if (!File.Exists(_file))
                    File.Create(_file).Dispose();
            }
        }
    }
}