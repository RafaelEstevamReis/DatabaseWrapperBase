namespace Simple.DatabaseWrapper.Structures
{
    public interface IHashFunction
    {
        uint ComputeHash(object o);
    }
}
