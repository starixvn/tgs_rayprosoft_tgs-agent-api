import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { useBrandToken } from "./index"

export const useBrandPlayerList = ({ brandCode, currency }) => {
  //TODO:
}

export const useCreatePlayer = ({ brandCode, currency }, formData) => {
  let { token: brandToken, mutate: mutateBrandToken } = useBrandToken({
    brandCode,
    currency
  })

  let keys =
    brandCode && currency && formData
      ? [`/api/Player/create`, formData]
      : undefined

  let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
    return fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`
      },
      body: JSON.stringify(formData)
    })
      .then(toDataOrError)
      .catch((error) => {
        console.log("TODO: auto retry after get new brand token?")
        mutateBrandToken()
        return Promise.reject(error)
      })
  })

  return {
    result,
    error,
    isLoading: keys && !result && !error,
    ...rest
  }
}

export const useCheckPlayerUsername = ({ brandCode, currency }, formData) => {
  let { token: brandToken, mutate: mutateBrandToken } = useBrandToken({
    brandCode,
    currency
  })

  let keys =
    brandToken && formData
      ? [`/api/Player/checkUsername`, formData, brandCode, currency]
      : undefined

  let { data: result, error, ...rest } = useSWR(keys, (url, formData) => {
    return fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`
      },
      body: JSON.stringify(formData)
    })
      .then(toDataOrError)
      .catch((error) => {
        console.log("TODO: auto retry after get new brand token?")
        mutateBrandToken()
        return Promise.reject(error)
      })
  })

  return {
    result,
    error,
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
  let { token: brandToken, mutate: mutateBrandToken } = useBrandToken({
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

  let { data: result, error, mutate, ...rest } = useSWR(
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
    isLoading: keys && !result && !error,
    error,
    mutate: mutateBrandToken,
    mutate: (data, shouldRevalidate) => {
      cache.delete(keys)
      cache.delete(ResultCacheKey)
      mutate(data, shouldRevalidate)
    },
    deleteCache: () => {
      cache.delete(keys)
      cache.delete(ResultCacheKey)
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
      .then(toDataOrError)
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
    }).then(toDataOrError)
  )

  return {
    ...data,
    ...rest
  }
}

export const usePlayerInfo = ({ brandCode, currency, username } = {}) => {
  let { token: brandToken, mutate: mutateBrandTokenCache } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, mutate: mutatePlayerTokenCache } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let keys =
    brandToken && playerToken
      ? [`/api/player/info`, brandCode, currency, username]
      : undefined

  let { data, ...rest } = useSWR(keys, (url, brandCode, currency, username) => {
    return fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`,
        Token: playerToken
      }
    }).then(toDataOrError)
  })

  return {
    data: data?.data,
    ...rest
  }
}

export const useUpdatePlayerInfo = (
  { brandCode, currency, username } = {},
  formData
) => {
  let { token: brandToken, mutate: mutateBrandTokenCache } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, mutate: mutatePlayerTokenCache } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let keys =
    brandToken && playerToken && formData
      ? [`/api/player/update`, brandCode, currency, username, formData]
      : undefined

  let { data: result, ...rest } = useSWR(
    keys,
    (url, brandCode, currency, username, formData) => {
      return fetch(url, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        },
        body: JSON.stringify(formData)
      }).then(toDataOrError)
    }
  )

  return {
    result,
    isLoading: keys && !result,
    ...rest
  }
}

export const useChangePlayerPassword = (
  { brandCode, currency, username } = {},
  formData
) => {
  let { token: brandToken, mutate: mutateBrandTokenCache } = useBrandToken({
    brandCode,
    currency
  })
  let { token: playerToken, mutate: mutatePlayerTokenCache } = usePlayerToken({
    brandCode,
    username,
    currency
  })

  let keys =
    brandToken && playerToken && formData
      ? [`/api/player/changePassword`, brandCode, currency, username, formData]
      : undefined

  let { data: result, error, ...rest } = useSWR(
    keys,
    (url, brandCode, currency, username, formData) => {
      return fetch(url, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${brandToken}`,
          Token: playerToken
        },
        body: JSON.stringify(formData)
      }).then(toDataOrError)
    }
  )

  return {
    result,
    error,
    isLoading: keys && !result && !error,
    ...rest
  }
}
