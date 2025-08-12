# PlayFab Integration for Mr. Box

## Overview

This integration adds online leaderboards to Mr. Box using PlayFab, a complete backend platform for games. With this integration, players can compete globally and see their rankings on online leaderboards.

## Features

- Online leaderboards for both Classic and Time Attack modes
- Automatic player authentication using device ID
- Seamless integration with the existing game flow
- Easy-to-use UI for displaying online leaderboards

## Files Added

1. **PlayFabManager.cs**: Core script that handles PlayFab authentication and leaderboard submissions
2. **PlayFabLeaderboardUI.cs**: UI script for displaying online leaderboards
3. **PlayFabSetup.cs**: Helper script for setting up PlayFab in your scene
4. **PlayFabIntegrationGuide.md**: Detailed guide for setting up PlayFab

## Quick Setup

1. Create a PlayFab account and get your Title ID
2. Install the PlayFab SDK in your Unity project
3. Add the PlayFabManager prefab to your scene
4. Configure your Title ID in the PlayFabManager component
5. Set up the PlayFabLeaderboardUI in your scene
6. Add an "Online" button to your existing leaderboard UI

For detailed instructions, see the **PlayFabIntegrationGuide.md** file.

## How It Works

### Authentication

Players are automatically authenticated using their device ID. This means they don't need to create an account or log in manually. The game generates a unique ID for each device and uses it to identify the player on PlayFab.

### Score Submission

When a game ends, the score is automatically submitted to the appropriate leaderboard based on the game mode (Classic or Time Attack). The GameManager.cs script has been updated to handle this.

### Leaderboard Display

The PlayFabLeaderboardUI script handles fetching and displaying the online leaderboard data. It shows player names, ranks, and scores, and highlights the current player's entry.

## Customization

You can customize the following aspects of the integration:

- Leaderboard statistic names in PlayFabManager.cs
- UI appearance in PlayFabLeaderboardUI.cs
- Authentication method in PlayFabManager.cs (currently uses device ID)

## Troubleshooting

If you encounter issues with the PlayFab integration, check the following:

1. Make sure your Title ID is correct in PlayFabManager
2. Check that your device has an internet connection
3. Look for error messages in the Unity Console
4. Verify that the statistic names match between your code and PlayFab dashboard

## Next Steps

Now that you have PlayFab integrated, you can explore additional features such as:

- Player profiles and customization
- Cloud saves for player progress
- In-app purchases
- Push notifications
- Analytics and reporting

## Support

For questions about this integration, please refer to the PlayFab documentation or contact the developer.