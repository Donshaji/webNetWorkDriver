using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace webNetWorkDriver.model
{
    public class TcpFetcher
    {
        public async Task<string> FetchAsync(string host, int port, string path, bool isHttps = false, int timeoutSeconds = 10)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            using var client = new TcpClient();
            await client.ConnectAsync(host, port, cts.Token);
            using var stream = client.GetStream();

            Stream networkStream = stream;
            if (isHttps)
            {
                var sslStream = new System.Net.Security.SslStream(stream, false, 
                    (sender, certificate, chain, sslPolicyErrors) => true, null);
                await sslStream.AuthenticateAsClientAsync(host);
                networkStream = sslStream;
            }

            string request =
                $"GET {path} HTTP/1.1\r\n" +
                $"Host: {host}\r\n" +
                "Connection: close\r\n" +
                "\r\n";

            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            await networkStream.WriteAsync(requestBytes, cts.Token);

            using var reader = new System.IO.StreamReader(networkStream, Encoding.UTF8);
            string response = await reader.ReadToEndAsync(cts.Token);

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

        // Extract title
        public string ExtractTitle(string htmlBody)
        {
            var titleRegex = new Regex(@"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = titleRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value.Trim() : "No title found";
        }

        // Extract meta description
        public string ExtractMetaDescription(string htmlBody)
        {
            var metaRegex = new Regex(@"<meta\s+name=[""']?description[""']?\s+content=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var match = metaRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value : "No description found";
        }

        // Extract meta keywords
        public string ExtractMetaKeywords(string htmlBody)
        {
            var metaRegex = new Regex(@"<meta\s+name=[""']?keywords[""']?\s+content=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var match = metaRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value : "No keywords found";
        }

        // Extract charset
        public string ExtractCharset(string htmlBody)
        {
            var charsetRegex = new Regex(@"<meta\s+charset=[""']?([\w-]+)[""']?", RegexOptions.IgnoreCase);
            var match = charsetRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value : "UTF-8 (default)";
        }

        // Extract viewport meta tag
        public string ExtractViewport(string htmlBody)
        {
            var viewportRegex = new Regex(@"<meta\s+name=[""']?viewport[""']?\s+content=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var match = viewportRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value : "No viewport defined";
        }

        // Extract all stylesheet links
        public string ExtractStylesheets(string htmlBody)
        {
            var linkRegex = new Regex(@"<link[^>]*rel=[""']?stylesheet[""']?[^>]*href=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var matches = linkRegex.Matches(htmlBody);

            if (matches.Count == 0)
                return "No external stylesheets found";

            var cssBuilder = new StringBuilder();
            foreach (Match match in matches)
            {
                cssBuilder.AppendLine(match.Groups[1].Value);
            }

            return cssBuilder.ToString();
        }

        // Extract all script sources
        public string ExtractScripts(string htmlBody)
        {
            var scriptRegex = new Regex(@"<script[^>]*src=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var matches = scriptRegex.Matches(htmlBody);

            if (matches.Count == 0)
                return "No external scripts found";

            var scriptBuilder = new StringBuilder();
            foreach (Match match in matches)
            {
                scriptBuilder.AppendLine(match.Groups[1].Value);
            }

            return scriptBuilder.ToString();
        }

        // Extract author meta tag
        public string ExtractAuthor(string htmlBody)
        {
            var authorRegex = new Regex(@"<meta\s+name=[""']?author[""']?\s+content=[""'](.*?)[""']", RegexOptions.IgnoreCase);
            var match = authorRegex.Match(htmlBody);

            return match.Success ? match.Groups[1].Value : "No author found";
        }

        // Extract all metadata in one formatted string
        public string ExtractAllMetadata(string htmlBody)
        {
            var metadataBuilder = new StringBuilder();

            metadataBuilder.AppendLine("=== PAGE METADATA ===");
            metadataBuilder.AppendLine($"Title: {ExtractTitle(htmlBody)}");
            metadataBuilder.AppendLine($"Description: {ExtractMetaDescription(htmlBody)}");
            metadataBuilder.AppendLine($"Keywords: {ExtractMetaKeywords(htmlBody)}");
            metadataBuilder.AppendLine($"Author: {ExtractAuthor(htmlBody)}");
            metadataBuilder.AppendLine($"Charset: {ExtractCharset(htmlBody)}");
            metadataBuilder.AppendLine($"Viewport: {ExtractViewport(htmlBody)}");
            metadataBuilder.AppendLine();

            metadataBuilder.AppendLine("=== EXTERNAL STYLESHEETS ===");
            metadataBuilder.AppendLine(ExtractStylesheets(htmlBody));
            metadataBuilder.AppendLine();

            metadataBuilder.AppendLine("=== EXTERNAL SCRIPTS ===");
            metadataBuilder.AppendLine(ExtractScripts(htmlBody));

            return metadataBuilder.ToString();
        }
    }
}