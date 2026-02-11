using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    public static SpritesManager Instance { get; private set;}
    public Sprite[] spritesHair;
    public Sprite[] spritesEyes;
    public Sprite[] spritesNose;
    public Sprite[] spritesMouth;
    public Sprite[] spritesHead;
    public Sprite[] spritesTren;
    public Sprite[] spritesBody;
    public Sprite[] spritesVaiTrai;
    public Sprite[] spritesTayTrai;
    public Sprite[] spritesBanTayTrai;
    public Sprite[] spritesVaiPhai;
    public Sprite[] spritesTayPhai;
    public Sprite[] spritesBanTayPhai;
    public Sprite[] spritesBaChau;
    public Sprite[] spritesDuiTrai;
    public Sprite[] spritesChanTrai;
    public Sprite[] spritesBanChanTrai;
    public Sprite[] spritesDuiPhai;
    public Sprite[] spritesChanPhai;
    public Sprite[] spritesBanChanPhai;
    public Sprite[] spritesWeapon;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
