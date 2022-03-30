using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadow : MonoBehaviour
{
    
    public Vector2 ShadowOffset;
    public Material ShadowMaterial;
    SpriteRenderer spriteRenderer;
    GameObject shadowGameobject;

    void Start() {
        // Renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create a new gameobject to be used as drop shadow
        shadowGameobject = new GameObject("Shadow");
        // Create a new SpriteRenderer for Shadow object
        SpriteRenderer shadowSpriteRenderer = shadowGameobject.AddComponent<SpriteRenderer>();

        // Set the shadow gameobject's sprite to the original sprite
        shadowSpriteRenderer.sprite = spriteRenderer.sprite;
        // Set the shadow gameobject's material to the shadow material we created
        shadowSpriteRenderer.material = ShadowMaterial;

        // Update the sorting layer of the shadow to always lie behind the sprite
        shadowSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    void LateUpdate() {
        // Update the position and rotation of the sprite's shadow with moving sprite
        shadowGameobject.transform.localPosition = transform.localPosition + (Vector3)ShadowOffset;
        shadowGameobject.transform.localRotation = transform.localRotation;
    }
}