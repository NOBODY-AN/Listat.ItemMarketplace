﻿using Domain.Models.Controllers.Query;

namespace Domain.Models.Auctions.GetAuctions
{
    public sealed record SearchByAllNamesPageQuery(string Name, MarketStatus Status, SortOrder SortOrder, AuctionSortKey SortKey, int Limit, int Page);
    public sealed record SearchByAllNamesCursorQuery(string Name, MarketStatus Status, SortOrder SortOrder, AuctionSortKey SortKey, int Limit, int Cursor);
    public sealed record AdvancedSearchPageQuery(string? Name, string? SellerName, MarketStatus? Status, SortOrder SortOrder, AuctionSortKey SortKey, int Limit, int Page);
    public sealed record AdvancedSearchCursorQuery(string Name, string SellerName, MarketStatus Status, SortOrder SortOrder, AuctionSortKey SortKey, int Limit, int Cursor);
}
