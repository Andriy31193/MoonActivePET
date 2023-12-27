public interface IAuthUI
{
    void SetActiveAuthPanel(bool active);
    
    string GetInputText(AuthInputType inputType);
}
public enum AuthInputType
{
    Username,
    Password
}
