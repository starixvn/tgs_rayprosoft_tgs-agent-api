import useSWR, { cache } from "swr"
import { useBrandToken, usePlayerToken } from "../../brand/client"
import { toValue as toPlatformType } from "../../platform"

export const useGameProviderList = (
  { brandCode, username, currency } = {},
  { platform, language } = {},
  initialData
) => {
  let { token: brandToken } = useBrandToken(
    { brandCode, currency },
    initialData?.brandToken ? { token: initialData?.brandToken } : undefined
  )
  let { token: playerToken } = usePlayerToken(
    { brandCode, username, currency },
    undefined,
    initialData?.playerToken ? { token: initialData?.playerToken } : undefined
  )

  let keys =
    brandToken && playerToken && platform && language && currency
      ? [
          `/api/gameProvider/list`,
          brandToken,
          playerToken,
          platform,
          language,
          currency
        ]
      : undefined

  let { data = {}, ...rest } = useSWR(
    keys,
    (url, brandToken, playerToken, platform, language, currency) =>
      fetch(url, {
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
  )

  return {
    items: data?.gameproviders,
    ...rest
  }
}
