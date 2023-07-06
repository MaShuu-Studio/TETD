using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class UnitEditorSetImageIcon : MonoBehaviour
{
    [SerializeField] private UnitEditor editor;
    private Button button;
    private Image image;
    private RectTransform rect;

    public List<Sprite> Sprites { get { return sprites; } }
    private List<Sprite> sprites;
    private float spf = 0.1f;
    private IEnumerator coroutine;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(async () => await FindImage());
        rect = transform as RectTransform;
    }
    public void SetSpf(float spf)
    {
        if (spf < 0.001f) spf = 0.001f;
        this.spf = spf;
    }

    public void Clear()
    {
        if (sprites == null) sprites = new List<Sprite>();
        sprites.Clear();

        if (editor != null)
            editor.UpdatePortrait(null);

        if (coroutine != null)
            StopCoroutine(coroutine);

        image.sprite = null;
    }

    public void SetSprites(Sprite[] sprites)
    {
        if (this.sprites == null) this.sprites = new List<Sprite>();
        this.sprites.Clear();

        if (sprites != null)
        {
            foreach (var sprite in sprites)
            {
                this.sprites.Add(sprite);
            }
        }
        else
        {
            this.sprites.Add(null);
        }
        PlayAnimation();
    }

    private async Task FindImage()
    {
        List<Sprite> sprites = await Data.DataManager.FindSprites();
        if (sprites.Count == 0) return;

        if (editor != null)
            editor.UpdatePortrait(sprites[0]);

        this.sprites = sprites;
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        if (sprites == null || sprites.Count == 0) return;

        if (coroutine != null) 
            StopCoroutine(coroutine);

        if (sprites[0] == null)
        {
            image.sprite = null;
            return;
        }

        float width = (float)sprites[0].texture.width / sprites[0].texture.height * rect.sizeDelta.y;
        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

        coroutine = ChangeSprite();
        StartCoroutine(coroutine);
    }

    private IEnumerator ChangeSprite()
    {
        float time = 0;
        int index = 0;

        while (true)
        {
            image.sprite = sprites[index];
            while (time < spf)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time -= spf;
            index = (index + 1) % sprites.Count;
            yield return null;
        }
    }

    private void OnEnable()
    {
        PlayAnimation();
    }
}
