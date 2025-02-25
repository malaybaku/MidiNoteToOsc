using NAudio.Midi;

namespace Baxter.MidiToOsc
{
    sealed class MidiInputObserver
    {
        public event Action<(int key, bool down)>? NoteChanged;
        public event Action<bool>? SustainPedalChanged;

        private MidiIn? midiIn;
        private bool _sustainPedalIsOn = false;

        public bool WriteLogOnInput { get; set; }

        public int GetDeviceCount() => MidiIn.NumberOfDevices;

        public void WriteDeviceIndexAndNames()
        {
            var count = MidiIn.NumberOfDevices;

            for (var i = 0; i < count; i++)
            {
                WriteLog($"[{i}]: {MidiIn.DeviceInfo(i).ProductName}");
            }
        }

        public void Start(int deviceIndex)
        {
            Stop();

            // 指定されたデバイスインデックスでMIDI入力を初期化
            midiIn = new MidiIn(deviceIndex);
            midiIn.MessageReceived += OnMidiMessageReceived;
            midiIn.ErrorReceived += OnMidiErrorReceived;
            midiIn.Start();

            WriteLog($"MIDI入力の取得を開始します: [{deviceIndex}] {MidiIn.DeviceInfo(deviceIndex).ProductName}");
        }

        public void Stop()
        {
            try
            {
                if (midiIn != null)
                {
                    midiIn.MessageReceived -= OnMidiMessageReceived;
                    midiIn.ErrorReceived -= OnMidiErrorReceived;
                    midiIn.Stop();
                }
            }
            catch (Exception ex)
            {
                WriteLog($"MIDI入力の停止処理でエラーが発生しました: {ex.Message}");
                WriteLog($"※デバイスを外した場合、このエラーが出るのは想定動作です。");
            }
            midiIn = null;
            _sustainPedalIsOn = false;
        }

        private void OnMidiMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            var messageType = e.MidiEvent.CommandCode;

            if (messageType == MidiCommandCode.NoteOn)
            {
                var noteEvent = (NoteEvent)e.MidiEvent;
                var isNoteOn = noteEvent.Velocity > 0;
                NoteChanged?.Invoke((noteEvent.NoteNumber, isNoteOn));
                if (WriteLogOnInput)
                {
                    WriteLog($"Midi.NoteOn, note={noteEvent.NoteNumber}, Note is actually on?={isNoteOn}");
                }
            }
            else if (messageType == MidiCommandCode.NoteOff)
            {
                var noteEvent = (NoteEvent)e.MidiEvent;
                NoteChanged?.Invoke((noteEvent.NoteNumber, false));
                if (WriteLogOnInput)
                {
                    WriteLog($"Midi.NoteOff, note={noteEvent.NoteNumber}");
                }
            }
            else if (messageType == MidiCommandCode.ControlChange)
            {
                // sustain pedalのメッセージだったらイベントを適宜発火する
                // 64以上の値であればON、それ以外はOFFとする
                // とくにON, OFFが切り替わったときだけイベントが発火するようにしている
                var controlEvent = (ControlChangeEvent)e.MidiEvent;
                if (controlEvent.Controller == MidiController.Sustain)
                {
                    var isOn = controlEvent.ControllerValue >= 64;
                    if (_sustainPedalIsOn != isOn)
                    {
                        _sustainPedalIsOn = isOn;
                        SustainPedalChanged?.Invoke(isOn);
                        if (WriteLogOnInput)
                        {
                            WriteLog($"Midi.SustainPedal, isOn={isOn}");
                        }
                    }
                }
            }
        }

        private void OnMidiErrorReceived(object? sender, MidiInMessageEventArgs e)
        {
            WriteLog($"*MIDI Error: {e.RawMessage}");
        }

        private void WriteLog(string message) => Console.WriteLine(message);
    }
}
