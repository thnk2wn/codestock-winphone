using System;

namespace CodeStock.App
{
    public interface IApp
    {
        DateTime StartupTime { get; }

        bool IsNewApp { get;  }

        bool IsDarkTheme { get; }

        string MapsApiKey { get; }
    }
}
