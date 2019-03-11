using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class FakeSender
    {
        public int Port { get; set; }
        public int ResponesSize { get; set; }
        public string Address { get; set; }
        Socket socket;

        public List<byte[]> DataToSend;

        public FakeSender(string addres, int port, int responseSize = 256)
        {
            Address = addres;
            Port = port;
            ResponesSize = responseSize;
            DataToSend = new List<byte[]>();
        }

        public void Send()
        {
            Task.Run(new Action(() =>
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                ResponseHandler();
                while (true)
                {
                  
                    if (DataToSend.Count > 0)
                    {
                        socket.Send(DataToSend.First());
                        DataToSend.Remove(DataToSend.First());
                    }
                }
            }));
        }

        void ResponseHandler()
        {
            Task.Run(new Action(() =>
            {
                while (true)
                {
                        do
                        {
                            Console.WriteLine("i am work");
                            byte[] data = new byte[ResponesSize];
                            int bytes = socket.Receive(data, data.Length, 0);
                            Console.WriteLine(Encoding.Unicode.GetString(data, 0, bytes));
                        DataToSend.Add(Encoding.Unicode.GetBytes("I am NIkon"));
                        }
                        while (socket.Available > 0);           
                }
            }));
        }

        public string Send(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(Address), Port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Connect(ipPoint);
                socket.Send(data);

                data = new byte[ResponesSize];

                int bytes = 0;



                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

            return builder.ToString();
        }

        public string Send(string message)
        {
            return Send(Encoding.Unicode.GetBytes(message));
        }

       
    }
}
