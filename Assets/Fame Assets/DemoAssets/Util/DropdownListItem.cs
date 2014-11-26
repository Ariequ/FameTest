using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DropDownListItem {
	public int SelectedListItem = -1;
	public bool DropdownVisible = false; 
	public List<GUIListItem> list = new List<GUIListItem>();
	public Vector2 ScrollPos;
	public string displayText;

	public DropDownListItem (string displayText) {
		this.displayText = displayText;
	}

	public void Add(GUIListItem item){
		list.Add(item);
	}

	public void Clear(){
		list.Clear();
		SelectedListItem = -1;
		DropdownVisible = false;
	}

	public String GetSelected(){
		if(SelectedListItem == -1){
			return "None";
		} else {
			return list[SelectedListItem].Name();
		}
	}
}
