using UnityEngine.Networking;

public class ForceAceptAll : CertificateHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
