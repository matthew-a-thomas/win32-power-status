using System;
using PowerStatus;

namespace Example;

static class Program
{
    static void Main()
    {
        var statusProvider = new PowerStatusProvider();
        var status = statusProvider.GetStatus();
        Console.WriteLine($"{status}");

        using (statusProvider.Subscribe(
            e => Console.WriteLine(e.ToString()),
            PowerStatusNotification.BatteryProportionChanged(proportion => Console.WriteLine($"The battery proportion is {proportion}")),
            PowerStatusNotification.BatterySaverIsOn(isOn => Console.WriteLine($"Battery saver {(isOn ? "is" : "is not")} on")),
            PowerStatusNotification.CurrentMonitorDisplayState(state => Console.WriteLine($"The current monitor's display state is {state}")),
            PowerStatusNotification.LidIsOpen(isOpen => Console.WriteLine($"The lid {(isOpen ? "is" : "is not")} open")),
            PowerStatusNotification.PowerSchemePersonalityChanged(personality => Console.WriteLine($"The power scheme is {personality}")),
            PowerStatusNotification.PowerSource(powerSource => Console.WriteLine($"The power source is {powerSource}")),
            PowerStatusNotification.PrimaryMonitorIsOn(isOn => Console.WriteLine($"The primary monitor {(isOn ? "is" : "is not")} on")),
            PowerStatusNotification.SessionDisplayState(state => Console.WriteLine($"The session display is {state}")),
            PowerStatusNotification.SystemAwayModeChanged(awayMode => Console.WriteLine($"The system is {awayMode} away mode")),
            PowerStatusNotification.SystemIsBusy(() => Console.WriteLine("The system is busy")),
            PowerStatusNotification.UserIsActive(isActive => Console.WriteLine($"The user {(isActive ? "is" : "is not")} active"))
        ))
        {
            Console.WriteLine("Press any key to stop listening to events...");
            Console.ReadKey(true);
        }
    }
}