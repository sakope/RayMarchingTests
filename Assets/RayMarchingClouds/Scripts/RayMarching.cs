using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class RayMarching : MonoBehaviour
{
    [SerializeField] Camera      renderCam;
    [SerializeField] Material    material;
    [SerializeField] CameraEvent camEvent;
    [SerializeField] bool        rayFromCamera;

    [SerializeField] Vector3 rayCameraPos     = Vector3.zero;
    [SerializeField] Vector3 rayCameraUp      = Vector3.up;
    [SerializeField] Vector3 rayCameraForward = Vector3.forward;

    [SerializeField] Texture2D noiseOffsetTex;

    [Header("Motion")]
    [SerializeField] bool    isMove;
    [SerializeField] Vector3 direction;
    [SerializeField] float   force;

    int camPosID, camForwardID, camUpID, camRightID, aspectID, fovID;

    void Awake()
    {
        camPosID     = Shader.PropertyToID("_CamPos");
        camForwardID = Shader.PropertyToID("_CamForward");
        camUpID      = Shader.PropertyToID("_CamUp");
        camRightID   = Shader.PropertyToID("_CamRight");
        aspectID     = Shader.PropertyToID("_AspectRatio");
        fovID        = Shader.PropertyToID("_Fov");

        Shader.SetGlobalTexture("_NoiseOffsets", this.noiseOffsetTex);

        int lowResRenderTarget = Shader.PropertyToID("_LowResRenderTarget");
        CommandBuffer cb = new CommandBuffer();
        cb.GetTemporaryRT(lowResRenderTarget, -1, -1, 0, FilterMode.Trilinear, RenderTextureFormat.ARGB32);
        cb.Blit(lowResRenderTarget, lowResRenderTarget, this.material);
        cb.Blit(lowResRenderTarget, BuiltinRenderTextureType.CameraTarget);
        cb.ReleaseTemporaryRT(lowResRenderTarget);
        renderCam.AddCommandBuffer(camEvent, cb);
    }

    void Update()
    {
        if(rayFromCamera)
        {
            material.SetVector(camPosID,     renderCam.transform.position);
            material.SetVector(camForwardID, renderCam.transform.forward);
            material.SetVector(camUpID,      renderCam.transform.up);
            material.SetVector(camRightID,   renderCam.transform.right);
        }
        else
        {
            material.SetVector(camPosID,     rayCameraPos);
            material.SetVector(camForwardID, rayCameraForward);
            material.SetVector(camUpID,      rayCameraUp);
            material.SetVector(camRightID,   Vector3.Cross(rayCameraUp, rayCameraForward));
        }

        Shader.SetGlobalFloat(aspectID, (float)Screen.width / (float)Screen.height);
        Shader.SetGlobalFloat(fovID, Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2f);

        if (isMove) rayCameraPos += direction * force;
    }
}
