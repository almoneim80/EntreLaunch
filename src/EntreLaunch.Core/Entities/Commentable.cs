namespace EntreLaunch.Entities;

public interface ICommentable
{
    public static string GetCommentableType(Type t)
    {
        return (string)t.GetMethod("GetCommentableType")!.Invoke(null, null)!;
    }

    public static abstract string GetCommentableType();
}
