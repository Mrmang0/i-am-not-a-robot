
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTcpServer
{
    class Programs
    {
        static int port = 8005; // порт для приема входящих запросов
        static Socket handler;
        static void Msain(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("25.3.124.57"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();



                    Task.Run(new Action(() =>
                    {
                        while (true)
                        {
                            do
                            {
                                int bytes = 0; // количество полученных байтов
                                byte[] data = new byte[256]; // буфер для получаемых данных
                                bytes = handler.Receive(data);
                                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                                Console.WriteLine(Encoding.Unicode.GetString(data, 0, bytes));
                            }
                            while (handler.Available > 0);
                        }

                    }));


                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ
                    Task.Run(new Action(() =>
                    {
                        for (int i = 0; i < 99; i++)
                        {
                            byte[] data = new byte[256]; // буфер для получаемых данных
                            string message = Console.ReadLine();
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);

                        }
                    }));


                    // закрываем сокет
                    Console.WriteLine("soecket vse");
                }
            }
            catch (Exception ex)
            {
                CloseAsync();
                Console.WriteLine(ex.Message);
            }
        }



        static void CloseAsync()
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}