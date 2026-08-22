1) implement basic http connect 
2) ping to ip
3) dns connection to ip
4) implement basic https connect

## HTTPS Implementation Plan & Explanation

### How HTTPS Works (The Theory)
HTTPS (Hypertext Transfer Protocol Secure) is an extension of standard HTTP that encrypts communication using Transport Layer Security (TLS) or its predecessor, Secure Sockets Layer (SSL). 

#### What is SSL?
Secure Sockets Layer (SSL) was the original cryptographic protocol designed by Netscape in the 1990s to provide communication security over a computer network. Though "SSL" is still the widely used colloquially (as in "SSL Certificate"), the actual SSL protocol versions (1.0, 2.0, 3.0) are now deprecated due to vulnerabilities. TLS (Transport Layer Security) is the modern, secure successor to SSL, though the industry still often refers to TLS connections as "SSL".

1. **TCP Handshake**: A standard TCP connection is established between the client and server, typically on port 443.
2. **SSL/TLS Handshake**: 
    - **ClientHello**: The client sends a message listing supported cipher suites and SSL/TLS versions.
    - **ServerHello**: The server responds with the chosen cipher suite and its digital certificate (containing the public key).
    - **Certificate Validation**: The client verifies the server's SSL certificate against trusted Root Certificate Authorities. This ensures the server is who it claims to be.
    - **Key Exchange**: Using the server's public key, the client securely sends a pre-master secret (or parameters for Diffie-Hellman key exchange). Both parties then generate symmetric session keys.
3. **Encrypted Data Transfer**: All subsequent HTTP traffic (GET/POST requests, headers, and body) is encrypted symmetrically using the session keys. This prevents eavesdropping and tampering.

### Implementation Steps (C#/.NET)
To implement HTTPS connections in a C# WPF application like this project, you can approach it at different levels of abstraction:

**Option A: High-Level (HttpClient)**
If you just need to make secure web requests:
- Use `System.Net.Http.HttpClient`.
- Simply pass HTTPS URLs (`https://...`). It handles TLS automatically.
- Optional: Configure `HttpClientHandler.ServerCertificateCustomValidationCallback` if you need to allow self-signed certificates.

**Option B: Low-Level (TcpClient & SslStream)**
If you are building a custom network driver and need raw packet-level control:
1. **Connect via TCP**: Use `TcpClient` to connect to the target IP/domain on port 443.
2. **Upgrade to TLS**: Wrap the raw `NetworkStream` in a `System.Net.Security.SslStream`.
3. **Authenticate**: Call `SslStream.AuthenticateAsClientAsync(targetHost)`. This triggers the TLS handshake.
4. **Certificate Validation**: Provide a `RemoteCertificateValidationCallback` to the `SslStream` constructor if you want to inspect certificates or bypass validation errors.
5. **Read/Write**: Send your raw HTTP request strings (e.g., `GET / HTTP/1.1...`) and read responses through the `SslStream`. The stream handles the encryption/decryption transparently.

### Action Items
- [ ] Decide on the abstraction level (HttpClient for standard requests, or SslStream for a custom driver).
- [ ] Add a UI toggle/setting to support port 443 and the HTTPS scheme.
- [ ] Write the connection logic (e.g., implementing `SslStream` wrap over an existing TCP connection).
- [ ] Add logic for handling certificate validation (e.g., warning the user if a certificate is invalid vs. dropping the connection).
- [ ] Write tests against known secure endpoints to verify the TLS handshake and data transfer.