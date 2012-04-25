// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingService.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
//   Logging service - provides a mechanism to diagnostically trace statement from WP7 application, this service
//   can be enabled or disabled as and when required. The service persists to isolated storage in the directory defined
//   by the applicationName passed in on the constructor. The service also provides the ability to clear down the file
//   as and when required.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Microsoft.Phone.Info;
using Microsoft.Phone.Reactive;

namespace Phone.Common.Diagnostics.Logging
{
    /// <summary>
    /// Logging service - provides a mechanism to diagnostically trace statement from WP7 application, this service 
    /// can be enabled or disabled as and when required. The service persists to isolated storage in the directory defined
    /// by the applicationName passed in on the constructor. The service also provides the ability to clear down the file
    /// as and when required.
    /// IMPORTANT - In debug mode the WriteDiagnostics method will query for user and device properties using the methods,
    /// DeviceExtendedProperties and UserExtendedProperties, these methods affect the status of the application in the
    /// application store.
    /// http://msdn.microsoft.com/en-us/library/microsoft.phone.info.deviceextendedproperties.getvalue(v=VS.92).aspx
    /// These method are compiled out for a release build using ifdef DEBUG statements.
    /// </summary>
    public sealed class LoggingService : ILogManager, IDisposable
    {
        /// <summary>
        /// The failed to write.
        /// </summary>
        private const string FailedToWrite = "Failed to write to log.";

        /// <summary>
        /// The failed to write diagnostics.
        /// </summary>
        private const string FailedToWriteDiagnostics = "Failed to write diagnostics to log.";

        /// <summary>
        /// The failed to enable.
        /// </summary>
        private const string FailedToEnable = "Failed to enable logging.";

        /// <summary>
        /// The failed to disable.
        /// </summary>
        private const string FailedToDisable = "Failed to disable logging.";

        /// <summary>
        /// The failed to read messages.
        /// </summary>
        private const string FailedToReadMessages = "Failed to read messages from file.";

        /// <summary>
        /// The failed to clear.
        /// </summary>
        private const string FailedToClear = "Failed to clear log file.";

        /// <summary>
        /// Event fired when log file has been modified.
        /// </summary>
        public event EventHandler LogModified = delegate { };

        /// <summary>
        /// The application name.
        /// </summary>
        private readonly string applicationName;

        /// <summary>
        /// The log path including the log file name;
        /// </summary>
        private readonly string logPath;

        /// <summary>
        /// The write frequency of writing to file.
        /// </summary>
        private const int WriteFrequency = 500;

        /// <summary>
        /// The isloated storage log filename.
        /// </summary>
        private const string LogFilename = "log.dat";

        /// <summary>
        /// The message format, date and time is pre appened to the message.
        /// </summary>
        private const string MESSAGE_FORMAT = "{0:yyyy-MM-dd HH:mm:ss.ffff} - {1}";

        /// <summary>
        /// The number format info.
        /// </summary>
        private readonly NumberFormatInfo numberFormatInfo = new NumberFormatInfo { NumberGroupSizes = new[] { 3 }, NumberGroupSeparator = "," };

        /// <summary>
        /// Pending message queue - messages are written to the in memory queue and then written to file asynchronously.
        /// </summary>
        private readonly Queue<string> pendingMessages = new Queue<string>();

        /// <summary>
        /// The sync object to make accessing the log file thread safe.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// The isolated storage file.
        /// </summary>
        private IsolatedStorageFile isolatedStorage;

        /// <summary>
        /// Flag indicating persisting to file is occurring.
        /// </summary>
        private volatile bool persistingToFile;

        /// <summary>
        /// Flag indicating logging is enabled.
        /// </summary>
        private volatile bool enabled;

        /// <summary>
        /// The write observer.
        /// </summary>
        private IDisposable writeObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingService"/> class.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application using the logging service.
        /// </param>
        public LoggingService(string applicationName)
        {
            this.applicationName = applicationName;
            this.isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            var lowerApplicationName = this.applicationName.ToLowerInvariant();
            this.isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (!this.isolatedStorage.DirectoryExists(lowerApplicationName))
            {
                this.isolatedStorage.CreateDirectory(lowerApplicationName);
            }

            this.logPath = Path.Combine(lowerApplicationName, LogFilename);

            Debug.WriteLine(string.Format("Manufacturer - {0}", GetDeviceExtendedProperties("DeviceManufacturer")));
            Debug.WriteLine(string.Format("Name - {0}", GetDeviceExtendedProperties("DeviceName")));
            _logBuffer = new StringBuilder();

            this.Level = LogLevel.Error;
            this.DebugOut = true;
            this.MessageFormat = MESSAGE_FORMAT;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.writeObserver != null)
            {
                this.writeObserver.Dispose();
                this.writeObserver = null;

                if (this.AutoPersist == LogAutoPersist.Periodic || this.AutoPersist == LogAutoPersist.Dispose)
                    this.PersistQueueToFile(isDisposing:true);

                this.enabled = false;
            }

            if (this.isolatedStorage != null)
            {
                this.isolatedStorage.Dispose();
                this.isolatedStorage = null;
            }

            _logBuffer = null;
        }

        /// <summary>
        /// Gets the path to the log file.
        /// </summary>
        public string LogPath
        {
            get { return this.logPath; }
        }

        public LogAutoPersist AutoPersist { get; set; }

        /// <summary>
        /// Gets all the current message in the file.
        /// </summary>
        public IEnumerable<string> Messages
        {
            get
            {
                try
                {
                    return this.ReadFile();
                }
                catch (Exception exn)
                {
                    throw new LoggingException(FailedToReadMessages, exn);
                }
            }
        }

        public string LogBuffer()
        {
            if (null == _logBuffer)
                _logBuffer = new StringBuilder();
            return _logBuffer.ToString();
        }

        public string ReadFileText()
        {
            var text = string.Empty;
            try
            {
                using (var sr = new StreamReader(this.isolatedStorage.OpenFile(this.logPath,
                                                                               FileMode.OpenOrCreate,
                                                                               FileAccess.Read,
                                                                               FileShare.ReadWrite)))
                {
                    text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception exn)
            {
                Debug.WriteLine("LoggingService: Failed to read file, message - '{0}'.", exn.Message);
            }

            return text;
        }

        /// <summary>
        /// Enables the logging service.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Enable()
        {
            try
            {
                if (this.enabled)
                {
                    return this;
                }

                this.enabled = true;

                this.writeObserver = Observable.Interval(TimeSpan.FromMilliseconds(WriteFrequency))
                    .SubscribeOn(Scheduler.ThreadPool)
                    .Subscribe(t => this.PersistQueueToFile());

                return this;
            }
            catch (Exception exn)
            {
                throw new LoggingException(FailedToEnable, exn);
            }
        }

        /// <summary>
        /// Disables the logging service.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Disable()
        {
            try
            {
                if (!this.enabled)
                {
                    return this;
                }

                if (this.writeObserver != null)
                {
                    this.writeObserver.Dispose();
                    this.writeObserver = null;
                }

                this.PersistQueueToFile();

                this.enabled = false;

                return this;
            }
            catch (Exception exn)
            {
                throw new LoggingException(FailedToDisable, exn);
            }
        }

        public LogLevel Level { get; set; }

        public bool DebugOut { get; set; }

        public string MessageFormat { get; set; }

        /// <summary>
        /// Writes a formatted message to the log with the arguments.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message">
        /// The message to be written.
        /// </param>
        /// <param name="args">
        /// The message argument.
        /// </param>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILog Write(LogLevel level, string message, params object[] args)
        {
            try
            {
                var builtMessage = string.Format(MessageFormat, DateTime.Now, string.Format(message, args));

                if (DebugOut)
                    Debug.WriteLine(builtMessage);

                if (!this.enabled)
                {
                    return this;
                }

                var canLog = (int)level >= (int)this.Level;
                if (!canLog) return this;

                if (null == _logBuffer)
                    _logBuffer = new StringBuilder();
                _logBuffer.AppendLine(builtMessage);

                this.Enqueue(new List<string> { builtMessage });

                return this;
            }
            catch (Exception exn)
            {
                throw new LoggingException(FailedToWrite, exn);
            }
        }

        

        /// <summary>
        /// Writes diagnostics information about the current state of the device to the log.
        /// In release mode user and device extended properties are excluded.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILog WriteDiagnostics()
        {
            try
            {
                if (!this.enabled)
                {
                    return this;
                }

                var now = DateTime.Now;
#if DEBUG
                var anid = string.Format("ANID - {0}", GetUserExtendedProperties("ANID"));

                var deviceManufacturer = string.Format("Manufacturer - {0}", GetDeviceExtendedProperties("DeviceManufacturer"));
                var deviceName = string.Format("Name - {0}", GetDeviceExtendedProperties("DeviceName"));
                var deviceUniqueId = string.Format("Unique Id - {0}", GetDeviceExtendedProperties("DeviceUniqueId"));
                var deviceFirmwareVersion = string.Format("Firmware Version - {0}", GetDeviceExtendedProperties("DeviceFirmwareVersion"));
                var deviceHardwareVersion = string.Format("Hardware Version - {0}", GetDeviceExtendedProperties("DeviceHardwareVersion"));
                var deviceTotalMemory = string.Format("Total RAM - {0} bytes", ((long)GetDeviceExtendedProperties("DeviceTotalMemory")).ToString("N0", this.numberFormatInfo));
                var applicationCurrentMemoryUsage = string.Format("Current Memory Usage - {0} bytes", ((long)GetDeviceExtendedProperties("ApplicationCurrentMemoryUsage")).ToString("N0", this.numberFormatInfo));
                var applicationPeakMemoryUsage = string.Format("Peak Memory Usage - {0} bytes", ((long)GetDeviceExtendedProperties("ApplicationPeakMemoryUsage")).ToString("N0", this.numberFormatInfo));
#endif

                var totalMemory = string.Format("Allocated memory - {0} bytes", GC.GetTotalMemory(true).ToString("N0", this.numberFormatInfo));
                var isolatedFreespace = string.Format("Isolated storage free memory - {0} bytes", this.isolatedStorage.AvailableFreeSpace.ToString("N0", this.numberFormatInfo));
                var isolatedQuota = string.Format("Isolated storage quota - {0} bytes", this.isolatedStorage.Quota.ToString("N0", this.numberFormatInfo));

                var messages = new List<string>
                                   {
#if DEBUG
                                       string.Format(MessageFormat, now, anid), 
                                       string.Format(MessageFormat, now, deviceManufacturer), 
                                       string.Format(MessageFormat, now, deviceName), 
                                       string.Format(MessageFormat, now, deviceUniqueId), 
                                       string.Format(MessageFormat, now, deviceFirmwareVersion), 
                                       string.Format(MessageFormat, now, deviceHardwareVersion), 
                                       string.Format(MessageFormat, now, deviceTotalMemory), 
                                       string.Format(MessageFormat, now, applicationCurrentMemoryUsage), 
                                       string.Format(MessageFormat, now, applicationPeakMemoryUsage), 
#endif
                                       string.Format(MessageFormat, now, Microsoft.Devices.Environment.DeviceType), 
                                       string.Format(MessageFormat, now, totalMemory), 
                                       string.Format(MessageFormat, now, isolatedFreespace), 
                                       string.Format(MessageFormat, now, isolatedQuota)
                                   };

                messages.ForEach(m => Debug.WriteLine(m));

                this.Enqueue(messages);

                return this;
            }
            catch (Exception exn)
            {
                throw new LoggingException(FailedToWriteDiagnostics, exn);
            }
        }

        /// <summary>
        /// Clears the contents of the log file.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Clear()
        {
            try
            {
                lock (this.sync)
                {
                    this.pendingMessages.Clear();
                    this.ClearFile();
                    _logBuffer = null;
                }

                return this;
            }
            catch (Exception exn)
            {
                throw new LoggingException(FailedToClear, exn);
            }
        }

        /// <summary>
        /// Adds messages to the queue for writing to file.
        /// </summary>
        /// <param name="messages">
        /// The messages
        /// </param>
        private void Enqueue(List<string> messages)
        {
            messages.ForEach(m => this.pendingMessages.Enqueue(m));
        }

        private StringBuilder _logBuffer;

        public bool WritePendingToFile()
        {
            return PersistQueueToFile();
        }

        /// <summary>
        /// The persist queue to file.
        /// </summary>
        private bool PersistQueueToFile(bool isDisposing = false)
        {
            if (this.persistingToFile) return false;
            if (this.AutoPersist == LogAutoPersist.None) return false;
            if (!isDisposing && this.AutoPersist == LogAutoPersist.Dispose) return false;

            var foundMessages = false;

            lock (this.sync)
            {
                this.persistingToFile = true;

                var messages = new List<string>();
                while (this.pendingMessages.Count != 0)
                {
                    var logMsg = this.pendingMessages.Dequeue();
                    messages.Add(logMsg);
                }

                if (messages.Count() != 0)
                {
                    foundMessages = true;
                    this.WriteFile(messages);
                }

                this.persistingToFile = false;
            }

            return foundMessages;
        }

        /// <summary>
        /// Reads the contents of the log file.
        /// </summary>
        /// <returns>
        /// Returns an enumerable collection of persisted messages.
        /// </returns>
        private IEnumerable<string> ReadFile()
        {
            var messages = new List<string>();
            try
            {
                using (var sr = new StreamReader(this.isolatedStorage.OpenFile(this.logPath, 
                                                                               FileMode.OpenOrCreate, 
                                                                               FileAccess.Read, 
                                                                               FileShare.ReadWrite)))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        messages.Add(line);
                    }

                    sr.Close();
                }
            }
            catch (Exception exn)
            {
                Debug.WriteLine("LoggingService: Failed to read file, message - '{0}'.", exn.Message);
            }

            return messages;
        }

        /// <summary>
        /// Writes messages to the file.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        private void WriteFile(List<string> messages)
        {
            try
            {
                using (var sw = new StreamWriter(this.isolatedStorage.OpenFile(this.logPath, 
                                                                               FileMode.OpenOrCreate, 
                                                                               FileAccess.Write, 
                                                                               FileShare.ReadWrite)))
                {
                    messages.ForEach(sw.WriteLine);
                    //var sb = new StringBuilder();
                    //messages.ForEach(m=> sb.AppendLine(m));
                    //sw.Write(sb.ToString());
                    //sw.Flush();
                    //sw.Close();
                }

                this.LogModified(this, new EventArgs());
            }
            catch (Exception exn)
            {
                Debug.WriteLine("LoggingService: Failed to write to file, message - '{0}'.", exn.Message);
            }
        }

        /// <summary>
        /// Clears the log file.
        /// </summary>
        private void ClearFile()
        {
            try
            {
                using (new StreamWriter(this.isolatedStorage.CreateFile(this.LogPath)))
                {
                }
            }
            catch (Exception exn)
            {
                Debug.WriteLine("LoggingService: Failed to clear file, message - '{0}'.", exn.Message);
            }
        }

        /// <summary>
        /// Gets a device extended properties or returns an empty string.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// Returns the device extended properties.
        /// </returns>
        private static object GetDeviceExtendedProperties(string propertyName)
        {
            try
            {
#if DEBUG
                object value;
                if (DeviceExtendedProperties.TryGetValue(propertyName, out value))
                {
                    return value.GetType() == typeof(byte[]) ? ConvertByteArrayToString((byte[])value) : value;
                }

#endif

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a user extended properties or returns an empty string.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// Returns the user extended properties.
        /// </returns>
        private static object GetUserExtendedProperties(string propertyName)
        {
            try
            {
#if DEBUG
                object value;
                if (UserExtendedProperties.TryGetValue(propertyName, out value))
                {
                    return value.GetType() == typeof(byte[]) ? ConvertByteArrayToString((byte[])value) : value;
                }

#endif
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a byte array to string.
        /// </summary>
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        /// <returns>
        /// Returns the converted byte array to string.
        /// </returns>
        private static string ConvertByteArrayToString(IEnumerable<byte> value)
        {
            var stringValue = string.Empty;
            value.ToList().ForEach(b => stringValue += b);

            return stringValue;
        }
    }
}