import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "../brand"
import { usePlayerToken } from "../brand/player"

export const useGameUrl = ({ brandCode, username, currency }, data) => {
  let { token: brandToken, mutate: mutateBrandToken } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, mutate: mutatePlayerToken } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let url =
    brandToken && playerToken && data?.gameProviderCode && data?.gameCode
      ? [`/api/game/geturl`, data]
      : undefined

  let { data: result, mutate, ...rest } = useSWR(url, (url, data) => {
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
      })
      .catch((error) => {
        mutate(undefined, false)
        let { err, errmsg } = error
        console.warn("err", err, "errmsg", errmsg)
        console.log("TODO: use err code to know which token expired?")
        // mutatePlayerToken()
        return Promise.reject(error)
      })
  })

  return {
    url: result?.url,
    ...rest
  }
}
