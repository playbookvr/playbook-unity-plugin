# Playbook Unity Plugin

[Playbook](https://www.playbookxr.com/) is the most intuitive rapid prototyping tool for immersive experiences. This plugin allows creators to seamlessly import their designs from Figma to Unity for use in 3D and XR projects. 

## Installation

1. Clone the repo and open in **Unity version 2021.3.2 or higher**
2. Open the Example scene and tab to the Playbook prefab in scene hierarchy 
3. [Generate your Figma access token](https://www.figma.com/developers/api#access-tokens)
4. In the `FigmaSyncManager` component, enter the access token in `Figma API Token` field and enter the URL of the file you'd like to import in `Figma File URL`  **Note: Figma file must be owned by the account associated with the access token entered.**

<img width="325" alt="IMG_6307" src="https://user-images.githubusercontent.com/16522243/198509539-ace847ae-dc8b-4076-a887-b987cd86859d.png">

5. For best results, configure the Figma frame you'd like to import to match the following scale and position:

<img width="235" alt="image" src="https://user-images.githubusercontent.com/16522243/198511982-e877f278-af32-4f9c-b566-45b2ac868c34.png">

## How to Use

1. Enter Play Mode and click the `Import From Figma` button on the `FigmaSyncManager` component
2. Elements nested in the <ins>first frame</ins> in the hierarchy of your Figma file will be imported into the scene as textured planes. If a change is made to an element in Figma, importing again will update the element 

<img width="240" alt="image" src="https://user-images.githubusercontent.com/16522243/198510742-020d8128-adde-480c-b487-a39e8ca38265.png">

3. In the `ExportManager` component, choose whether you'd like to export `All` imported elements or just those that are `Selected` 
4. Click `Save Prefabs` to save elements to the `Exported` folder in the Unity project

<img width="206" alt="IMG_3804" src="https://user-images.githubusercontent.com/16522243/198511383-f63e372e-47ce-4211-a7fd-ea29065dffa3.png">

5. Click `Clear Folders` to clear saved elements from the `Exported` and `Resources` folders
