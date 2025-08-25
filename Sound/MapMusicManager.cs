using DarkTonic.MasterAudio;
using UnityEngine;

public static class MapMusicManager
{
    private static MapComponent.MapType? _currentMapType = null;

    public static void PlayMusicForMap(MapComponent map)
    {
        if (map == null) return;

        string playlistName = GetPlaylistName(map.mapType);
        if (string.IsNullOrEmpty(playlistName)) return;

        var controller = MasterAudio.OnlyPlaylistController;
        if (controller == null)
        {
            return;
        }

        bool isAlreadyPlaying = controller.IsSongPlaying(playlistName);

        if (isAlreadyPlaying)
        {
            return;
        }

        controller.StartPlaylist(playlistName);
        _currentMapType = map.mapType;
    }


    private static string GetPlaylistName(MapComponent.MapType type)
    {
        return type switch
        {
            MapComponent.MapType.Tutorial => "TutorialTheme",
            MapComponent.MapType.ShallowForest => "ShallowForestTheme",
            MapComponent.MapType.DeepForest => "DeepForestTheme",
            MapComponent.MapType.Town => "TownTheme",
            MapComponent.MapType.MiddleBoss => "BossTheme",
            MapComponent.MapType.FinalBoss => "FinalBossTheme",
            _ => ""
        };
    }
}