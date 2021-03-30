#region
using UnityEngine;
#endregion
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    //Sprites that will be used randomly in this spriteRenderer
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Generate();
    }

    private void Update()
    {

    }

    //Generate and assign one of the sprites randomly
    public void Generate()
    {
        if (sprites.Length > 0) spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
