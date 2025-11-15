namespace EntreLaunch.DTOs;

// import result class
public class ImportResult
{
    public int Added { get; set; }

    public int EntreLaunchdated { get; set; }

    public int Failed { get; set; }

    public int Skipped { get; set; }

    public List<ImportError>? Errors { get; set; }
    public List<string>? Messages { get; set; }

    public void AddError(int row, string message)
    {
        Failed++;
        Errors ??= new List<ImportError>();

        Errors.Add(new ImportError
        {
            Row = row,
            Message = message,
        });
    }

    public void AddMessage(string message)
    {
        Messages ??= new List<string>();
        Messages.Add(message);
    }
}

// import error class
public class ImportError
{
    public int Row { get; set; }

    public string Message { get; set; } = string.Empty;
}

// related objects map
public class TypedRelatedObjectsMap : Dictionary<Type, RelatedObjectsMap>
{
}

// related objects map
public class RelatedObjectsMap : Dictionary<string, Dictionary<object, BaseEntityWithId?>>
{
    public List<string> IdentifierPropertyNames { get; set; } = new List<string>();

    public List<string> SurrogateKeyPropertyNames { get; set; } = new List<string>();

    public List<SurrogateForeignKeyAttribute> SurrogateKeyPropertyAttributes { get; set; } = new List<SurrogateForeignKeyAttribute>();
}

// type identifiers 
public class TypeIdentifiers : Dictionary<Type, IdentifierValues>
{
}

// identifier values 
public class IdentifierValues : Dictionary<string, List<object>>
{
    public List<string> IdentifierPropertyNames { get; set; } = new List<string>();

    public List<string> SurrogateKeyPropertyNames { get; set; } = new List<string>();

    public List<SurrogateForeignKeyAttribute> SurrogateKeyPropertyAttributes { get; set; } = new List<SurrogateForeignKeyAttribute>();
}
