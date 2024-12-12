using System.Diagnostics;
using Xabe.FFmpeg;

public class FFmpegHelper
{
    public FFmpegHelper(string ffmpegPath)
    {
        if (string.IsNullOrEmpty(ffmpegPath))
        {
            throw new ArgumentNullException("ffmpeg程序路径不能为空", nameof(ffmpegPath));
        }

        bool checkResult = CheckFFmpegInstalltion(ffmpegPath);
        if (!checkResult)
        {
            throw new ArgumentNullException("ffmpeg程序信息异常");
        }
        FFmpeg.SetExecutablesPath(ffmpegPath);
    }

    public async Task<IMediaInfo> GetMediaInfoAsync(string inputPath)
    {
        if (string.IsNullOrEmpty(inputPath))
        {
            throw new ArgumentNullException("媒体文件路径不能为空", nameof(inputPath));
        }

        return await FFmpeg.GetMediaInfo(inputPath);
    }

    public IConversion CreateConversionAsync(IStream? videoStream, IStream? audioStream, string outputPath, List<string>? parames)
    {
        var conversion = FFmpeg.Conversions.New();

        if (videoStream != null)
        {
            conversion.AddStream(videoStream);
        }

        if (audioStream != null)
        {
            conversion.AddStream(audioStream);
        }

        if (parames != null && parames.Count != 0)
        {
            foreach (var pa in parames)
            {
                conversion.AddParameter(pa);
            }
        }

        if (!string.IsNullOrEmpty(outputPath))
        {
            conversion.SetOutput(outputPath);
        }

        return conversion;
    }

    private static bool CheckFFmpegInstalltion(string ffmpegPath)
    {
        try
        {
            var ffmpegExePath = Path.Combine(ffmpegPath, Environment.OSVersion.Platform == PlatformID.Win32NT ? "ffmpeg.exe" : "ffmpeg");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegExePath,
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ffmpeg程序信息异常:{ex.Message}");
        }

        return false;
    }
}