import { toDataOrError } from "./response"

export const urlWith = (searchParams = {}, url) => {
  let urlSearchParams = Object.entries(searchParams).reduce(
    (searchParams, [key, value]) => {
      if (isEmptyString(value) || isNilOrUndefined(value)) {
        return searchParams
      }

      if (Array.isArray(value)) {
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
  { body, data = {}, headers = {}, ...options } = {}
) => {
  data = Object.entries(data).reduce((data, [key, value]) => {
    if (isEmptyString(value) || isNilOrUndefined(value)) {
      return data
    }

    data[key] = value
    return data
  }, {})
  return fetch(url, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...headers
    },
    body: body ?? JSON.stringify(data),
    ...options
  }).then(toDataOrError)
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
