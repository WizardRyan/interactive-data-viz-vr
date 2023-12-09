using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public static class DataMaster
{

    private static List<Texture2D> heatmaps = new List<Texture2D>();
    public static int StartYear = 1884;
    public static int EndYear = 2022;

    private static List<Texture2D> rainFallMaps = new List<Texture2D>();
    private static int startYearPrecip = 2000;
    private static int endYearPrecip = 2022;



    static DataMaster()
    {
        for(int i = StartYear; i <= EndYear; i++)
        {
            string path = $"./Assets/Images/Heatmaps/gt_{i}_720x360.jpg";
            var imageBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(720, 360);
            tex.LoadImage(imageBytes);
            tex.name = i.ToString();
            heatmaps.Add(tex);
        }

        for (int i = startYearPrecip; i <= endYearPrecip; i++) 
        {
            string path = $"./Assets/Images/Rainfall/GPM_3IMERGM_{i}-12.JPEG";
            var imageBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(512, 256);
            tex.LoadImage(imageBytes);
            tex.name = i.ToString();
            rainFallMaps.Add(tex);
        }
    }
    
    public static Texture2D GetHeatMapTex(int year)
    {
        if (year < StartYear) year = StartYear;
        if (year > EndYear) year = EndYear;

        var index = year - StartYear;
        
        return heatmaps[index];
    }

    public static bool RainfallDataExists(int year)
    {
        return year >= startYearPrecip && year <= endYearPrecip;
    }

    public static Texture2D GetRainfallTex(int year)
    {
        if (year < startYearPrecip) year = startYearPrecip;
        if (year > endYearPrecip) year = endYearPrecip;

        var index = year - startYearPrecip;

        return rainFallMaps[index];
    }
}
