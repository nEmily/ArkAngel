using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour {

    public GameObject targetDescr;
    public Image targetImage;
    public GameObject[] thumbnails;
    private PlayerStatus ps;

    public Item[] list;
    private Dictionary<string, Item> dict;
    private ArrayList inventory = new ArrayList();

    int index = 0;

    public void InitiateInv()
    {
        ps = GameObject.FindWithTag("PlayerStatus").GetComponent<PlayerStatus>();
        targetImage.gameObject.SetActive(false);
        targetDescr.GetComponent<Text>().text = null;
        foreach (GameObject t in thumbnails)
        {
            t.SetActive(false);
        }
        SetUpDict();
    }

    // Update is called once per frame
    void Update () {
		if (inventory.Count > 1)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                index = WrapIndex(index - 1);
                SelectItem(dict[(String)inventory[index]]);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                index = WrapIndex(index - 1);
                SelectItem(dict[(String)inventory[index]]);
            }
        } 
    }

    void SetUpDict()
    {
        dict = list.ToDictionary(item => item.name, value => value);
    }

    Item GetItem(string name)
    {
        return dict[name];
    }

    int WrapIndex(int i)
    {
        if (i < 0)
        {
            i = inventory.Count - 1;
        }

        if (i >= inventory.Count)
        {
            i = 0;
        }

        return i;
    }

    void SelectItem(Item item)
    {
        targetDescr.GetComponent<Text>().text = ps.ReplaceGender(ps.ReplaceName(item.description));
        targetImage.sprite = item.sprite;
    }

    public bool IsInInventory(string name)
    {
        return inventory.Contains(name);
    }

    public void AddToInventory(string name)
    {
        if (dict == null)
        {
            SetUpDict();
        }

        if (!IsInInventory(name))
        {
            inventory.Add(name);
            thumbnails[inventory.Count - 1].SetActive(true);
            thumbnails[inventory.Count - 1].GetComponent<Image>().sprite = GetItem(name).sprite;
            if (inventory.Count == 1)
            {
                targetImage.gameObject.SetActive(true);
                SelectItem(dict[(String)inventory[index]]);
            }
        }
    }

    public void RemoveFromInventory(string name)
    {
        if (dict != null && IsInInventory(name))
        {
            thumbnails[inventory.Count - 1].GetComponent<Image>().sprite = null;
            thumbnails[inventory.Count - 1].SetActive(false);
            inventory.Remove(name);
            if (inventory.Count == 0)
            {
                targetImage.sprite = null;
                targetDescr.GetComponent<Text>().text = null;
                targetImage.gameObject.SetActive(false);
            }
        } else
        {
            Debug.Log("Item is not in inventory.");
        }
    }

    [Serializable]
    public class Item
    {
        public string name;
        public Sprite sprite;
        [TextArea(1, 18)]
        public string description;

    }
}
