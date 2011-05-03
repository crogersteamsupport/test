using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.WebUtils
{
  
  public class MenuTreeItem
  {
    private MenuTreeItems _items;

    public MenuTreeItem(string id, string caption, string imageUrl, string data)
    {
      _items = new MenuTreeItems();
      ID = id;
      Caption = caption;
      ImageUrl = imageUrl;
      Data = data;
    }
    public string ID { get; set; }
    public string Caption { get; set; }
    public string ImageUrl { get; set; }
    public string Data { get; set; }
    public MenuTreeItems Items { get { return _items; } }
  }

  public class MenuTreeItems : List<MenuTreeItem> { }

  public class MenuTree
  {
    private MenuTreeItems _items;
    public MenuTreeItems Items
    {
      get { return _items; }
    }

    public MenuTree()
    {
      _items = new MenuTreeItems();
    }

    private string GetMenuItemHtml(MenuTreeItem item, bool isCollapsable, bool isSelected)
    {
      string s;

      if (isCollapsable)
      {
s = @"<div id=""{0}"" class=""ui-menutree-item ui-menutree-state-default ui-corner-all"">
  <div class=""ui-menutree-action ui-menutree-expanded""></div>
  <div class=""ui-menutree-icon"" style=""background-image: url('{2}');""></div>
  <div class=""ui-menutree-text"">{1}</div>
  <span class=""ui-menutree-data"">{3}</span>
</div>";
      }
      else
      {
s = @"<div id=""{0}"" class=""ui-menutree-item ui-menutree-state-default ui-corner-all"">
  <div class=""ui-menutree-action ui-menutree-noaction""></div>
  <div class=""ui-menutree-icon"" style=""background-image: url('{2}');""></div>
  <div class=""ui-menutree-text"">{1}</div>
  <span class=""ui-menutree-data"">{3}</span>
</div>";
      }

      string result = string.Format(s, item.ID, item.Caption, item.ImageUrl, item.Data);
      if (isSelected) result = result.Replace("ui-menutree-state-default", "ui-menutree-state-selected");
      return result;
    }

    public string Html(string selectedMenuItemID)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("<div class=\"ui-menutree\">");
      AddItems(builder, Items, true, selectedMenuItemID);
      builder.Append("</div>");
      return builder.ToString();
    }

    private void AddItems(StringBuilder builder, MenuTreeItems items, bool isRoot, string selectedMenuItemID)
    {
      if (!isRoot) builder.Append(@"<div class=""ui-menutree-subitems"">");
      foreach (MenuTreeItem item in items)
      {
        bool hasSubItems = item.Items.Count > 0;
        builder.Append(GetMenuItemHtml(item, hasSubItems, item.ID == selectedMenuItemID));
        if (hasSubItems) AddItems(builder, item.Items, false, selectedMenuItemID);
      }
      if (!isRoot) builder.Append("</div>");
    }
  }



}
