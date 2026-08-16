using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace webNetWorkDriver.model
{
    public class TcpFetcher
    {
        public async Task<string> FetchAsync(string host, int port, string path)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(host, port);

            using var stream = client.GetStream();

            // Build a minimal raw HTTP/1.1 request
            string request =
                $"GET {path} HTTP/1.1\r\n" +
                $"Host: {host}\r\n" +
                "Connection: close\r\n" +
                "\r\n";

            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

            using var reader = new System.IO.StreamReader(stream, Encoding.UTF8);
            string response = await reader.ReadToEndAsync();

            return response;
        }
        // Extract just the HTML body (strip HTTP headers)
        public string GetHtmlBody(string rawResponse)
        {
            int headerEnd = rawResponse.IndexOf("\r\n\r\n");
            if (headerEnd < 0)
                return rawResponse;

            return rawResponse.Substring(headerEnd + 4);
        }

        // Extract CSS from <style> tags
        public string ExtractCss(string htmlBody)
        {
            var styleRegex = new Regex(@"<style[^>]*>(.*?)</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var matches = styleRegex.Matches(htmlBody);

            if (matches.Count == 0)
                return "/* No inline CSS found */";

            var cssBuilder = new StringBuilder();
            foreach (Match match in matches)
            {
                cssBuilder.AppendLine(match.Groups[1].Value);
                cssBuilder.AppendLine();
            }

            return cssBuilder.ToString();
        }
    }
}
