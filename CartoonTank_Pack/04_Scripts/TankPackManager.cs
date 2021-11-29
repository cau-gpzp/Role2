using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankPackManager : MonoBehaviour {

    public Vector3 rotateSpeeds = new Vector3(0.0f, 0.0f, 0.0f);
    public Material TankBaseMT;
    public Material TankGunMT;
    public Material TankItemMT;
    public Material TankMarkerMT;

    public Texture[] TankBaseTextures;
    public Texture[] TankGunTextures;
    public Texture[] TankItemTextures;
    public Texture[] TankMarkerTextures;

    public GameObject TankModel;
    public GameObject Prop;
    public GameObject Bullet;
    public GameObject[] TankParts;
    public GameObject[] TankCannons;

    public GameObject ColorButton;
    public GameObject MarkButton;
    public GameObject PartsButton;

    public TextMesh TankCannonTypeText;
    private string[] TypeTextList = new string[7] { "Tank A", "Tank B", "Tank C", "Tank D", "Tank E", "Bullet", "Object" };

    private int NowTankColorIndex = 0;
    private int NowTankMarkerIndex = 0;
    private int NowTankCannonIndex = 0;
    private const int MaxSize = 5;

    private bool CheckAutoRotate;
    private bool CheckParts;
    private Quaternion originRot;

    void Awake()
    {
        TankBaseMT.SetTexture("_MainTex", TankBaseTextures[NowTankColorIndex]);
        TankGunMT.SetTexture("_MainTex", TankGunTextures[NowTankColorIndex]);
        TankItemMT.SetTexture("_MainTex", TankItemTextures[NowTankColorIndex]);
        TankMarkerMT.SetTexture("_MainTex", TankMarkerTextures[NowTankMarkerIndex]);
        CheckAutoRotate = true;
        CheckParts = false;
        originRot = transform.rotation;
        
    }

    void Update()
    {
        if(CheckAutoRotate)
            transform.Rotate(rotateSpeeds * Time.smoothDeltaTime);
    }

    public void NextColor()
    {
        NowTankColorIndex = (NowTankColorIndex + 1) % MaxSize;
        TankBaseMT.SetTexture("_MainTex", TankBaseTextures[NowTankColorIndex]);
        TankGunMT.SetTexture("_MainTex", TankGunTextures[NowTankColorIndex]);
        TankItemMT.SetTexture("_MainTex", TankItemTextures[NowTankColorIndex]);
    }

    public void NextMarker()
    {
        NowTankMarkerIndex = (NowTankMarkerIndex + 1) % MaxSize;
        TankMarkerMT.SetTexture("_MainTex", TankMarkerTextures[NowTankMarkerIndex]);
    }

    public void ToggleParts()
    {
        CheckParts = !CheckParts;
        foreach (GameObject go in TankParts)
        {
            go.SetActive(CheckParts);
        }
    }

    public void NextCannon()
    {
        if (NowTankCannonIndex == TypeTextList.Length - 1)
            return;

        if (NowTankCannonIndex < TankCannons.Length - 1)
        {
            if (!TankModel.activeSelf)
            {
                TankModel.SetActive(true);
                TankCannons[TankCannons.Length - 1].SetActive(false);
                TankCannons[NowTankCannonIndex].SetActive(true);
            }
            else 
            {
                TankCannons[NowTankCannonIndex].SetActive(false);
                TankCannons[++NowTankCannonIndex].SetActive(true);
            }
        }
        else if(NowTankCannonIndex < TypeTextList.Length)
        {
            if (NowTankCannonIndex == TankCannons.Length - 1)
            {
                TankModel.SetActive(false);
                Prop.SetActive(false);
                Bullet.SetActive(true);
                if (ColorButton.activeSelf)
                {
                    ColorButton.SetActive(false);
                    MarkButton.SetActive(false);
                    PartsButton.SetActive(false);
                }
            }
            else
            {
                Prop.SetActive(true);
                Bullet.SetActive(false);
            }
            NowTankCannonIndex++;
        }

        TankCannonTypeText.text = TypeTextList[NowTankCannonIndex];
    }

    public void PrevCannon()
    {
        if (NowTankCannonIndex < TankCannons.Length)
        {
            if (NowTankCannonIndex > 0)
            {
                TankCannons[NowTankCannonIndex].SetActive(false);
                TankCannons[--NowTankCannonIndex].SetActive(true);
            }
        }
        else if (NowTankCannonIndex < TypeTextList.Length)
        {
            if (NowTankCannonIndex == TypeTextList.Length - 1)
            {
                Prop.SetActive(false);
                Bullet.SetActive(true);
            }
            else
            {
                TankModel.SetActive(true);
                Prop.SetActive(true);
                Bullet.SetActive(false);
                if (!ColorButton.activeSelf)
                {
                    ColorButton.SetActive(true);
                    MarkButton.SetActive(true);
                    PartsButton.SetActive(true);
                }
            }
            NowTankCannonIndex--;
        }

        TankCannonTypeText.text = TypeTextList[NowTankCannonIndex];
        
        /*
        TankCannons[NowTankCannonIndex].SetActive(false);
        NowTankCannonIndex = (NowTankCannonIndex > 0) ? NowTankCannonIndex - 1 : TankCannons.Length - 1;
        TankCannons[NowTankCannonIndex].SetActive(true);
        TankCannonTypeText.text = TypeTextList[NowTankCannonIndex];
         * */
    }

    public void ToggleAutoRotate()
    {
        CheckAutoRotate = !CheckAutoRotate;
        if (!CheckAutoRotate)
            transform.rotation = originRot;
    }
}
