# Cognitive Services Scenarios #
A collection of scenarios using the [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services/en-us/)

## ImageDescription ##

The ImageDescription application for Windows 10 can be used to search for images and describe their contents with speech. The app can be deployed on a desktop PC, Tablet or Windows 10 IoT device.

### Setup ###

To get started, follow these steps:

1. Clone the repository and open the ImageDescription solution in Visual Studio. 
2. Restore the Nuget package for Microsoft.ProjectOxford.Vision

The app works as follows:

- search for images on the web by typing a query and pressing enter or clicking the "Search" button. 
- Afterwards, 4 random images will appear 
- By clicking one of the images, the Vision API is triggered and returns the description of the image
- Afterwards, the text is spoken in English with a female voice, which can be changed in the code behind

## Authors ##

### Daniel Heinze ###
- Technical Evangelist @ Microsoft
- Twitter: [@starlord_daniel](https://twitter.com/starlord_daniel)
- Facebook: [danielsdevblog](https://www.facebook.com/danielsdevblog)
- LinkedIn: [LinkedIn](https://de.linkedin.com/in/daniel-heinze)
- Mail: [Mail](mailto:daniel.heinze@microsoft.com)