using System;
using System.Management.Automation;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TraceSANCertificate
{
    [Cmdlet(VerbsDiagnostic.Trace, "SANCertificate")]
    [OutputType(typeof(SANCertificate))]

    public class TraceSANCertificate: PSCmdlet
    {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Hostname or IP Address to connect to")]
        [ValidateNotNullOrEmpty]
        public new string Host { get; set; }

        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true, HelpMessage = "Specify the TCP Port for the source host")]
        [ValidateNotNullOrEmpty]
        public int Port { get; set; } = 443;

        [Parameter(HelpMessage = "Specify TCP Ports to try for each discovered SAN hostname")]
        [ValidateNotNullOrEmpty]
        public int[] TryPorts { get; set; } = { 443 };

        [Parameter(HelpMessage = "Specify Connection Timeout in milliseconds (ms)")]
        [ValidateNotNullOrEmpty]
        public int Timeout { get; set; } = 1000;

        protected override void ProcessRecord()
        {
            WriteVerbose("Connecting to host: " + Host + " TCP Port: " + Port);
            X509Certificate2 certificate = GetCertificateFromHost(Host, Port, Timeout);

            if (certificate == null)
                throw new Exception("Failed to retrieve a certificate from specified host");

            IEnumerable<string> sans = GetSubjectAlternativeName(certificate);
            WriteVerbose("Yielded " + sans.Count() + " Subject Alternative Names");

            sans = sans.Where(validated => ValidateHostname(validated) && validated != Host);
            WriteVerbose("Yielded " + sans.Count() + " valid hostnames");

            List<SANCertificate> sanCertificates = new List<SANCertificate>();

            foreach (string san in sans)
            {
                foreach (int port in TryPorts)
                {
                    WriteVerbose("Checking hostname: " + san + " Port: " + port);
                    X509Certificate2 sancert = GetCertificateFromHost(san, port, Timeout);

                    if (sancert != null)
                        sanCertificates.Add(new SANCertificate(certificate, sancert, san, port));
                }
            }

            WriteObject("Source Certificate Subject Name: " + certificate.Subject.Split('=')[1].Split(',').First() + " Thumbprint: " + certificate.Thumbprint);

            sanCertificates.ForEach(WriteObject);
        }

        private class SANCertificate
        {
            public string Hostname { get; private set; }
            public int Port { get; private set; }
            public string SubjectName { get; private set; }
            public string Thumbprint { get; private set; }
            public DateTime Expiry { get; private set; }
            public bool Match { get; private set; }

            public SANCertificate(X509Certificate2 sourceCert, X509Certificate2 cert, string hostname, int port)
            {
                Hostname = hostname;
                Port = port;
                SubjectName = cert.Subject.Split('=')[1].Split(',').First();
                Thumbprint = cert.Thumbprint;
                Expiry = cert.NotAfter;
                Match = (sourceCert.Thumbprint == cert.Thumbprint);
            }
        }

        private static X509Certificate2 GetCertificateFromHost(string host, int port, int timeout)
        {
            X509Certificate2 cert = null;
            try
            {
                TcpClient client = new TcpClient();

                client.ConnectAsync(host, port).Wait(timeout);

                SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCertificate));

                sslStream.ReadTimeout = timeout;

                sslStream.AuthenticateAsClient(host);

                cert = new X509Certificate2(sslStream.RemoteCertificate);

                client.Close();
                sslStream.Dispose();
                client.Close();

                return cert;
            }
            catch
            {
                return cert;
            }
        }

        private static bool ValidateCertificate(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static bool ValidateHostname(string hostname)
        {
            return Regex.Match(hostname, @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$").Success;
        }

        private static IEnumerable<string> GetSubjectAlternativeName(X509Certificate2 cert)
        {
            List<string> result = new List<string>();


            string subjectAlternativeName = cert.Extensions.Cast<X509Extension>()
                                                .Where(n => n.Oid.Value == "2.5.29.17") //n.Oid.FriendlyName=="Subject Alternative Name")
                                                .Select(n => new AsnEncodedData(n.Oid, n.RawData))
                                                .Select(n => n.Format(true))
                                                .FirstOrDefault();

            if (subjectAlternativeName != null)
            {
                string[] alternativeNameSplit = subjectAlternativeName.Split(new[] { "=", ":", ",", "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < alternativeNameSplit.Length / 2; i++)
                    result.Add(alternativeNameSplit[2 * i + 1]);
            }

            return result;
        }
    }
}