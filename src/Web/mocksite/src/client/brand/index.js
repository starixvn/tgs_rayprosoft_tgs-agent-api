import useSWR, { cache } from "swr"
import { toDataOrError } from "../response"
import { handleError } from "../Error"

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

const useApiKey = (code) => {
  let brands = useBrands()
  return brands.find((brand) => brand.brandCode === code)?.apiKey ?? ""
}

export const useBrands = () => {
  //TODO: paging required?
  let url = `/global-api/brand/apikeys?showAll=true`

  const CacheKey = url
  let brands = cache.get(CacheKey) ?? undefined

  let { data } = useSWR(
    url,
    (url) =>
      fetch(url)
        .then(toDataOrError)
        .then((data) => {
          let brands = data?.items
          cache.set(CacheKey, brands)
          return brands
        }),
    { initialData: brands }
  )

  return data ?? []
}

export const useBrandAnnouncementList = ({ brandCode, currency }) => {
  let { token: brandToken, ...brandTokenHook } = useBrandToken({
    brandCode,
    currency
  })

  let keys =
    brandToken && currency
      ? [`/api/announcement/list`, brandCode, currency]
      : undefined
  let { data: result, error, ...rest } = useSWR(keys, (url) =>
    fetch(url, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${brandToken}`
      }
    })
      .then(toDataOrError)
      .catch(handleError({ brandTokenHook }))
  )

  return {
    items: result?.data,
    error,
    isLoading: keys && !result && !error,
    ...rest
  }
}
