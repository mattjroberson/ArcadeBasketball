using UnityEngine;

public interface ISortableSprite
{
    SpriteRenderer SpriteRenderer { get; }

    float SortPosition { get;  }
}
