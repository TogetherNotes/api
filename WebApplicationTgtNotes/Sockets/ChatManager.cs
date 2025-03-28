using System;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Sockets;
using WebApplicationTgtNotes.DTO;
using System.Collections.Concurrent;
using WebApplicationTgtNotes.Models;

namespace WebApplicationTgtNotes.Sockets
{
    public class ChatManager
    {
        private const int Port = 5000;
        private TcpListener _server;
        private static readonly ConcurrentDictionary<int, TcpClient> ConnectedClients = new ConcurrentDictionary<int, TcpClient>();

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
            var buffer = new byte[1024];
            int byteCount;
            int currentUserId = -1;

            try
            {
                // Primer missatge hauria de ser l'ID del client
                byteCount = stream.Read(buffer, 0, buffer.Length);
                var userIdMessage = Encoding.UTF8.GetString(buffer, 0, byteCount);
                if (!int.TryParse(userIdMessage, out currentUserId))
                {
                    SendResponse(stream, "Invalid user ID");
                    return;
                }

                ConnectedClients[currentUserId] = client;
                Console.WriteLine($"[INFO] User {currentUserId} connected");

                // Enviar missatges pendents
                using (var db = new TgtNotesEntities())
                {
                    var pendingMessages = db.messages
                        .Where(m => m.chats.user1_id == currentUserId || m.chats.user2_id == currentUserId)
                        .Where(m => m.sender_id != currentUserId && (m.is_read ?? false) == false)
                        .ToList();

                    foreach (var msg in pendingMessages)
                    {
                        SendResponse(stream, JsonConvert.SerializeObject(new
                        {
                            from = msg.sender_id,
                            content = msg.content,
                            sent = msg.send_at
                        }));

                        msg.is_read = true;
                    }
                    db.SaveChanges();
                }

                // Loop de recepció de missatges
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine($"[RECEIVED] {message}");

                    var data = JsonConvert.DeserializeObject<SocketsDTO>(message);

                    if (data == null || data.sender_id <= 0 || data.receiver_id <= 0 || string.IsNullOrWhiteSpace(data.content))
                    {
                        SendResponse(stream, "Invalid message format");
                        continue;
                    }

                    using (var db = new TgtNotesEntities())
                    {
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

                        var newMessage = new messages
                        {
                            sender_id = data.sender_id,
                            content = data.content,
                            send_at = DateTime.Now,
                            is_read = false,
                            chat_id = chat.id
                        };
                        db.messages.Add(newMessage);
                        db.SaveChanges();
                    }

                    // Si el receptor està connectat, envia-li el missatge
                    if (ConnectedClients.TryGetValue(data.receiver_id, out var receiverClient))
                    {
                        var receiverStream = receiverClient.GetStream();
                        SendResponse(receiverStream, JsonConvert.SerializeObject(new
                        {
                            from = data.sender_id,
                            content = data.content
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            finally
            {
                if (currentUserId > 0)
                {
                    ConnectedClients.TryRemove(currentUserId, out _);
                    Console.WriteLine($"[INFO] User {currentUserId} disconnected");
                }

                client.Close();
            }
        }

        private void SendResponse(NetworkStream stream, string message)
        {
            var response = Encoding.UTF8.GetBytes(message);
            stream.Write(response, 0, response.Length);
        }
    }
}