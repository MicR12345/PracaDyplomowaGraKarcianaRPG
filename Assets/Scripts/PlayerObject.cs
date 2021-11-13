using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    Player player;
    Sprite playerSprite;

    SpriteRenderer spriteRenderer;
    Material spriteMaterial;

    GameManager gm;
    public PlayerObject(GameManager _gm,Player _player, Sprite _playerSprite,Material _spriteMaterial)
    {
        player = _player;
        playerSprite = _playerSprite;
        spriteMaterial = _spriteMaterial;

        gm = _gm;

        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSprite;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
