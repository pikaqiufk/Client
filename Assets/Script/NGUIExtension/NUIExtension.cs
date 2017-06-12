
using System.Collections.Generic;
using UnityEngine;

public static class UITableExtension
{
	public static void AddChild(this UITable table, Transform t)
	{
		t.parent = table.gameObject.transform;
		table.repositionNow = true;
	}

	public static bool RemoveChild(this UITable table, Transform t)
	{
		List<Transform> list = table.GetChildListEx();

		if (list.Remove(t))
		{
			t.parent = null;
			GameObject.Destroy(t.gameObject);
			table.repositionNow = true;
			return true;
		}
		return false;
	}

	public static List<Transform> GetChildListEx(this UITable table)
	{
		Transform myTrans = table.transform;
		List<Transform> list = new List<Transform>();

		for (int i = 0; i < myTrans.childCount; ++i)
		{
			Transform t = myTrans.GetChild(i);
			if (!table.hideInactive || (t))
				list.Add(t);
		}

		// Sort the list using the desired sorting logic
		if (table.sorting != UITable.Sorting.None)
		{
			if (table.sorting == UITable.Sorting.Alphabetic) list.Sort(UIGrid.SortByName);
			else if (table.sorting == UITable.Sorting.Horizontal) list.Sort(UIGrid.SortHorizontal);
			else if (table.sorting == UITable.Sorting.Vertical) list.Sort(UIGrid.SortVertical);
			else if (table.onCustomSort != null) list.Sort(table.onCustomSort);
			//else table.Sort(list);
		}
		return list;
	}
}