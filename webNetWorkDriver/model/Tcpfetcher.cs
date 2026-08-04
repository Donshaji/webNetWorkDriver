using System.Net.Sockets;
using System.Text;

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
    }
}
