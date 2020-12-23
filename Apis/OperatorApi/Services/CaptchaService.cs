using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.OperatorApi.Models.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models.Exceptions;

namespace TGS.Apis.OperatorApi.Services
{
    public interface ICaptchaService
    {
        Task<CaptchaInfo> CreateAsync(int charCount = 4, int width = 85, int height = 40);

        Task VerifyAsync(string answer, string captcha, int timeOut = 600);
    }

    public sealed class CaptchaService : ICaptchaService
    {
        private const string ContentType = "image/jpeg";
        private const string Secret = "XyfqwI9h";
        private static IList<char> _characters;

        private static readonly IList<string> FontNames = new[]
        {
            "Helvetica", "Arial", "Lucida Family", "Verdana", "Tahoma", "Trebuchet MS", "Georgia", "Times"
        };

        // use thread safe dictionary to prevent the brute-force attack from bots
        private static readonly ConcurrentDictionary<string, DateTime> Dic = new();

        private readonly ISymmetricCryptoProvider _cryptoProvider;

        public CaptchaService(ISymmetricCryptoProviderFactory cryptoProviderFactory)
        {
            _characters = new List<char>();

            // remove 0、o、O
            for (var c = '0'; c <= '9'; c++)
            {
                if (c == '0')
                {
                    continue;
                }

                _characters.Add(c);
            }

            for (var c = 'a'; c < 'z'; c++)
            {
                if (c == 'o')
                {
                    continue;
                }

                _characters.Add(c);
            }

            for (var c = 'A'; c < 'Z'; c++)
            {
                if (c == 'O')
                {
                    continue;
                }

                _characters.Add(c);
            }

            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.DES);
        }

        async Task<CaptchaInfo> ICaptchaService.CreateAsync(int charCount, int width, int height)
        {
            var random = new Random();
            var chars = Enumerable.Range(0, charCount)
                .Select(_ =>
                {
                    var r = random.Next(_characters.Count);
                    return _characters[r];
                }).ToArray();

            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);

            // fill background to white
            graphics.Clear(Color.White);

            DrawDisorderLinesAndPoints(random, bitmap, graphics, width / 2, height);

            // draw border
            var pen = new Pen(Color.Silver);
            graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);

            var x = 1;
            const int y = 5;

            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var color = Color.FromArgb(random.Next(100, 122), random.Next(100, 122), random.Next(100, 122));

            foreach (var c in chars)
            {
                var fontName = FontNames[random.Next(0, FontNames.Count - 1)];
                var font = new Font(fontName, random.Next(15, 20));

                // fade the color of font
                using var brush = new LinearGradientBrush(rectangle, color, color, 90f, true);
                brush.SetSigmaBellShape(0.5f);
                graphics.DrawString(c.ToString(), font, brush, x + random.Next(-2, 2), y + random.Next(-5, 5));
                x = x + width / charCount;
            }

            await using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Jpeg);

            var answer = await GetAnswerAsync(chars);

            return new()
            {
                Image = memoryStream.ToArray(),
                Captcha = answer,
                ContentType = ContentType
            };
        }

        async Task ICaptchaService.VerifyAsync(string answer, string captcha, int timeout)
        {
            if (answer.IsNullOrEmpty() || captcha.IsNullOrEmpty())
            {
                throw new InvalidCaptchaException("Required parameters of captcha are missing");
            }

            // a captcha can only be used once
            if (Dic.ContainsKey(captcha))
            {
                throw new InvalidCaptchaException("Invalid captcha is used");
            }

            Dic.TryAdd(captcha, DateTime.Now);

            // clear old captcha stored data
            foreach (var d in Dic)
            {
                var day = (d.Value - DateTime.Now).Days;
                if (day > 1)
                {
                    Dic.TryRemove(d.Key, out _);
                }
            }

            var value = await _cryptoProvider.DecryptStringAsync(captcha, Secret, Secret);
            var temp = value.Split('|');
            if (!DateTime.TryParse(temp[1], out var dateTime))
            {
                throw new InvalidCaptchaException("Failed to decrypt the captcha");
            }

            var sec = (DateTime.Now - dateTime).TotalSeconds;

            if (sec > timeout)
            {
                throw new InvalidCaptchaException("The captcha is out of date");
            }

            var answerToVerify = temp[0];
            if (answerToVerify.IgnoreCaseNotEquals(answer))
            {
                throw new InvalidCaptchaException("The answer is incorrect");
            }
        }

        private Task<string> GetAnswerAsync(char[] chars)
        {
            var answer = chars.JoinAsString(string.Empty);
            return _cryptoProvider.EncryptStringAsync($"{answer}|{DateTime.Now}", Secret, Secret);
        }

        /// <summary>
        /// Draw disorder lines and points
        /// </summary>
        private static void DrawDisorderLinesAndPoints(Random random, Bitmap bitmap, Graphics graphics, int lineCount,
            int pointCount)
        {
            var colors = new[]
            {
                Color.AliceBlue,
                Color.Azure,
                Color.CadetBlue,
                Color.Beige,
                Color.Chartreuse
            };

            // disorder lines
            for (var i = 0; i < lineCount; i++)
            {
                var x1 = random.Next(bitmap.Width);
                var x2 = random.Next(bitmap.Width);
                var y1 = random.Next(bitmap.Height);
                var y2 = random.Next(bitmap.Height);

                var pen = new Pen(colors[random.Next(0, colors.Length - 1)]);
                graphics.DrawLine(pen, x1, y1, x2, y2);
            }

            // disorder points
            for (var i = 0; i < pointCount; i++)
            {
                var x = random.Next(bitmap.Width);
                var y = random.Next(bitmap.Height);
                bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
        }
    }
}