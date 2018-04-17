# WPF ListView Edit Template

This sample shows how a row-based edit mode can be implemented for WPF ListViews. This can be useful when the ItemTemplate is very rich and contains a lot of elements like TextBoxes and ComboBoxes.

The sample defines a simple read-only template using only TextBlocks. Only the selected row will use the rich template, generating the input elements on the fly. If possible, the focus will be set according to the click location that selected the item so that the user doesn't need more clicks (see the comments in https://github.com/softawaregmbh/samples-wpf-listtemplate/blob/master/MainWindow.xaml.cs for more info).

Adjacent rows can be selected by using Ctrl + Up/Down.
