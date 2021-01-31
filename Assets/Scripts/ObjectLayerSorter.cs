using System.Collections.Generic;
using UnityEngine;

public class ObjectLayerSorter : MonoBehaviour
{
    //NOTE: I hate the way this list is implemented, but cant think of a simpler way to
    //get interface references in the inspector. (Lists not serializable)
    //This implementation can break if passed gameobject doesnt have Sortable Sprite Interface

    private List<ISortableSprite> sprites;

    [SerializeField] private GameObject sp1;
    [SerializeField] private GameObject sp2;
    [SerializeField] private GameObject sp3;
    [SerializeField] private GameObject sp4;
    [SerializeField] private GameObject sp5;


    public void Start()
    {
        sprites = new List<ISortableSprite>
        {
            sp1.GetComponent<ISortableSprite>(),
            sp2.GetComponent<ISortableSprite>(),
            sp3.GetComponent<ISortableSprite>(),
            sp4.GetComponent<ISortableSprite>(),
            sp5.GetComponent<ISortableSprite>()
        };
    }

    void Update()
    {
        sprites.Sort(SortByY);

        for(int i = 0; i < sprites.Count; i++)
        {
            sprites[i].SpriteRenderer.sortingOrder = i;
        }
    }

    static int SortByY(ISortableSprite p1, ISortableSprite p2)
    {
        float p1_y = p1.SortPosition;
        float p2_y = p2.SortPosition;

        return p2_y.CompareTo(p1_y);
    }
}
