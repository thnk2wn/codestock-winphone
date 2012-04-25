// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLog.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
//   Interface defining the logging API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Phone.Common.Diagnostics.Logging
{
    public sealed class DebugLog : ILog
    {
        public ILog Write(LogLevel level, string message, params object[] args)
        {
            var messageWithParameters = string.Format(message, args);
            Debug.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToUniversalTime(), messageWithParameters));

            return this;
        }

        public ILog WriteDiagnostics()
        {
            Debug.WriteLine(string.Format("{0} - NOT IMPLEMENTED!", DateTime.Now.ToUniversalTime()));
            return this;
        }
    }
}