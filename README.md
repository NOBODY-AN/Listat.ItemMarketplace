# Listat.ItemMarketplace
Marketplace database (Code-First approach) and REST API to query marketplace auctions.
The project implements logging using Serilog. Caching using memory cache and Decorator pattern. There is also an attempt to use DDD.

<h3>Auction API v4</h3>
description: Optional filtering (by status, by seller, by name), filtering by name is case insensitive, any position. Sorting (by key: CreatedDt, Price, by order: ASC, DESC).
  
routing: '/api/v4/auction'
