Requires Visual Studio 2015 to view/make changes to the files of the project 
https://www.visualstudio.com/downloads/


.xaml files = takes care of the appearance of a page
.xaml.cs = handles the functionality


Imported libraries:
Windows.Networking.Proximity
https://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.proximity



Referenced external libraries:
* NdefLibrary = easily parse and create standardized NDEF records for NFC tags
* SlidePane = Simple control to allow sliding between two panels (sliding menu)


Overview:
* MainPage.xaml 
   * The overall user interface
      * SlidePane = the sliding menu
      * ContentFrame = holds the content of the screen, will update its contents depending on what menu item is selected
* MainPage.xaml.cs
   * Handles menu button presses and page navigations
   * Updates ContentFrame with items from the Pages folder


* /Pages/ - Contains content to display
   * /Pages/ReadPage
   * /Pages/WritePage
   * /Page/Questionaire        currently broken because the url to the survey link has been removed from the salesforce sandbox


* /Styles/Custom.xaml 
   * Contains styling for the sliding menu buttons


* Globals.cs
   * Contains global variables