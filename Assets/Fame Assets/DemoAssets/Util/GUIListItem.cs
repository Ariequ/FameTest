using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GUIListItem //The class that contains our list items 
{
    public bool Selected = false;
    public string mName;

    public GUIListItem(bool mSelected, string mName)
    {
        Selected = mSelected;
        this.mName = mName;
    }
    public GUIListItem(string mName)
    {
        Selected = false;
        this.mName = mName;
    }

    public String Name()
    {
        return mName;
    }
    public void Enable()
    {
        Selected = true;
    }
    public void Disable()
    {
        Selected = false;
    }
}

