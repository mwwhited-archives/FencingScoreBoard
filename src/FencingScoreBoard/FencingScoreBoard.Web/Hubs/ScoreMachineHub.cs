using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FencingScoreBoard.Web.Hubs
{
    public class ScoreMachineHub : Hub
    {
        private static object _lastScore;

        public static bool Recording { get; private set; }
        public static string OtherVideo { get; private set; }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine(this.Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task SendData(JObject data)
        {
            var messageType = (string)data["messageType"];
            if (messageType == "SpecialAction")
            {
                var action = (string)data.Property("Action");
                if (new[] { "StartBout", "EndBout" }.Contains(action, StringComparer.InvariantCultureIgnoreCase))
                {
                    Recording = string.Equals("StartBout", action, StringComparison.InvariantCultureIgnoreCase);
                }

                var channel = (string)data.Property("Channel");
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    if (string.Equals(channel, "RESET", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OtherVideo = null;
                    }
                    else
                    {
                        OtherVideo = channel.ToUpper();
                    }
                }
            }

            var payload = new { source = this.Context.ConnectionId, data, recording = Recording };
            if (messageType == "ScoreMachine")
            {
                _lastScore = Merge(payload, data);
            }
            else if (messageType == "ClientConnected" && _lastScore != null)
            {
                await Clients.Caller.SendAsync("ReceiveData", _lastScore);
            }

            await Clients.All.SendAsync("ReceiveData", payload);
        }

        private object Merge(dynamic payload, JObject data)
        {
            JObject existing = payload.data;

            foreach (var child in data.Children().OfType<JProperty>())
            {
                existing[child.Name] = child.Value;
            }
            return new
            {
                source = payload.source,
                data = existing,
            };
        }
    }
}
