# PlayFab Integration Guide for Mr. Box

This guide will help you set up PlayFab for online leaderboards in your Mr. Box game.

## Step 1: Create a PlayFab Account

1. Go to [PlayFab.com](https://playfab.com/) and sign up for a free account
2. Create a new title for your game (e.g., "Mr. Box")
3. Note your Title ID from the PlayFab dashboard

## Step 2: Install the PlayFab SDK in Unity

### Option 1: Using the PlayFab Editor Extensions (Recommended)

1. In Unity, go to **Window > Asset Store**
2. Search for "PlayFab Editor Extensions"
3. Download and import the package
4. After importing, go to **Window > PlayFab > Editor Extensions**
5. Log in with your PlayFab account
6. Select your title and click "Install PlayFab SDK"

### Option 2: Manual Installation

1. Download the latest Unity SDK from [PlayFab GitHub](https://github.com/PlayFab/UnitySDK/releases)
2. Import the package into your Unity project

## Step 3: Configure PlayFab in Your Game

1. Locate the `PlayFabManager` GameObject in the scene hierarchy
2. In the Inspector, enter your Title ID in the "Title Id" field
3. Make sure the following statistics are set up:
   - Classic Leaderboard Statistic: `classic_score`
   - Time Attack Leaderboard Statistic: `time_attack_score`

## Step 4: Set Up Leaderboard UI

1. Find the `PlayFabLeaderboardUI` GameObject in your scene
2. Make sure all UI references are properly assigned:
   - Leaderboard Panel
   - Content Parent
   - Entry Prefab
   - Title Text
   - Classic Button
   - Time Attack Button
   - Close Button
   - Loading Indicator
   - Status Text

## Step 5: Configure Your PlayFab Dashboard

1. Log in to your PlayFab dashboard
2. Go to **Leaderboards** in the left menu
3. Create two new leaderboards:
   - Name: "Classic Score", Statistic Name: `classic_score`, Sort Direction: Descending, Reset Frequency: Never
   - Name: "Time Attack Score", Statistic Name: `time_attack_score`, Sort Direction: Descending, Reset Frequency: Never

## Step 6: Test Your Integration

1. Run your game
2. Play a few rounds in both Classic and Time Attack modes
3. Check the leaderboards in your game
4. Verify scores are being submitted by checking the PlayFab dashboard

## Troubleshooting

- If scores aren't appearing, check the Unity Console for error messages
- Verify your Title ID is correct
- Make sure your device has an internet connection
- Check that the statistic names match exactly between your code and PlayFab dashboard

## Next Steps

Now that you have PlayFab integrated, you can explore additional features:

- Player authentication with Facebook, Google, or custom login
- Cloud saves for player progress
- In-app purchases
- Push notifications
- Analytics and reporting

Refer to the [PlayFab documentation](https://docs.microsoft.com/en-us/gaming/playfab/) for more information on these features.