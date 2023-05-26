using System.Security.Cryptography;
public class Program
{
    static byte[]? permKey;
    static byte[]? permIV;

    public static void Main(string[] args)
    {
        //Set the console to white and text to black
        Console.BackgroundColor = ConsoleColor.White;
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Black;

        Console.WriteLine("Welcome to the Message Encryptor");
        PromptMessageEncryptor();
    }
    public static void PromptMessageEncryptor()   
    {
        Console.WriteLine("Would you want to Encrypt (Y) or Decrypt (N) a message? ( Y | N )");
        bool encryptOrDecrypt = YNQuestion();
        if (encryptOrDecrypt) TakeMessageToEncrypt(); else TakeEcryptionToDecrypt();
    }
    public static bool YNQuestion()
    {
        string userInput;
        do
        {
            userInput = Console.ReadLine() ?? string.Empty;
            userInput = userInput.ToUpper();
        }
        while (userInput != "Y" && userInput != "N");
        return userInput == "Y";
    }

    public static void TakeMessageToEncrypt()
    {
        Console.WriteLine("Please introduce a message to encrypt.");
        string userInput = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Would you want to provide your own (Y) encryption kwy and initialization vector? If not (N), a new one will be created. ( Y | N )");
        bool provideKeyAndIV = YNQuestion();
        if (provideKeyAndIV) FetchUserEncryptionKeyAndIV(); else GenerateNewEncryptionKeyAndIV();
        byte[] encrypted = Encrypt(userInput, permKey!, permIV!);

        string base64 = Convert.ToBase64String(encrypted);
        string keyBase64 = Convert.ToBase64String(permKey!);
        string ivBase64 = Convert.ToBase64String(permIV!);

        Console.Clear();
        Console.WriteLine("Your text has been encrypted.");
        Console.WriteLine($"Encrypted data: {base64}");
        Console.WriteLine($"Data was encrypted with:");
        Console.WriteLine($"Key: {keyBase64}");
        Console.WriteLine($"IV: {ivBase64}");
        Console.WriteLine("Press ANY KEY to start over, or press the ESC key to exit.");
        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
        Console.Clear();
        PromptMessageEncryptor();
    }

    public static void TakeEcryptionToDecrypt()
    {
        Console.WriteLine("Please introduce an encryption to decrypt.");
        string userInput = Console.ReadLine() ?? string.Empty;
        byte[] bytes = Convert.FromBase64String(userInput);
        Console.WriteLine("Please provide the Encryption Key and Initialization Vector.");
        FetchUserEncryptionKeyAndIV();      
        string decrypted = Decrypt(bytes, permKey!, permIV!);
        Console.Clear();
        Console.WriteLine("Your data has been decrypted.");
        Console.WriteLine($"Decrypted text: {decrypted}");
        Console.WriteLine("Press ANY KEY to start over, or press the ESC key to exit.");
        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.Escape)
        {
            Environment.Exit(0);
        }
        Console.Clear();
        PromptMessageEncryptor();
    }

    public static void GenerateNewEncryptionKeyAndIV()
    {
        Console.Clear();
        using (Aes aes = Aes.Create())
        {
            permKey = aes.Key;
            permIV = aes.IV;
        }
    }
    public static void FetchUserEncryptionKeyAndIV()
    {
        Console.Clear();
        permKey = null;
        string? userEK = string.Empty;
        permIV = null;
        string? userIV = string.Empty;

        do
        {
            if (permKey == null || userEK == string.Empty)
            {
                Console.WriteLine("Please introduce a valid Encryption key.");
                userEK = Console.ReadLine() ?? string.Empty;
                permKey = Convert.FromBase64String(userEK);
            }
            if (permIV == null || userIV == string.Empty)
            {
                Console.WriteLine("Please introduce a valid Encryption Initialization Vector.");
                userIV = Console.ReadLine() ?? string.Empty;
                permIV = Convert.FromBase64String(userIV);
            }
            Console.Clear();
            Console.WriteLine("Key or IV is not valid.");
        }
        while (permKey == null || userEK == string.Empty || permIV == null || userIV == string.Empty);
        Console.Clear();
    }

    static byte[] Encrypt(string simpletext, byte[] key, byte[] iV)
    {
        byte[] cipheredtext;
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iV);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(simpletext);
                    }
                    cipheredtext = memoryStream.ToArray();
                }
            }
        }
        return cipheredtext;
    }

    static string Decrypt(byte[] cipheredtext, byte[] key, byte[] iV)
    {
        string simpletext = String.Empty;
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iV);
            using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        simpletext = streamReader.ReadToEnd();
                    }
                }
            }
        }
        return simpletext;
    }


}