using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShadowScript : MonoBehaviour
{

    private SpriteRenderer sprite;
    private bool focus = false;

    private float lifetime;
    private float start_alpha = .3f;
    private float alpha_step = .5f;
    private float scale_step = .6f;
    private float smallest_scale = .4f;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        Color color = sprite.color;
        color.a = start_alpha;
        sprite.color = color;

        focus = true;
    }

    public void Focus(float lifetime)
    {
        this.lifetime = lifetime;
    }

    void FixedUpdate()
    {
        if (focus == true) {

            Color color = sprite.color;
            color.a += (alpha_step / lifetime) * Time.deltaTime;

            sprite.color = color;

            float scale = transform.localScale.x;
            scale -= (scale_step / lifetime) * Time.deltaTime;

            transform.localScale = new Vector3(scale, scale, transform.localScale.z);

            if (scale <= smallest_scale) {
                Destroy(gameObject);
            }
        }
    }
}
