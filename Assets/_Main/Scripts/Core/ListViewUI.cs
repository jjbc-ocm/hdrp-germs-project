using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListViewUI<S, T> : UI<T> 
    where S : UI<S> 
    where T : MonoBehaviour
{
    protected List<S> items;

    [SerializeField]
    private S prefabItem;

    [SerializeField]
    private Transform uiItemView;

    protected void DeleteItems()
    {
        if (items == null)
        {
            items = new List<S>();
        }

        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }

        items.Clear();
    }

    protected void RefreshItems<Data>(IEnumerable<Data> data, System.Action<S, Data> onBeforeRefresh, bool isInitialized = false)
    {
        if (items == null)
        {
            items = new List<S>();
        }

        var index = 0;

        foreach (var datum in data)
        {
            if (!isInitialized)
            {
                var newItem = Instantiate(prefabItem, uiItemView);

                newItem.RefreshUI((self) => onBeforeRefresh(self, datum));

                items.Add(newItem);
            }
            else
            {
                items[index].RefreshUI((self) => onBeforeRefresh(self, datum));
            }

            index++;
        }
    }
}
