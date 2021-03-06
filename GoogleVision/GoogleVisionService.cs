﻿using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;

namespace GoogleVision
{
    public class GoogleVisionService
    {
        private readonly VisionService _visionService;
        private readonly int _maxResults;

        public GoogleVisionService(string serviceAccountKeyPath, int maxResults)
        {
            if (string.IsNullOrEmpty(serviceAccountKeyPath))
            {
                _visionService = GoogleAuthenticator.CreateDefaultAuthorizedClient();
            }
            else
            {
                _visionService = GoogleAuthenticator.CreateAuthorizedClient(serviceAccountKeyPath);
            }

            _maxResults = maxResults;
        }

        public Annotations SendRequest(string imagePath)
        {
            var imageArray = File.ReadAllBytes(imagePath);
            var imageContent = Convert.ToBase64String(imageArray);

            var responses = _visionService.Images.Annotate(new BatchAnnotateImagesRequest()
            {
                Requests = new[]
                {
                    new AnnotateImageRequest()
                    {
                        Features = new[]
                        {
                            new Feature()
                            {
                                Type = DetectionTypes.LabelDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.TextDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.LandmarkDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.LogoDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.FaceDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.SafeSearchDetection,
                                MaxResults = _maxResults
                            },
                            new Feature()
                            {
                                Type = DetectionTypes.ImageProperties,
                                MaxResults = _maxResults
                            },
                        },
                        Image = new Image() {Content = imageContent}
                    }
                }
            }).Execute();

            var labelAnnotations = new List<EntityAnnotation>();
            var textAnnotations = new List<EntityAnnotation>();
            var landmarkAnnotations = new List<EntityAnnotation>();
            var logoAnnotations = new List<EntityAnnotation>();
            var faceAnnotations = new List<FaceAnnotation>();
            var safeSearchAnnotation = new SafeSearchAnnotation();
            var imageProperties = new ImageProperties();
            var webDetection = new WebDetection();
            var cropHints = new CropHintsAnnotation();

            foreach (var response in responses.Responses)
            {
                if (response.LabelAnnotations != null)
                {
                    foreach (var labelAnnotation in response.LabelAnnotations)
                    {
                        labelAnnotations.Add(labelAnnotation);
                    }
                }
                
                if (response.LandmarkAnnotations != null)
                {
                    foreach (var landmarkAnnotation in response.LandmarkAnnotations)
                    {
                        landmarkAnnotations.Add(landmarkAnnotation);
                    }
                }
                
                if (response.LogoAnnotations != null)
                {
                    foreach (var logoAnnotation in response.LogoAnnotations)
                    {
                        logoAnnotations.Add(logoAnnotation);
                    }
                }
                
                if (response.TextAnnotations != null)
                {
                    foreach (var textAnnotation in response.TextAnnotations)
                    {
                        textAnnotations.Add(textAnnotation);
                    }
                }

                if (response.FaceAnnotations != null)
                {
                    foreach (var faceAnnotation in response.FaceAnnotations)
                    {
                        faceAnnotations.Add(faceAnnotation);
                    }
                }

                if (response.WebDetection != null)
                {
                    webDetection = response.WebDetection;
                }

                if (response.CropHintsAnnotation != null)
                {
                    cropHints = response.CropHintsAnnotation;
                }

                if (response.SafeSearchAnnotation != null)
                {
                    safeSearchAnnotation = response.SafeSearchAnnotation;
                }

                if (response.ImagePropertiesAnnotation != null)
                {
                    imageProperties = response.ImagePropertiesAnnotation;
                }
            }
            
            return new Annotations(labelAnnotations, textAnnotations, logoAnnotations, landmarkAnnotations, faceAnnotations, safeSearchAnnotation, imageProperties, webDetection, cropHints);
        }
    }
}
