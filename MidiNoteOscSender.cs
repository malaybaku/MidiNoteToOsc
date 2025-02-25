using Newtonsoft.Json.Linq;
using Rug.Osc;
using System.Net;

namespace Baxter.MidiToOsc
{
    record OscSendSettings(string IpAddress, int Port, string MessageAddress)
    {

        public static OscSendSettings LoadSettings()
        {
            var exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configFilePath = Path.Combine(exeDirectory, "config.json");

            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"OSC送信先の設定ファイルが想定したファイスパスに存在しません: {configFilePath}");
                Console.WriteLine("デフォルト設定を適用します");
                return CreateDefault();
            }

            try
            {
                var content = File.ReadAllText(configFilePath);
                var config = JObject.Parse(content);

                var oscIpAddress = config["OscIpAddress"]?.ToString() ?? "127.0.0.1";
                var oscPort = ((int?)config["OscPort"]) ?? 9000;
                var messageAddress = config["MessageAddress"]?.ToString() ?? "/baxter/midi";
                return new OscSendSettings(oscIpAddress, oscPort, messageAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ファイルのパース中にエラーが発生しました: {ex.Message}");
                Console.WriteLine("デフォルト設定を適用します");
                return CreateDefault();
            }
        }

        private static OscSendSettings CreateDefault() => new("127.0.0.1", 9000, "/baxter/midi");

        public override string ToString() => $"[IpAddress:Port:MessageAddress] {IpAddress}:{Port}:{MessageAddress}";
    }

    class MidiNoteOscSender : IDisposable
    {
        private readonly OscSendSettings _settings;
        private readonly OscSender _sender;

        public MidiNoteOscSender(OscSendSettings settings) 
        {
            _settings = settings;

            var ipAddress = IPAddress.TryParse(_settings.IpAddress, out var validIp)
                ? validIp
                : IPAddress.Loopback;
            _sender = new OscSender(ipAddress, 0, _settings.Port);
            _sender.Connect();
        }

        public bool WriteLogOnSend { get; set; }

        public void SendNote(int note, bool isOn)
        {
            var message = new OscMessage(_settings.MessageAddress, note, isOn);
            _sender.Send(message);
            if (WriteLogOnSend)
            {
                Console.WriteLine($"OSC Send, note={note} isOn={isOn}");
            }
        }

        public void SendSustainPedal(bool isOn)
        {
            var message = new OscMessage(_settings.MessageAddress + "/sustain", isOn);
            _sender.Send(message);
            if (WriteLogOnSend)
            {
                Console.WriteLine($"OSC Send, sustain={isOn}");
            }
        }

        public void Dispose() => _sender.Dispose();
    }
}
