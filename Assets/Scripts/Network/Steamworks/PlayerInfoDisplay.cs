﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Steamworks;
public class PlayerInfoDisplay : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;


    [SerializeField] public Texture2D profileImage = null;
    [SerializeField] public string displayNameText = null;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    #region Server
    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
    }
    #endregion

    #region Client

    public override void OnStartClient()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }


    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId)
    {
        var cSteamId = new CSteamID(newSteamId);

        displayNameText = SteamFriends.GetFriendPersonaName(cSteamId); //Getting name

        int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);

        if (imageId == -1) { return; }

        profileImage = GetSteamImageAsTexture(imageId); //If we dont have it locally,gets from Steam
    }


    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == steamId)
        {
            profileImage = GetSteamImageAsTexture(callback.m_iImage);
        }
    }


    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height*4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

    #endregion

}
