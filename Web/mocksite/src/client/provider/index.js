import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "../brand"
import { usePlayerToken } from "../brand/player"
import { toValue as toPlatformType } from "../../platform"

export const useGameProviderList = (
  { brandCode, username, currency } = {},
  { platform, language } = {}
) => {
  let { token: brandToken } = useBrandToken({ brandCode, currency })
  let { token: playerToken } = usePlayerToken({ brandCode, username, currency })

  let keys =
    brandToken && playerToken && platform && language && currency
      ? [
          `/api/gameProvider/list`,
          brandCode,
          username,
          platform,
          language,
          currency
        ]
      : undefined

  let { data = {}, ...rest } = useSWR(
    keys,
    (url, brandCode, username, platform, language, currency) => {
      return fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        },
        body: JSON.stringify({
          platformtype: toPlatformType(platform),
          language,
          currency
        })
      }).then(toDataOrError)
    }
  )

  return {
    items: data?.gameproviders,
    ...rest
  }
}

export const useGameList = (
  { brandCode, username, currency },
  { gameProviderCode, platform, language }
) => {
  let {
    token: brandToken,
    deleteCache: deleteBrandTokenCache
  } = useBrandToken({ brandCode, currency })
  let {
    token: playerToken,
    deleteCache: deletePlayerTokenCache
  } = usePlayerToken({ brandCode, username, currency })

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
      })
        .then(toDataOrError)
        .catch((error) => {
          deletePlayerTokenCache()
          deleteBrandTokenCache()
          console.error("TODO: mutate token when 401 error")
          return error
        })
    }
  )

  return {
    items: data?.games ?? [],
    ...rest
  }
}
