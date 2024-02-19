using System.Security.Cryptography;
using System.Text;

namespace test_api_rest.Utils
{
    public class Utils
    {
        public static string GetSessionKeyBase64EncryptedWithPublicKey(string aesKeyAndIvBytesBase64, string publicKeyPem)
        {
            string sessionKeyBase64EncryptedWithPublicKey = "";

            try
            {
                // Convertir la clave pública PEM a RSAParameters
                //RSAParameters rsaParams = ;

                // Encriptar la clave AES y el vector IV (en formato Base64) con la clave pública
                using (RSA rsa = GetPublicKeyFromPEM(publicKeyPem))
                {
                    //rsa.ImportParameters(rsaParams);
                    byte[] encryptedAesKeyAndIv = rsa.Encrypt(Encoding.UTF8.GetBytes(aesKeyAndIvBytesBase64), RSAEncryptionPadding.Pkcs1);

                    sessionKeyBase64EncryptedWithPublicKey = Convert.ToBase64String(encryptedAesKeyAndIv);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones aquí, si es necesario
                Console.WriteLine("Error: " + ex.Message);
            }

            return sessionKeyBase64EncryptedWithPublicKey;
        }


        private static RSA GetPublicKeyFromPEM(string publicKeyPem)
        {
            try
            {
                // Eliminar los encabezados y pies de página del PEM
                publicKeyPem = publicKeyPem.Replace("-----BEGIN PUBLIC KEY-----", "")
                                            .Replace("-----END PUBLIC KEY-----", "")
                                            .Replace("\r", "").Replace("\n", "");

                // Convertir la clave pública de Base64 a byte[]
                byte[] publicKeyBytes = Convert.FromBase64String(publicKeyPem);

                // Crear una instancia de RSA para importar la clave pública
                RSA rsa = RSA.Create();

                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _); // Importa la clave pública
                return rsa; // Retorna la instancia de RSA con la clave pública importada

            }
            catch (Exception ex)
            {
                // Manejo de excepciones aquí, si es necesario
                Console.WriteLine($"Error al obtener la clave pública: {ex.Message}");
            }
            return null;
        }

        public static string EncryptAes256WithIvBase64(string plainText, string aesKeyAndIvBytesBase64)
        {
            string cipherTextBase64 = "";

            try
            {
                // Extraer la clave AES y el vector de inicialización de aesKeyAndIvBytesBase64
                byte[] aesKeyAndIv = Convert.FromBase64String(aesKeyAndIvBytesBase64);
                byte[] aesKey = new byte[32];
                byte[] iv = new byte[16];
                Array.Copy(aesKeyAndIv, 0, aesKey, 0, 32);
                Array.Copy(aesKeyAndIv, 32, iv, 0, 16);

                // Encriptar el plainText con la clave AES
                using (Aes aes = Aes.Create())
                {
                    aes.Key = aesKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7; // PKCS7 es equivalente a PKCS5Padding en Java

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                        cipherTextBase64 = Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones aquí, si es necesario
                Console.WriteLine("Error: " + ex.Message);
            }

            return cipherTextBase64;
        }

        public static string DecryptAes256WithIvBase64(string cipherTextBase64, string aesKeyAndIvBytesBase64)
        {
            string plainText = "";
            try
            {
                // Decodificar la clave AES y el vector de inicialización de aesKeyAndIvBytesBase64
                byte[] decryptedAesKeyAndIv = Convert.FromBase64String(aesKeyAndIvBytesBase64);
                byte[] decryptedAesKey = new byte[32];
                byte[] iv = new byte[16];
                Array.Copy(decryptedAesKeyAndIv, 0, decryptedAesKey, 0, 32);
                Array.Copy(decryptedAesKeyAndIv, 32, iv, 0, 16);

                // Crear la clave y el vector de inicialización para el cifrado AES
                using (Aes aes = Aes.Create())
                {
                    aes.Key = decryptedAesKey;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    // Desencriptar el cipherTextBase64 con la clave AES
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    byte[] encryptedPassword = Convert.FromBase64String(cipherTextBase64);
                    using (MemoryStream ms = new MemoryStream(encryptedPassword))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                plainText = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones aquí, si es necesario
            }
            return plainText;
        }


        public static string GetIvBytesBase64()
        {
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.GenerateIV(); // Generar el vector de inicialización (IV) aleatorio
                byte[] ivBytes = aes.IV;

                return Convert.ToBase64String(ivBytes);

            }
        }


        public static string GetAesKeyAndIvBytesBase64()
        {
            string aesKeyAndIvBytesBase64 = "";

            try
            {
                // Generar una clave AES 256
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = 256; // Establecer el tamaño de la clave a 256 bits
                    aes.GenerateKey(); // Generar la clave
                    byte[] aesKey = aes.Key;

                    aes.GenerateIV(); // Generar el vector de inicialización (IV) aleatorio
                    byte[] ivBytes = aes.IV;

                    // Combinar la clave AES y el vector de inicialización en un solo arreglo de bytes
                    byte[] aesKeyAndIvBytes = new byte[aesKey.Length + ivBytes.Length];
                    Array.Copy(aesKey, aesKeyAndIvBytes, aesKey.Length);
                    Array.Copy(ivBytes, 0, aesKeyAndIvBytes, aesKey.Length, ivBytes.Length);

                    // Convertir la clave AES con IV a formato Base64
                    aesKeyAndIvBytesBase64 = Convert.ToBase64String(aesKeyAndIvBytes);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones aquí, si es necesario
                Console.WriteLine("Error: " + ex.Message);
            }

            return aesKeyAndIvBytesBase64;
        }

        public static string GetAesKeyAndIvBytesBase64(string aesKeyAndIvBytesBase64, string ivBytesBase64)
        {
            string mergeAesKeyAndIvBytesBase64 = "";
            try
            {
                // Decodificar la clave AES y el IV desde base64 a bytes
                byte[] aesKeyAndIvBytes = Convert.FromBase64String(aesKeyAndIvBytesBase64);
                byte[] ivBytes = Convert.FromBase64String(ivBytesBase64);

                // Obtener la longitud de la clave AES (en este caso, 256 bits)
                int aesKeyLength = 256 / 8;

                // Extraer la clave AES original de los bytes
                byte[] originalAesKeyBytes = new byte[aesKeyLength];
                Array.Copy(aesKeyAndIvBytes, 0, originalAesKeyBytes, 0, aesKeyLength);

                // Crear la nueva clave combinando el nuevo IV y la clave AES original
                byte[] newAesKeyAndIvBytes = new byte[aesKeyLength + ivBytes.Length];
                Array.Copy(originalAesKeyBytes, 0, newAesKeyAndIvBytes, 0, aesKeyLength);
                Array.Copy(ivBytes, 0, newAesKeyAndIvBytes, aesKeyLength, ivBytes.Length);

                // Codificar los nuevos bytes en base64
                mergeAesKeyAndIvBytesBase64 = Convert.ToBase64String(newAesKeyAndIvBytes);
            }
            catch (Exception ex)
            {
                // Manejar excepciones adecuadamente aquí
                Console.WriteLine(ex.Message);
            }
            return mergeAesKeyAndIvBytesBase64;
        }


    }
}
