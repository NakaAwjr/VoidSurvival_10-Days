using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideColliderOnPlay : MonoBehaviour
{

    private TilemapRenderer tilemapRenderer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (tilemapRenderer != null ) tilemapRenderer.enabled = false;
        if (spriteRenderer != null ) spriteRenderer.enabled = false;
    }
}
