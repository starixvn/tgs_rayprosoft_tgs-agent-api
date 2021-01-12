import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "../brand"
import { usePlayerToken } from "../brand/player"
import { toValue as toPlatformType } from "../../platform"
import { gameStatusFromInt } from "../../game/GameStatus"
import { handleError } from "../Error"

export const useGameProviderList = (
  { brandCode, username, currency } = {},
  { platform, language } = {}
) => {
  let { token: brandToken, ...brandTokenHook } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, ...playerTokenHook } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let keys =
    brandToken && playerToken && platform && language && currency
      ? [
          `/api/gameProvider/list?platformtype=${toPlatformType(
            platform
          )}&language=${language}&currency=${currency}`,
          brandCode,
          username
        ]
      : undefined

  let { data = {}, ...rest } = useSWR(keys, (url, brandCode, username) => {
    return fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`,
        Token: playerToken
      }
    })
      .then(toDataOrError)
      .catch(handleError({ brandTokenHook, playerTokenHook }))
  })

  return {
    items: data?.data,
    ...rest
  }
}

export const useGameList = (
  { brandCode, username, currency },
  { gameProviderCode, platform, language }
) => {
  let { token: brandToken, ...brandTokenHook } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, ...playerTokenHook } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let keys =
    brandToken &&
    playerToken &&
    gameProviderCode &&
    platform &&
    language &&
    currency
      ? [
          `/api/game/list?gpcode=${gameProviderCode}&platformtype=${toPlatformType(
            platform
          )}&language=${language}&currency=${currency}`,
          brandToken,
          playerToken
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
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        }
      })
        .then(toDataOrError)
        .then(({ data = [], ...rest }) => ({
          ...rest,
          items: data.map(({ status, ...game }) => ({
            ...game,
            status: gameStatusFromInt(status)
          }))
        }))
        .catch(handleError({ brandTokenHook, playerTokenHook }))
    }
  )

  return {
    items: data?.items ?? [],
    ...rest
  }
}
