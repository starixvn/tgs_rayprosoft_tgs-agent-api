import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"

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
        .then(toDataOrError)
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

const useApiKey = (code = TGS) => {
  const store = {
    TGS: "xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV"
  }
  console.warn("TODO: add gpi api key")
  return Reflect.get(store, code)
}
