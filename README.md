# MidiNoteToOsc

MIDIのノート入力とサステインペダルの情報をOSC(Open Sound Control)メッセージに変換して送信するコンソールアプリケーションです。

- Windowsでのみ動作します。作成者はWindows 11で動作を確認しています。
- [cluster](https://cluster.mu/)で音響信号をビジュアライズする目的で作成しています。
  - 詳細はnote記事も参照下さい。(※noteの公開後、URLを更新します)
- とくに、Releasesページに含まれるunitypackageは上記のnote記事に関連したファイルです。
  - このunitypackageに対応するアセットはレポジトリには直接含めていません。

## インストール

[MidiNoteToOsc_exe.zip](https://github.com/malaybaku/MidiNoteToOsc/releases/download/0.1/MidiNoteToOsc_exe.zip)

または、Releasesページで`MidiNoteToOsc_exe.zip`を選択し、ダウンロードして解凍します。

このとき、必要に応じて解凍前にzipファイルのプロパティウィンドウを開き、下部の「セキュリティ→許可する」をチェックして適用して下さい。この操作を行わないとexeファイルやdllファイルが正しく実行できない場合があります。

※ Releasesページで同じく公開している[MidiVisualizer_cluster_example.unitypackage](https://github.com/malaybaku/MidiNoteToOsc/releases/download/0.1/MidiVisualizer_cluster_example.unitypackage)はコンソールアプリ本体ではなく、コンソールアプリと組み合わせて使う想定のclusterワールドのパッケージファイルです。

## 使い方

起動: 

- `MidiToOsc.exe`を実行します。
- この時点でPCに接続済みのデバイスがあれば、それに接続します。

終了:

- Qキー、ENTERキーを順に押すとアプリケーションが終了します。

MIDIデバイスの切り替え:

1. Lキーを押すと、PCが認識しているMIDI入力デバイスの一覧が番号つきで表示されます。
2. その後、Sキーを押し、一覧に表示されたデバイス番号を指定してENTERで確定することで、MIDIデバイスを切り替えます。

デバッグ表示:

- Mキーを押すと、MIDIの入力を検出できているかどうかのデバッグ表示のオン/オフを切り替えます。この表示はデフォルトではオフになっています。
- Oキーを押すと、OSCのメッセージを出力できているかどうかのデバッグ表示のオン/オフを切り替えます。この表示はデフォルトではオフになっています。


## OSCメッセージの送信先を変更するには

OSCメッセージの送信先はデフォルトでは以下になっています。

- IPアドレス: `127.0.0.1`
- ポート: 9000
- OSCメッセージのアドレス: `/baxter/midi` (サステインペダルは `/baxter/midi/sustain`)

`MidiToOsc.exe`が含まれるディレクトリに下記のような `config.json` をテキストファイルとして作成すると、IPアドレス、ポート、OSCメッセージのアドレスを指定できます。

```
{
    "OscIpAddress": "192.168.0.12",
    "OscPort": 31984,
    "MessageAddress": "/myMessage/myAddr"
}
```

サステインペダルのOSCメッセージのアドレスは`/sustain`を追加したものになります。上記の設定の場合、`/myMessage/myAddr/sustain`になります。

## 作成者の開発環境

- Windows 11
- Microsoft Visual Studio Community 2022 Version 17.13.0

## ライセンス

- プログラム自体はpublic domainです。
- Releasesに含まれるunitypackageもpublic domainに基づきます。

プログラムの依存先ライブラリはいずれもMIT Licenseに基づいて公開されています。ライセンスは[License_Credit.md](./License_Credit.md)に記載しています。このファイルは実行ファイルのzipにも含まれています。

- [NAudio](https://github.com/naudio/NAudio)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Rug.Osc](https://bitbucket.org/rugcode/rug.osc/src/master/)
