using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface SelectableUnit
{
    bool IsSelected
    {
        get;
        set;
    }
    int ID
    {
        get;
    }
}
