using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class WavUtility_Helper
{
    const int HEADER_SIZE = 44;

    public static byte[] FromAudioClip(AudioClip clip, out int length, bool trim = false)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        if (trim)
            samples = TrimSilence(samples, 0.01f);

        var bytesData = ConvertAudioClipDataToInt16ByteArray(samples);
        var bytes = ConvertByteToWav(bytesData, clip.channels, clip.frequency);
        length = bytes.Length;
        return bytes;
    }

    private static float[] TrimSilence(float[] samples, float min)
    {
        var start = 0;
        var end = samples.Length - 1;

        while (start < samples.Length && Mathf.Abs(samples[start]) < min) start++;
        while (end > start && Mathf.Abs(samples[end]) < min) end--;

        var len = end - start + 1;
        var trimmed = new float[len];
        Array.Copy(samples, start, trimmed, 0, len);
        return trimmed;
    }

    private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        var dataStream = new MemoryStream();

        const int x = sizeof(short);

        foreach (var t in data)
        {
            var value = (short)(t * short.MaxValue);
            var bytes = BitConverter.GetBytes(value);
            dataStream.Write(bytes, 0, x);
        }

        var bytesData = dataStream.ToArray();

        dataStream.Dispose();

        return bytesData;
    }

    private static byte[] ConvertByteToWav(byte[] src, int channels, int sampleRate)
    {
        var stream = new MemoryStream();

        var fileSize = HEADER_SIZE + src.Length;

        // RIFF header
        stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes(fileSize - 8), 0, 4);
        stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

        // fmt subchunk
        stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
        stream.Write(BitConverter.GetBytes(16), 0, 4); // Subchunk1Size
        stream.Write(BitConverter.GetBytes((ushort)1), 0, 2); // AudioFormat
        stream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);
        stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
        stream.Write(BitConverter.GetBytes(sampleRate * channels * 2), 0, 4); // ByteRate
        stream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2); // BlockAlign
        stream.Write(BitConverter.GetBytes((ushort)16), 0, 2); // BitsPerSample

        // data subchunk
        stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(src.Length), 0, 4);
        stream.Write(src, 0, src.Length);

        return stream.ToArray();
    }
}
