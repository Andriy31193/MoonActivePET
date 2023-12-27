using Unity.VisualScripting;

public class VerificationRequest
{
    public string request;
    public VerificationType verificationType;    

    public VerificationRequest(string r, VerificationType verification)
    {
        this.request = r;
        this.verificationType = verification;
    }
    public string GetRequest()
    {
        switch (verificationType)
        {
            case VerificationType.Username:
                return request;
            case VerificationType.Token:
                string correct_token = request.Contains(" ")? request.Split(' ')[1]:request;
                return "Bearer " + correct_token;
        }

        return request;
    }
}
public enum VerificationType
{
    Username,
    Token
}