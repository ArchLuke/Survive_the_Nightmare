using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public bool doLight = true;
    public GameObject light;

    public AudioCollection thunder;
    
    void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
                light.SetActive(true);
                // float ran = Random.Range(1f, 4f);
                // bool doubleLight = (ran > 3f);
                // if (doubleLight)
                // {
                //     if (AudioManager.instance && doLight)
                //         AudioManager.instance.PlayOneShotSound( thunder.audioGroup,
                //             thunder.audioClip, transform.position,
                //             thunder.volume,
                //             thunder.spatialBlend,
                //             thunder.priority );
                //     yield return new WaitForSeconds(0.1f);
                //     light.SetActive(false);
                //     yield return new WaitForSeconds(0.1f);
                //     light.SetActive(true);
                //
                // }
                yield return new WaitForSeconds(Random.Range(0.2f, 1.2f));
                light.SetActive(false);
                yield return new WaitForSeconds(Random.Range(1f, 5f));
            
        }
            
        
    }
}