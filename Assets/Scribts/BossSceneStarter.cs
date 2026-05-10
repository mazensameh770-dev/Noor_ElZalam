using UnityEngine;

public class BossSceneStarter : MonoBehaviour
{
    void Start()
    {
        IntroCutscene intro =FindFirstObjectByType<IntroCutscene>();
        if(intro != null )
            intro.enabled = true;
    }
}
