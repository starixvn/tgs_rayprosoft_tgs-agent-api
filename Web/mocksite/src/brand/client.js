import useSWR, { cache } from "swr"

const toDataOrError = (response) => {
  if (!response.ok) {
    return response
      .json()
      .catch((error) => {
        return Promise.reject({
          title: response.statusText
        })
      })
      .then((data) => {
        return Promise.reject(data)
      })
  }

  return response.json()
}

const TGS = "TGS"

export const useBrandToken = ({ brandCode, currency }) => {
  let apiKey = useApiKey(brandCode)
  let keys =
    brandCode && currency && apiKey
      ? ["/api/auth/jwt", brandCode, currency]
      : undefined

  const ResultCacheKey = `${useBrandToken.name}:${brandCode}:${currency}`
  let cached = cache.get(ResultCacheKey)
  let { data, mutate, ...rest } = useSWR(
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
      })
        .then((response) => response.json())
        .then((result) => {
          cache.set(ResultCacheKey, result)
          return result
        }),
    {
      initialData: keys && cached?.token ? cached : undefined
    }
  )

  return {
    token: data?.token,
    expires: data?.expires,
    mutate,
    deleteCache: () => {
      cache.delete(keys)
      cache.delete(ResultCacheKey)
      mutate(undefined)
    },
    clearCache: () => cache.clear(),
    ...rest
  }
}

const DefaultPassword = "TgsP@ssw0rd"
const playerTokenCacheKey = ({ brandCode, currency, username }) =>
  `playerToken:${brandCode}:${currency}:${username}`

const deletePlayerTokenCache = ({ brandCode, currency, username }) =>
  cache.delete(playerTokenCacheKey({ brandCode, currency, username }))

const playerTokenFormDataCacheKey = ({ brandCode, currency, username }) =>
  `playerTokenFormData:${brandCode}:${currency}:${username}`

export const usePlayerToken = (
  { brandCode, currency, username } = {},
  data
) => {
  let { token: brandToken, deleteCache: deleteBrandCache } = useBrandToken({
    brandCode,
    currency
  })

  let formDataCacheKey = playerTokenFormDataCacheKey({
    brandCode,
    currency,
    username
  })
  data = data ??
    cache.get(formDataCacheKey) ?? {
      username,
      password: DefaultPassword,
      bypassCaptcha: true,
      captcha: "captcha",
      answer: "answer"
    }

  let body =
    data && data?.username && data?.password ? JSON.stringify(data) : undefined
  let keys =
    brandCode && brandToken && username && body
      ? [`/api/player/login`, brandCode, username, body]
      : undefined

  const ResultCacheKey = playerTokenCacheKey({ brandCode, currency, username })
  let cached = cache.get(ResultCacheKey)

  let { data: result, mutate, ...rest } = useSWR(
    keys,
    (url, brandCode, username, body) => {
      return fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`
        },
        body
      })
        .then(toDataOrError)
        .then((result) => {
          let autoLoginFormData = JSON.parse(body)
          cache.set(formDataCacheKey, {
            username: autoLoginFormData.username,
            password: autoLoginFormData.password,
            bypassCaptcha: true,
            captcha: "captcha",
            answer: "answer",
            formDataFromCache: true
          })
          cache.set(ResultCacheKey, result)
          return result
        })
    },
    {
      initialData: keys && cached?.token ? cached : undefined
    }
  )

  return {
    token: result?.token,
    isLoading: keys && !result,
    mutate,
    deleteCache: () => {
      cache.delete(keys)
      cache.delete(ResultCacheKey)
      mutate(undefined)
    },
    clearCache: () => cache.clear(),
    ...rest
  }
}

export const useLogout = ({ brandCode, currency, username }, playerToken) => {
  let { token: brandToken, deleteCache: deleteBrandCache } = useBrandToken({
    brandCode,
    currency
  })

  let keys =
    brandToken && playerToken
      ? [`/api/player/logout`, brandToken, playerToken]
      : undefined
  if (keys) {
    deletePlayerTokenCache({ brandCode, currency, username })
  }
  let { data: result, ...rest } = useSWR(keys, (url, brandToken, playerToken) =>
    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`,
        Token: playerToken
      }
    })
      .then((response) => response.json())
      .then((result) => {
        deletePlayerTokenCache({ brandCode, currency, username })
        return result
      })
  )

  return {
    result,
    isLoading: keys && !result,
    deleteCache: () => {
      cache.delete(keys)
    },
    clearCache: () => cache.clear(),
    ...rest
  }
}

export const useCaptcha = ({ brandCode, currency }) => {
  let { token: brandToken } = useBrandToken({ brandCode, currency })
  let keys = brandToken ? [`/api/player/captcha`, brandCode] : undefined

  let { data = {}, ...rest } = useSWR(keys, (url, brandCode) =>
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
