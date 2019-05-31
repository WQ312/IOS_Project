using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour {
    private MeshRenderer meshRender;
    private Renderer render;
	// Use this for initialization
	void Start () {

        meshRender = GetComponent<MeshRenderer>();
        render = GetComponent<Renderer>();
        meshRender.enabled = false;
	}
	
	
    public void ScaleUp()
    {
        meshRender.enabled = true;
        StartCoroutine(ScalingUp());
    }

    IEnumerator ScalingUp()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(4, 4, 1);
        Color startColor = render.material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        float t = 0;
        while (t < GameManager.Instance.boxScaleUpTime)
        {
            t += Time.deltaTime;
            float factor = t / GameManager.Instance.boxScaleUpTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, factor);
            render.material.color = Color.Lerp(startColor, endColor, factor);
            yield return null;
        }

        meshRender.enabled = false;
        transform.localScale = Vector3.one;
        render.material.color = startColor;
    }


}
