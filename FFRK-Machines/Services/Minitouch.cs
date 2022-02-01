using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFRK_Machines.Services
{
    public class Minitouch : IDisposable
    {
        private object _lock = new object();
        private Socket client;
        private SocketError errorCode;
        private byte[] clientBuffer = new byte[128];
        private Header header;

        class Header
        {
            public string Version { get; set; }
            public int MaxContacts { get; set; }
            public int MaxX { get; set; }
            public int MaxY { get; set; }
            public int MaxPressure { get; set; }
            public int Pid { get; set; }
            public bool IsRotated { get; set; }
        }

        public Task<bool> Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect("localhost", 1111);
            var bufferSize = client.Receive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, out errorCode);
            if (errorCode != SocketError.Success || bufferSize == 0)
            {
                Dispose();
                return Task.FromResult(false);
            }
            ParseHeader(clientBuffer);

            return Task.FromResult(true);
        }

        public Task Tap(int X, int Y)
        {
            if (header.IsRotated)
            {
                var x1 = X;
                X = Y;
                Y = header.MaxY - x1;
            }
            client.Send(Encoding.ASCII.GetBytes($"d 0 {X} {Y} 50\nc\n"));
            client.Send(Encoding.ASCII.GetBytes("u 0\nc\n"));
            return Task.CompletedTask;
        }
        
        private void ParseHeader(byte[] buffer)
        {
            header = new Header();
            var headerString = Encoding.ASCII.GetString(buffer);
            var lines = headerString.Split('\n');
            var args = lines[1].Split(' ');
            header.Version = lines[0].Split(' ')[1];
            header.MaxContacts = int.Parse(args[1]);
            header.MaxX = int.Parse(args[2]);
            header.MaxY = int.Parse(args[3]);
            header.MaxPressure = int.Parse(args[4]);
            header.IsRotated = header.MaxX > header.MaxY;
            header.Pid = int.Parse(lines[2].Split(' ')[1]);
        }
        public bool IsRunning
        {
            get
            {
                if (client != null)
                    return client.Connected;
                return false;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (client != null)
                {
                    client.Close();
                    client = null;
                }
            }
        }
    }
}
