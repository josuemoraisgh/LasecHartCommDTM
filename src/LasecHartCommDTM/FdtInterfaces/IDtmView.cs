using System;
using System.Runtime.InteropServices;

namespace LasecHartCommDTM.FdtInterfaces
{
    [Guid("B4B6B3E7-639D-460B-B9A0-6C7F7EB20003")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IDtmView
    {
        void Show();
        void Hide();
    }
}
