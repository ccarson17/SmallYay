using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace SmallYay.Services
{
    public class ErrorLog
    {
        public int maxErrors = 10; // setting for max amount of errors to save
        public List<ErrorLogEntry> LogEntries = new List<ErrorLogEntry>();

        public ErrorLog(bool getEntries)
        {
            if(getEntries)
                LogEntries = GetErrorLogs();
        }

        public List<ErrorLogEntry> GetErrorLogs()
        {
            try
            {
                var error_log_task = SecureStorage.GetAsync("error_log");
                var error_log = error_log_task.Result;
                return JsonConvert.DeserializeObject<List<ErrorLogEntry>>(error_log);
            }
            catch (Exception ex)
            {
                return new List<ErrorLogEntry>();
            }
        }

        public void LogError(Exception ex)
        {
            ErrorLogEntry error = new ErrorLogEntry()
            {
                EntryDateTime = DateTime.Now,
                ExceptionMessage = ex.Message,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException.ToString()
            };
            var errorlog = GetErrorLogs();
            if(errorlog.Count < maxErrors)
            {
                errorlog.Add(error);
            }
            else
            {
                var ordered_error_log = errorlog.OrderBy(x => x.EntryDateTime).ToList();
                ordered_error_log[0] = error;
                errorlog = ordered_error_log;
            }
            SecureStorage.SetAsync("error_log", JsonConvert.SerializeObject(errorlog));
        }
    }

    public class ErrorLogEntry
    {
        public DateTime EntryDateTime { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionSource { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
    }
}
