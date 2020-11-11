import { toDataOrError } from "./response"
import useSWR, { cache } from "swr"

export const urlWith = (searchParams = {}, url) => {
  let urlSearchParams = Object.entries(searchParams).reduce(
    (searchParams, [key, value]) => {
      if (isEmptyString(value) || isNilOrUndefined(value)) {
        return searchParams
      }

      if (Array.isArray(value) && value.length > 0) {
        let result = value.reduce((result, item) => {
          return result ? `${result}&${key}=${item}` : `${key}=${item}`
        }, "")
        return searchParams ? `${searchParams}&${result}` : result
      }

      return searchParams
        ? `${searchParams}&${key}=${value}`
        : `${key}=${value}`
    },
    ""
  )
  return urlSearchParams ? `${url}?${urlSearchParams}` : url
}

export const get = (url = "", { searchParams = {}, ...options } = {}) => {
  url = urlWith(searchParams, url)
  return fetch(url, options).then(toDataOrError)
}

export const put = (url, options) => fetchApi("PUT", url, options)

export const post = (url, options) => fetchApi("POST", url, options)

const fetchApi = (
  method = "",
  url,
  { headers = {}, body, data, ...options } = {}
) => {
  headers = data
    ? {
        "Content-Type": "application/json",
        ...headers
      }
    : headers

  return fetch(url, {
    method,
    headers,
    body: body ?? JSON.stringify(dropEmptyValues(data)),
    ...options
  }).then(toDataOrError)
}

const dropEmptyValues = (record) => {
  if (record === null || typeof record != "object") {
    //Note. typeof null = "object"
    return record
  }

  return Object.entries(record).reduce((data, [key, value]) => {
    if (isEmptyString(value) || isNilOrUndefined(value)) {
      return data
    }

    data[key] = value
    return data
  }, {})
}

export const useGet = (url) => {
  let fetchData = !!url
  let key = fetchData ? url : null
  const { data, error, isValidating, mutate } = useSWR(key, get)

  const clearCache = () => {
    cache.clear(url)
  }

  return {
    data,
    isLoading: !error && !data && fetchData,
    isValidating,
    error,
    isError: error,
    mutate,
    clearCache
  }
}

export const useCreatePost = (url, data) => {
  let fetchData = !!url && data !== undefined
  let keys = fetchData ? [url, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => post(url, { data })
  )

  const clearCache = () => cache.clear(url)

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate,
    clearCache
  }
}

export const usePostFormData = (url, formData) => {
  let fetchData = !!url && !!formData
  let keys = fetchData ? [url, formData] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, formData) =>
      post(url, {
        body: formData
      })
  )

  const clearCache = () => cache.clear(url)

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate,
    clearCache
  }
}

export const useUpdatePut = (url, data) => {
  let fetchData = !!url && !!data
  let keys = fetchData ? [url, data] : null
  let { data: result, error, isValidating, mutate } = useSWR(
    keys,
    (url, data) => put(url, { data })
  )

  const clearCache = () => cache.clear(url)

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate,
    clearCache
  }
}

export const useDelete = (url, options) => {
  let fetchData = !!url
  let key = fetchData ? url : null
  const { data: result, error, isValidating, mutate } = useSWR(key, (url) =>
    fetchApi("DELETE", url, options)
  )

  const clearCache = () => {
    cache.clear(url)
  }

  return {
    result,
    isLoading: !error && !result && fetchData,
    isValidating,
    error,
    isError: error,
    mutate,
    clearCache
  }
}

const isEmptyString = (value) => typeof value == "string" && value === ""

const isNilOrUndefined = (value) =>
  typeof value === "undefined" || value === null

export default {
  get,
  put,
  post,
  delete: (url, options) => fetchApi("DELETE", url, options)
}
