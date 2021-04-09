using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Handles
{
    public class ApiHandleHolder
    {
        public string HandleId { get; }

        public List<HqApiUser> Users { get; }

        public string Password { get; }

        public bool Secure => !string.IsNullOrEmpty(Password);

        public bool ClientCanSend { get; }

        public ApiHandleHolder(string id, bool clientCanSend = false, string password = null)
        {
            this.Users = new List<HqApiUser>();
            this.HandleId = id;
            this.ClientCanSend = clientCanSend;
            this.Password = password;
        }

        public void Add(HqApiUser user)
        {
            lock (Users) Users.Add(user);

            user.OnDisconnected += (client) =>
            {
                lock (Users) Users.Remove(client);
            };

            user.OnReceive += (client, str) =>
            {
                if (!ClientCanSend)
                {
                    ConsoleLogging.Instance.LogInformation($"Client {client.Connection.ConnectionInfo.ClientIpAddress} tried to send data on handle that does not support sending [{HandleId}].", true);
                    return;
                }
                OnMessage?.Invoke(client, str);
            };
        }

        public event Action<HqApiUser, string> OnMessage;

        public void Push(IHqApiOutgoingMessage message)
        {
            List<HqApiUser> targets = null;
            lock (Users)
            {
                if (Users.Count == 0) return;
                targets = new List<HqApiUser>();
                targets.AddRange(Users.Where(user => user.Connected));
            }
            if (targets.Count == 0) return;

            var data = message.Serialize();
            if (Password != null)
            {
                // encrypt tea
            }

            //foreach (var hqApiUser in targets) await hqApiUser.Send(data);
            Parallel.ForEach(targets, async (item) =>
            {
                await item.Send(data);
            });
        }

        public async Task PushTo(HqApiUser user, IHqApiOutgoingMessage message)
        {
            if (user.Connected) await user.Send(message.Serialize());
        }
    }
}
