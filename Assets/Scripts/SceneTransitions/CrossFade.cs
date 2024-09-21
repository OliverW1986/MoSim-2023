using System.Collections;
using UnityEngine;

public class CrossFade : SceneTransition
{
    [SerializeField] private CanvasGroup crossFade;

    public override IEnumerator AnimateTransitionIn() 
    {
        // var tweener = crossFade.DOFade(1f, 0.3f);
        // yield return tweener.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
    }   

    public override IEnumerator AnimateTransitionOut() 
    {
        // var tweener = crossFade.DOFade(0f, 0.3f);
        // yield return tweener.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
    }  
}
