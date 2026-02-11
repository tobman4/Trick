# Trick

Export acount stats for prometheus

## About

Trick is a .NET 10.0 application designed to export League of Legends account statistics to Prometheus. It utilizes the Riot Games API and Data Dragon to fetch real-time game data and player metrics.

The application tracks various metrics including:
- **Game Statistics:** Game length, win/loss records.
- **Champion Mastery:** Mastery levels and scores for individual champions.
- **Player Actions:** Detailed ping usage (blue, caution, push, missing, on-my-way, all-in, help, vision).
- **Performance:** Gold earned and vision scores.

**Configuration:**
To run the application, you must provide a valid `RiotToken` in your configuration (e.g., via `appsettings.json` or environment variables).

## Metrics

| Metric Name | Type | Description |
| :--- | :--- | :--- |
| `game_length` | Histogram | Game length in seconds |
| `player_total_level` | Gauge | Player total level |
| `player_total_score` | Gauge | Player total score |
| `champion_level` | Gauge | Mastery level on a champion |
| `champion_score` | Gauge | Mastery score on a champion |
| `player_blue_ping` | Counter | Number of blue pings used |
| `player_caution_ping` | Counter | Number of caution pings used |
| `player_push_ping` | Counter | Number of push pings used |
| `player_missing_ping` | Counter | Number of missing pings used |
| `player_omw_ping` | Counter | Number of on-my-way pings used |
| `player_allIn_ping` | Counter | Number of all-in pings used |
| `player_help_ping` | Counter | Number of help pings used |
| `player_vision_ping` | Counter | Number of vision pings used |
| `player_enemyVision_ping` | Counter | Number of enemy vision pings used |
| `games_count` | Counter | Total games observed |
| `games_won` | Counter | Total games won |
| `player_champ_games` | Counter | Total games played on a champion |
| `player_champ_games_won` | Counter | Total games won on a champion |
| `player_gold_earned` | Counter | Total gold earned |
| `player_vision` | Histogram | Player vision score |
