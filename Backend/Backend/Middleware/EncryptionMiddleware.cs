using Backend.DataAbstraction.Security;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Middleware
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        public EncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRsaKeyProvider rsaKeyProvider)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/crypto") || path.StartsWith("/photo")))
            {
                await _next(context);
                return;
            }

            byte[] aesKey = null;

            // Decrypt incoming request
            if (context.Request.ContentLength != null && context.Request.ContentLength > 0 && context.Request.Method != "GET")
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var bodyText = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                try
                {
                    var encryptedReq = JsonSerializer.Deserialize<EncryptedRequestPayload>(bodyText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (encryptedReq != null && !string.IsNullOrEmpty(encryptedReq.EncryptedKey) && !string.IsNullOrEmpty(encryptedReq.Iv) && !string.IsNullOrEmpty(encryptedReq.Payload))
                    {
                        var rsa = rsaKeyProvider.GetRsa();
                        aesKey = rsa.Decrypt(Convert.FromBase64String(encryptedReq.EncryptedKey), RSAEncryptionPadding.OaepSHA256);
                        var iv = Convert.FromBase64String(encryptedReq.Iv);
                        var cipherBytes = Convert.FromBase64String(encryptedReq.Payload);

                        using var aes = Aes.Create();
                        aes.Key = aesKey;
                        aes.IV = iv;

                        using var decryptor = aes.CreateDecryptor();
                        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                        var decryptedStream = new MemoryStream(decryptedBytes);
                        context.Request.Body = decryptedStream;
                        context.Request.ContentLength = decryptedBytes.Length;
                    }
                }
                catch (Exception)
                {
                    // Fallback to normal processing if deserialization or decryption fails
                    context.Request.Body.Position = 0;
                }
            }

            // Capture response
            var originalResponseBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body = originalResponseBody;

            if (aesKey != null)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var plainResponseBytes = responseBody.ToArray();

                if (plainResponseBytes.Length > 0 && (context.Response.ContentType?.Contains("application/json") == true || context.Response.StatusCode >= 400))
                {
                    using var aes = Aes.Create();
                    aes.Key = aesKey;
                    aes.GenerateIV(); // Ensure a fresh IV for the response

                    using var encryptor = aes.CreateEncryptor();
                    var encryptedBytes = encryptor.TransformFinalBlock(plainResponseBytes, 0, plainResponseBytes.Length);

                    var encryptedResponse = new EncryptedResponsePayload
                    {
                        Iv = Convert.ToBase64String(aes.IV),
                        Payload = Convert.ToBase64String(encryptedBytes)
                    };

                    var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    var secureJson = JsonSerializer.Serialize(encryptedResponse, serializeOptions);

                    // Reset content-length and replace with secure payload
                    context.Response.ContentType = "application/json";
                    context.Response.ContentLength = null; // Let Kestrel set it based on the chunk/writes
                    await context.Response.WriteAsync(secureJson);
                    return;
                }
            }

            // If we did not encrypt, write the original back
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);
        }
    }
}
