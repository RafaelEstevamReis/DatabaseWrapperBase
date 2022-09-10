namespace Simple.DatabaseWrapper.Structures
{
    public interface IHashFunction
    {
        int ComputeHash(object o);
    }
}
