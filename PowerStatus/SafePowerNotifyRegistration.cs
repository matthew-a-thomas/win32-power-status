using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Power;

namespace PowerStatus;

sealed class SafePowerNotifyRegistration : SafeOneShotHandle<HPOWERNOTIFY>
{
    public SafePowerNotifyRegistration(HPOWERNOTIFY registration) : base(registration)
    { }

    protected override void Release(bool silent, HPOWERNOTIFY handle)
    {
        if (!PInvoke.UnregisterPowerSettingNotification(handle) && !silent)
            throw new Win32Exception(Marshal.GetLastSystemError());
    }
}