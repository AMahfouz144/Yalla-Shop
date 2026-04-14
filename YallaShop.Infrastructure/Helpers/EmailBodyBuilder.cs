using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YallaShop.Infrastructure.Services;

namespace YallaShop.Infrastructure.Helpers
{
    public static class EmailBodyBuilder
    {
        public static string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
        {
            // build path relative to the application's base directory so it works in development and after publish
            var basePath = Path.GetDirectoryName(typeof(EmailService).Assembly.Location);

            var templatePath = Path.Combine(basePath!, "Templates", $"{template}.html");

            Console.WriteLine($"Looking for email template at: {templatePath}");

            var streamReader = new StreamReader(templatePath);
            var body = streamReader.ReadToEnd();

            streamReader.Close();

            foreach (var item in templateModel)
                body = body.Replace(item.Key, item.Value);

            return body;
        }
    }
}
