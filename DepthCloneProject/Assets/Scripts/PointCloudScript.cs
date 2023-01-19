using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PointCloudScript : MonoBehaviour
{
    [SerializeField]
    ARCameraManager m_CameraManager;
    [SerializeField]
    AROcclusionManager m_OcclusionManager;
    [SerializeField]
    RawImage m_cameraView;
    [SerializeField]
    RawImage m_grayDepthView;
    [SerializeField]
    RawImage m_confidenceView;
    [SerializeField]
    Visualizer m_visualizer;

    [SerializeField]
    float near;
    [SerializeField]
    float far;

    Texture2D m_CameraTexture;
    Texture2D m_DepthTexture_Float;
    Texture2D m_DepthTexture_BGRA;
    Texture2D m_DepthConfidenceTexture_R8;
    Texture2D m_DepthConfidenceTexture_RGBA;

    Vector3[] vertices = null;
    Color[] colors = null;

    float cx, cy, fx, fy;
    bool isScanning = true;

    // Start is called before the first frame update
    void Start()
    {
        m_visualizer.transform.parent = m_CameraManager.transform;
    }

    void OnEnable(){
        // Register for frame received events
        if(m_CameraManager != null){
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDisable()
    {
        // Unregister for frame received events
        if(m_CameraManager != null){
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    unsafe void UpdateRawImage(Texture2D texture, XRCpuImage cpuImage, TextureFormat format){
        // Create a conversion params in vertical flip mode
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, format, XRCpuImage.Transformation.MirrorY);

        // Get the required size of the buffer
        var rawTextureData = texture.GetRawTextureData<byte>();

        // Make sure the buffer has enough size to get covnerted data(converted data should be same with orignal data)
        Debug.Assert(rawTextureData.Length == cpuImage.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat),
            "The Texture2D is not the same size as the converted data.");

        // Convert the image to the desired format
        cpuImage.Convert(conversionParams, rawTextureData);

        // Apply the texture data
        texture.Apply();
    }

    unsafe void UpdateCameraImage(){
        // Acquire the latest image
        if(!m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image)){
            return;
        }

        using (image){
            // Get the image format
            // The format is always RGBA32
            var format = TextureFormat.RGBA32;

            // Initialize the texture if required
            if(m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height){
                m_CameraTexture = new Texture2D(image.width, image.height, format, false);
            }

            // Get RawImage
            UpdateRawImage(m_CameraTexture, image, format);

            // Update the RawImage
            m_cameraView.texture = m_CameraTexture;
        }
    }

    void UpdateEnvironmentDepthImage(){
        if(!m_OcclusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image)){
            return;
        }

        using (image){
            // Get the image format
            if(m_DepthTexture_Float == null || m_DepthTexture_Float.width != image.width || m_DepthTexture_Float.height != image.height){
                m_DepthTexture_Float = new Texture2D(image.width, image.height, image.format.AsTextureFormat(), false);
            }
            
            // Get the image data
            if(m_DepthTexture_BGRA == null || m_DepthTexture_BGRA.width != image.width || m_DepthTexture_BGRA.height != image.height){
                m_DepthTexture_BGRA = new Texture2D(image.width, image.height, TextureFormat.BGRA32, false);
            }

            UpdateRawImage(m_DepthTexture_Float, image, image.format.AsTextureFormat());

            ConvertFloatToGrayScale(m_DepthTexture_Float, m_DepthTexture_BGRA);

            // Update the RawImage
            m_grayDepthView.texture = m_DepthTexture_BGRA;
        }
    }

    void UpdateEnvironmentConfidenceImage(){
        if(!m_OcclusionManager.TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage image)){
            return;
        }

        using (image){
            // Get the image format
            if(m_DepthConfidenceTexture_R8 == null || m_DepthConfidenceTexture_R8.width != image.width || m_DepthConfidenceTexture_R8.height != image.height){
                m_DepthConfidenceTexture_R8 = new Texture2D(image.width, image.height, TextureFormat.R8, false);
            }
            
            // Get the image data
            if(m_DepthConfidenceTexture_RGBA == null || m_DepthConfidenceTexture_RGBA.width != image.width || m_DepthConfidenceTexture_RGBA.height != image.height){
                m_DepthConfidenceTexture_RGBA = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
            }

            UpdateRawImage(m_DepthConfidenceTexture_R8, image, image.format.AsTextureFormat());

            ConvertR8ToConfidenceMap(m_DepthConfidenceTexture_R8, m_DepthConfidenceTexture_RGBA);
        }
    }

    void ConvertFloatToGrayScale(Texture2D txFloat, Texture2D txGray){
        int length = txGray.width * txGray.height;
        Color[] depthPixels = txFloat.GetPixels();
        Color[] colorPixels = txGray.GetPixels();

        for(int index = 0; index < length; index++){
            var value = (depthPixels[index].r - near) / (far - near);

            colorPixels[index].r = value;
            colorPixels[index].g = value;
            colorPixels[index].b = value;
            colorPixels[index].a = 1;
        }
        txGray.SetPixels(colorPixels);
        txGray.Apply();
    }
    
    void ConvertR8ToConfidenceMap(Texture2D txR8, Texture2D txRGBA){
        Color32[] r8 = txR8.GetPixels32();
        Color32[] rgba = txRGBA.GetPixels32();

        for(int i = 0; i < r8.Length; i++){
            switch(r8[i].r){    // 0: low, 1: medium, 2: high
                case 0:
                    rgba[i].r = 255;
                    rgba[i].g = 0;
                    rgba[i].b = 0;
                    rgba[i].a = 255;
                    break;
                case 1:
                    rgba[i].r = 0;
                    rgba[i].g = 255;
                    rgba[i].b = 0;
                    rgba[i].a = 255;
                    break;
                case 2:
                    rgba[i].r = 0;
                    rgba[i].g = 0;
                    rgba[i].b = 255;
                    rgba[i].a = 255;
                    break;
            }
            txRGBA.SetPixels32(rgba);
            txRGBA.Apply();
        }
    }

    void ReprojectPointCloud(){
        print("Depth: " + m_DepthTexture_Float.width + ", " + m_DepthTexture_Float.height);
        print("Color: " + m_CameraTexture.width + ", " + m_CameraTexture.height);
        int width_depth = m_DepthTexture_Float.width;
        int height_depth = m_DepthTexture_Float.height;
        int width_camera = m_CameraTexture.width;

        if(vertices == null || colors == null){ // Initialize
            vertices = new Vector3[width_depth * height_depth];
            colors = new Color[width_depth * height_depth];

            XRCameraIntrinsics intrinsic;   // Get camera intrinsics
            m_CameraManager.TryGetIntrinsics(out intrinsic);
            print("intrinsics: " + intrinsic);

            float ratio = (float) width_depth / (float) width_camera;
            // 초점 거리
            fx = intrinsic.focalLength.x * ratio;
            fy = intrinsic.focalLength.y * ratio;

            cx = intrinsic.principalPoint.x * ratio;
            cy = intrinsic.principalPoint.y * ratio;
        }

        Color[] depthPixels = m_DepthTexture_Float.GetPixels();

        int index_dst;
        float depth;
        for(int depth_y = 0; depth_y < height_depth; depth_y++){
            index_dst = depth_y * width_depth;
            for(int depth_x = 0; depth_x < width_depth; depth_x++){
                index_dst = depth_y * width_depth + depth_x;
                depth = depthPixels[index_dst].r;

                if(depth > 0.0f){
                    vertices[index_dst].x = -depth * (depth_x - cx) / fx;
                    vertices[index_dst].y = -depth * (depth_y - cy) / fy;
                    vertices[index_dst].z = depth;
                }
                else{
                    vertices[index_dst].x = 0;
                    vertices[index_dst].y = 0;
                    vertices[index_dst].z = -999;
                }
                index_dst++;
            }
        }

        m_visualizer.UpdateMeshInfo(vertices, colors);
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs){
        // Update the camera image
        UpdateCameraImage();

        // Update the depth image
        UpdateEnvironmentDepthImage();

        // Update the environment confidence image
        UpdateEnvironmentConfidenceImage();

        // Reproject the point cloud
        if(isScanning){
            ReprojectPointCloud();
        }
    }

    public void SwitchScanMode(bool flg){
        isScanning = flg;
        if(flg){
            m_visualizer.transform.parent = m_CameraManager.transform;
            m_visualizer.transform.localPosition = Vector3.zero;
            m_visualizer.transform.localRotation = Quaternion.identity;
        }else{
            m_visualizer.transform.parent = null;
        }
    }
}
