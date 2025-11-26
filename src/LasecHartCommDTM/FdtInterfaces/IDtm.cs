using System;
using System.Runtime.InteropServices;

namespace LasecHartCommDTM.FdtInterfaces
{
    [Guid("B4B6B3E7-639D-460B-B9A0-6C7F7EB20001")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IDtm
    {
        void Initialize(IntPtr hostHandle);
        void Uninitialize();

        void GetDeviceInfo(
            [MarshalAs(UnmanagedType.BStr)] out string deviceTag,
            [MarshalAs(UnmanagedType.BStr)] out string deviceType,
            [MarshalAs(UnmanagedType.BStr)] out string vendor,
            [MarshalAs(UnmanagedType.BStr)] out string version);
    }
}
