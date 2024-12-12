
string basePath = AppContext.BaseDirectory;
string rootPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\"));
string resourcePath = Path.Combine(rootPath, "resource");
string outputPath = Path.Combine(rootPath, "output");

FFmpegHelper ffmpegHelper = new FFmpegHelper(resourcePath);
Compressor compressor = new Compressor(ffmpegHelper);
string inputPath = Path.Combine(resourcePath, "test.mp4");
string outPath = Path.Combine(outputPath, "out.mp4");

var progress = new Progress<double>(precent =>
{
    Console.WriteLine($"压缩进度:{precent:F2}%");
});

bool compressResult = await compressor.CompressWithProgressAsync(inputPath, outPath, progress);

if (compressResult)
{
    Console.WriteLine("压缩完成");
}
