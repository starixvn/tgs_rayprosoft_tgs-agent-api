import useSWR, { cache } from "swr"

const TGS = "TGS"

export const useBrandToken = ({ brandCode, currency }, initialData) => {
  let apiKey = useApiKey(brandCode)
  let keys =
    brandCode && currency && apiKey
      ? ["/api/auth/jwt", brandCode, currency]
      : undefined

  let { data, ...rest } = useSWR(
    keys,
    (url, brandCode, currency) =>
      fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          client: apiKey,
          currency
        })
      }).then((response) => response.json()),
    {
      initialData
    }
  )

  return {
    token: data?.token,
    /*
    expires: data?.expires,
    */
    ...rest
  }
}

const DefaultPassword = "TgsP@ssw0rd"
export const usePlayerToken = (
  { brandCode, currency, username } = {},
  data,
  initialData
) => {
  let { token: brandToken } = useBrandToken(
    { brandCode, currency },
    { token: initialData?.brandToken }
  )
  let keys =
    brandToken && username
      ? [
          `/api/player/login`,
          brandToken,
          username,
          JSON.stringify(
            data ?? {
              username,
              password: DefaultPassword,
              bypassCaptcha: true,
              captcha: "captcha",
              answer: "answer"
            }
          )
        ]
      : undefined

  let { data: result, ...rest } = useSWR(
    keys,
    (url, brandToken, username, body) => {
      return fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`
        },
        body
      }).then((response) => {
        let result = response.json()
        return result
      })
    }
  )

  return {
    token: result?.token,
    /*
    expires: result?.expires,
    */
    ...rest
  }
}

export const useCaptcha = ({ brandCode, currency }, initialData) => {
  let { token: brandToken } = useBrandToken(
    { brandCode, currency },
    { token: initialData?.brandToken }
  )
  let keys = brandToken ? [`/api/player/captcha`, brandToken] : undefined

  let { data = {}, ...rest } = useSWR(keys, (url, brandToken) =>
    fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`
      }
    }).then((response) => response.json())
  )

  return {
    ...data,
    ...rest
  }
}

const useApiKey = (code = TGS) => {
  const store = {
    TGS: "xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV"
  }

  return Reflect.get(store, code)
}
