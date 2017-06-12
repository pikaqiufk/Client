using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClientDataModel;
using DataTable;

public class CharacterIcosLogic : MonoBehaviour
{
    public UIGridSimple GridSimple;
	void Start ()
	{
	    var list = new List<CharacterIcoData>();
        Table.ForeachCharacterBase((record) =>
        {
            var data = new CharacterIcoData();
            data.CharacterId = record.Id;
            list.Add(data);
            return true;
        });
        ObservableCollection<CharacterIcoData> chartDatas = new ObservableCollection<CharacterIcoData>(list);
        GridSimple.Source = chartDatas;
	}

    void OnDestroy()
    {
    }
}
