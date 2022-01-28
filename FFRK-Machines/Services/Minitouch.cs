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
        private byte[] clientBuffer = new byte[1024];

        public Task<bool> Connect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect("localhost", 1111);
            var bufferSize = client.Receive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, out errorCode);
            //client.BeginReceive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, OnReceive, this);
            if (errorCode != SocketError.Success || bufferSize == 0)
            {
                Dispose();
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task Tap(int X, int Y)
        {

            client.Send(Encoding.ASCII.GetBytes($"d 0 {X} {Y} 50\nc\n"));
            client.Send(Encoding.ASCII.GetBytes("u 0\nc\n"));
            return Task.CompletedTask;
        }

        //private void OnReceive(IAsyncResult ar)
        //{
        //    try
        //    {
        //        var minitouchClient = (Minitouch)ar.AsyncState;

        //        SocketError errorCode;
        //        var bufferSize = client.EndReceive(ar, out errorCode);
        //        if (errorCode != SocketError.Success)
        //        {
        //            Dispose();
        //            return;
        //        }
    
        //    }
        //    catch { }

        //}

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
