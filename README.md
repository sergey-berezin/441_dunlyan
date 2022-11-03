- [Lab work .NET technology in 7th semester](#lab-work-net-technology-in-7th-semester)
  - [Project1-Lab1](#project1-lab1)
    - [**Introduction**](#introduction)
    - [**Usage**](#usage)
  - [Project2-Lab2](#project2-lab2)
    - [**Introduction**](#introduction-1)
    - [**Usage**](#usage-1)
      - [**How to add assembly references**](#how-to-add-assembly-references)
  - [Project3-Lab3](#project3-lab3)
    - [**Introduction**](#introduction-2)
    - [**Usage**](#usage-2)
      - [**Persistent storage technology used**](#persistent-storage-technology-used)
      - [**How to install the necessary plugins for this project**](#how-to-install-the-necessary-plugins-for-this-project)
      - [**How to view tables in the database**](#how-to-view-tables-in-the-database)
      - [**Program example and some references**](#program-example-and-some-references)
# Lab work .NET technology in 7th semester


##  Project1-Lab1 


 
### **Introduction**
The first lab assignment for .Net in the 7-th semester
### **Usage**
Download code folder,open the folder **PROJECT1** in VScode. 

Create a new terminal, then enter the command line in the terminal.  

First need to compile the library.
```shell
dotnet build
```
Then go to the library folder.
```shell 
cd .\Lib_EmotionFerPlus\
``` 
Execute the package command.
```shell
dotnet pack
```
Return to previous directory.
```shell
cd ..
```
Go to application directory.
```shell
cd .\app\
```
Run this program.
```shell 
dotnet run
```




## Project2-Lab2 


### **Introduction**
The second lab assignment for .Net in the 7-th semester.
### **Usage**
Project 2 needs to use the package created by Project 1  
* **Environment**:
    * **OS : Windows 11**
    * **Software Environment : Visual Studio 2022**  
#### **How to add assembly references**

    
**1.**  
 Tools->Options...  


<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020172518.png" height=70% width=70% >  
 

 
<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020172518.png" height=70% width=70% >

**2.**  
 NuGet Package Manager->Package Sources.  
 Click the plus sign to add a new assembly reference.  
 

<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173040.png" height=70% width=70% >  

<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020173040.png" height=70% width=70% >  


**3.**  
 Click the ellipsis(...).  
 Then need find the directory where the file with the suffix nupkg generated by the command line ```dotnet pack``` in lab1 is located.  


<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173114.png" height=70% width=70% >  

<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020173114.png" height=70% width=70% >  


**4.**  
 Usually this file is located in the directory ``` \lib\bin\Debug\ ``` .  
 

<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173203.png" height=70% width=70% >  

<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020173203.png" height=70% width=70% >  



**5.**    
Click the update button.  


<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173238.png" height=70% width=70% >  

<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020173238.png" height=70% width=70% >  


**6.**  
After updating the assembly source, we can open the nuget package manager to install the packages that need to be added for the current project.  


<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173310.png" height=70% width=70% >  

<img src="https://github.com/YUAN-DL/Images/blob/master/Images/lenovo20221020173310.png" height=70% width=70% >  


**7.**  
Install.  


<img src="https://raw.githubusercontent.com/YUAN-DL/Images/master/Images/lenovo20221020173527.png" height=70% width=70% > 

## Project3-Lab3 
### **Introduction**
The third lab assignment for .Net in the 7-th semester.  

### **Usage**
Project-3 is a continuation of Project-2,added new functionality in project 3 to save the results of project 2 in permanent storage, and added some database operations in this project.
#### **Persistent storage technology used**
* Entity Framework Core  
  
#### **How to install the necessary plugins for this project**
* In Visual Studio  
  * Install the necessary plugins
    - **Tools > NuGet Package Manager > Package Manager Console**
      ```shell
      Install-Package Microsoft.EntityFrameworkCore.Sqlite 
      ```

      ```shell
      Install-Package Microsoft.EntityFrameworkCore.Design 
      ```

      ```shell
      Install-Package Microsoft.EntityFrameworkCore.Tools 
      ```
  * Create database  
    - **First, need to change the path to create the database in the datacontext.cs file to the local folder path**
        ```C#
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source=C:\\Users\\ydl74\\source\\repos\\Project3\\Project3\\ImageAnalysis.db");
        ```
    - **Then run the following commands in Package Manager Console (PMC)**
        ```shell
        Add-Migration InitialCreate
        ``` 

        ```shell
        Update-Database
        ```
    
####  **How to view tables in the database**
* In **Visual Studio Code**  
  * Install plugin **SQLite** (v0.14.1)   
####  **Program example and some references**   
  * https://learn.microsoft.com/ru-ru/ef/core/get-started/overview/first-app?tabs=visual-studio
