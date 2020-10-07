import { toDataOrError } from "./response"

export const urlWith = (searchParams = {}, url) => {
  let urlSearchParams = Object.entries(searchParams).reduce(
    (searchParams, [key, value]) => {
      if (isEmptyString(value) || isNilOrUndefined(value)) {
        return searchParams
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

const fetchApi = (
  method = "",
  url,
  { body, data = {}, headers = {}, ...options } = {}
) =>
  fetch(url, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...headers
    },
    body: body ?? JSON.stringify(data),
    ...options
  }).then(toDataOrError)

const isEmptyString = (value) => typeof value == "string" && value === ""

const isNilOrUndefined = (value) =>
  typeof value === "undefined" || value === null

export default {
  get,
  put: (url, options) => fetchApi("PUT", url, options),
  post: (url, options) => fetchApi("POST", url, options),
  delete: (url, options) => fetchApi("DELETE", url, options)
}
