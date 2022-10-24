using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Playbook;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlaybookImage : Element
{
    [SerializeField] Renderer _renderer;
    [SerializeField] Texture[] stockImages;

    [HideInInspector] public Texture image;

    Camera userCam;
    RoundedQuadMesh roundedQuad;
    RoundedCornersModifier rcm;

    public bool lockAspectRatio;

    private Material origMaterial;
    private IEnumerator lerpLoadingCircleCoroutine;
    private IEnumerator convertImageToBytesCoroutine;
    private IEnumerator blurCoroutineInst;
    private IEnumerator setFullSizeCoroutineInst;
    [SerializeField] private Material blurMaterial;

    private string _lowResURL, _highResURL;
    
    // Start is called before the first frame update

    public int imageId;


    protected override void Awake()
    {
        base.Awake();
        rcm = GetComponent<RoundedCornersModifier>();
    }

    private void Start()
    {

        UpdateTexture();
    }

    private void Update()
    {
        
    }

    public void ResetStyles()
    {
        //Reset rounded corners
        rcm.StepperOnValueChanged(0);
    }

    public void UpdateTexture()
    {
        //if (_lowResURL == null && _highResURL == null)
        //{
        //    imageId = Random.Range(0, stockImages.Length);
        //    SetRandImage(imageId);
        //}
    }

    public void SetRandImage(int i)
    {
        var _renderer = GetComponentInChildren<Renderer>();
        image = stockImages[i];
        _renderer.material.SetTexture("_BaseMap", image);
        _renderer.material.SetTexture("_EmissionMap", image);
    }

    public void SetImage(byte[] bytes)
    {
        var renderer = GetComponentInChildren<Renderer>();
        Texture2D i = new Texture2D(1, 1);
        i.LoadImage(bytes);
        image = i;
        _renderer.material.SetTexture("_BaseMap", image);
        _renderer.material.SetTexture("_EmissionMap", image);
        _renderer.material.SetColor("_EmissionColor", new Color(0.3f, 0.3f, 0.3f, 1f));
        Material m = _renderer.material;
        Instantiate(m);
        _renderer.material = m;
    }

    public void SetImage(Texture i)
    {
        var _renderer = GetComponentInChildren<Renderer>();
        image = i;
        _renderer.material.SetTexture("_BaseMap", image);
        _renderer.material.SetTexture("_EmissionMap", image);
        _renderer.material.SetColor("_EmissionColor", new Color(0.3f, 0.3f, 0.3f, 1f));
        Material m = _renderer.material;
        Instantiate(m);
        _renderer.material = m;
    }

    public void SaveImageUrl(string lowResURL, string highResURL)
    {
        _lowResURL = lowResURL;
        _highResURL = highResURL;
    }

    //public void SerializeImage(ImageData imageData)
    //{
    //    imageData.highResURL = _highResURL;
    //    imageData.lowResURL = _lowResURL;
    //}

    public float RoundedEdgeValue()
    {
        var r = GetComponentInChildren<RoundedCornersModifier>();
        if (r) return r.RoundedEdgeVal;

        return 0f;
    }

    //public void SetSlider(PlaybookStepper stepper)
    //{
    //    var r = GetComponentInChildren<RoundedCornersModifier>();
    //    if (r) r.SetStepper(stepper);
    //}

    //public void UnsetSlider()
    //{
    //    var r = GetComponentInChildren<RoundedCornersModifier>();
    //    if (r) r.UnsetStepper();
    //}

    Texture2D Decompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    private void OnDisable()
    {
        var ren = GetComponentInChildren<Renderer>();
        //CancelPreviousDownload(ren);
    }
   

    //public void SetFullSize(Renderer ren, UnsplashPhoto _photo, Texture mipMapTexture)
    //{
    //    setFullSizeCoroutineInst = SetFullSizeCoroutine(ren, _photo, mipMapTexture);
    //    StartCoroutine(setFullSizeCoroutineInst);
    //}
    
    //private IEnumerator SetFullSizeCoroutine(Renderer ren, UnsplashPhoto _photo, Texture mipMapTexture)
    //{
    //    var www = new WWW("http://google.com"); // TODO: change to another fast-loading URL with constant uptime
    //    yield return www;
    //    if (www.error != null) {
    //        if (_unsplashGalleryScript && _unsplashGalleryScript.enabled)
    //            _unsplashGalleryScript.EnableNoInternetGO();
    //    } else {
    //        //var prevImg = _currentImg.image;
    //        unloadedImageIcon.SetActive(false);
    //        SetImage(mipMapTexture);
    //        origMaterial = ren.material;
    //        ren.material = blurMaterial;
    //        ren.material.mainTexture = mipMapTexture;
        
    //        //_imageDownloader = new UnsplashDownloader();
    //        _imageDownloader.DownloadPhotoAsync(_photo, new Progress<float>(OnDownloadPlaybookImageProgress), UnsplashPhotoSize.Regular)
    //            .ContinueWith(t => {
    //                if(t.IsCanceled){
    //                    ren.material = origMaterial;
    //                    SetImage(mipMapTexture);
    //                    //ShowErrorLoading(true, "Loading Canceled");
    //                }else if(t.IsFaulted){
    //                    //ren.material = _currentImg.origMaterial;
    //                    //_currentImg.SetImage(prevImg);
    //                    //ShowErrorLoading(true, "ERROR");
    //                    //Debug.LogException(t.Exception);
    //                }else
    //                {
    //                    SaveImageUrl(_photo.urls.thumb, _photo.urls.regular);
    //                    blurCoroutineInst = LerpBlurToImageElement(blurMaterial, ren, t.Result);
    //                    StartCoroutine(blurCoroutineInst);
    //                    //DownloadRawImage(t.Result);
    //                    //ren.material = _currentImg.origMaterial;
    //                    //_currentImg.SetImage(t.Result);
    //                    //SetTexture(t.Result);
    //                    //ShowRawImage(true);
    //                }

    //                //ShowLoading(false);            
    //            }, TaskScheduler.FromCurrentSynchronizationContext()).LogExceptions();
    //    }
    //    setFullSizeCoroutineInst = null;
    //}
    
    IEnumerator LerpBlurToImageElement(Material blurMat, Renderer ren, Texture2D result)
    {
        var blurIntensityRef = "Vector1_1844C1E0";
        if (ren.material.HasProperty(Shader.PropertyToID(blurIntensityRef)))
        {
            var speed = 9f;
            var time = 0f;
            var startVal = blurMat.GetFloat(blurIntensityRef);
            var endVal = 0.1f;
            while (!Mathf.Approximately(ren.material.GetFloat(blurIntensityRef), endVal))
            {
                var value = Mathf.Lerp(startVal, endVal, speed * time);
                ren.material.SetFloat(blurIntensityRef, value);
                time += Time.deltaTime;
                yield return null;
            }

            ren.material = origMaterial;
            SetImage(result);
        }
        blurCoroutineInst = null;
    }
    
    //private void OnDownloadPlaybookImageProgress(float progress)
    //{
    //    if (!loadingProgressIndicatorCanvas || !loadingProgressIndicator || !loadingProgressIndicatorHolder) return;
    //    if (!loadingProgressIndicatorHolder.activeInHierarchy)
    //        ToggleLoadingProgressIndicatorHolder(true);

    //    if (progress <= 0)
    //        loadingProgressIndicator.fillAmount = 0;
        
    //    if (lerpLoadingCircleCoroutine != null)
    //    {
    //        StopCoroutine(lerpLoadingCircleCoroutine);
    //        lerpLoadingCircleCoroutine = null;
    //    }
    //    lerpLoadingCircleCoroutine = LerpLoadCircle(progress);
    //    StartCoroutine(lerpLoadingCircleCoroutine);
    //    //loadingProgressIndicator.fillAmount = progress;
    //}
    
    //private IEnumerator LerpLoadCircle(float lerpTo)
    //{
    //    var speed = lerpTo >= 1 ? 40f : 7f;
    //    var time = 0f;
    //    var startFillAmount = loadingProgressIndicator.fillAmount;
    //    while (!Mathf.Approximately(loadingProgressIndicator.fillAmount, lerpTo))
    //    {
    //        loadingProgressIndicator.fillAmount = Mathf.Lerp(startFillAmount, lerpTo, speed * time);
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    loadingProgressIndicator.fillAmount = lerpTo;
        
    //    //if (lerpTo >= 1)
    //    //    ToggleLoadingProgressIndicatorHolder(false);

    //    lerpLoadingCircleCoroutine = null;
    //}
   
}
