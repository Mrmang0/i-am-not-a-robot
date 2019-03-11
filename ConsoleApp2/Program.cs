using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ConsoleApp2;

namespace SocketTcpClient
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "25.3.124.57"; // адрес сервера
        static void Main(string[] args)
        {
            var a = new FakeSender(address, port);
            a.Send();
            a.DataToSend.Add(Encoding.Unicode.GetBytes("ads"));
            while (true)
            {

            }
        }

        static void send()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                Console.Write("Введите сообщение:");
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт


                while (true)
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    Console.WriteLine("respond");
                    Console.WriteLine(socket.Available);
                    Console.WriteLine("ответ сервера: " + builder.ToString());
                    if (builder.ToString() == "close")
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}