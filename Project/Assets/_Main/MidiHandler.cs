using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class MidiHandler : MonoBehaviour
{
    private const string OutputDeviceName = "Microsoft GS Wavetable Synth";

    private OutputDevice _outputDevice;
    private Playback _playback;

    //This is an ugly quick way to just choose index of file in StreamingAssets
    [Header("quick way to just choose index of file in StreamingAssets")]
    public Object midiFileObject;

    void Start()
    {
        //foreach (var path in Directory.GetFiles(Application.streamingAssetsPath))
        //{
        //    print($"is {midiFileObject.name} == {path}");
        //    if (midiFileObject.name.Contains(path))
        //    {
        //        var midiPath = Path.Combine(Application.streamingAssetsPath, path);
        //        var midiFile = MidiFile.Read(midiPath);
        //        print("Loading: " + midiPath);
        //        InitializeOutputDevice();
        //        InitializeFilePlayback(midiFile);
        //        StartPlayback();
        //    }
        //}

        var midiPath = Path.Combine(Application.streamingAssetsPath, midiFileObject.name + ".mid");
        var midiFile = MidiFile.Read(midiPath);
        print(midiFile.OriginalFormat);
        foreach (var trackChunk in midiFile.GetTrackChunks())
        {
            //trackChunk.
        }
        //midiFile.GetChannels().ToList()[0].
        print("Loading: " + midiPath);
        InitializeOutputDevice();
        InitializeFilePlayback(midiFile);
        StartPlayback();
    }

    private void InitializeOutputDevice()
    {
        Debug.Log($"Initializing output device [{OutputDeviceName}]...");

        var allOutputDevices = OutputDevice.GetAll();
        if (!allOutputDevices.Any(d => d.Name == OutputDeviceName))
        {
            var allDevicesList = string.Join(System.Environment.NewLine, allOutputDevices.Select(d => $"  {d.Name}"));
            Debug.Log($"There is no [{OutputDeviceName}] device presented in the system. Here the list of all device:{System.Environment.NewLine}{allDevicesList}");
            return;
        }

        _outputDevice = OutputDevice.GetByName(OutputDeviceName);
        Debug.Log($"Output device [{OutputDeviceName}] initialized.");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Releasing playback and device...");

        if (_playback != null)
        {
            _playback.NotesPlaybackStarted -= OnNotesPlaybackStarted;
            _playback.NotesPlaybackFinished -= OnNotesPlaybackFinished;
            _playback.Dispose();
        }

        if (_outputDevice != null)
            _outputDevice.Dispose();

        Debug.Log("Playback and device released.");
    }

    private void InitializeFilePlayback(MidiFile midiFile)
    {
        Debug.Log("Initializing playback...");

        _playback = midiFile.GetPlayback(_outputDevice);
        _playback.Loop = true;
        _playback.NotesPlaybackStarted += OnNotesPlaybackStarted;
        _playback.NotesPlaybackFinished += OnNotesPlaybackFinished;

        Debug.Log("Playback initialized.");
    }

    private void StartPlayback()
    {
        Debug.Log("Starting playback...");
        _playback.Start();
    }

    private void OnNotesPlaybackFinished(object sender, NotesEventArgs e)
    {
        LogNotes("Notes finished:", e);
    }

    private void OnNotesPlaybackStarted(object sender, NotesEventArgs e)
    {
        LogNotes("Notes started:", e);
    }

    private void LogNotes(string title, NotesEventArgs e)
    {
        var message = new StringBuilder()
            .AppendLine(title)
            .AppendLine(string.Join(System.Environment.NewLine, e.Notes.Select(n => $"  {n}")))
            .ToString();
        Debug.Log(message.Trim());
    }
}
