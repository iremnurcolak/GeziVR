using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WikiArtPainting 
{
    public string title;
    public string contentId;
    public string artistContentId;
    public string artistName;
    public string completitionYear;
    public string yearAsString;
    public string width;
    public string image;
    public string height;   
}

 [Serializable]
 public class RootObject
 {
    public WikiArtPainting[] paintings;
 }

/*paintings icin
public string artistUrl;
    public string url;
    public int[] dictionaries;
    public string location;
    public string period;
    public string serie;
    public string genre;
    public string material;
    public string style;
    public string technique;
    public string sizeX;
    public string sizeY;
    public string diameter;
    public string auction;
    public string yearOfTrade;
    public string lastPrice;
    public string galleryName;
    public string tags;
    public string description;
    public string title;
    public long contentId;
    public int artistContentId;
    public string artistName;
    public int completitionYear;
    public string yearAsString;
    public int width;
    public string image;
    public int height;
    */