using System.Net;
using System.Reflection;
using NAudio;
using NAudio.Wave;

public class LoopStream : WaveStream
{
    WaveStream sourceStream;

    /// <summary>
    /// Creates a new Loop stream
    /// </summary>
    /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
    /// or else we will not loop to the start again.</param>
    public LoopStream(WaveStream sourceStream)
    {
        this.sourceStream = sourceStream;
        this.EnableLooping = true;
    }

    /// <summary>
    /// Use this to turn looping on or off
    /// </summary>
    public bool EnableLooping { get; set; }

    /// <summary>
    /// Return source stream's wave format
    /// </summary>
    public override WaveFormat WaveFormat
    {
        get { return sourceStream.WaveFormat; }
    }

    /// <summary>
    /// LoopStream simply returns
    /// </summary>
    public override long Length
    {
        get { return sourceStream.Length; }
    }

    /// <summary>
    /// LoopStream simply passes on positioning to source stream
    /// </summary>
    public override long Position
    {
        get { return sourceStream.Position; }
        set { sourceStream.Position = value; }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            if (bytesRead == 0)
            {
                if (sourceStream.Position == 0 || !EnableLooping)
                {
                    // something wrong with the source stream
                    break;
                }
                // loop
                sourceStream.Position = 0;
            }
            totalBytesRead += bytesRead;
        }
        return totalBytesRead;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        Mp3FileReader? reader = null;

        try
        {
            reader = new Mp3FileReader(Assembly.GetEntryAssembly()!.GetManifestResourceStream("cat.cat.mp3"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            reader = new Mp3FileReader("cat.mp3");
        }

        LoopStream loopthingy = new LoopStream(reader);
        var waveout = new WaveOutEvent();
        waveout.Init(loopthingy);
        waveout.Play();

        while (true) {
            downloadCat().Wait();
        }
    }

    static async Task downloadCat()
    {
        Console.WriteLine("Obtaining cat image...");

        HttpClient client = new HttpClient();
        var image = await client.GetAsync("https://cataas.com/cat/cute");
        image.EnsureSuccessStatusCode();

        Random res = new Random();
        string str = "abcdefghijklmnopqrstuvwxyz".ToUpper();
        int size = 5;

        // Initializing the empty string
        string ran = "";

        for (int i = 0; i < size; i++)
        {

            // Selecting a index randomly
            int x = res.Next(26);

            // Appending the character at the 
            // index to the random string.
            ran = ran + str[x];
        }

        var filename = $"cat{ran}.png";

        var fs = new FileStream(filename, FileMode.CreateNew);
        await image.Content.CopyToAsync(fs);
        fs.Close();

        var thing = new System.Diagnostics.Process();

        thing.StartInfo.UseShellExecute = true;
        thing.StartInfo.FileName = filename;
        thing.Start();

        Thread.Sleep(1000);
        File.Delete(filename);
        Console.WriteLine("Cat image saved!");
    }
}