using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBuilding : MonoBehaviour
{
    public GameObject prefab;
    public GameObject sphere;
    public GameObject image;
    public GameObject Buildings;
    
    private int res = 20;
    private int taille_max = 10;
    private float[,] density_map;
    List<Vector3> positions = new List<Vector3>();
    List<Vector3> batiments;
    private float[] remap;

    // Start is called before the first frame update
    void Start()
    {
        density_map = new float[res,res];

        ///ex
        positions.Add(new Vector3(2.0f,0.0f,2.0f));
        positions.Add(new Vector3(2.0f,0.0f,1.7f));
        positions.Add(new Vector3(-5.0f,0.0f,-4.0f));
        foreach (var position in positions)
        {
            Instantiate(sphere, position, Quaternion.identity);
        }
        batiments = new List<Vector3>(){new Vector3(0.5f,0.0f,-3.0f),new Vector3(-2.0f,0.0f,0.0f),new Vector3(2.0f,0.0f,2.0f)};
        ///

        remap = MinMaxVect(positions);
        Create_DM();
        CreateBuildings();
        Texture2D tex = GetTexturefromArray(density_map,res);
        image.GetComponent<RawImage>().texture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void CreateBuilding(float taille,Vector3 pos){
        GameObject Building =new GameObject("Building");
        Building.transform.parent = Buildings.transform;
        for (int i =0;i< (int)taille;i++){
            Instantiate(prefab, pos + Vector3.up*0.5f*i, Quaternion.identity).transform.parent = Building.transform;
        }
        
    }

    private void CreateBuildings(){
        int posX=0;
        int posZ=0;
        foreach (var batiment in batiments)
        {
            posX = (int)Remap(batiment.x,remap[0],remap[1],0.0f,res-1);
            posZ = (int)Remap(batiment.z,remap[2],remap[3],0.0f,res-1);
            int taille = taille_max*(int)(density_map[posX,posZ]);
            CreateBuilding(taille,batiment);
        }
        
    }

   
    private void Create_DM(){
        int posX=0;
        int posZ=0;
        float max=0;
        foreach (var position in positions)
        {
            posX = (int)Remap(position.x,remap[0],remap[1],0.0f,res-1.0f);
            posZ = (int)Remap(position.z,remap[2],remap[3],0.0f,res-1.0f);
            density_map[posX,posZ] +=1;
            if(posX<res-1){
                density_map[posX+1,posZ] +=0.8f;
            }
            if(posX>0){
                density_map[posX-1,posZ] +=0.8f;
            }
            if(posZ<res-1){
                density_map[posX,posZ+1] +=0.8f;
            }
            if(posZ>0){
                density_map[posX,posZ-1] +=0.8f;
            }
            if(density_map[posX,posZ]>max){
                max = density_map[posX,posZ];
            }

        }
        //normalisation
        for(int i=0;i<res;++i){
            for(int j=0;j<res;++j){
                density_map[j,i] = density_map[j,i]/max;
            }
        }
    }

    //retourne min/max respectivement en x et z
    private static float[] MinMaxVect(List<Vector3> list){
        float min_x = list[0].x;
        float max_x = list[0].x;
        float min_z = list[0].z;
        float max_z = list[0].z;
        float current_x = 0;
        float current_z = 0;
        for(int i=1;i<list.Count;i++)
        {
            current_x = list[i].x;
            current_z = list[i].z;
            if(min_x>current_x){
                min_x = current_x;
            }
            if(max_x<current_x){
                max_x = current_x;
            }
            if(min_z>current_z){
                min_z = current_z;
            }
            if(max_z<current_z){
                max_z = current_z;
            }
        }
        float[] values = new float[4] {min_x,max_x,min_z,max_z};
        return values;
    }

    public static Texture2D GetTexturefromArray(float[,] array,int res){
        Texture2D tex = new Texture2D(res,res);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        Color[] colors = new Color[res*res];
        float value = 0;
        for(int i=0;i<res;++i){
            for(int j=0;j<res;++j){
                value = array[j,i];
                colors[i*res +j] = new Color(value,value,value,1.0f);   
                //colors[i*res +j] = Color.white;
                Debug.Log(colors[i*res +j]);   
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }


 //remap entre 2 ranges de nombres
    private static float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source-sourceFrom)*(targetTo-targetFrom)/(sourceTo-sourceFrom);
    }
  public List<Vector3> Concatenate(List<Vector3> firstList, List<Vector3> secondlist)
    {
        var result = new List<Vector3>(firstList.Count + secondlist.Count);
        result.AddRange(firstList);
        result.AddRange(secondlist);
        return result;
    }

}


