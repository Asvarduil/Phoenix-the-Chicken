using System.Collections.Generic;
using SimpleJSON;

public abstract class SimpleJsonMapper<T> : IMapper<T>
{
    // Methods that fulfill the IMapper interface.
    public abstract List<T> Map(object rawSource);
    public abstract object UnMap(T sourceObject);

    // Technology-specific methods
    public abstract List<T> MapFromJson(JSONNode parsed);

    // Single-item conversions
    public abstract T ImportState(JSONClass node);
    public abstract JSONClass ExportState(T data);
}
