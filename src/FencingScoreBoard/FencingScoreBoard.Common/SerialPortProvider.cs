using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace FencingScoreBoard.WpCommonf
{
    public class SerialPortProvider : IDisposable
    {
        private string PortName { get; set; }
        private SerialPort Port { get; set; }

        private Func<Memory<byte>, Task> OnReceived;

        public SerialPortProvider(Func<Memory<byte>, Task> onReceived)
        {
            OnReceived = onReceived;
        }

        public void Dispose()
        {
            Port?.Dispose();
        }

        public IEnumerable<string> ListPorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Open(string portName)
        {
            var port = Port;
            if (port == null || !port.IsOpen)
            {
                Port = null;
                if (port != null)
                {
                    port.Dispose();
                }

                PortName = portName;
                port = new SerialPort(portName);
                port.DataReceived += Port_DataReceived;
                port.ErrorReceived += Port_ErrorReceived;
                port.Open();

                Port = port;
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.WriteLine($"{PortName}:ERROR: {e.EventType}");
        }

        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = Port;
            var buffer = new byte[port.ReadBufferSize];
            var read = port.Read(buffer, 0, buffer.Length);
            var data = buffer.AsMemory().Slice(0, read);
            await OnReceived(data);
        }
    }
}
