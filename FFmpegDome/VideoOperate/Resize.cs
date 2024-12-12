using Xabe.FFmpeg;

public class Resize
{
    private readonly FFmpegHelper _ffmpegHelper;

    public Resize(FFmpegHelper ffmpegHelper)
    {
        _ffmpegHelper = ffmpegHelper;
    }

    public async Task<bool> ResizeAsync(string inputPath, string outputPath, int width, int height)
    {
        try
        {
            var mediaInfo = await Xabe.FFmpeg.FFmpeg.GetMediaInfo(inputPath);

            IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault()?.SetSize(width, height);
            IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            var conversion = await Xabe.FFmpeg.FFmpeg.Conversions.New().AddStream(videoStream).AddStream(audioStream).SetOutput(outputPath).Start();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"调整视频分辨率异常：{ex.Message}");
        }

        return false;
    }
}