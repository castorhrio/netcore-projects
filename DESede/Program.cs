namespace DESede;

class Program
{
    static void Main(string[] args)
    {
        ThreeDES des = ThreeDES.GetTripleDESInstance();

        string data = "Hello World";
        string key = "A314BA5A3C85E86KK887WSWS";

        Console.WriteLine($"加密字符串为{data}");
        var encryptStr = des.Encrypt(data, key);
        Console.WriteLine($"加密后结果为：{encryptStr}");

        var decryptStr = des.Decrypt(encryptStr, key);
        Console.WriteLine($"解密后字符串为{decryptStr}");
    }
}
