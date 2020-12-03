import useSWR, { cache } from "swr"
import { useBrandToken, usePlayerToken } from "../brand/client"
import { toValue as toPlatformType } from "../platform"

export const useGameList = (
  { brandCode, username, currency },
  { gameProviderCode, platform, language }
) => {
  let { token: brandToken } = useBrandToken({ brandCode, currency })
  let { token: playerToken } = usePlayerToken({ brandCode, username, currency })

  let keys =
    brandToken &&
    playerToken &&
    gameProviderCode &&
    platform &&
    language &&
    currency
      ? [
          `/api/game/list`,
          brandToken,
          playerToken,
          gameProviderCode,
          platform,
          language,
          currency
        ]
      : undefined

  let { data = {}, ...rest } = useSWR(
    keys,
    (
      url,
      brandToken,
      playerToken,
      gameProviderCode,
      platform,
      language,
      currency
    ) => {
      return fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        },
        body: JSON.stringify({
          gpcode: gameProviderCode,
          platformtype: toPlatformType(platform),
          language,
          currency
        })
      }).then((response) => response.json())
    }
  )

  return {
    items: data?.games ?? [],
    ...rest
  }
}

//TODO: save to cache result?
//TODO: need language?
export const useGameUrl = (
  { brandCode, username, currency },
  { gameProviderCode, gameCode }
) => {
  let { token: brandToken } = useBrandToken({ brandCode, currency })
  let { token: playerToken } = usePlayerToken({ brandCode, username, currency })

  let keys =
    brandToken && playerToken && gameProviderCode && gameCode
      ? [
          `/api/game/geturl`,
          brandToken,
          playerToken,
          gameProviderCode,
          gameCode
        ]
      : undefined
  let { data = {}, ...rest } = useSWR(
    keys,
    (url, brandToken, playerToken, gameProviderCode, gameCode) => {
      return fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        },
        body: JSON.stringify({
          gpcode: gameProviderCode,
          gcode: gameCode
        })
      }).then((response) => response.json())
    }
  )

  return {
    url: data?.url,
    ...rest
  }
}
