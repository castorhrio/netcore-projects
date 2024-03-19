using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

public class ThreeDES
{
    private static ThreeDES? _instance { get; set; }
    private static readonly object _lock = new object();
    private ThreeDES()
    {
    }

    public static ThreeDES GetTripleDESInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ThreeDES();
                }
            }
        }

        return _instance;
    }

    //3DES加密
    public string Encrypt(string data, string key)
    {
        try
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            //if key length is 16, convert to 24
            // byte[] allKeys = new byte[24];
            // Buffer.BlockCopy(keyBytes, 0, allKeys, 0, 16);
            // Buffer.BlockCopy(keyBytes, 0, allKeys, 16, 8);

            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = keyBytes;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = tripleDES.CreateEncryptor();
            byte[] resultBytes = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return ByteToHex(resultBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("des encrypt error: " + ex.Message);
        }
    }

    //3DES解密
    public string Decrypt(string data, string key)
    {
        try
        {
            byte[] dataBytes = HexToByte(data);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            //if key length is 16, convert to 24
            // byte[] allKeys = new byte[24];
            // Buffer.BlockCopy(keyBytes, 0, allKeys, 0, 16);
            // Buffer.BlockCopy(keyBytes, 0, allKeys, 16, 8);

            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = keyBytes;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = tripleDES.CreateDecryptor();
            byte[] resultBytes = transform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Encoding.UTF8.GetString(resultBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("des decrypt error: " + ex.Message);
        }
    }

    //字节数组转16进制字符串
    private string ByteToHex(byte[] data)
    {
        string result = string.Empty;
        if (data.Length > 0)
        {
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i].ToString("X2");
            }
        }

        return result;
    }

    private byte[] HexToByte(string data)
    {
        byte[] bytes = new byte[data.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
        }

        return bytes;
    }
}