import useSWR, { cache } from "swr"
import { useBrandToken, usePlayerToken } from "../../brand/client"
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
      }).then((response) => response.json())
    }
  )

  return {
    items: data?.gameproviders,
    ...rest
  }
}
