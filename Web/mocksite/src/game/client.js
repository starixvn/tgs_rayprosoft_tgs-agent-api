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

export const useGameUrl = ({ brandCode, username, currency }, data) => {
  let { token: brandToken } = useBrandToken({ brandCode, currency })
  let { token: playerToken } = usePlayerToken({ brandCode, username, currency })

  let url =
    brandToken && playerToken && data?.gameProviderCode && data?.gameCode
      ? [`/api/game/geturl`, data]
      : undefined

  let { data: result, ...rest } = useSWR(url, (url, data) => {
    return fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`,
        Token: playerToken
      },
      body: JSON.stringify({
        gpcode: data?.gameProviderCode,
        gcode: data?.gameCode
      })
    }).then((response) => response.json())
  })

  return {
    url: result?.url,
    ...rest
  }
}
