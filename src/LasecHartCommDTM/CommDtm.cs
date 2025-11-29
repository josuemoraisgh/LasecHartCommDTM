using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using LasecHartCommDTM.FdtInterfaces;
using HartEngine;

namespace LasecHartCommDTM
{
    [Guid("B4B6B3E7-639D-460B-B9A0-6C7F7EB20010")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class CommDtm : IDtm, ICommChannel
    {
        private ChannelManager _manager;
        private bool _initialized;
        private bool _open;
        private byte[] _lastResponse = Array.Empty<byte>();

        private string _mode = "serial";
        private string _serialPort = "COM1";
        private string _udpHost = "127.0.0.1";
        private int _udpPort = 20000;
        private int _baud = 1200;

        public CommDtm()
        {
            _manager = new ChannelManager();
        }

        #region IDtm

        public void Initialize(IntPtr hostHandle)
        {
            _initialized = true;
        }

        public void Uninitialize()
        {
            _manager?.Dispose();
            _open = false;
            _initialized = false;
        }

        public void GetDeviceInfo(out string deviceTag, out string deviceType, out string vendor, out string version)
        {
            deviceTag = "Lasec HART Communication DTM";
            deviceType = "HART Communication";
            vendor = "JosueLab";
            version = "1.0.0";
        }

        #endregion

        #region Config

        public void Configure(string mode, string serialPort, string udpHost, int udpPort, int baud)
        {
            if (!_initialized)
                throw new InvalidOperationException("DTM not initialized");

            _mode = mode?.ToLowerInvariant() ?? "serial";
            _serialPort = serialPort ?? "COM1";
            _udpHost = string.IsNullOrWhiteSpace(udpHost) ? "127.0.0.1" : udpHost;
            _udpPort = udpPort <= 0 ? 20000 : udpPort;
            _baud = baud <= 0 ? 1200 : baud;

            _manager.Configure(_mode, _serialPort, _udpHost, _udpPort, _baud);
            _open = true;
        }

        #endregion

        #region ICommChannel

        public void Open()
        {
            if (!_initialized)
                throw new InvalidOperationException("DTM not initialized");

            if (_open) return;

            _manager.Configure(_mode, _serialPort, _udpHost, _udpPort, _baud);
            _open = true;
        }

        public void Close()
        {
            _manager.Dispose();
            _open = false;
        }

        public void SendFrame(byte[] request, int length, int timeoutMs)
        {
            if (!_open)
                throw new InvalidOperationException("Channel not open");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (length > request.Length) length = request.Length;

            var frame = new byte[length];
            Array.Copy(request, frame, length);

            _lastResponse = _manager.SendAndReceive(frame, timeoutMs);
        }

        public int ReceiveFrame(byte[] buffer, int bufferLength, int timeoutMs)
        {
            if (_lastResponse == null || _lastResponse.Length == 0)
                return 0;

            var n = Math.Min(bufferLength, _lastResponse.Length);
            Array.Copy(_lastResponse, buffer, n);
            return n;
        }

        #endregion

        // --------------------------------------------------------------------
        // 🔥 REGISTRO FDT DTM (necessário para o PACTware encontrar o DTM)
        // --------------------------------------------------------------------
        #region COM_FDT_Registration

        // Categoria COM FDT DTM oficial (FDT 1.x)
        private const string FdtDtmCategoryId = "{036D1490-387B-11D4-86E1-00E0987270B9}";

        [ComRegisterFunction]
        public static void Register(Type t)
        {
            try
            {
                string clsidKeyPath = @"CLSID\" + t.GUID.ToString("B");

                using (var clsidKey = Registry.ClassesRoot.OpenSubKey(clsidKeyPath, writable: true))
                {
                    if (clsidKey != null)
                    {
                        clsidKey.CreateSubKey(@"Implemented Categories\" + FdtDtmCategoryId);
                    }
                }

                using (var catKey = Registry.ClassesRoot.CreateSubKey(@"Component Categories\" + FdtDtmCategoryId))
                {
                    if (catKey != null)
                        catKey.SetValue(null, "FDT DTM");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao registrar categoria FDT DTM: " + ex.Message);
            }
        }

        [ComUnregisterFunction]
        public static void Unregister(Type t)
        {
            try
            {
                string clsidKeyPath = @"CLSID\" + t.GUID.ToString("B");

                using (var clsidKey = Registry.ClassesRoot.OpenSubKey(clsidKeyPath, writable: true))
                {
                    if (clsidKey != null)
                    {
                        clsidKey.DeleteSubKeyTree(@"Implemented Categories\" + FdtDtmCategoryId, throwOnMissingSubKey: false);
                    }
                }
            }
            catch
            {
                // silenciosamente ignora falhas
            }
        }

        #endregion
    }
}