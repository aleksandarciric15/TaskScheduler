   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	       GUI.NewTaskWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	     � Title$  Task Settings��5   	   $	��745��6   $	��661��6,   .��5	      � $	��723��6   $	��Top=�6    � RowDefinitions 5
   
   �� 5      $
��504* �6   �� 5      $
��152* �6   �� 5      $	��67* �6   .+�5   
   �� -   5         ��Name$ typeOfTasks��5      $
��Left=�6(   $��	35,48,0,0q�6C   $	��Top=�6V   $	��245��6n   $��35��6z   �� 5   
   $��Type of the task��6   $
��Left=�6+   $��
323,49,0,0q�6F   $	��Top=�6Z   $	��213��6r   $��18(�6~   $��34��6�   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ deadlineTime��5      $
��Left=�6+   $��
35,111,0,0q�6F   $	��Top=�6Z   $��31��6r   $	��245��6~   $��18(�6�   �� 5   
   $��Deadline��6   $
��Left=�6#   $��323,111,0,0q�6>   $	��Top=�6S   $��31��6k   $	��213��6w   $��18(�6�   �� 5   
   -   5         ��Name$ maxExecTime��5      $
��Left=�6'   $��
35,176,0,0q�6B    �� TextWrapping$
 Wrap=�6V   $	��Top=�6j   $	��245��6�   $��34��6�   $��18(�6�   �� 5   
   $ ��Maximum execution time [s]��6   $
��Left=�65   $��323,176,0,0q�6P   $	��Top=�6e   $��34��6}   $	��262��6�   $��18(�6�   �� 5   
   -   5        $ maxDegreeOfParallelism��5      $
��Left=�62   $��
35,240,0,0q�6M   $
 Wrap=�6a   $	��Top=�6u   $	��245��6�   $��37��6�   $��18(�6�   �� 5   
   $#��Maximum degree of parallelism��6   $
��Left=�68   $	��Top=�6S   $��323,240,0,0q�6k   $��37��6�   $	��271��6�   $��18(�6�   �� 5   
   -   5        $ taskPriority��5      $
��Left=�6)   $��
35,306,0,0q�6D   $	��Top=�6X   $	��245��6p   $��34��6|   $��18(�6�   �� 5   
   $��Task priority��6   $
��Left=�6(   $��323,305,0,0q�6C   $	��Top=�6X   $��34��6p   $	��247��6|   $��18(�6�   �� 5   
   $��Resources:
��6   $
��Left=�6/   $��
35,391,0,0q�6J   $	��Top=�6^   $��39��6v   $	��245��6�   $��18(�6�   �� 5   
   -   5        $��AddResource��5      $
��Left=�6'   $��323,391,0,0q�6B   $	��Top=�6W   $��39��6o   $	��173��6{   $��18(�6�   6�   �� 5   
   -   5         ��Name$ 
resourceLb��5      $��
0,435,0,90q�6Q   $��2��6e   �� 5   
   $��Output folder:��6   $
��Left=�6)   $��33��6D   $��	10,78,0,0q�6P   $	��Top=�6c   $	��177��6{   $��18(�6�   $��1��6�   �� 5   
   -   5        $��Add output folder��5      $
��Left=�6-   $��
323,78,0,0q�6H   $	��Top=�6\   $��33��6t   $	��173��6�   $��18(�6�   $��1��6�   �� 5   
   -	   5        $��Create taske��5      $
��Left=�6(   $��	458,3,0,0q�6C   $	��Top=�6V   $��42��6n   $	��178��6z   $��18(�6�   $��2��6�   6�   �� 5   
   -
   5         ��Name$ outputFolder��5      $�� ��6&   $
��Left=�61   $��
35,116,0,0q�6L   $��1��6`   $	��Top=�6m   $��31��6�   $	��559��6�   5"      5#      