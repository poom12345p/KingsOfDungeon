using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.Video;
public class PressAny : MonoBehaviour {

    public GameObject uiManage;
    public GameObject videoPlayer;
   public VideoPlayer video;
    bool clicked = false;
    bool isVideoPlay=true;
    bool  canInput=true;
    
    

    private void Start() {
        clicked= false;
         video.loopPointReached += EndReached;
    }

	void Update () {

		 if (Input.anyKey && !clicked&&canInput)
        {
            if(isVideoPlay)
            {
                videoPlayer.SetActive(false);
                isVideoPlay=false;
                canInput=false;
                Invoke("delayKey",0.1f);
            }
            else 
            {
                uiManage.GetComponent<UIManager>().TapToStart();
                clicked = true;
            }
        }
        

	}

    public void delayKey()
    {
        canInput=true;
    }

       void EndReached(UnityEngine.Video.VideoPlayer vp)
      {
           videoPlayer.SetActive(false);
                isVideoPlay=false;
      }
}
