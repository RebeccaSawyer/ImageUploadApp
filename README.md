# ImageUploadApp

The app uploads images to Azure blob storage and detects if there are any people or faces using Microsoft Azure Computer Vision API. 
It sends a back a response with result displayed on the page.

To make it work, first you have to create an account for Azure Storage and select Blob storage as Account kind. 
Go to your resource and add your connection string to appsetings.json.

https://portal.azure.com/#create/Microsoft.StorageAccount-ARM

Next, create a Computer Vision resource and add COMPUTER_VISION_SUBSCRIPTION_KEY and COMPUTER_VISION_ENDPOINT strings to appsetings.json.

https://portal.azure.com/#create/Microsoft.CognitiveServicesComputerVision

Make sure you have the following NuGet packages in case they don't install as dependencies: 
Microsoft.Azure.CoginitiveServices.Vision.Computer Vision

WindowsAzure.Storage

In ImageUploadController you might need to add your own _blobContainerName string because you'll need to create a new container for yor blob storage.

Application uses .NET Core for back end and React for front end. It still has default pages, the new one added is Upload Images tab. No CSS was added.
The app doesn't upload multiple images at once. 
