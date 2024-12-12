using Xabe.FFmpeg;

public class Compressor
{
    private readonly FFmpegHelper _ffmpegHelper;

    public Compressor(FFmpegHelper ffmpegHelper)
    {
        _ffmpegHelper = ffmpegHelper ?? throw new ArgumentNullException(nameof(ffmpegHelper));
    }

    public async Task<bool> CompressAsync(string inputPath, string outputPath, int crf = 23)
    {
        try
        {
            var mediaInfo = await _ffmpegHelper.GetMediaInfoAsync(inputPath);

            IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault();
            IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            List<string> parames = new List<string>
            {
                $"-crf {crf}",
                "-preset medium"
            };

            var conversion = _ffmpegHelper.CreateConversionAsync(videoStream, audioStream, outputPath, parames);
            await conversion.Start();
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"视频压缩异常{ex.Message}");
        }
        return false;
    }

    public async Task<bool> CompressWithProgressAsync(string inputPath, string outputPath, IProgress<double> progress, int crf = 25)
    {
        try
        {
            var mediaInfo = await _ffmpegHelper.GetMediaInfoAsync(inputPath);

            IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault();
            IStream? audioStream = mediaInfo.AudioStreams.FirstOrDefault();

            List<string> parames = new List<string>
            {
                $"-crf {crf}",
                "-preset medium"
            };

            var conversion = _ffmpegHelper.CreateConversionAsync(videoStream, audioStream, outputPath, parames);
            conversion.OnProgress += (sender, args) =>
            {
                var percent = (double)args.Duration.Ticks / args.TotalLength.Ticks * 100;
                progress.Report(percent);
            };

            await conversion.Start();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"压缩进度异常: {ex.StackTrace}");
        }

        return false;


    }
}