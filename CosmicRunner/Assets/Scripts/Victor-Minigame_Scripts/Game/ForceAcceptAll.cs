using UnityEngine;
using UnityEngine.Networking;

namespace AB
{
    // Force accept del API, para no tener problemas con el self signed certificate
    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
