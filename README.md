# DR2-Test
The project is created with intention of testing Dark Rift 2 and possible network architecture that can be on the main game

## Folder structure
Inside the main folder **DR2-Test** except usually Unity files and folders we have two other folders:
 - The **Backend** folder is the folder that has the server-side DR2 C# implementation and
 - The **Server** folder where we have the local server instance.
 
## Run the project
To run the project we first need to start the server instance and we do that by running `dotnet Lib/DarkRift.Server.Console.dll` from the **Server** folder. Then
we can open *SampleScene* in *Unity* and use *ParrelSync* to create multiple instances of the editor and test the project.

## Making changes
After we make changes to the backend implementation, we need to build the project from Visual Studio. When we do that couple of `dll`s are created and then copied on the **Server** folder and in the **Unity** folder. This is achieved by executing post-build events on `Backend.csproj` and `GameModels.csproj`. 
By doing this we have the updates on the server instance and we can access `GameModels` on `Unity` as plugins. 
