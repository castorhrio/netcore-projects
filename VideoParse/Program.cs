
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

Console.WriteLine("请输入视频地址");
var videoUrl = Console.ReadLine();

if (string.IsNullOrEmpty(videoUrl))
{
    Console.WriteLine("视频地址不能为空!");
    return;
}

// 提取嵌入资源到临时目录
var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "VideoDownloader");
if (!Directory.Exists(tempDir))
{
    Directory.CreateDirectory(tempDir);
}

var ytPath = ExtractResourceToFile("yt-dlp.exe", tempDir);
var ffmpegPath = ExtractResourceToFile("ffmpeg.exe", tempDir);

var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Download");
if (!Directory.Exists(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

var outputTemplate = $"{outputDirectory}/%(title)s_%(upload_date)s.%(ext)s";

// var arguments = $"-o \"{outputTemplate}\" --ffmpeg-location \"{ffmpegPath}\" \"{videoUrl}\"";
// 下载参数：强制指定视频格式为 mp4
var argument = $"-o \"{outputTemplate}\" --ffmpeg-location \"{ffmpegPath}\" -f \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/best\" \"{videoUrl}\"";

try
{
    Console.WriteLine("开始解析并下载视频...");

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = ytPath,
            Arguments = argument,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        }
    };

    process.OutputDataReceived += (sender, e) => HandleProgress(e.Data); // 处理标准输出
    process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data); // 打印错误

    process.Start();

    // 开始异步读取输出
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    // string output = await process.StandardOutput.ReadToEndAsync();
    // string error = await process.StandardError.ReadToEndAsync();

    process.WaitForExit();
    if (process.ExitCode == 0)
    {
        Console.WriteLine("视频下载完成");
        OpenFolder(outputDirectory);
    }
    else
    {
        Console.WriteLine("视频下载失败");
    }

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"视频解析异常:{ex.Message}");
}


static string ExtractResourceToFile(string resourceName, string outPutDic)
{
    var assembly = Assembly.GetExecutingAssembly();
    var resourcePath = $"VideoParse.resource.{resourceName}";

    var outPutPath = Path.Combine(outPutDic, resourceName);
    if (!File.Exists(outPutPath))
    {
        using var resourceStream = assembly.GetManifestResourceStream(resourcePath);
        if (resourceStream == null)
        {
            throw new Exception($"资源 {resourceName} 未找到");
        }

        using var fileStream = new FileStream(outPutPath, FileMode.Create, FileAccess.Write);
        resourceStream.CopyTo(fileStream);
    }

    return outPutPath;
}

static void HandleProgress(string? data)
{
    if (string.IsNullOrEmpty(data))
        return;

    // 解析下载进度的正则表达式
    var progressRegex = new Regex(@"\[download\]\s+(\d+\.\d+)%");
    var match = progressRegex.Match(data);

    if (match.Success)
    {
        // 获取当前进度百分比
        var progress = match.Groups[1].Value;
        Console.Write($"\r下载进度：{progress}%");
    }
}

static void OpenFolder(string path)
{
    try
    {
        if (OperatingSystem.IsWindows())
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = path,
                UseShellExecute = true
            });
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", path);
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("xdg-open", path);
        }
        else
        {
            Console.WriteLine("无法识别的操作系统，无法打开目录");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"打开目录失败：{e.Message}");
    }
}