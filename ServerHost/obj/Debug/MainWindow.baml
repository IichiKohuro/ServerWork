   M S B A M L   `   `   `  ����E  AServerHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ServerHost
ServerHost  5   	       ServerHost.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3ig$http://schemas.infragistics.com/xaml     5   	   *localclr-namespace:ServerHost     5   	   \ XHardcodet.Wpf.TaskbarNotification, Version=1.0.5.0, Culture=neutral, PublicKeyToken=null)tb http://www.hardcodet.net/taskbar  5   	   V RMaterialDesignThemes.Wpf, Version=2.3.2.1021, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	      MaterialDesignBody#���   5	   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5
   	      TextRenderingMode$
 Auto=�5   	   
������   MaterialDesignFont#���  5   	    � Title$ Сервер ПЦН��5   	   $	��450��5   	   $	��800��6    � WindowStartupLocation$ CenterScreen=�5   	    � Topmost .  � WindowState$ Normal=�5       � ShowInTaskbar .  � AllowsTransparency .  � WindowStyle$
 None=�5   #   	 � 	Resources	 5        back  	backBrush
 �� Color 	 back2  
backBrush2��%�   &	       &	      7  &	 7     &	 V     7  ��5   
   #0f110f��6&   �� 5   
   8
   6,   ��5   
   #243743��6'   �� 5   
   8
   6-   5      .��5!      �� -   5        $��15q�5!       �� CornerRadius$ 7��6     MaterialDesignPaper#��[ 5#       J� Effect 5&   
   3  -System.Windows.Media.Effects.DropShadowEffect  5'         	Direction$	 270Q�6      ShadowDepth$ 3Q�6/      
BlurRadius$ 12Q�6?      Color$ Black��6O      Opacity$ 1Q�6]   ._�5+   
   �  � ColumnDefinitions 5.      �� 5/      $	��250 �6#   �� 50      $��* �6#   .+�54      �� #��[ 6   $ 7 0 0 7��55       57        58      $	 360Q�6'   $ 2Q�67   $ 8Q�6G   $ 	LightGray��59   '   ._�5=      �  � RowDefinitions 5?      �� 5A      $
��Auto �6(   �� 5C      $��* �6(   �� 5E      $
��Auto �6(   .+�5I      ��  �� Orientation$ 
Horizontal=�6!   $��0��6:   $��Center=�5J   !   $��0 10 0 0q�6>   .+�5K      �� $%��/Images/Logo/Сервер 3.png��6    $	��160��5L       $	��160��6,   $��Center=�5M       $��Center=�6=   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Сервер ПЦН��5N       5P      �� 5S      $ Vertical=�6!   $��1��68   .+�5V      #  MaterialDesignThemes.Wpf.Card  $��	10 5 10 0q�6.   $��5 4 5 0q�6B   .��5X      �� $ Vertical=�6)   .+�5[   "   ��    MaterialDesignBody1TextBlock#��[ 6,   $#��Отправка заявок��5\   (   $��DemiBold"�6?   $��Center=�5]   (   $��0 5 5 0q�5^   (    " PrimaryHueLightForegroundBrush#���  5_   (   � 5b   "   $	��0 3q�6'    5e   &   �� 5f   *   $
��Auto �6;   �� 5g   *   $��* �6;   �� 5h   *   $
��Auto �6;   .+�5l   &   '  !MaterialDesignThemes.Wpf.PackIcon  $��16��6>   $��16��6I   $��Center=�5m   :   $	��0 5q�6U    � ToolTip$' !Состояние сервиса��5n   :   �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.2.1021, Culture=neutral, PublicKeyToken=null]]   Kind 5l   U   �� *ServiceState+5n   :   �� 5r   &    "	 MaterialDesignCaptionTextBlock#��� 	 60   $��Center=�5t   ,   $��1��6G   $��5 0 0 0q�6W   
�������$��Italic$�5u   >   ��5s   ,   �� *ServerStatus+ �� FallbackValue$ Запускается...��5u   >   )  #MaterialDesignThemes.Wpf.DialogHost  5x   &   $��2��6@   $��Center=�5y   )   $��Center=�6F      CloseOnClickAway .     DialogContent 5{   &   �� *LoadingContent+   IsOpen 5|   &   �� *IsDialogOpen+.��5~   *   �� $��Center=�61   $��Center=�6L   $��30��5   1   $��25��6=   $% Запустить сервис��5�   1    ^� 	Placement$ Right=�6L    
 MaterialDesignToolButton#��[
 5�   1   ��5�   1   �� *MailServiceDialogCommand+.��5�   .     $��16��6F   $��16��6Q   $��0q�6]    5�   B   �� *StateService+ �� 	Converter *   $ServerHost.StateServiceIconConverter  $	 Run��5�   +   5�   '   5�   #   5�      5�      5�      5�      5�      � 5�      $��1��6    5�      �� 5�      $
��Auto �6$   �� 5�      $��* �6$   .+�5�      � $��0 5 10 0q�6    5�      �� 5�      $��* �6+   �� 5�      $
��Auto �6+   �� 5�      $
��Auto �6+   �� 5�      $
��Auto �6+   �� 5�      $
��Auto �6+   .+�5�      '	  !MaterialDesignThemes.Wpf.PopupBox	  $��1��6.   $��0 5 15 5q�6>    	  PlacementMode$ BottomAndAlignRightEdges=�5�   +    	  	StaysOpen .    PrimaryHueMidBrush#���  5�   .     	  PopupContent.  5�      �� .+�5�      �� $��Hello World��6%   �� 5�      $��
Nice Popup��6%   �� 5�      $��Can't Touch This��6%   {�.  6@   �� 5�      �� 5�      $��Goodbye��6%   5�      5�      �� 5�      #��[
 6   $��2��5�   A   $+ %Свернуть окно в трей��5�      $��40��6<   ��5�      �� *MinimizeCommand+.��5�        $ TableRow=�62   
������5�   2   5�      �� 5�      #��[
 6   $��3��5�   A   $# Развернуть окно��5�      $��40��67   ��5�      �� *MaximizeCommand+.��5�        $ Maxcdn=�62   
����d�5�   2   5�      �� 5�      $��4��6>   $% Выключить сервер��5�      $��40��68   #��[
 5�      ��5�      �� *CloseCommand+.��5�        -   5        $��	closeIcon��5�   E   $ CloseCircle=�62     SecondaryAccentBrush#��[ 5�   2   5�      5�      5�      5�      '
  !MaterialDesignThemes.Wpf.Snackbar
  5�      $��2��6&   $��Stretch=�5�      ! 
  IsActive! 5�      �� *ShowCloseDialog+" 
  Message." 5�      .  (MaterialDesignThemes.Wpf.SnackbarMessage  #   ActionContent$# OK��5�   1   $;��5Вы уверены? Выключить сервер?��5�   1   $   ActionCommand$ 5�   1   �� *CloseCommand+5�   1   5�      5�      5�      5�      