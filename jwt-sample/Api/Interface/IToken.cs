namespace jwt_sample.Api.Interface
{
    public interface IToken
    {
        string GetToken(string userid);
    }
}