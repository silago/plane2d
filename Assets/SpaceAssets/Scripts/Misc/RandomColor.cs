#region
using UnityEngine;
#endregion
[RequireComponent(typeof(SpriteRenderer))]
public class RandomColor : MonoBehaviour
{
    public Color[] colors;
    //From 0 to 100, if 0, always visible 
    [Range(0.0f, 100.0f)]
    public int invisibleProbability = 30;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Generate();
    }

    private void Update()
    {

    }

    //Generate a new random color or hide the object
    public void Generate()
    {
        if (invisibleProbability > 0 && Random.Range(0, 100) < invisibleProbability)
        {
            spriteRenderer.color = Color.clear;
            return;
        }
        var colorSelected = Random.Range(0, colors.Length);
        if (colors.Length > 0) spriteRenderer.color = colors[colorSelected];
    }
}
