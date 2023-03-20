using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WikiArtArtist 
{   
   public string image;
   public string artistName;
   public string birthDayAsString;
   public string deathDayAsString;
   public string key;
   public string extract;
}

[Serializable]
public class Page
{
   public WikiArtArtist[] pages;
}

[Serializable]
public class Artists
{
   public WikiArtArtist[] artists;
}