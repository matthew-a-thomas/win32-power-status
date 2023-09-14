# PowerStatus

[![Build and test](https://github.com/matthew-a-thomas/win32-power-status/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/matthew-a-thomas/win32-power-status/actions/workflows/build-and-test.yml)
[![Nuget](https://img.shields.io/nuget/v/PowerStatus)](https://www.nuget.org/packages/PowerStatus/)

C# wrapper for win32's [System Power Status](https://learn.microsoft.com/en-us/windows/win32/power/system-power-status)

## Examples

### Get a snapshot of the current power status

```csharp
var statusProvider = new PowerStatusProvider();
var status = statusProvider.GetStatus();
Console.WriteLine($"{status}");
```

```
PowerStatus { AcLineStatus = Offline, BatteryFlags = 0, BatteryLifeProportion = 0.47, BatterySaver = False, BatteryTime = 01:53:54, FullTime =  }
```

### Receive notifications when something changes

```csharp
var statusProvider = new PowerStatusProvider();
using (statusProvider.Subscribe(
    e => Console.WriteLine(e.ToString()),
    PowerStatusNotification.BatteryProportionChanged(proportion =>
        Console.WriteLine($"The battery proportion is {proportion}")),
    PowerStatusNotification.BatterySaverIsOn(isOn =>
        Console.WriteLine($"Battery saver {(isOn ? "is" : "is not")} on")),
    PowerStatusNotification.CurrentMonitorDisplayState(state =>
        Console.WriteLine($"The current monitor's display state is {state}")),
    PowerStatusNotification.LidIsOpen(isOpen =>
        Console.WriteLine($"The lid {(isOpen ? "is" : "is not")} open")),
    PowerStatusNotification.PowerSchemePersonalityChanged(personality =>
        Console.WriteLine($"The power scheme is {personality}")),
    PowerStatusNotification.PowerSource(powerSource =>
        Console.WriteLine($"The power source is {powerSource}")),
    PowerStatusNotification.PrimaryMonitorIsOn(isOn =>
        Console.WriteLine($"The primary monitor {(isOn ? "is" : "is not")} on")),
    PowerStatusNotification.SessionDisplayState(state =>
        Console.WriteLine($"The session display is {state}")),
    PowerStatusNotification.SystemAwayModeChanged(awayMode =>
        Console.WriteLine($"The system is {awayMode} away mode")),
    PowerStatusNotification.SystemIsBusy(() =>
        Console.WriteLine("The system is busy")),
    PowerStatusNotification.UserIsActive(isActive =>
        Console.WriteLine($"The user {(isActive ? "is" : "is not")} active"))
))
{
    Console.WriteLine("Press any key to stop listening to events...");
    Console.ReadKey(true);
}
```

```
Press any key to stop listening to events...
The battery proportion is 0.47
Battery saver is not on
The lid is open
The power scheme is Automatic
The power source is DC
The primary monitor is on
The current monitor's display state is On
The system is busy
The user is active
The session display is On
```