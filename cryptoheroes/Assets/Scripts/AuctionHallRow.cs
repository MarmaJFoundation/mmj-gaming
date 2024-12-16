
using EnhancedUI;
using EnhancedUI.EnhancedScroller;

public class AuctionHallRow : EnhancedScrollerCellView
{
    public AuctionHallCell[] rowCellViews;

    public void SetData(ref SmallList<AHItemData> data, AuctionHallController auctionHallController, int startingIndex)
    {
        // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
        for (var i = 0; i < rowCellViews.Length; i++)
        {
            var dataIndex = startingIndex + i;

            // if the sub cell is outside the bounds of the data, we pass null to the sub cell
            rowCellViews[i].SetData(auctionHallController, dataIndex, dataIndex < data.Count ? data[dataIndex] : null);
        }
    }
}