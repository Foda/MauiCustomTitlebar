# TitlebarSample

![image](https://github.com/Foda/MauiCustomTitlebar/assets/890772/c80e6a39-ec57-47d1-879e-ac233c139256)

## Overview

This is a sample of how to customize the titlebar in a .NET MAUI app for Windows. This example shows how to customize the titlebar by creating a custom titlebar content view that is displayed in the titlebar area (`TitlebarContent.xaml`).

The example titlebar control contains a title label, a search-box, and a avatar button. The titlebar is databound to a singleton instance of the `TitlebarViewModel`, and input to the search-box is shown on the main page.


## Details


### App.Windows.xaml.cs

This file contains the logic used to set the titlebar content view, the datacontext for that view, and logic to handle updating the titlebar hit-testing area


### TitlebarContent.xaml

This is the custom titlebar control. You can add any controls you want to this view, and use the `InputTransparent` property to make them non-interactive.


### Platforms/Windows/App.xaml
This file contains the XAML template used to provide the custom titlebar content (`MauiAppTitleBarTemplate`). This is where the color or height of the titlebar can be customized.


## Customize this sample

You can customize this sample by changing the `TitlebarContent` component. Any control in this component with the `InputTransparent` property set to `true` will not be interactive.
