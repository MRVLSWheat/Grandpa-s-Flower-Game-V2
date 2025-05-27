using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RubenInventiory : MonoBehaviour
{
    private List<Item> items = new();
    public UnityEvent<List<Item>> onInventoryUpdated;


    public void AddItem(Item item)
    {
        if (items.Contains(item))
            return;

        items.Add(item);
        onInventoryUpdated?.Invoke(items);
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public Item DropItem(Item item)
    {
        if (items.Contains(item))
            return default;

        var found = items.Find(i => i.name == item.name);
        items.Remove(found);
        onInventoryUpdated?.Invoke(items);
        return found;
    }

}

public class Item : MonoBehaviour
{ }