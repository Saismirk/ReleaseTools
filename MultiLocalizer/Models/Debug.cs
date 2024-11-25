using System;
using Avalonia.Logging;
using Avalonia.Notification;
using MultiLocalizer.ViewModels;

namespace MultiLocalizer.Models;

public static class Debug {
    public static void Log(string message) {
        Console.WriteLine(message);
        MainWindowViewModel.Manager
                           .CreateMessage()
                           .Accent("#1751C3")
                           .Animates(true)
                           .Background("#333")
                           .HasBadge("Info")
                           .HasMessage(message)
                           .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                           .Queue();
    }
}