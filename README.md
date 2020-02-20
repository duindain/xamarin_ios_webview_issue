# xamarin_ios_webview_issue
Xamarin forms webview issue in loading runtime downloaded assets from the apps local folders

Example for showing iOS rendering a webview with explicit path files works in simulator and fails in real device
Works correctly in Android in Emulator and Physical devices

I tried multiple methods for adding assets including;
* embedding in manifest
* adding to app specific folders
* direct referencing from a folder within the apps library directory
* Adding base URL to the libraries folder
* Relative and exact paths


In the following two screenshots from Android the emulator and the physical device both can load fully defined paths, files from the assets folder and an image from an online source, they cannot load the relative file from the apps library folder or a manifest file

Android Emulator             |  Pixel 2 real device
:-------------------------:|:-------------------------:
<img src="https://github.com/duindain/xamarin_ios_webview_issue/blob/master/screenshots/Android%209%20Emulator.png" width="300" height="550"> | <img src="https://github.com/duindain/xamarin_ios_webview_issue/blob/master/screenshots/Real%20android%20-%20Pixel%202.png" width="300" height="550">

In the following two screenshots from iOS the Simulator can load fully defined paths, files from the resources folder and an image from an online source, the real iPad device cannot load the full defined path image but can load the others

iOS Simulator             |  iPad real device
:-------------------------:|:-------------------------:
<img src="https://github.com/duindain/xamarin_ios_webview_issue/blob/master/screenshots/iOS%20Simulator%20-%20iPhone%208%20-%202020-02-20%20at%2013.24.51.png" width="300" height="550"> | <img src="https://github.com/duindain/xamarin_ios_webview_issue/blob/master/screenshots/Real%20iOS%20iPad%20-%20%20Screenshot.png" width="300" height="550">


