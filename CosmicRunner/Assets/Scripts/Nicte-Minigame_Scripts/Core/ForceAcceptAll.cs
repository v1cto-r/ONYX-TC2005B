using UnityEngine;
using UnityEngine.Networking;

namespace Nicte.Minigame{
public class ForceAcceptAll: CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) // Unity llama a este método cuando recibe el certificado del servidor.
    {
        return true; // Siempre devolvemos true, o sea: aceptamos cualquier certificado sin cuestionarlo.
    }
}
}