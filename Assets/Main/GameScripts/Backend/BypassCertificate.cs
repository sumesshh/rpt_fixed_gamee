using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always accept
        return true;
    }
}

