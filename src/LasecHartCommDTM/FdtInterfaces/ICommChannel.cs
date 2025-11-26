using System;
using System.Runtime.InteropServices;

namespace LasecHartCommDTM.FdtInterfaces
{
    [Guid("B4B6B3E7-639D-460B-B9A0-6C7F7EB20002")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface ICommChannel
    {
        void Open();
        void Close();

        void SendFrame(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] request,
            int length,
            int timeoutMs);

        int ReceiveFrame(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer,
            int bufferLength,
            int timeoutMs);
    }
}
