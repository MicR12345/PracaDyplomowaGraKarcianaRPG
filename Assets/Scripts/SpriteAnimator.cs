using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//https://stackoverflow.com/questions/37805594/unity-sprite-two-images-animation-at-runtime
public class SpriteAnimator : MonoBehaviour
{
    bool continueAnimation = false;
    bool continueAnimationOnce = false;

    SpriteRenderer displaySprite;
    List<Sprite> sprites;
    int currentFrame = 0;
    public string animationName = "";
    float normalAnimationSpeed = 1f;

    List<Sprite> playOnceSprites;
    public void setupSprites(SpriteRenderer _displaySprite, List<Sprite> _sprites, string _animationName,float _normalAnimationSpeed)
    {
        //Set where the animated sprite will be updated
        displaySprite = _displaySprite;
        sprites = _sprites;
        animationName = _animationName;
        normalAnimationSpeed = _normalAnimationSpeed;
    }
    public void PlayOnce(List<Sprite> _playOnceSprites, float _animationSpeedPlayOnce)
    {
        playOnceSprites = _playOnceSprites;
        currentFrame = 0;
        startAnimationOnce(_animationSpeedPlayOnce);
    }
    private IEnumerator startAnimationCRT()
    {
        continueAnimation = true;

        WaitForSeconds waitTime = new WaitForSeconds(normalAnimationSpeed);
        while (continueAnimation)
        {
            displaySprite.sprite = sprites[currentFrame++];
            if (currentFrame >= sprites.Count)
            {
                currentFrame = 0;
            }
            yield return waitTime;
        }

        continueAnimation = false;
    }
    private IEnumerator startOneOffAnimationCRT(float time)
    {
        continueAnimationOnce = true;
        WaitForSeconds waitTime = new WaitForSeconds(time);
        while (continueAnimationOnce)
        {
            displaySprite.sprite = playOnceSprites[currentFrame++];
            if (currentFrame >= playOnceSprites.Count)
            {
                currentFrame = 0;
                continueAnimationOnce = false;
                startAnimation();
            }


            yield return waitTime;
        }
        continueAnimationOnce = false;
    }
    public void startAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(startAnimationCRT());
    }
    public void startAnimationOnce(float time)
    {
        StopAllCoroutines();
        StartCoroutine(startOneOffAnimationCRT(time));
    }

    public void stopAnimation()
    {
        StopAllCoroutines();
    }
}