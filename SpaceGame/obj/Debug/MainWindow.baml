   M S B A M L   `   `   `  ����D  @SpaceGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:SpaceGame	SpaceGame  5   	       SpaceGame.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:SpaceGame     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��7.5* �6   �� 5      $
��2.5* �6   .+�5   
   �� -   5        $��
GameCanvas��5      
���   �$��0��66   5      �� 5   
   $��1��6   
���   �
�������$��1q�6H   ._�5      � $��1��6   
���   � � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $��Enemies killed��6   $��24(�62   
��� � �$��10q�6S   $��0��6_    �� TextAlignment$ Center=�6l   �� 5      -   5         ��Name$ scoreEnemiesKilled��5      $��0��68   $��24(�6A   
��� � �$��10q�6b   $��1��6n   $ Center=�6{   �� 5      $��Level��6   $��24(�6)   
��� � �$��10q�6J   $��2��6V   $ Center=�6c   �� 5      -   5        $ LevelTextBlock��5      $��1��64   $��24(�6=   
��� � �$��10q�6^   $��3��6j   $ Center=�6w   5      5      5      5       