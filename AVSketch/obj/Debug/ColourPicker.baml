   M S B A M L   `   `   `  ����C  ?AVSketch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:AVSketchAVSketch  5   	       AVSketch.ColourPicker   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:AVSketch     5   	     � Title$  ColourPicker��5   	   $	��350��6   $	��300��6+   .��5	      �  � RowDefinitions 5
   
   �� 5      �� 5      $��120px �6   .+�5   
   �� -   5         ��Name$ colour_shower��5      $��180px��6+   $��180px��69   6K   � 5   
   $��1��6    5      �� 5      $
��30px �6    �� 5      $
��30px �6    �� 5      $
��30px �6    �� 5      $
��30px �6     � ColumnDefinitions 5      �� 5      $
��40px �6#   �� 5      �� 5      $
��40px �6#   .+�5      �� $��0��6   $��0��6%   $��Center=�65   $��3q�6P   Red:6a   �� 5       $��1��6   $��0��6%   $��Center=�65   $��3q�6P   Green:6c   �� 5!      $��2��6   $��0��6%   $��Center=�65   $��3q�6P   Blue:6b   �� 5#      -   5         ��Name$ 
red_slider��5#      $��0��6)   $��1��66   $��Center=�6F    � Maximum$	 255Q�6a   6�   �� 5$      -   5        $ green_slider��5$      $��1��6+   $��1��68   $��Center=�6H   $	 255Q�6c   6�   �� 5%      -   5        $ blue_slider��5%      $��2��6*   $��1��67   $��Center=�6G   $	 255Q�6b   6�   �� 5'      -   5         ��Name$ red_text��5'      $��0��6(   $��2��65   $
��24px��6E   $	��2,2q�6S   6�   �� 5(      -   5        $ 
green_text��5(      $��1��6*   $��2��67   $
��24px��6G   $	��2,2q�6U   6�   �� 5)      -   5        $ 	blue_text��5)      $��2��6)   $��2��66   $
��24px��6F   $	��2,2q�6T   6�   �� 5+      -   5         ��Name$ close��5+      $��3��6$   $��1��61   $	��2,2q�6A   .��6b   OK5-      5.      50      51      