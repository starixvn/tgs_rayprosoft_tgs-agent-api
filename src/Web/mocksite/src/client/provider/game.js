import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "../brand"
import { usePlayerToken } from "../brand/player"
import { handleError } from "../Error"

export const useGameUrl = (
    { brandCode, username, currency, platformtype },
    data
) => {
    let { token: brandToken, ...brandTokenHook } = useBrandToken({
        brandCode,
        currency
    })
    let { token: playerToken, ...playerTokenHook } = usePlayerToken({
        brandCode,
        username,
        currency,
        platformtype
    })

    let keys =
        brandToken &&
        playerToken &&
        data?.gameProviderCode &&
        data?.gameCode &&
        data?.gameId &&
        data?.language
            ? [
                  `/api/game/geturl?gpcode=${data?.gameProviderCode}&gid=${data?.gameId}&language=${data?.language}`,
                  data
              ]
            : undefined

    let { data: result, ...rest } = useSWR(keys, (url) => {
        return fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${brandToken}`,
                Token: playerToken
            }
        })
            .then(toDataOrError)
            .then((data) => {
                let { err } = data
                if (err) {
                    return Promise.reject({
                        err,
                        errmsg: data.errmsg
                    })
                }
                return data
            })
            .catch(handleError({ brandTokenHook, playerTokenHook }))
    })

    return {
        url: result?.url,
        ...rest
    }
}
