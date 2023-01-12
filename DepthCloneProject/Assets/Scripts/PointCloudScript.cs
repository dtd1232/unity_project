using System.Reflection.Metadata;
using System.Reflection;
using System.Net.Mime;
using System.Diagnostics;
using System.Globalization;
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

    Texture2D m_cameraTexture;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){
        // Register for frame received events
        if(m_CameraManager != null){
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void void OnDisable()
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
        var raw TextureData = texture.GetRawTextureData<byte>();

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
            vat format = TextureFormat.RGBA32;

            // Initialize the texture if required
            if(m_cameraTexture == null || m_cameraTexture.width != image.width || m_cameraTexture.height != image.height){
                m_cameraTexture = new Texture2D(image.width, image.height, format, false);
            }

            // Get RawImage
            UpdateRawImage(m_CameraTexture, image, format);

            // Update the RawImage
            m_cameraView.texture = m_cameraTexture;
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
            var value = 
        }
    }
}
