using UnityEngine;
using System.Collections;

public class DbhAnimationController : MonoBehaviour
{
    public int startIndex, endIndex;
    public int nameLength = 3;
    public float normalizedTime = 0f;
    public float timePerFrame = 0.1f;
    public string preName = "饺子下锅_00";
    public bool ifCycle = false;
    public int freeMemoryFrequency = 10;

    private SpriteRenderer thisSpriteRenderer;
    private int currentIndex;
    private int cycleCounter = 0;
    private int freeMemCounter = 0;
    private float timePreFrameCounter = 0;
    private bool lockAnimation = false;
    private bool processLock = false;  

    private void Start()
    {
        currentIndex = startIndex;

        gameObject.AddComponent<SpriteRenderer>();
        thisSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        thisSpriteRenderer.sortingOrder = 10;
    }

    private void Update()
    {
        if (!processLock && !lockAnimation)
        {
            timePreFrameCounter += Time.deltaTime;
            if (timePreFrameCounter >= timePerFrame)
            {
                timePreFrameCounter = 0;
                currentIndex++;
                //if(currentIndex == endIndex)
                //{
                //    StartCoroutine(WaitForSync());
                //}
                LoadAnimationFrame();
            }
        }
    }

    private string AnimationNameNumberToString(int number, int width = 3)
    {
        string targetString = number.ToString();
        int calNumber = number;
        int controlNumber = 1;
        for(int i = 0; i < width - 1; i++)
        {
            controlNumber *= 10;
        }
        
        for( ;calNumber < controlNumber; calNumber*=10)
        {
            targetString = "0" + targetString;
        }

        return targetString;
    }

    private void LoadAnimationFrame()
    {
        if (!lockAnimation)
        {
            thisSpriteRenderer.sprite = Resources.Load<Sprite>("TestAnimation/" + preName + AnimationNameNumberToString(currentIndex, nameLength));
            freeMemCounter++;
            if (freeMemCounter >= freeMemoryFrequency)
            {
                freeMemCounter = 0;
                StartCoroutine(WaitForSync());
            }
        }
        
        if (!lockAnimation && currentIndex == endIndex)
        {
            lockAnimation = true;
        }

        if(ifCycle && lockAnimation)
        {
            cycleCounter++;
            currentIndex = startIndex;
            lockAnimation = false;
        }

        normalizedTime = cycleCounter + (currentIndex*1.0f - startIndex) / (endIndex - startIndex);
    }

    IEnumerator WaitForSync()
    {
        processLock = true;
        yield return Resources.UnloadUnusedAssets();
        processLock = false;
    }
}
