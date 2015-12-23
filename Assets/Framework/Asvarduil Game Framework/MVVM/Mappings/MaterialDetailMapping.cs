using System;
using System.Collections.Generic;
using SimpleJSON;

public class MaterialDetailMapping : SimpleJsonMapper<MaterialDetail>
{
    public override JSONClass ExportState(MaterialDetail data)
    {
        throw new NotImplementedException();
    }

    public override MaterialDetail ImportState(JSONClass node)
    {
        throw new NotImplementedException();
    }

    public override List<MaterialDetail> Map(object rawSource)
    {
        throw new NotImplementedException();
    }

    public override List<MaterialDetail> MapFromJson(JSONNode parsed)
    {
        throw new NotImplementedException();
    }

    public override object UnMap(MaterialDetail sourceObject)
    {
        throw new NotImplementedException();
    }
}
