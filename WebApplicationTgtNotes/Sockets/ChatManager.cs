using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WebApplicationTgtNotes.DTO;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Sockets
{
    public class ChatManager
    {
        private const int Port = 5000;
        private TcpListener _server;

        public void Start()
        {
            _server = new TcpListener(IPAddress.Any, Port);
            _server.Start();
            Console.WriteLine($"[SERVER] Listening on port {Port}...");

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                Console.WriteLine("[SERVER] Client connected.");

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object obj)
        {
            var client = (TcpClient)obj;
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int byteCount;

            while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine($"[RECEIVED MESSAGE] {message}");

                SocketsDTO data;
                try
                {
                    data = JsonConvert.DeserializeObject<SocketsDTO>(message);
                }
                catch
                {
                    Console.WriteLine("[ERROR] Invalid JSON");
                    SendResponse(stream, "Invalid message format");
                    continue;
                }

                if (data.sender_id <= 0 || data.receiver_id <= 0 || string.IsNullOrWhiteSpace(data.content))
                {
                    Console.WriteLine("[ERROR] Missing fields");
                    SendResponse(stream, "Missing required fields");
                    continue;
                }

                using (var db = new TgtNotesEntities())
                {
                    if (!db.app.Any(a => a.id == data.sender_id) || !db.app.Any(a => a.id == data.receiver_id))
                    {
                        Console.WriteLine("[ERROR] Invalid users");
                        SendResponse(stream, "Invalid user IDs");
                        continue;
                    }

                    var chat = db.chats.FirstOrDefault(c =>
                        (c.user1_id == data.sender_id && c.user2_id == data.receiver_id) ||
                        (c.user1_id == data.receiver_id && c.user2_id == data.sender_id));

                    if (chat == null)
                    {
                        chat = new chats
                        {
                            date = DateTime.Now,
                            user1_id = data.sender_id,
                            user2_id = data.receiver_id
                        };
                        db.chats.Add(chat);
                        db.SaveChanges();
                    }

                    db.messages.Add(new messages
                    {
                        sender_id = data.sender_id,
                        content = data.content,
                        send_at = DateTime.Now,
                        is_read = false,
                        chat_id = chat.id
                    });

                    db.SaveChanges();
                    Console.WriteLine("[OK] Message saved to DB");
                    SendResponse(stream, "Message received and stored");
                }
            }

            Console.WriteLine("[SERVER] Client disconnected.");
            client.Close();
        }

        private void SendResponse(NetworkStream stream, string message)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}