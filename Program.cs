namespace Baxter.MidiToOsc;

public class Program
{
    private static readonly MidiInputObserver _observer = new();
    private static MidiNoteOscSender _sender;

    public static void Main(string[] args)
    {
        // やること
        // - MIDIの入力監視スタート
        // - キー入力によってデバッグ表示をon/off

        Console.WriteLine("""
このアプリケーションはMIDI入力をOSCに変換します。

利用できるキー操作:
        
基本操作
- [Q] アプリケーションを終了します。
- [L] PCに接続されたMIDIデバイス一覧を表示します。
- [S] デバイス番号を指定して、MIDI入力の取得とOSCメッセージの送信を開始します。
        
デバッグ用の操作
- [M] MIDI入力のデバッグログ表示をon/offします (デフォルト: off)
- [O] OSC出力のデバッグログ表示をon/offします (デフォルト: off)

デバイスが1つ以上接続されている場合、自動で接続します...
""");

        _observer.WriteDeviceIndexAndNames();
        if (_observer.GetDeviceCount() > 0)
        {
            _observer.Start(0);
        }
        else
        {
            Console.WriteLine("デバイスが検出されませんでした。以下の操作を行って下さい。");
            Console.WriteLine("- MIDIデバイスを接続");
            Console.WriteLine("- [L]キーでデバイス一覧を表示し、デバイスが認識されたことを確認");
            Console.WriteLine("- [S]キーでMIDI入力の取得を開始");
        }

        var oscSendSettings = OscSendSettings.LoadSettings();
        Console.WriteLine($"OSC送信設定: {oscSendSettings}");
        _sender = new MidiNoteOscSender(oscSendSettings);
        _observer.NoteChanged += value => _sender.SendNote(value.key, value.down);
        _observer.SustainPedalChanged += value => _sender.SendSustainPedal(value);

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();
            switch (char.ToUpper(key.KeyChar))
            {
                case 'Q':
                    QuitApp();
                    return;
                case 'L':
                    ShowDeviceList();
                    break;
                case 'S':
                    SelectAndStartToObserveMidiInput();
                    break;
                case 'M':
                    ToggleMidiInputDebugLogActive();
                    break;
                case 'O':
                    ToggleOscOutputDebugLogActive();
                    break;
                default:
                    // 知らないキーなのでスルー
                    break;
            }
        }
    }


    // Q key
    private static void QuitApp()
    {
        _observer.Stop();
        _sender?.Dispose();        
        
        Console.WriteLine("MIDI入力の検出とOSC送信を停止しました。ENTERで完全にアプリケーションを終了します。");
        Console.ReadLine();
    }

    // L key
    private static void ShowDeviceList() => _observer.WriteDeviceIndexAndNames();

    // S key
    private static void SelectAndStartToObserveMidiInput()
    {
        Console.Write("MIDIデバイスの番号を入力してください: ");
        var line = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(line, out var deviceIndex) ||
            !(deviceIndex >= 0 && deviceIndex < _observer.GetDeviceCount())
            )
        {
            Console.WriteLine("指定した数値が適切な範囲を超えています。Lキーでデバイス一覧を再確認してください。");
            return;
        }
        _observer.Start(deviceIndex);
        Console.Write($"MIDIデバイスの入力取得を開始しました: {deviceIndex}");
    }

    // M key
    private static void ToggleMidiInputDebugLogActive()
    {
        _observer.WriteLogOnInput = !_observer.WriteLogOnInput;
        Console.WriteLine($"MIDI入力時のデバッグログのon/offを切り替えました: 現在のログ送信={_observer.WriteLogOnInput}");
    }

    // O Key
    private static void ToggleOscOutputDebugLogActive()
    {
        _sender.WriteLogOnSend = ! _sender.WriteLogOnSend;
        Console.WriteLine($"OSC送信のデバッグログのon/offを切り替えました: 現在のログ送信={_sender.WriteLogOnSend}");
    }
}
